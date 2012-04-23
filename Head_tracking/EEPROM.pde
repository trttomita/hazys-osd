#include <avr/eeprom.h>
void readEEPROM()
{
    eeprom_read_block(accZero,(void*)1,sizeof(accZero));
    eeprom_read_block(magZero, (void*)(1+sizeof(accZero)), sizeof(magZero));
    eeprom_read_block(accTrim, (void*)(1+sizeof(accZero)+ sizeof(magZero)), sizeof(accTrim));
    eeprom_read_block(PPM_Micro,(void*)(1+sizeof(accZero)+ sizeof(magZero)+sizeof(accTrim)),sizeof(PPM_Micro)); //读EEPROM微调值
    eeprom_read_block(Backward,(void*)(1+sizeof(accZero)+ sizeof(magZero)+sizeof(accTrim)+sizeof(PPM_Micro)),sizeof(Backward)); //读EEPROM倒向
    eeprom_read_block(Channel,(void*)(1+sizeof(accZero)+ sizeof(magZero)+sizeof(accTrim)+sizeof(PPM_Micro)+sizeof(Backward)),sizeof(Channel));//通道指定
    eeprom_read_block(PPM_ON,(void*)(1+sizeof(accZero)+ sizeof(magZero)+sizeof(accTrim)+sizeof(PPM_Micro)+sizeof(Backward)+sizeof(Channel)),sizeof(PPM_ON));//内部合成PPM信号
}
void writeParams()
{
    eeprom_write_block(accZero, (void*)1, sizeof(accZero));
    eeprom_write_block(magZero, (void*)(1+sizeof(accZero)), sizeof(magZero));
    eeprom_write_block(accTrim, (void*)(1+sizeof(accZero)+ sizeof(magZero)), sizeof(accTrim));
    eeprom_write_block(PPM_Micro,(void*)(1+sizeof(accZero)+ sizeof(magZero)+sizeof(accTrim)),sizeof(PPM_Micro));//写EEPROM微调值
    eeprom_write_block(Backward,(void*)(1+sizeof(accZero)+ sizeof(magZero)+sizeof(accTrim)+sizeof(PPM_Micro)),sizeof(Backward)); //写EEPROM倒向
    eeprom_write_block(Channel,(void*)(1+sizeof(accZero)+ sizeof(magZero)+sizeof(accTrim)+sizeof(PPM_Micro)+sizeof(Backward)),sizeof(Channel));//通道指定
    eeprom_write_block(PPM_ON,(void*)(1+sizeof(accZero)+ sizeof(magZero)+sizeof(accTrim)+sizeof(PPM_Micro)+sizeof(Backward)+sizeof(Channel)),sizeof(PPM_ON));//内部合成PPM信号
    readEEPROM();
}

/*
static eep_entry[] = {
  &accZero, sizeof(accZero)
, &magZero, sizeof(magZero)
, &accTrim, sizeof(accTrim) 
, &PPM_Micro, sizeof(PPM_Micro) 
, &Backward, sizeof(Backward)
, &Channel, sizeof(Channel)
, &PPM_ON, sizeof(PPM_ON)
};*/ 

