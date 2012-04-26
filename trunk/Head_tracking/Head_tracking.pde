/*********** RC alias *****************/
#define ROLL       0
#define PITCH      1
#define YAW        2

static uint32_t current_ms = 0;
Accelerometer acc;
Gyro gyro;
Compass mag;
AHRS ahrs;
//Keyboard keyboard;
Menu menu;
PPM ppm;

//static uint16_t acc_1G;             // 1G测量的加速度
//static int16_t  acc_25deg;

//static int16_t  gyroADC[3],accADC[3],magADC[3];
//static int16_t  accSmooth[3];       // 平滑和规格化的引力力矢量X / Y / Z轴的投影，作为测量加速度
//static int16_t  accTrim[2] = {0, 0};
//static int16_t  heading;            //罗盘液晶显示值0-180,-180-0
//static int16_t  Compass;            //罗盘实际值0-1800,-1800-0

//static uint8_t KeyEnable = 1;           //按键使能标志
//static uint8_t Page = 1;                //界面标志
//static uint8_t Interface = 1;           //界面位置
//static uint8_t Submenu = 0;             //子菜单标志
//static int16_t  Correct= 0;             //罗盘修正值
//static uint16_t PPM_data[8] = {1120,1120,1120,1120,1120,1120,1120,1120}; //PPM宽度值
//static uint8_t PPM_Connection = 0;      //头追信号连接到输出PPM标志位,0不输出，1输出
//static int8_t PPM_Micro[3] = {0,0,0};   //微调修正量
//static uint8_t Backward[3] = {0,0,0};   //倒向
//static uint8_t Channel[3]  = {0,0,0};   //通道指定
//static uint8_t PPM_ON[2] = {0,0};       //1内部合成PPM信号1:开启，0关闭  2遥控器类型1:Futaba，0:JR
//static uint8_t PPM_Order = 0;           //PPM值输出顺序
//static uint8_t PPM_Out_Order = 0;       //PPM输入顺序
//static uint16_t PPM_Meter = 0;          //PPM信号输出累计长度
//static uint16_t PPM_Read1 = 0;           //输入信号宽度值量1
//static uint16_t PPM_Read2 = 0;           //输入信号宽度值量1


// **************
// gyro+acc IMU
// **************
//static int16_t gyroData[3] = {0,0,0};
//static int16_t gyroZero[3] = {0,0,0};
//static int16_t accZero[3]  = {0,0,0};
//static int16_t magZero[3]  = {0,0,0};
//static int16_t angle[2]    = {0,0};  // 在0.1度的180度的倍数= 1800绝对角度倾斜
//static int8_t  smallAngle25 = 1;

//static int8_t ab = 1;

void annexCode()   //这个代码是excetuted在每个循环中，不会与控制回路的干扰，如果持续时间小于650微秒
{
    static uint32_t serialTime;
//////////COM打印调用时间
    if (currentTime > serialTime)   // 50Hz
    {
        serialCom();
        
        serialTime = currentTime + 20000;
    }
}

//液晶欧拉角刷新显示，100mS刷新一次
/*
void lcdBrush()
{
    static uint32_t Brush;
    if(System_ms > Brush)
    {
        if(Page == 1)
        {
           if(Interface == PITCH)
               lcd_Fraction();
        }
        Brush = System_ms + 100;
    }
}*/

void setup()
{
		menu.init();
		ppm.init();
		Serial.begin(115200);             //设置COM波特率
		
		ahrs.init();
		
		menu.printf("Booting....");
		while (!gyro.ready());
}

void loop ()
{
		current_ms = millis();
		ahrs.update();
		ppm.update();
		menu.update();
}
