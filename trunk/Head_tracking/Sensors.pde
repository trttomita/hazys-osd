
#define ACC_ORIENTATION(X, Y, Z)  {accADC[ROLL]  = Z; accADC[PITCH]  = Y; accADC[YAW]  = -X;}

#define GYRO_ORIENTATION(X, Y, Z) {gyroADC[ROLL] = X; gyroADC[PITCH] = -Z; gyroADC[YAW] = Y;}

#define MAG_ORIENTATION(X, Y, Z)  {magADC[ROLL]  = Z; magADC[PITCH]  = X; magADC[YAW]  = Y;}




#define BMA180_ADDRESS 0x80
#define ITG3200_ADDRESS 0XD0
#define ITG3200_SMPLRT_DIV 0
#define ITG3200_DLPF_CFG   2

uint8_t rawADC[6];



// ************************************************************************************************************
// I2C general functions
// ************************************************************************************************************

// Mask prescaler bits : only 5 bits of TWSR defines the status of each I2C request
#define TW_STATUS_MASK	(1<<TWS7) | (1<<TWS6) | (1<<TWS5) | (1<<TWS4) | (1<<TWS3)
#define TW_STATUS       (TWSR & TW_STATUS_MASK)

void i2c_init(void)
{
    TWSR = 0;        // no prescaler => prescaler = 1
    TWBR = ((16000000L / 100000L) - 16) / 2; // change the I2C clock rate
    TWCR = 1<<TWEN;  // enable twi module, no interrupt
}

void i2c_rep_start(uint8_t address)
{
    TWCR = (1<<TWINT) | (1<<TWSTA) | (1<<TWEN) | (1<<TWSTO); // send REPEAT START condition
    waitTransmissionI2C(); // wait until transmission completed
    checkStatusI2C(); // check value of TWI Status Register
    TWDR = address; // send device address
    TWCR = (1<<TWINT) | (1<<TWEN);
    waitTransmissionI2C(); // wail until transmission completed
    checkStatusI2C(); // check value of TWI Status Register
}

void i2c_stop(void)
{
    TWCR = (1 << TWINT) | (1 << TWEN) | (1 << TWSTO);
    waitTransmissionI2C();
    checkStatusI2C();
}


void i2c_write(uint8_t data )
{
    TWDR = data; // send data to the previously addressed device
    TWCR = (1<<TWINT) | (1<<TWEN);
    waitTransmissionI2C(); // wait until transmission completed
    checkStatusI2C(); // check value of TWI Status Register
}

uint8_t i2c_readAck()
{
    TWCR = (1<<TWINT) | (1<<TWEN) | (1<<TWEA);
    waitTransmissionI2C();
    return TWDR;
}

uint8_t i2c_readNak(void)
{
    TWCR = (1<<TWINT) | (1<<TWEN);
    waitTransmissionI2C();
    return TWDR;
}

void waitTransmissionI2C()
{
    uint16_t count = 255;
    while (!(TWCR & (1<<TWINT)))
    {
        count--;
        if (count==0)   //we are in a blocking state => we don't insist
        {
            TWCR = 0;  //and we force a reset on TWINT register
            break;
        }
    }
}

void checkStatusI2C()
{
    if ( TW_STATUS  == 0xF8)   //TW_NO_INFO : this I2C error status indicates a wrong I2C communication.
    {
        TWCR = 0;
        delay(10);
    }
}


void i2c_getSixRawADC(uint8_t add, uint8_t reg)
{
    i2c_rep_start(add); //дI2C��ַ
    i2c_write(reg);         // Start multiple read at the reg register
    i2c_rep_start(add +1);  // I2C read direction => I2C address + 1
    for(uint8_t i = 0; i < 5; i++)
        rawADC[i]=i2c_readAck();
    rawADC[5]= i2c_readNak();
}

void i2c_writeReg(uint8_t add, uint8_t reg, uint8_t val)
{
    i2c_rep_start(add+0);  // I2C write direction
    i2c_write(reg);        // register selection
    i2c_write(val);        // value to write in register
}

uint8_t i2c_readReg(uint8_t add, uint8_t reg)
{
    i2c_rep_start(add+0);  // I2C write direction
    i2c_write(reg);        // register selection
    i2c_rep_start(add+1);  // I2C read direction
    return i2c_readNak();  // Read single register and return value
}

// ****************
// 陀螺的共同部分
// ****************

void GYRO_Common()
{
    static int16_t previousGyroADC[3] = {0,0,0};
    static int32_t g[3];
    uint8_t axis;

    if (calibratingG>0)
    {
        for (axis = 0; axis < 3; axis++)
        {
            // Reset g[axis] at start of calibration
            if (calibratingG == 400) g[axis]=0;
            // Sum up 400 readings
            g[axis] +=gyroADC[axis];
            // Clear global variables for next reading
            gyroADC[axis]=0;
            gyroZero[axis]=0;
            if (calibratingG == 1)
            {
                gyroZero[axis]=g[axis]/400;
            }
        }
        calibratingG--;
    }
    for (axis = 0; axis < 3; axis++)
    {
        gyroADC[axis]  -= gyroZero[axis];
        //anti gyro glitch, limit the variation between two consecutive readings
        gyroADC[axis] = constrain(gyroADC[axis],previousGyroADC[axis]-800,previousGyroADC[axis]+800);
        previousGyroADC[axis] = gyroADC[axis];
    }
}

// ****************
// ACC的共同部分
// ****************

void ACC_Common()
{
    static int32_t a[3];

    if (calibratingA>0)
    {
        for (uint8_t axis = 0; axis < 3; axis++)
        {
            if (calibratingA == 400) a[axis]=0;
            // Sum up 400 readings
            a[axis] +=accADC[axis];
            accADC[axis]=0;
            accZero[axis]=0;
        }
        // Calculate average, shift Z down by acc_1G and store values in EEPROM at end of calibration
        if (calibratingA == 1)
        {
            accZero[ROLL]  = a[ROLL]/400;
            accZero[PITCH] = a[PITCH]/400;
            accZero[YAW]   = a[YAW]/400-acc_1G; // for nunchuk 200=1G
            accTrim[ROLL]   = 0;
            accTrim[PITCH]  = 0;
            writeParams();//ACC校准值写入EEPROM
        }
        calibratingA--;
    }
    accADC[ROLL]  -=  accZero[ROLL] ;
    accADC[PITCH] -=  accZero[PITCH];
    accADC[YAW]   -=  accZero[YAW] ;
}

// ************************************************************************************************************
// I2C Accelerometer BMA180
// ************************************************************************************************************

void ACC_init ()
{
    delay(10);
    //default range 2G: 1G = 4096 unit.
    i2c_writeReg(BMA180_ADDRESS,0x0D,1<<4); // register: ctrl_reg0  -- value: set bit ee_w to 1 to enable writing
    delay(5);
    uint8_t control = i2c_readReg(BMA180_ADDRESS, 0x20);
    control = control & 0x0F; // register: bw_tcs reg: bits 4-7 to set bw -- value: set low pass filter to 10Hz (bits value = 0000xxxx)
    control = control | 0x00;
    i2c_writeReg(BMA180_ADDRESS, 0x20, control);
    delay(5);
    control = i2c_readReg(BMA180_ADDRESS, 0x30);
    control = control & 0xFC;
    control = control | 0x02;
    i2c_writeReg(BMA180_ADDRESS, 0x30, control);
    delay(5);
    acc_1G = 512;
}
/*
注意：屏蔽掉 ACC_getADC ()函数，串口输出值X,Y,MAG全部为0，这说明
这个函数与angle[0]，angle[1]，heading全局变量有关系,这个函数在
IMU.pde文件直接调用
*/

void ACC_getADC ()
{
    TWBR = ((16000000L / 400000L) - 16) / 2;  // Optional line.  Sensor is good for it in the spec.
    i2c_getSixRawADC(BMA180_ADDRESS,0x02);
    //usefull info is on the 14 bits  [2-15] bits  /4 => [0-13] bits  /8 => 11 bit resolution
    ACC_ORIENTATION(  - ((rawADC[1]<<8) | rawADC[0])/32 ,
                      - ((rawADC[3]<<8) | rawADC[2])/32 ,
                      ((rawADC[5]<<8) | rawADC[4])/32 );
    ACC_Common();
}



// ************************************************************************************************************
// I2C陀螺ITG3200
// ************************************************************************************************************

void Gyro_init()
{
    delay(100);
    i2c_writeReg(ITG3200_ADDRESS, 0x3E, 0x80); //register: Power Management  --  value: reset device
    delay(5);
    i2c_writeReg(ITG3200_ADDRESS, 0x16, 0x18 + ITG3200_DLPF_CFG); //register: DLPF_CFG - low pass filter configuration
    delay(5);
    i2c_writeReg(ITG3200_ADDRESS, 0x3E, 0x03); //register: Power Management  --  value: PLL with Z Gyro reference
    delay(100);
}

void Gyro_getADC ()
{
    TWBR = ((16000000L / 400000L) - 16) / 2; // change the I2C clock rate to 400kHz
    i2c_getSixRawADC(ITG3200_ADDRESS,0X1D);
    GYRO_ORIENTATION(  + ( ((rawADC[2]<<8) | rawADC[3])/4) , // range: +/- 8192; +/- 2000 deg/sec
                       - ( ((rawADC[0]<<8) | rawADC[1])/4 ) ,
                       - ( ((rawADC[4]<<8) | rawADC[5])/4 ) );
    GYRO_Common();
}




// ************************************************************************************************************
//I2C罗盘常用的功能
// ************************************************************************************************************
void Mag_getADC()
{
    static uint32_t t,tCal = 0;
    static int16_t magZeroTempMin[3];
    static int16_t magZeroTempMax[3];
    uint8_t axis;
    if ( currentTime < t ) return; //每次读取间隔100毫秒
    t = currentTime + 100000;
    TWBR = ((16000000L / 400000L) - 16) / 2; // I2C时钟速度改变至400kHz
    Device_Mag_getADC();
    if (calibratingM == 1)
    {
        tCal = t;
        for(axis=0; axis<3; axis++)
        {
            magZero[axis] = 0;
            magZeroTempMin[axis] = 0;
            magZeroTempMax[axis] = 0;
        }
        calibratingM = 0;
    }
    magADC[ROLL]  -= magZero[ROLL];
    magADC[PITCH] -= magZero[PITCH];
    magADC[YAW]   -= magZero[YAW];
    if (tCal != 0)
    {
        if ((t - tCal) < 30000000)   //罗盘执行30S校准
        {
            for(axis=0; axis<3; axis++)
            {
                if (magADC[axis] < magZeroTempMin[axis]) magZeroTempMin[axis] = magADC[axis];
                if (magADC[axis] > magZeroTempMax[axis]) magZeroTempMax[axis] = magADC[axis];
            }
        }
        else
        {
            tCal = 0;
            for(axis=0; axis<3; axis++)
                magZero[axis] = (magZeroTempMin[axis] + magZeroTempMax[axis])/2;
            writeParams();
        }
    }
}


// ************************************************************************************************************
// I2C Compass HMC5883
// ************************************************************************************************************
// I2C adress: 0x3C (8bit)   0x1E (7bit)
// ************************************************************************************************************
void Mag_init()
{
    delay(100);
    i2c_writeReg(0X3C ,0x01 ,0x40 );
    i2c_writeReg(0X3C ,0x02 ,0x00 ); //register: Mode register  --  value: Continuous-Conversion Mode
}

void Device_Mag_getADC()
{
    i2c_getSixRawADC(0X3C,0X03);

    MAG_ORIENTATION( ((rawADC[4]<<8) | rawADC[5]) ,
                     -((rawADC[0]<<8) | rawADC[1]) ,
                     -((rawADC[2]<<8) | rawADC[3]) );
}

void initSensors()
{
    delay(100);
    i2c_init();
    delay(100);
    Gyro_init();
    ACC_init();
    acc_25deg = acc_1G * 0.423;
    Mag_init();
}
