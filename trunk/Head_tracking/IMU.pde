void computeIMU ()
{
    uint8_t axis;
    static int16_t gyroADCprevious[3] = {0,0,0};
    int16_t gyroADCp[3];
    int16_t gyroADCinter[3];
    static int16_t lastAccADC[3] = {0,0,0};
    static uint32_t timeInterleave = 0;
    static int16_t gyroYawSmooth = 0;
    ACC_getADC();
    Gyro_getADC();
    getEstimatedAttitude();
    for (axis = 0; axis < 3; axis++)
        gyroADCp[axis] =  gyroADC[axis];
    timeInterleave=micros();
    while((micros()-timeInterleave)<650) ; //empirical, interleaving delay between 2 consecutive reads
    for (axis = 0; axis < 3; axis++)
    {
        gyroADCinter[axis] =  gyroADC[axis]+gyroADCp[axis];
        gyroData[axis] = (gyroADCinter[axis]+gyroADCprevious[axis]+1)/3;
        gyroADCprevious[axis] = gyroADCinter[axis]/2;
    }
}

// 简化的IMU的基于“辅助过滤”

//******  高级用户设置 *******************
/* 设置低通滤波器系数为ACC */
/* 增加此值会减少ACC噪声（在GUI可见），但会增加ACC滞后时间*/
/* 注释掉这，如果你不想过滤器，在所有.*/
/* WMC的默认值：8*/
#define ACC_LPF_FACTOR 8

/* 陀螺仪/ ACC互补过滤设置陀螺重量 */
/* 增加此值会减少和延缓滤波器的输出累积的影响*/
/* WMC的默认值：300*/
#define GYR_CMPF_FACTOR 310.0f
//#define GYR_CMPF_FACTOR 0

/* 陀螺/磁强计的互补滤波器设置陀螺仪重量 */
/* 增加此值会减少和延缓滤波器的输出磁强计的影响*/
/* WMC的默认值：N / A*/
#define GYR_CMPFM_FACTOR 200.0f


//****** 高级用户设置结束 *************

#define INV_GYR_CMPF_FACTOR   (1.0f / (GYR_CMPF_FACTOR  + 1.0f))
#define INV_GYR_CMPFM_FACTOR  (1.0f / (GYR_CMPFM_FACTOR + 1.0f))

#define GYRO_SCALE ((2380 * PI)/((32767.0f / 4.0f ) * 180.0f * 1000000.0f)) //should be 2279.44 but 2380 gives better result


typedef struct fp_vector
{
    float X;
    float Y;
    float Z;
} t_fp_vector_def;

typedef union
{
    float   A[3];
    t_fp_vector_def V;
} t_fp_vector;

int16_t _atan2(float y, float x)
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
    z *= (180.0f / PI * 10);
    return z;
}
// 旋转小角度近似估计向量（S），根据陀螺仪的数据
void rotateV(struct fp_vector *v,float* delta)
{
    fp_vector v_tmp = *v;
    v->Z -= delta[ROLL]  * v_tmp.X + delta[PITCH] * v_tmp.Y;
    v->X += delta[ROLL]  * v_tmp.Z - delta[YAW]   * v_tmp.Y;
    v->Y += delta[PITCH] * v_tmp.Z + delta[YAW]   * v_tmp.X;
}

void getEstimatedAttitude()
{
    uint8_t axis;
    int16_t accMag = 0;
    static t_fp_vector EstG,EstM;
    static int16_t mgSmooth[3],accTemp[3];  //平滑和标准化X / Y / Z轴的磁矢量的投影，通过磁力仪测量
    static uint16_t previousT;
    uint16_t currentT = micros();
    float scale, deltaGyroAngle[3];

    scale = (currentT - previousT) * GYRO_SCALE;
    previousT = currentT;
    // 初始化
    for (axis = 0; axis < 3; axis++)
    {
        deltaGyroAngle[axis] = gyroADC[axis]  * scale;

        accTemp[axis] = (accTemp[axis] - (accTemp[axis] >>4)) + accADC[axis];
        accSmooth[axis] = accTemp[axis]>>4;
#define ACC_VALUE accSmooth[axis]

        accMag += (ACC_VALUE * 10 / (int16_t)acc_1G) * (ACC_VALUE * 10 / (int16_t)acc_1G);

#if defined(MG_LPF_FACTOR)
        mgSmooth[axis] = (mgSmooth[axis] * (MG_LPF_FACTOR - 1) + magADC[axis]) / MG_LPF_FACTOR; // LPF for Magnetometer values
#define MAG_VALUE mgSmooth[axis]
#else
#define MAG_VALUE magADC[axis]
#endif
    }
    rotateV(&EstG.V,deltaGyroAngle);
    rotateV(&EstM.V,deltaGyroAngle);
    if ( ( 36 < accMag && accMag < 196 ) || smallAngle25 )
        for (axis = 0; axis < 3; axis++)
        {
            int16_t acc = ACC_VALUE;
#if not defined(TRUSTED_ACCZ)
            if (smallAngle25 && axis == YAW)
                acc = acc_1G;
#endif
            EstG.A[axis] = (EstG.A[axis] * GYR_CMPF_FACTOR + acc) * INV_GYR_CMPF_FACTOR;
        }
    for (axis = 0; axis < 3; axis++)
        EstM.A[axis] = (EstM.A[axis] * GYR_CMPFM_FACTOR  + MAG_VALUE) * INV_GYR_CMPFM_FACTOR;
    angle[ROLL]  =  _atan2(EstG.V.X , EstG.V.Z) ;                                                //屏蔽此语句，无Y轴数据输出
    angle[PITCH] =  _atan2(EstG.V.Y , EstG.V.Z) ;                                                //屏蔽此语句，无X轴数据输出
    Compass = _atan2( EstG.V.X * EstM.V.Z - EstG.V.Z * EstM.V.X , EstG.V.Z * EstM.V.Y - EstG.V.Y * EstM.V.Z  );   //屏蔽此for语句，无罗盘度数数据输出
    heading = Compass / 10;    
}


