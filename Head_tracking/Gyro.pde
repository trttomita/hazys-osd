#define ITG3200_ADDRESS 0XD0

class Gyro
{
public:
	void init();
	void update();
	void calibrate();
public:
	int16_t data[3];	
private:
	int16_t zero[3];
	int8_t calibrating;	
};

void Gyro::init()
{
		delay(100);
    i2c_writeReg(ITG3200_ADDRESS, 0x3E, 0x80); //register: Power Management  --  value: reset device
    delay(5);
    i2c_writeReg(ITG3200_ADDRESS, 0x16, 0x18 + ITG3200_DLPF_CFG); //register: DLPF_CFG - low pass filter configuration
    delay(5);
    i2c_writeReg(ITG3200_ADDRESS, 0x3E, 0x03); //register: Power Management  --  value: PLL with Z Gyro reference
    delay(100);
}

void Gyro::calibrate()
{
		calibrating = 400;
}

void Gyro::update()
{
		TWBR = ((16000000L / 400000L) - 16) / 2; // change the I2C clock rate to 400kHz
    i2c_getSixRawADC(ITG3200_ADDRESS,0X1D);
    GYRO_ORIENTATION(  + ( ((rawADC[2]<<8) | rawADC[3])/4) , // range: +/- 8192; +/- 2000 deg/sec
                       - ( ((rawADC[0]<<8) | rawADC[1])/4 ) ,
                       - ( ((rawADC[4]<<8) | rawADC[5])/4 ) );
   	
   	static int16_t previousdata[3] = {0,0,0};
    static int32_t g[3];
    uint8_t axis;

    if (calibrating>0)
    {
        for (axis = 0; axis < 3; axis++)
        {
            // Reset g[axis] at start of calibration
            if (calibrating == 400) g[axis]=0;
            // Sum up 400 readings
            g[axis] += data[axis];
            // Clear global variables for next reading
            data[axis]=0;
            zero[axis]=0;
            if (calibrating == 1)
            {
                zero[axis]=g[axis]/400;
            }
        }
        calibrating--;
    }
    for (axis = 0; axis < 3; axis++)
    {
        data[axis]  -= zero[axis];
        //anti gyro glitch, limit the variation between two consecutive readings
        data[axis] = constrain(data[axis],previousdata[axis]-800,previousdata[axis]+800);
        previousdata[axis] = data[axis];
    }
}