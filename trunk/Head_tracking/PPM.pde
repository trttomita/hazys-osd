#include <avr/interrupt.h>
#include <avr/eeprom.h>

enum RADIO_TYPE {RADIO_JR, RADIO_FUTABA};

struct PPM_setting
{
		int8_t chan[3];
		int8_t trim[3];
		int8_t rev[3];
		int8_t atv[3];
		bool synt;
		bool type;
};

PPM_setting PPM_setting_eeprom EEMEM;

class PPM
{
public:
		void init();
		void update();
		inline void save();
public:
		PPM_setting setting;
private:
		//uint16_t input[8];
		uint16_t output[8];
		

		const uint16_t MAX = 550;
		const uint16_t NEUTRAL = 1120;
		const uint16_t CONSTANT = 400;
		const uint16_t CORR_LARGE = 8; //PPM输出可变宽度修正量
		const uint16_t CORR_SMALL = 4; //PPM输出不变宽度修正量
		const uint16_t FRAME = 22000;
		const int8_t TRIM = 4;
		const int8_t TRIM_MAX = 120;
};

void PPM::init()
{
	 DDRD |= (1<<2);
   DDRD &= ~(1<<3);
   PORTD |=(1<<3);
   
   eeprom_read_block(&setting, &PPM_setting_eeprom, sizeof(setting));
   
   if(setting.type == RADIO_FUTABA)
   {
   	   PPM_H;      //Fuata
   	   EICRA |= (1<<ISC11);    //INT1 引脚下降沿引发中断
   }
   else 
   {
   			PPM_L;    //JR
   			EICRA |= (1<<ISC10);    //INT1 引脚上升沿引发中断
   }		
   
   if (setting.synt)   
   		EIMSK |= (1<<INT1);        //外部中断请求 1 使能
   
   TIMSK1 = 0x01;             //定时器1溢出中断
   TCCR1A = 0x00;
   TCCR1B = 0x02;             //启动定时器*/
}

inline void PPM::save()
{
		eeprom_write_block(&setting, &PPM_setting_eeprom, sizeof(setting));
}

void PPM::update()
{
   for(uint8_t i = 0; i < 3; i++)
   {
     if(setting.chan[i]>0)                     //如果轴有指定PPM通道
     {
     		int16_t ppm = ((uint32_t)ahrs.data[i]) * setting.atv[i] * 550 / 9000;	//atv是终点
				ppm += setting.trim[i];
     		if (ppm > MAX)
     				ppm = MAX;
     		else
     				ppm = -MAX;
     		
     		if (rev[i])
     				ppm = NEUTRAL - ppm;
     		else
     				ppm = NEUTRAL + ppm;	
     				
     		cli();
     		output[setting.chan[i]-1] = ppm;
     		sei();		
     } 
   }
}

#define PPM_H PORTD |= (1<<2)
#define PPM_L PORTD &= ~(1<<2)

static inline void set_ppm(uint16_t value)
{
		uint16_t v = 0xFFFF - value << 1;
		TCNT1H = v >> 8;
		TCNT1L = v;
}
/*
//定时器1初值
#define PPM_Outp(data) {TCNT1H = (0xffff-(data)*2)>>8;TCNT1L=0xffff-(data)*2;}

static uint8_t PPM_Sign = 1;
static uint16_t PPM_Outp[8] = {1120,1120,1120,1120,1120,1120,1120,1120};

void PPM_Refresh()
{
   uint8_t i;
   for(i=0;i<8;i++)
      PPM_Outp[i] = PPM_data[i];
}



//微调量，导向，通道指定传感器数据 4个功能融合到PPM_data相应位置上
void PPM_Transfer()
{
   uint8_t i;
   int16_t y;
   uint16_t z;
   for(i=0;i<3;i++)
   {
     if(Channel[i]>0)                     //如果轴有指定PPM通道
     { 
        if(Backward[i])                   //根据导向设置在中立点上进行＋－修正微调
        {
           PPM_data[Channel[i]-1] = PPM_Neutral + PPM_Micro[i];     //正方向修正微调
           if(PPM_Connection)                                       //传感器信号连接到PPM通道数据
           {  
               if(i < 2)                                            //X,Y轴数据送PPM数组
               {
                   z = PPM_data[Channel[i]-1] + angle[i];
                   if(z> (Comp_Large+PPM_Neutral))                       //高钳位
                      PPM_data[Channel[i]-1] =  Comp_Large+PPM_Neutral;  //超过最大值输出中立点＋最大行程
                   else if(z < (Comp_Small+PPM_Neutral))
                      PPM_data[Channel[i]-1] = Comp_Small+PPM_Neutral;   //低钳位
                   PPM_data[Channel[i]-1] = z;                           //输出实际值
               }
               else if(i==2)                                        //罗盘值
               {
                   y = Compass - Correct;
                   if(y>1800)
                     y = y - 3600;
                   else if(y < -1800)
                     y = y + 3600;
                   z =  PPM_data[Channel[i]-1] + y;
                   if(z > (Comp_Large+PPM_Neutral))                       //高钳位
                        PPM_data[Channel[i]-1] = Comp_Large+PPM_Neutral;
                   else if(z < (Comp_Small+PPM_Neutral))                  //低钳位
                        PPM_data[Channel[i]-1] = Comp_Small+PPM_Neutral;
                   else
                        PPM_data[Channel[i]-1] = z;                       //范围内正常输出             
               }
           }    
        }     
       else
       {
           PPM_data[Channel[i]-1] =PPM_Neutral - PPM_Micro[i];      //逆方向修正微调
           if(PPM_Connection)                                       //传感器信号连接到PPM通道数据
           {  
               if(i < 2)                                            //X,Y轴数据送PPM数组
               {
                   z = PPM_data[Channel[i]-1] + ~angle[i];          //X,Y钳位
                   if(z> (Comp_Large+PPM_Neutral))                       
                      PPM_data[Channel[i]-1] =  Comp_Large+PPM_Neutral;
                   else if(z < (Comp_Small+PPM_Neutral))
                       PPM_data[Channel[i]-1] = Comp_Small+PPM_Neutral;
                   else
                       PPM_data[Channel[i]-1] = z;
               } 
               else if(i==2)                                        //罗盘值
               {
                   y = Compass - Correct;
                   if(y>1800)
                     y = y - 3600;
                   else if(y < -1800)
                     y = y + 3600;
                   z = PPM_data[Channel[i]-1] + ~y;          
                   if(z > (Comp_Large+PPM_Neutral))                       //高钳位
                        PPM_data[Channel[i]-1] = Comp_Large+PPM_Neutral;
                   else if(z < (Comp_Small+PPM_Neutral))                  //低钳位
                        PPM_data[Channel[i]-1] = Comp_Small+PPM_Neutral;
                   else
                        PPM_data[Channel[i]-1] = z;                       //范围内正常输出                       
               }
           }
       }
     } 
   }
}*/

//PPM输出,定时器1中断溢出
//SIGNAL(SIG_OVERFLOW1)
ISR(TIMER1_OVF_vect)
{
		static bool ppm_sign = false;
		static uint8_t ppm_index = 0;
		static uint16_t ppm_all = 0;
				       
    if(ppm_sign)//不变宽度
    {
       if(ppm.setting.type == RADIO_FUTABA) 
       		PPM_L;  //Futaba
       else 
       		PPM_H;           //JR
       set_ppm(PPM.CONSTANT - PPM.CORR_SMALL);//PPM_Constant - PPM_Correct_Small);
       ppm_sign = 0;
       ppm_index ++;
    }
    else
    {
    	   if(ppm.setting.type == RADIO_FUTABA) 
          		PPM_H; //Futaba
          else 
          		PPM_L;          //JR
       if(ppm_index < 9)
       {
          set_ppm(ppm.output[ppm_index-1] - CORR_LARGE);
          ppm_sign = 1;
          //PPM_Meter += PPM_Constant + PPM_Outp[PPM_Order - 1];//累计每通道总长度
          ppm_all += PPM.CONSTANT + ppm.output[ppm_index-1];
       }
       else
       {
					set_ppm(PPM.FRAME - ppm_all - PPM.CONSTANT - PPM.CORR_LARGE);
          //PPM_Outp(PPM_Length - PPM_Meter - PPM_Constant - PPM_Correct_Large);
          ppm_all = 0;
          ppm_index = 0;
          ppm_sign = 1;
	  			//PPM_Meter = 0;
          //PPM_Order = 0;
          //PPM_Sign = 1;
          //PPM_Refresh();      //刷新输出数据
       }
    }
}


//外部中断1执行函数

SIGNAL(INT1_vect)
{
		static uint32_t last = 0;
		static uint32_t curr = 0;
		static uint8_t index = 0;
		
		curr = micros();
		uint32_t width = curr - last;
		
		if (width > 3000 && width < PPM.FRAME)
		{
				index = 0;
		}
		else if (width < 3000)
		{
				index++;
				if (index != ppm.chan[0] && index != ppm.chan[1] && index != ppm.chan[2])
				{
						ppm.output[index-1] = width - PPM.CONSTANT;
				}
		}
		
		last = curr;
		
		/*
    //PPM_Read1 = micros();             //记录系统uS
    if(last > curr PPM_Read2 > PPM_Read1)        //如果系统时间溢出,丢一帧信号，重新记时,不做处理
    {
          PPM_Read2 = PPM_Read1; 
          return;
    } 
    else 
    {
        if(PPM_Read1-PPM_Read2>PPM_Width && PPM_Read1-PPM_Read2<PPM_Length) //同步头不处理数据
        {   
             PPM_Read2 = PPM_Read1;
             PPM_Out_Order = 0;
        }
        else if(PPM_Read1-PPM_Read2<PPM_Width)
        {  
             PPM_Out_Order ++;
             if((PPM_Out_Order==Channel[0])||(PPM_Out_Order==Channel[1])||(PPM_Out_Order==Channel[2]))
             {
                PPM_Read2 = PPM_Read1;
                return;
             }
             else
             {
                PPM_data[PPM_Out_Order - PITCH] = PPM_Read1 - PPM_Read2 - PPM_Constant;
                PPM_Read2 = PPM_Read1;
             }        
        } 
    }*/
} 

void Interrupt_init()
{
   DDRD |= (1<<2);
   DDRD &= ~(1<<3);
   PORTD |=(1<<3);
   if(PPM_ON[1])
      PPM_H;      //Fuata
   else PPM_L;    //JR
   if(PPM_ON[1])  //Futaba
      EICRA |= (1<<ISC11);    //INT1 引脚下降沿引发中断
   else          //JR
      EICRA |= (1<<ISC10);    //INT1 引脚上升沿引发中断
   EIMSK |= (1<<INT1);        //外部中断请求 1 使能
   TIMSK1 = 0x01;             //定时器1溢出中断
   TCCR1A = 0x00;
   TCCR1B = 0x02;             //启动定时器*/
   
}
