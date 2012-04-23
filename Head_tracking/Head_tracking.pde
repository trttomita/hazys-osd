/*********** RC alias *****************/
#define ROLL       0
#define PITCH      1
#define YAW        2
#define THROTTLE   3
#define AUX1       4
#define AUX2       5
#define CAMPITCH   6
#define CAMROLL    7
#define EIGHT      8

#define Micro_Amount 4   //微调量的大小
#define Micro_Large 120  //微调量最大值
#define Micro_Small -120 //微调量最小值
#define Comp_Large 550   //
#define Comp_Small -550  //
#define PPM_Neutral 1120 //PPM信号中立点
#define PPM_Constant 400  //每通道信号不变宽度值
#define PPM_Length 22000 //一帧PPM完整信号宽度
#define PPM_Correct_Large 8 //PPM输出可变宽度修正量
#define PPM_Correct_Small 4  //PPM输出不变宽度修正量

#define PPM_Width 3000      //同步头最小宽度


static uint32_t currentTime = 0;   //读系统uS变量
static uint32_t System_ms = 0;     //读系统mS变量
static uint16_t cycleTime = 0;     // 这是在微第二，实现了全循环的数量，它可以相差一点点，是考虑到在PID回路
static uint16_t calibratingA = 0;  // ACC校准开关，变量=400执行校准
static uint8_t  calibratingM = 0;  // 磁定向校准开关，变量＝1执行磁校准
static uint16_t calibratingG;      // 陀螺校准执行开关，变量＝400执行陀螺校准
static uint16_t acc_1G;             // 1G测量的加速度
static int16_t  acc_25deg;

static int16_t  gyroADC[3],accADC[3],magADC[3];
static int16_t  accSmooth[3];       // 平滑和规格化的引力力矢量X / Y / Z轴的投影，作为测量加速度
static int16_t  accTrim[2] = {0, 0};
static int16_t  heading;            //罗盘液晶显示值0-180,-180-0
static int16_t  Compass;            //罗盘实际值0-1800,-1800-0

static uint8_t KeyEnable = 1;           //按键使能标志
static uint8_t Page = 1;                //界面标志
static uint8_t Interface = 1;           //界面位置
static uint8_t Submenu = 0;             //子菜单标志
static int16_t  Correct= 0;             //罗盘修正值
static uint16_t PPM_data[8] = {1120,1120,1120,1120,1120,1120,1120,1120}; //PPM宽度值
static uint8_t PPM_Connection = 0;      //头追信号连接到输出PPM标志位,0不输出，1输出
static int8_t PPM_Micro[3] = {0,0,0};   //微调修正量
static uint8_t Backward[3] = {0,0,0};   //倒向
static uint8_t Channel[3]  = {0,0,0};   //通道指定
static uint8_t PPM_ON[2] = {0,0};       //1内部合成PPM信号1:开启，0关闭  2遥控器类型1:Futaba，0:JR
static uint8_t PPM_Order = 0;           //PPM值输出顺序
static uint8_t PPM_Out_Order = 0;       //PPM输入顺序
static uint16_t PPM_Meter = 0;          //PPM信号输出累计长度
static uint16_t PPM_Read1 = 0;           //输入信号宽度值量1
static uint16_t PPM_Read2 = 0;           //输入信号宽度值量1


// **************
// gyro+acc IMU
// **************
static int16_t gyroData[3] = {0,0,0};
static int16_t gyroZero[3] = {0,0,0};
static int16_t accZero[3]  = {0,0,0};
static int16_t magZero[3]  = {0,0,0};
static int16_t angle[2]    = {0,0};  // 在0.1度的180度的倍数= 1800绝对角度倾斜
static int8_t  smallAngle25 = 1;

static int8_t ab = 1;

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
}

void setup()
{
    lcd_init();                        //液晶初始化
    lcd_Interface(Interface);          //显示界面
    Key_init();                        //按键初始化
    Serial.begin(115200);             //设置COM波特率
    initSensors();                     //I2C初始化
    readEEPROM();                      //装载EEPOM值
    Interrupt_init();                  //中断
    calibratingG = 400;                //上电执行一次陀螺校准   
}

void loop ()
{
    Mag_getADC();                     //融合后的数据输出，屏蔽无X,Y,罗盘数据输出
    computeIMU();                     //IMU辅助过滤,屏蔽有X和Y输出，无罗盘数据
    currentTime = micros();           //读系统运行了多少uS
    System_ms = millis();             //读系统运行了多少mS
    annexCode();                      //周期调用COM打印函数
    lcdBrush();
    Mag_getADC();
    PPM_Transfer();                    //数据合到PPM通道
    if(KeyEnable)                     //罗盘校准关闭按键检测
         Key_Value();
    else lcd_Mag();                   //罗盘校准画面
    
    
}
