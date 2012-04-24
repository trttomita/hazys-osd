#include <avr/io.h>
/***********************************************************************
宏定义:定义各功能引脚名称
说明  :RS_H,RS_L    数据/命令选择端 (H/L)
       RW_H,RW_L    读/写选择端 (H/L)
       E_H,E_L      使能端
       Data(data_a) 并行数据端
************************************************************************/
#define RS_H PORTC	|= (1<<3)
#define RS_L PORTC 	&= ~(1<<3)

#define RW_H PORTD 	|= (1<<4)
#define RW_L PORTD 	&= ~(1<<4)

#define E_H PORTD 	|= (1<<5)
#define E_L PORTD		&= ~(1<<5)

#define Data(data_a) {PORTB = data_a; PORTD|=0XC0; PORTD&= data_a | 0X3F;}

class LCD: public BetterStream
{
public:
		void init();
		void write(const char c);
		void set_pos(uint8_t x, uint8_t y);
		void blink(bool enable = true);
		void blink(uint8_t x, uint8_t y, bool enable = true);
		void clear();
private:
		void write(const char c, bool ins);
private:
		uint8_t pos;
};

void LCD::init()
{
		//确定数据端口方向
    DDRB = 0XFF;
    DDRD|= 0XF0;
    DDRC|= 0X08;
    RW_L;                       //整个液晶操作只写
    write(0x38, true);	//显示模式
    write(0x0c, true);	//设置光标
    write(0x06, true);	//屏幕移动
    write(0x01, true);	//指针清零
    delay(2);
}

void LCD::write(const char c, bool ins)
{
		E_L;
		if (ins)
    	RS_L;
    else
    	RS_H;
    Data(Byte);
    E_H;
    delayMicroseconds(50);
    E_L;
    RS_L;
}

void LCD::write(const uint8_t c)
{
		if (c == '|')
				write(pos|0x40, true);
		else
				write(c, false);
}

void LCD::set_pos(uint8_t x, uint8_t y)
{
		pos = x | 0x80;
		if (y)
				p |= 0x40;
		write(pos, true); 
}

void LCD::blink(bool enable = true)
{
		if (enable)
				write(0x0D, true);
		else
				write(0x0C, true);
}

void LCD::blink(uint8_t x, uint8_t y, bool enable = true)
{
		set_pos(x, y);
		blink(enable);
}

void LCD::clear()
{
		write(0x01, true);
		set_pos(0,0);
}




/*







void lcd_read_Byte(uint8_t Byte)              //写数据
{ 
    uint8_t a;
    E_L;
    RS_H;
    Data(Byte);
    E_H;
    delayMicroseconds(50);
    E_L;
    RS_L;
}

void led_read_Instruction(uint8_t Instruction)   //写指令
{
    uint8_t a;
    E_L;
    RS_L;
    Data(Instruction);
    E_H;
    delayMicroseconds(50);
    E_L;
    RS_H;

}

void lcd_ACSII(char *Code)
{
    while(*Code)
    {
        lcd_read_Byte(*Code);
        Code++;
    }    
}

//第1界面，显示X,Y,Z轴欧拉角数据
void lcd_Panel_1()
{
     led_read_Instruction(0x80);	//第一排
     lcd_ACSII("  Euler angles  ");
     led_read_Instruction(0x80+0x40);    //第二排
     lcd_ACSII("X    Y    Z     ");
}

//第2界面，X,Y,Z舵机微调
void lcd_Panel_2()
{  
    led_read_Instruction(0x80);	//第一排
    lcd_ACSII("Servo FineTuning");
    led_read_Instruction(0x80+0x40);    //第二排
    lcd_ACSII("X    Y    Z     ");
}

//第3界面 ,X,Y,Z信号倒向REV NOR
void lcd_Panel_3()
{
    led_read_Instruction(0x80);	//第一排
    lcd_ACSII("     Reverse    ");
    led_read_Instruction(0x80+0x40);    //第二排
    lcd_ACSII("X=    Y=    Z=  ");
}

//第4界面，X,Y,Z通道指定0－8，0为关闭轴的输出
void lcd_Panel_4()
{
    led_read_Instruction(0x80);	//第一排
    lcd_ACSII("  PPM_ channel  ");
    led_read_Instruction(0x80+0x40);    //第二排
    lcd_ACSII("X=    Y=    Z=   ");
}

//第5界面，开启或关闭合成PPM
void lcd_Panel_5()
{
    led_read_Instruction(0x80);	//第一排
    lcd_ACSII(" ON or OFF(PPM) ");
    led_read_Instruction(0x80+0x40);    //第二排
    lcd_ACSII("    PPM         ");
}

//第6界面罗盘校准
void lcd_Panel_6()
{
    led_read_Instruction(0x80);	//第一排
    lcd_ACSII("MAG__Calibration");
    led_read_Instruction(0x80+0x40);    //第二排
    lcd_ACSII("      Yes?      ");
}

//遥控器类型
void lcd_Panel_7()
{
   led_read_Instruction(0x80);	//第一排
   lcd_ACSII("  Radio System? ");
   led_read_Instruction(0x80+0x40);    //第二排
   lcd_ACSII("                ");
}

//显示与形参相关界面
void lcd_Interface(uint8_t Interface)
{
    switch(Interface)	
    {
       case PITCH:    lcd_Panel_1();	
	    break;			
       case YAW:      lcd_Panel_2(); lcd_Fine_tuning(1,4,PPM_Micro);
                                     lcd_Fine_tuning(6,4,PPM_Micro+1);
                                     lcd_Fine_tuning(11,4,PPM_Micro+2);		
	    break;			
       case THROTTLE: lcd_Panel_3(); lcd_Backward(2,Backward,3);
                                     lcd_Backward(8,Backward+1,3);
                                     lcd_Backward(14,Backward+2,3);		
	    break;
       case AUX1:     lcd_Panel_4(); lcd_Channel(2,Channel);
                                     lcd_Channel(8,Channel+1);
                                     lcd_Channel(14,Channel+2);				
	    break;
       case AUX2:     lcd_Panel_5(); lcd_Backward(10,PPM_ON,5);		
	    break;
       case CAMPITCH: lcd_Panel_6();		
	    break;
       case CAMROLL: lcd_Panel_7(); lcd_Backward(5,PPM_ON+1,7);
            break;
   }
}

//数值分离送显示,X显示开始格数,Position格数,Negative负数符号
void lcd_Separate(uint8_t X,uint8_t Position,uint8_t Negative,uint16_t Score)
{
     uint8_t a;
     led_read_Instruction(0x80+0x40+X);
     for(a=0;a<Position;a++)   //按指定格数清屏
        lcd_read_Byte(' ');
     led_read_Instruction(0x80+0x40+X); 
     
     if(Score>=100)
     {
          if(Negative) lcd_read_Byte('-');
          lcd_read_Byte(Score/100+0x30);         //写百位数
          lcd_read_Byte(Score%100/10+0x30);      //写十位数
          lcd_read_Byte(Score%10+0x30);          //写个位数
     }
     else if(Score>=10)
     {
          if(Negative) lcd_read_Byte('-');
          lcd_read_Byte(Score/10+0x30);         //写十位数
          lcd_read_Byte(Score%10+0x30);         //写个位数
     }
     else
     {
          if(Negative) lcd_read_Byte('-');
          lcd_read_Byte(Score%10+0x30);         //写个位数
     }
}
//界面一X,Y,Z数据显示
void lcd_Fraction()
{
     uint16_t Value;
     uint8_t Mark = 0;
     //X
     if(angle[0]&0X8000)  //如果X值为负数
     {
         Value = ~angle[0]+1; //将X值变成整形值
         Mark = 1;   //负数标志置位
         lcd_Separate(1,4,1,Value/10);       
     }
     else {lcd_Separate(1,4,0,angle[0]/10);Mark = 0;}
     //Y
     if(angle[1]&0x8000)
     {
         Value = ~angle[1]+1; //将Y值变成整形值
         Mark = 1;   //负数标志置位
         lcd_Separate(6,4,1,Value/10);
     }
     else {lcd_Separate(6,4,0,angle[1]/10);Mark = 0;}
     //Z
     if(heading&0x8000)
     {
         Value =180-(~heading+1)+180;  //计算负值实际度数
         lcd_Separate(12,4,0,Value);
     }
     else lcd_Separate(12,4,0,heading);
}

//界面2X,Y,Z微调显示
void lcd_Fine_tuning(uint8_t X,uint8_t Position,int8_t *Micro)
{
     uint8_t Value,Mark = 0;
     if(*Micro & 0X80)
     { 
         Value = ~*Micro+1;
         Mark = 1;
         lcd_Separate(X,Position,1,Value);
     }
     else {lcd_Separate(X,Position,0,*Micro);Mark = 0;}     
}

//3,5,7界面3( X,Y,Z倒向显示),5(开启关闭PPM合成),7(遥控器类型)
void lcd_Backward(uint8_t X,uint8_t *Micro,uint8_t Surface)
{
     if(Surface == THROTTLE)
     {
        led_read_Instruction(0x80+0x40+X);
        if(*Micro)
           lcd_read_Byte('r');
        else  lcd_read_Byte('n');
     }
 else if(Surface == AUX2)
    {
       led_read_Instruction(0x80+0x40+X);
       lcd_ACSII("   ");
       led_read_Instruction(0x80+0x40+X);
       if(*Micro == 1)
         lcd_ACSII("ON");
      else
        lcd_ACSII("OFF");
    } 
    else
    {
       led_read_Instruction(0x80+0x40+X);
      lcd_ACSII("      ");
      led_read_Instruction(0x80+0x40+X);
      if(*Micro == 0)
        lcd_ACSII("  JR  ");
      else
       lcd_ACSII("Futaba");
    }
}

//界面4 X,Y,Z通道指定显示
void lcd_Channel(uint8_t X,uint8_t *Micro)
{
    led_read_Instruction(0x80+0x40+X);
    lcd_ACSII("   ");
    led_read_Instruction(0x80+0x40+X);
    if(*Micro == 0)
        lcd_ACSII("off");
    else
       lcd_read_Byte(*Micro+0X30);
}

//6罗盘校准动画
void lcd_Mag()
{
    static uint32_t t,i,tCal = 0;
    if(System_ms < t) return;            //间隔2秒执行一次
        t = System_ms + 2000;
    if(tCal == 0)
    {
         led_read_Instruction(0x80+0x40+6);
         lcd_ACSII("    ");                   //清第二排显示
         led_read_Instruction(0x80+0x40);
         tCal = t;
    }
    else
    {
         if((t - tCal) < 34000)          //如果不到30秒时间
               lcd_read_Byte('>');         //写一个进度条
         else
         {
               tCal = 0;
               KeyEnable = 1;               //开启按键扫描
               lcd_Panel_1();               //跳到第1初始界面
               Interface = 1;               //界面翻页为1画面
         }
    }       
}
void lcd_init()
{
    //确定数据端口方向
    DDRB = 0XFF;
    DDRD|= 0XF0;
    DDRC|= 0X08;
    RW_L;                       //整个液晶操作只写
    led_read_Instruction(0x38);	//显示模式
    led_read_Instruction(0x0c);	//设置光标
    led_read_Instruction(0x06);	//屏幕移动
    led_read_Instruction(0x01);	//指针清零
    delay(2);
}
*/