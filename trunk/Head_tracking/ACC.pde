#define BMA180_ADDRESS 0x80

class Accelerometer
{
public:
		void init()
		void update();
		void calibrate();
		
public:
		
		int16_t data[3];
		
private:
		int16_t zero[3];
		int8_t calibrating;				
};

void Accelerometer::init()
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

void Accelerometer::calibrate()
{
		calibrating = 400;
}

void Accelerometer::update()
{
		TWBR = ((16000000L / 400000L) - 16) / 2;  // Optional line.  Sensor is good for it in the spec.
    i2c_getSixRawADC(BMA180_ADDRESS,0x02);
    //usefull info is on the 14 bits  [2-15] bits  /4 => [0-13] bits  /8 => 11 bit resolution
    ACC_ORIENTATION(  - ((rawADC[1]<<8) | rawADC[0])/32 ,
                      - ((rawADC[3]<<8) | rawADC[2])/32 ,
                      ((rawADC[5]<<8) | rawADC[4])/32 );
    static int32_t a[3];

    if (calibrating>0)
    {
        for (uint8_t axis = 0; axis < 3; axis++)
        {
            if (calibrating == 400) a[axis]=0;
            // Sum up 400 readings
            a[axis] +=data[axis];
            data[axis]=0;
            zero[axis]=0;
        }
        // Calculate average, shift Z down by acc_1G and store values in EEPROM at end of calibration
        if (calibrating == 1)
        {
            zero[ROLL]  = a[ROLL]/400;
            zero[PITCH] = a[PITCH]/400;
            zero[YAW]   = a[YAW]/400-acc_1G; // for nunchuk 200=1G
            accTrim[ROLL]   = 0;
            accTrim[PITCH]  = 0;
            writeParams();//ACC校准值写入EEPROM
        }
        calibrating--;
    }
    data[ROLL]  -=  zero[ROLL] ;
    data[PITCH] -=  zero[PITCH];
    data[YAW]   -=  zero[YAW] ;
}