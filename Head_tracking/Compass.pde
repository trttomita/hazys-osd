class Compass
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

void Compass::init()
{
		delay(100);
    i2c_writeReg(0X3C ,0x01 ,0x40 );
    i2c_writeReg(0X3C ,0x02 ,0x00 ); //register: Mode register  --  value: Continuous-Conversion Mode
}

void Compass::calibrate()
{
		calibrating = 1;
}

void Compass::update()
{
		static uint32_t t,tCal = 0;
    static int16_t magZeroTempMin[3];
    static int16_t magZeroTempMax[3];
    uint8_t axis;
    if ( currentTime < t ) return; //每次读取间隔100毫秒
    t = currentTime + 100000;
    TWBR = ((16000000L / 400000L) - 16) / 2; // I2C时钟速度改变至400kHz
    
    i2c_getSixRawADC(0X3C,0X03);

    MAG_ORIENTATION( ((rawADC[4]<<8) | rawADC[5]) ,
                     -((rawADC[0]<<8) | rawADC[1]) ,
                     -((rawADC[2]<<8) | rawADC[3]) );
                     
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