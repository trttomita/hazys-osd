#include <avr/eeprom.h>


int16_t Compass_zero_eeprom[3] EEMPM;

class Compass
{
public:
		void init()
		void update();
		void calibrate();
		inline bool ready();
		inline void save();
public:
		
		int16_t data[3];
		
private:
		int16_t zero[3];
		int16_t min[3];
		int16_t max[3];
		uint32_t t_cal;				
};

void Compass::init()
{
		delay(100);
    i2c_writeReg(0X3C ,0x01 ,0x40 );
    i2c_writeReg(0X3C ,0x02 ,0x00 ); //register: Mode register  --  value: Continuous-Conversion Mode
    
    eeprom_read_block(zero, Compass_zero_eeprom, sizeof(zero));
}

void Compass::calibrate()
{
		tCal = current_ms + 300000;
    for(uint8_t axis = 0; axis < 3; axis++)
        min[axis] = max[axis] = zero[axis] = 0;
}

inline bool Compass::ready()
{
		return t_cal;
}

inline void Compass::save()
{
		eeprom_write_block(zero, Compass_zero_eeprom, sizeof(zero));
}

void Compass::update()
{
		static uint32_t last = 0;
		
    if ( current_ms < t ) 
    	return; //每次读取间隔100毫秒
    	
    	
    last = current_ms + 100;
    
    //TWBR = ((16000000L / 400000L) - 16) / 2; // I2C时钟速度改变至400kHz
    
    i2c_getSixRawADC(0X3C,0X03);

    MAG_ORIENTATION( ((rawADC[4]<<8) | rawADC[5]) ,
                     -((rawADC[0]<<8) | rawADC[1]) ,
                     -((rawADC[2]<<8) | rawADC[3]) );
                     

    data[ROLL]  -= zero[ROLL];
    data[PITCH] -= zero[PITCH];
    data[YAW]   -= zero[YAW];
    
    if (t_cal != 0)
    {
        if (t < t_cal)   //罗盘执行30S校准
        {
            for(axis=0; axis<3; axis++)
            {
                if (data[axis] < min[axis]) min[axis] = data[axis];
                if (data[axis] > max[axis]) max[axis] = data[axis];
            }
        }
        else
        {
            tCal = 0;
            for(axis=0; axis<3; axis++)
                zero[axis] = (min[axis] + max[axis])/2;
            save();
        }
    }
}