#include <avr/io.h>

#define Key_Data (PINC&0x07) //判断键值

#define In    5  //中按键
#define Under 3  //下按键
#define On    6 //上按键


enum KEY {KEY_NONE = 0, KEY_UP = 1, KEY_ENTER = 2, KEY_DOWN = 4};

class Keyboard
{
public:
		void init();
		void update();
		uint8_t get_key();
		
private:
		uint8_t key;
		int8_t state;
		uint32_t time;
}

void Keyboard::init()
{
		DDRC&= 0XF8;
    PORTC|= 0X07;     //设置按键端口输入上拉  
}

void Keyboard::update()
{
		static uint8_t last_key = 0;
		if (System_ms < time)
			return;
		
		time = System_ms + 20;
		uint8_t cur_key = PINC & 0x07;
		switch (state)
		{
				case 0:	//初始
					if (cur_key)
					{
							last_key = cur_key;
							state++;
					}		
					break;
				case 1: //有按键
					if (last_key == cur_key)
					{
							key = cur_key;
							state++;
					}
					else
					{
							state = 0;
					}
					break;
				case 2: //
					if (cur_key != _key)
					{
							state = 0;
					}
					break;
		}	
}

uint8_t Keyboard::get_key()
{
		uint8_t k = 0;
		if (state == 2)
		{
				key;
				key = 0;
		}
		return k;
}











static uint8_t Key_1=1,Key_2=0,Key_3=1;


//主菜单钳位1-6
void Key_Interface()
{
    if(Interface>7) Interface = 1;
    if(Interface<1) Interface = 7;
    lcd_Interface(Interface);
}

//子菜单设置完成
void Interface_off()
{
    led_read_Instruction(0x0c); //关闭光标闪烁
    Page = PITCH;               //开启总翻页标志
    Submenu = 0;                //子菜单标志清0
    writeParams();              //保存到EEPROM
}

//光标闪烁
void Flicker(uint8_t x)
{
    led_read_Instruction(0x80+0x3f+x);
    led_read_Instruction(0x0d);
    Submenu ++;
}

//第2界面动作
void Key_Two(uint8_t Key_Value,int8_t *Amount,uint8_t X,uint8_t Cursor)
{
   if(Key_Value == Under)    //+
   {
       if(*Amount<Micro_Large)
           *Amount +=  Micro_Amount;
   }
   else if(Key_Value == On)//-
   { 
       if(*Amount>Micro_Small)
           *Amount -=  Micro_Amount; 
   }
   lcd_Fine_tuning(X,4,Amount);        //刷新显示
   led_read_Instruction(0x80+0x40+Cursor);//光标复位
   led_read_Instruction(0x0d); 
}

//第3和5界面动作
void Key_Three(uint8_t Key_Value,uint8_t *Amount,uint8_t Surface,uint8_t X,uint8_t Cursor)
{
   if(Key_Value == Under)    //+
   {
       if(*Amount<PITCH)
           *Amount =  PITCH;
       else *Amount =  ROLL;
   }
   else if(Key_Value == On)//-
   { 
       if(*Amount<PITCH)
           *Amount = PITCH;
       else *Amount = ROLL; 
   }
   lcd_Backward(X,Amount,Surface);             //刷新显示 
   led_read_Instruction(0x80+0x40+Cursor);       //光标复位
   led_read_Instruction(0x0d);
}

//第4界面动作
void Key_Four(uint8_t Key_Value,uint8_t *Amount,uint8_t X,uint8_t Cursor)
{
  if(Key_Value == Under)    //+           将通道钳位在5－8和0，0表示屏蔽
   {
       if(*Amount == ROLL)
       {
            *Amount = AUX1;
            PPM_data[*Amount - PITCH] = PPM_Neutral;  //通道归中立点
       }
       if(*Amount<EIGHT)
       {
           *Amount +=  PITCH;
           PPM_data[*Amount - YAW] = PPM_Neutral;  //通道归中立点
       }
   }
   else if(Key_Value == On)//-
   { 
       if(*Amount>AUX1)
       {
          *Amount -= PITCH;
          PPM_data[*Amount] = PPM_Neutral;   //通道归中立点
       }
       if(*Amount == AUX1) //关闭通道输出
       {
          *Amount = ROLL;
          PPM_data[AUX1] = PPM_Neutral;  //将上5通道归中立点
       }
   }
   lcd_Channel(X,Amount);                    //刷新显示 
   led_read_Instruction(0x80+0x40+Cursor);   //光标复位
   led_read_Instruction(0x0d);
}

void Key_Value()
{
    static uint32_t i,t;
    static uint8_t s;    
    if(Key_2==0)
    {
        if(Key_Data == 0X07) return;  //无按键按下跳出
        if(Key_1)
        {
            i = System_ms + 20;       //20mS延时消抖
            s = Key_Data;             //读第一次键值
            Key_1 = 0;
            return;
        }
        else if(Key_1 == 0)
               {
                    t = System_ms;
                    if(t > i)
                    {
                         if(s == Key_Data)     //第二次读键值
                         {
                             if(s == In)      //确定键按下,关闭总翻页标志
                             {
                                Page = YAW;
                                Submenu ++;
                             }  
                             if(Page == YAW)
                             {
                                 if(Interface == PITCH)        ///////////////////////////////////第1界面执行动作
                                 {
                                       calibratingA=400;       //ACC以当前角度清0
                                       Correct = Compass;      //记录罗盘修正值
                                       PPM_Connection = 1;     //头追信号连接到输出PPM
                                       Page = PITCH;           //开启总翻页标志
                                       Submenu = 0;            //子菜单标志清0 
                                 }
                                 else if(Interface == YAW)     //////////////////////////////////第2界面执行动作
                                 {       
                                   if(Submenu == PITCH)        //X光标闪烁
                                        Flicker(0);
                                   else if(Submenu == YAW) //X动作
                                        Key_Two(s,PPM_Micro,1,0-1);
                                   else if(Submenu == THROTTLE) //Y光标闪烁
                                        Flicker(6);
                                   else if(Submenu == AUX1) //Y动作
                                        Key_Two(s,PPM_Micro+1,6,5);
                                   else if(Submenu == AUX2) //Z光标闪烁
                                        Flicker(11);
                                   else if(Submenu == CAMPITCH)//Z动作
                                        Key_Two(s,PPM_Micro+2,11,10);
                                   else if(Submenu == CAMROLL)//界面2设置完成
                                        Interface_off();
                                 }
                                 else if(Interface == THROTTLE) //////////////////////////////////////////////第3界面执行动作
                                 {
                                      if(Submenu == PITCH)        //X光标闪烁
                                           Flicker(3);
                                      else if(Submenu == YAW) //X动作
                                           Key_Three(s,Backward,3,2,2);
                                      else if(Submenu == THROTTLE) //Y光标闪烁
                                           Flicker(9);
                                      else if(Submenu == AUX1) //Y动作
                                           Key_Three(s,Backward+1,3,8,8);
                                      else if(Submenu == AUX2) //Z光标闪烁
                                           Flicker(15);
                                      else if(Submenu == CAMPITCH) //Y动作
                                           Key_Three(s,Backward+2,3,14,14);
                                      else if(Submenu == CAMROLL)//界面3设置完成
                                           Interface_off();
                                 }
                                 else if(Interface == AUX1)  //////////////////////////////////////////////////第4界面执行动作
                                 {
                                       if(Submenu == PITCH)        //X光标闪烁
                                           Flicker(3);
                                       else if(Submenu == YAW) //X动作
                                           Key_Four(s,Channel,2,2);
                                       else if(Submenu == THROTTLE) //Y光标闪烁
                                           Flicker(9);
                                       else if(Submenu == AUX1) //Y动作
                                           Key_Four(s,Channel+1,8,8);
                                       else if(Submenu == AUX2) //Z光标闪烁
                                           Flicker(15);
                                       else if(Submenu == CAMPITCH) //Y动作
                                           Key_Four(s,Channel+2,14,14);
                                       else if(Submenu == CAMROLL)//界面4设置完成
                                           Interface_off();    
                                 }
                                 else if(Interface == AUX2)     //////////////////////////////////////////////第5界面执行动作
                                 {
                                      if(Submenu == PITCH)        //X光标闪烁
                                           Flicker(12);
                                      else if(Submenu == YAW)         
                                           Key_Three(s,PPM_ON,5,10,11);
                                      else if(Submenu == THROTTLE)//界面5设置完成
                                           Interface_off();             
                                 }
                                 else if(Interface == CAMPITCH)///////////////////////////////////////////////第6界面执行动作
                                 {
                                     KeyEnable = 0;            //关闭按键功能，防止校准过程中按键按下
                                     calibratingM=1;           //执行罗盘校准
                                     Page = PITCH;             //开启总翻页标志                                     
                                 }
                                 else if(Interface == CAMROLL)///////////////////////////////////////////////第7界面执行动作
                                 {
                                     if(Submenu == PITCH)        //X光标闪烁
                                           Flicker(1);
                                      else if(Submenu == YAW)         
                                           Key_Three(s,PPM_ON+1,7,5,1);
                                      else if(Submenu == THROTTLE)//界面7设置完成
                                           Interface_off();
                                 }                          
                             }  
                             if(Page == PITCH)  //界面翻页
                             {
                                  switch(s)
                                  {
                                      case Under: Interface++; Key_Interface();
                                          break;
                                      case On: Interface--; Key_Interface();
                                          break; 
                                  }
                             }
                             Key_2 = 1; 
                         }
                      else Key_1 = 1;     //2次键值不用，视为抖动，重新检测                         
                    }
               }
    }
    if(Key_2)     //松手检测
    {
        if(Key_3)
        {
            if(Key_Data == 0X07)  //说明检测到松手    
            {
                 s = System_ms+20;   //延时20mS
                 Key_3 = 0;
            }
            else  return;
        }
        if(Key_3 == 0)  //延时后在次松手检测
        {
            t = System_ms;
            if(t > s)
            {
                 if(Key_Data == 0X07)
                 {
                      Key_1 = 1;
                      Key_2 = 0;
                      Key_3 = 1;
                 }
                 else  Key_3 = 1;  //2次松手检测值不等，视为抖动，重新松手检测
            }
            else  return;
        }
    }    
}

void Key_init()
{
    DDRC&= 0XF8;
    PORTC|= 0X07;     //设置按键端口输入上拉    
}
