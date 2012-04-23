enum MENU {MENU_ATT, MENU_TRIM, MENU_REV, MENU_CHAN, MENU_PPM, MENU_MAG, MENU_RADIO};


class Menu
{
public:
		void init();
		void update();

private:
		void display_menu();

private:
		uint8_t menu_index;
		uint8_t submenu_index;
		static uint8_t submenu_count[];
};

//uint8_t Menu::submenu_count[] = {0, 3, 3, 3, 1, 1, 1};

void Menu::update()
{
		keyboard.update();
		uint8_t key = keyboard.get_key();
		
		switch (key)
		{
		case KEY_ENTER:
				on_key_enter();
				break;
		case KEY_UP:
				on_key_up();
		case 
		}
}

void Menu::on_key_enter()
{
		submenu_index++;
		switch (menu_index)
		{
		case MENU_ATT:
				break;
		case MENU_TRIM:
				break;
		case MENU_REV:
				break;
		case MENU_CHAN:
				break;
		case MENU_PPM:
				break;
		case MENU_RADIO:
				break;
		case MENU_MAG:
				break;
		}
}

void Menu::on_key_up()
{
		if (submenu_index == 0)
		{
				if (++menu_index > 6)
						menu_index = 0;
		}
		else
		{
				switch (menu_index)
				{
				case MENU_TRIM:
						
						break;
				case MENU_REV:
						break;
				case MENU_CHAN:
						break;
				case MENU_PPM:
						break;
				case MENU_RADIO:
						break;
				case MENU_MAG:
						break;
				}
		}
}


