#include <avr/eeprom.h>

enum MENU {MENU_ATT, MENU_CHAN, MENU_TRIM, MENU_ATV, MENU_REV, MENU_SYNT, MENU_RADIO, MENU_MAG};

const char* MENU_STR = 
{
	PSTR("  Euler angles|X    Y    Z"),
	PSTR("  Servo Channel|X    Y    Z"),
	PSTR("   Servo Trim|X    Y    Z"),
	PSTR(" Servo Endpoint|X    Y    Z"),
	PSTR("  Servo Reverse|X    Y    Z"),
	PSTR(" PPM Synthesise|E ="),
	PSTR("  Radio System|T ="),
	PSTR("Mag Calibration|");
}

class Menu: public LCD
{
public:
		void init();
		void update();

private:
		void display_main_menu();

private:
		int8_t menu_index;
		int8_t submenu_index;
		bool key_enable;
		Keyboard keyboard;
};

//uint8_t Menu::submenu_count[] = {0, 3, 3, 3, 1, 1, 1};
void Menu::init()
{
		LCD::init();
		keyboard.init();
		menu_index = submenu_index = 0;
		key_enable = true;
}


void Menu::update()
{
		static uint32_t last;
		
		keyboard.update();
		
		uint8_t key = keyboard.get_key();
		
		if (key_enable)
		{
		switch (key)
		{
		case KEY_ENTER:
				on_key_enter();
				break;
		case KEY_UP:
				on_key_up_down(true);
				break;
		case KEY_DONW:
				on_key_up_down(false);
				break;
		}
	}
		
		uint32_t time = millis();
		if (menu_index = MENU_ATT && time - last > 100)
		{
				display_data(0);
		}
		else if (menu_index = MENU_MAG && time - last > 1000)
		{
				if (!mag.ready())
						write('>');
				else
				{
						submenu = 0;
						key_enable = true;
						display_main_menu();
				}
		}
		last = time;
}

void Menu::on_key_enter()
{
		submenu_index++;
		
	
		switch (menu_index)
		{
		case MENU_ATT:
				acc.calibrate();
				submenu_index = 0;
				break;
		case MENU_TRIM:
		case MENU_REV:
		case MENU_CHAN:
		case MENU_ATV:
				if (submenu_index > 1)
						blink((submenu_index-1)*5-4, 1, false);
				if (submenu_index < 4)
						blink(submenu_index*5-4, 1);
				else
				{
						submenu_index = 0;
				}
				break;
		case MENU_SYNT:
				if (submenu_index == 1)
						blink(1, 0);
				else
				{
						blink(1, 0, false);
						submenu_index = 0;
				}		
				break;
		case MENU_RADIO:
				if (submenu_index = 1)
						blink(1, 0);
				else
				{
						blink(1, 0, false);
						submenu_index = 0;
				}
				break;
		case MENU_MAG:
				mag.calibrate();
				key_enable = false;
				break;
		}
		
		if (submenu_index == 0 && menu_index >= MENU_CHAN && menu_index <= MENU_RADIO)
		{
				ppm.save();
		}
}

void Menu::on_key_up_down(bool up)
{
		if (submenu_index == 0)
		{
				if (up)
				{	 
						if (++menu_index > 6)
								menu_index = 0;
				}
				else
				{
						if (--menu_index < 0)
								menu_index = 6;
				}
				display_main_menu();
		}
		else
		{
				switch (menu_index)
				{
				case MENU_TRIM:
					{
						int8_t trim = ppm.setting.trim[submenu_index-1] + up? PPM.TRIM: -PPM.TRIM;
						if (trim > PPM.TRIM_MAX)
								trim = PPM.TRIM_MAX;
						else if (trim < -PPM.TRIM_MAX)
								trim = -PPM.TRIM_MAX;
						ppm.setting.trim[submenu_index-1] = trim;
						break;
					}
				case MENU_REV:
						ppm.setting.rev[submenu_index-1] = !ppm.setting.rev[submenu_index-1];
						break;
				case MENU_CHAN:
						{
							int8_t chan = ppm.setting.chan[submenu_index-1] + up?1:-1;
							if (chan > 8)
								chan = 0;
							else if (chan < 0)
								chan = 8;
							else if (chan < 5)
								chan = 0;
							ppm.setting.chan[submenu_index-1] = chan;
							break;
						}
				case MENU_ATV:
						{
								int8_t atv = ppm.setting.atv[submenu_index-1] + up?1:-1;
								if (atv > 150)
									atv = 150;
								else if (atv < 0)
									atv = 0;
								ppm.setting.atv[submenu_index-1] = atv;
						}
						break;
				case MENU_SYNT:
						ppm.setting.synt = !ppm.setting.synt;
						break;
				case MENU_RADIO:
						ppm.setting.type = !ppm.setting.type;
						break;
				}
				display_data(submenu_index);
		}
}

void Menu::display_main_menu()
{
		clear();
		print_P(MENU_STR[menu_index]);
		display_data(0);
}

void Menu::display_data(uint8_t submenu)
{
		if (submenu == 0 && menu_index >= MENU_ATT && menu_index <= MENU_CHAN)
		{
				for (uint8_t i = 1; i <= 3; i++)
						display_data(i);
				return;
		}
		
		switch (menu_index)
		{
		case MENU_ATT:
				set_pos(submenu*5-4, 1);
				printf("%4i", ahrs.data[submenu-1]);
				break;
		case MENU_TRIM:
				set_pos(submenu*5-4, 1);
				printf("%4i", ppm.setting.trim[submenu_index-1]);
				break;
		case MENU_REV:
				set_pos(submenu*5-4, 1);
				print(ppm.setting.rev[submenu-1]?"rev":"nor");
				break;
		case MENU_CHAN:
				set_pos(submenu*5-4, 1);
				if (ppm.chan[submenu-1] == 0)
						print("off");
				else
						print("%4i", ppm.setting.chan[submenu-1]);
				break;
		case MENU_PPM:
				set_pos(3,1);
				print(ppm.setting.synt? "on":"off");
				break;
		case MENU_RADIO:
				set_pos(3,1);
				print(ppm.setting.type? "JR Propo", "Futaba");
				break;
		case MENU_MAG:
				break;
		}
}

