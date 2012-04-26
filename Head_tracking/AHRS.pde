#define PI	3.1416f


struct Vector<T>
{
		T data[3];
		
		void route(const Vector<T>& v)
		{
				Vector<T> tmp = *this;
				data[0] += tmp.data[1]*v.data[2] - tmp.data[2]*v.data[1];
				data[1] += tmp.data[2]*v.data[0] - tmp.data[0]*v.data[2];
				data[2] += tmp.data[0]*v.data[1] - tmp.data[1]*v.data[0];
		} 
}



class AHRS
{
public:
		void init();
		void update();
		
		int16_t data[3];

private:
		smooth_data();
		estimate_att();
		static float estimate_heading(float roll, float pitch);
		static float _atan2(float y, float x);
private:
		Vector<float> estG;
		Vector<float> estM;
				
		int16_t acc_smooth[3];
		int16_t mag_smooth[3];
		int16_t gyro_smooth[3];
		
		const float GYRO_ACC_FACTOR = 310f;
		const float GYRO_MAG_FACTOR = 200f;
		
		const float INV_GYRO_ACC_NOR = 1.0f/(1.0f-GYRO_ACC_FACTOR);
		const float INV_MAG_FACTOR = 1.0f/(1.0f-GYRO_MAG_FACTOR);
};

void AHRS::init()
{
		uint8_t axis;
		acc.init();
		gyro.init();
		mag.inti();
		
		gyro.calibrate();
		
		for (axis = 0; axis < 3; axis++)
		{
				acc_smooth[axis] = mag_smooth[axis] = gyro_smooth[axis] = 0;
		}
		
		for (uint8_t i = 0; i < 16; i++)
		{
				acc.update();
				mag.update();
				for (axis = 0; axis < 3; axis++)
				{
					acc_smooth[axis] += acc.data[axis];
					mag_smooth[axis] += mag.data[axis];
				}
		}
		
		for (axis = 0; axis < 3; axis++)
		{
				estG.data[axis] = acc_smooth[axis] = acc_smooth[axis] >> 4;
				estM.data[axis] = mag_smooth[axis] = mag_smooth[axis] >> 4;
		}
}
	
void AHRS::update()
{
		acc.update();
		gyro.update();
		mag.update();
		
		smooth_data();
		estimate_att();
}

void AHRS::smooth_data()
{
		for (uint8_t axis = 0; axis < 3; axis++)
    {
        acc_smooth[axis] = (acc_smooth[axis] - (acc_smooth[axis] >>4)) + acc.data[axis];
				mag_smooth[axis] = (mag_smooth[axis] - (mag_smooth[axis] >>3)) + mag.data[axis];
    }
}

void estimate_att()
{
		uint8_t axis;
		uint16_t acc_sm[3], mag_sm[3];
    Vector<float> dtheta;
    
		static uint16_t prev_time;
    uint16_t curr_time = micros();
    uint16_t dt = curr_time - prev_time;
    prev_time = curr_time;
    
		for (axis = 0; axis < 3; axis++)
		{
				acc_sm[axis] = acc_smooth[axis] >> 4;
				mag_sm[axis] = mag_smooth[axis] >> 3;
				uint16_t ratio = (acc_sm[axis] * 10 / Accelerometer::G);
				accMag += ratio * ratio;
				dtheta.data[axis] = gyro.data[axis]*dt;
		}
		
		estG.rotate(dtheta);
		estM.rotate(dtheat);
		
		 for (axis = 0; axis < 3; axis++)
     {
     		 estG.data[axis] = (estG.data[axis] * GYRO_ACC_FACTOR + acc_ms[axis]) * INV_GYRO_ACC_NOR;
     		 estM.data[axis] = (estM.data[axis] * GYRO_MAG_FACTOR + mag_ms[axis]) * INV_GYRO_MAG_NOR;
     }
     
     float pitch = _atan2(estG.data[0] , estG.data[2]);
     float roll = _atan2(estG.data[1] , estG.data[2]);
     
     data[0] = pitch * 1800.f / PI;
     data[1] = roll * 1800.f / PI;
     //data[0] = _atan2(estG.data[0] , estG.data[2]);    
     //data[1] = _atan2(estG.data[1] , estG.data[2]);    
     //data[2] = _atan2(estG.data[0] * estM.data[2] - estG.data[2]*estM.data[0], 
     //									estG.data[2] * estM.data[1] - estG.data[1]*estM.data[2]);  //屏蔽此for语句，无罗盘度数数据输出
     data[1] = estimate_heading(roll, pitch) * 1800.f / PI;
}


float AHRS::estimate_heading(float roll, float pitch)
{
//  Note - This function implementation is deprecated
//  The alternate implementation of this function using the dcm matrix is preferred
    float headX;
    float headY;
    float cos_roll;
    float sin_roll;
    float cos_pitch;
    float sin_pitch;
    cos_roll = cos(roll);
		sin_roll = sin(roll);
    cos_pitch = cos(pitch);
		sin_pitch = sin(pitch);

    // Tilt compensated magnetic field X component:
    headX = estM.data[0]*cos_pitch + estM.data[1]*sin_roll*sin_pitch + estM.data[2]*cos_roll*sin_pitch;
    // Tilt compensated magnetic field Y component:
    headY = estM.data[1]*cos_roll - estM.data[2]*sin_roll;
    // magnetic heading
    float heading = atan2(-headY, headX);
		
		return heading;
    // Declination correction (if supplied)
    /*if( fabs(_declination) > 0.0 )
    {
        heading = heading + _declination;
        if (heading > M_PI)    // Angle normalization (-180 deg, 180 deg)
            heading -= (2.0 * M_PI);
        else if (heading < -M_PI)
            heading += (2.0 * M_PI);
    }*/

    // Optimization for external DCM use. Calculate normalized components
    //heading_x = cos(heading);
    //heading_y = sin(heading);

}

float AHRS::_atan2(float y, float x)
{
		#define fp_is_neg(val) ((((byte*)&val)[3] & 0x80) != 0)
		
    float z = y / x;
    int16_t zi = abs(int16_t(z * 100));
    int8_t y_neg = fp_is_neg(y);
    if ( zi < 100 )
    {
        if (zi > 10)
            z = z / (1.0f + 0.28f * z * z);
        if (fp_is_neg(x))
        {
            if (y_neg) z -= PI;
            else z += PI;
        }
    }
    else
    {
        z = (PI / 2.0f) - z / (z * z + 0.28f);
        if (y_neg) z -= PI;
    }
    //z *= (180.0f / PI * 10);
    return z;
}