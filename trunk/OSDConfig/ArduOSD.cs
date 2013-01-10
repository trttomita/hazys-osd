using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

using uint8_t = System.Byte;



namespace OSDConfig
{
    class ArduOSD : OSD
    {
        /*Panels variables*/
        //Will come from APM telem port


        static float osd_vbat_A = 11.61f;                   // voltage in milivolt
        static float osd_curr_A = 3.2f;
        //static UInt16 osd_battery_remaining = 50;      // 0 to 100 <=> 0 to 1000
        //static UInt16 osd_vbat_A = 50;
        static byte osd_battery_pic = 0xb4;         // picture to show battery remaining
        static float osd_vbat_B = 10.05f;
        static float osd_curr_B = 1.0f;

        static UInt16 osd_mode = 3;                   // Navigation mode from RC AC2 = CH5, APM = CH8
        static byte osd_nav_mode = 4;               // Navigation mode from RC AC2 = CH5, APM = CH8

        static float osd_lat = -35.020938f;                    // latidude
        static float osd_lon = 117.883419f;                    // longitude
        static byte osd_satellites_visible = 7;     // number of satelites
        static byte osd_fix_type = 3;               // GPS lock 0-1=no fix, 2=2D, 3=3D

        static byte osd_got_home = 1;               // tels if got home position or not
        //static float osd_home_lat = 0;               // home latidude
        //static float osd_home_lon = 0;               // home longitude
        static float osd_home_alt = 100;
        static long osd_home_distance = 1000;          // distance from home
        static byte osd_home_direction = 0;             // Arrow direction pointing to home (1-16 to CW loop)

        static SByte osd_pitch = 5;                  // pitch form DCM
        static SByte osd_roll = 3;                   // roll form DCM
        //static byte osd_yaw = 0;                    // relative heading form DCM
        static float osd_heading = 0;                // ground course heading from GPS
        static float osd_alt = 200;                    // altitude
        static float osd_groundspeed = 12;            // ground speed
        static float osd_airspeed = 10;
        static UInt16 osd_throttle = 52;               // throtle
        static byte osd_rssi = 100;

        static byte wp_number = 1;
        static int wp_dist = 100;

        static float convertd = 0;
        static float converts = 0;
        static char dst_sym = '\0';
        static char spd_sym = '\0';
        //MAVLink session control
        static byte mavbeat = 1;
        //static float lastMAVBeat = 0;
        //static boolean waitingMAVBeats = 1;
        static byte apm_mav_type = 2;

        static byte osd_sys_status = 3;
        //static byte apm_mav_system = 7;
        //static byte apm_mav_component = 0;
        //static boolean enable_mav_request = 0;




        public ArduOSD()
        {
            Setting = new OSDSetting();

        }

        public override void Draw()
        {
            if (!mod)
                return;

            Clear();

            if (Setting.GetOption(OSDOption.M_ISO))
            {
                converts = 3.6f;
                convertd = 1.0f;
                spd_sym = (char)0x81;
                dst_sym = (char)0x8D;
            }
            else
            {
                converts = 2.23f;
                convertd = 3.28f;
                spd_sym = (char)0xfb;
                dst_sym = (char)0x66;
            }
            /*
            for (int i = 0; i < Setting.coord.GetLength(0); i++)
            {
                if ((Setting.IsEnabled((OSDItem)i)))
                {
                    curItem = (OSDItem)i;

                    SetPanel(Setting.coord[i, 0], Setting.coord[i, 1]);
                    OpenPanel();

                    switch (curItem)
                    {
                        case OSDItem.Cen:
                            panCenter();
                            break;
                        case OSDItem.Pit:
                            panPitch();
                            break;
                        case OSDItem.Rol:
                            panRoll();
                            break;
                        case OSDItem.BatA:
                            panBatt_A();
                            break;
                        case OSDItem.BatB:
                            panBatt_B();
                            break;
                        case OSDItem.GPSats:
                            panGPSats();
                            break;
                        case OSDItem.GPL:
                            panGPL();
                            break;
                        case OSDItem.GPS:
                            panGPS();
                            break;


                        case OSDItem.Rose:
                            setHeadingPatern();
                            panRose();
                            break;
                        case OSDItem.Head:
                            panHeading();
                            break;//13x3
                        case OSDItem.MavB:
                            panMavBeat();
                            break;//13x3

                        case OSDItem.HDis:
                            if (osd_got_home == 1) panHomeDis();
                            break;//13x3
                        case OSDItem.HDir:
                            if (osd_got_home == 1) panHomeDir(); //13x3
                            break;

                        case OSDItem.RSSI:
                            panRSSI();
                            break;
                        //if(osd_got_home == 1){
                        case OSDItem.CurA:
                            panCur_A();
                            break;
                        case OSDItem.CurB:
                            panCur_B();
                            break;
                        case OSDItem.Alt:
                            panAlt(); //
                            break;
                        case OSDItem.Vel:
                            panVel(); //
                            break;
                        //}
                        case OSDItem.Thr:
                            panThr(); //
                            break;
                        case OSDItem.FMod:
                            panFlightMode();  //
                            break;
                        case OSDItem.Hor:
                            panHorizon(Setting.coord[(int)OSDItem.Hor, 0], Setting.coord[(int)OSDItem.Hor, 1]); //14x5
                            break;
                        case OSDItem.SYS:
                            print_P(PSTR("Disarmed"));
                            break;

                    }
                    ClosePanel();
                }
            }
            */

            for (int i = 0; i < 26; i++)
            {
                if ((setting.enable & (1UL << i)) != 0)
                {
                    curItem = (OSDItem)i;

                    SetPanel(setting.coord[i, 0], setting.coord[i, 1]);
                    OpenPanel();

                    switch (i)
                    {
                        case OSD_ITEM_Cen:
                            print_P(PSTR("\x05\x03\x04\x05|\x15\x13\x14\x15"));
                            break;
                        case OSD_ITEM_Pit:
                            printf_P(PSTR("%4i\xb0\xb1"), osd_pitch);
                            break;
                        case OSD_ITEM_Rol:
                            printf_P(PSTR("%4i\xb0\xb2"), osd_roll);
                            break;
                        case OSD_ITEM_VBatA:
                            printf_P(PSTR("\xE2%5.2f\x8E"), (double)osd_vbat_A);
                            break;
                        case OSD_ITEM_VBatB:
                            printf_P(PSTR("\xE3%5.2f\x8E"), (double)osd_vbat_B);
                            break;
                        case OSD_ITEM_GPSats:
                            printf_P(PSTR("\x0f%2i"), osd_satellites_visible);
                            break;
                        case OSD_ITEM_GPL:
                            switch (osd_fix_type)
                            {
                                case 0:
                                case 1:
                                    print_P(PSTR("\x10\x20"));
                                    break;
                                case 2: //If not APM, x01 would show 2D fix
                                case 3:
                                    print_P(PSTR("\x11\x20"));//If not APM, x02 would show 3D fix
                                    break;
                            }
                            break;
                        case OSD_ITEM_GPS:
                            printf_P(PSTR("\x83%11.6f|\x84%11.6f"), (double)osd_lat, (double)osd_lon);
                            break;
                        case OSD_ITEM_Rose:
                            printf_P(PSTR("\x20\xc0\xc0\xc0\xc0\xc0\xc7\xc0\xc0\xc0\xc0\xc0\x20|\xd0%s\xd1"), getHeadingPatern(osd_heading));
                            break;
                        case OSD_ITEM_Head:
                            printf_P(PSTR("%4.0f\xb0"), (double)osd_heading);
                            break;//13x3
                        case OSD_ITEM_MavB:
                            if (mavbeat == 1)
                            {
                                print_P(PSTR("\xEA\xEC"));
                                mavbeat = 0;
                            }
                            else
                            {
                                print_P(PSTR("\xEA\xEB"));
                            }
                            break;//13x3
                        case OSD_ITEM_HDis:
                            if (osd_got_home == 1) printf_P(PSTR("\x1F%5.0f%c"), (double)(osd_home_distance * convertd), dst_sym);
                            break;//13x3
                        case OSD_ITEM_HDir:
                            if (osd_got_home == 1) DrawArrow();//panHomeDir(); //13x3
                            break;
                        case OSD_ITEM_WDir:
                            {
                                //int8_t wp_target_bearing_rotate_int = round(((float)wp_target_bearing - osd_heading) / 360.0 * 16.0) + 1; //Convert to int 0-16 
                                //if (wp_target_bearing_rotate_int < 0) wp_target_bearing_rotate_int += 16; //normalize
                                DrawArrow();
                            }
                            break;
                        case OSD_ITEM_WDis:
                            printf_P(PSTR("\x57%2i\x00%4.0f%c"), wp_number, (double)(wp_dist * convertd), dst_sym);
                            break;
                        case OSD_ITEM_RSSI:
                            printf_P(PSTR("\xE1%3i%%"), osd_rssi);
                            break;
                        case OSD_ITEM_CurrA:
                            printf_P(PSTR("\xE4%5.2f\x8F"), osd_curr_A);
                            break;
                        case OSD_ITEM_CurrB:
                            printf_P(PSTR("\xE5%5.2f\x8F"), osd_curr_B);
                            break;
                        case OSD_ITEM_Alt:
                            printf_P(PSTR("\xe6%5.0f%c"),
                                (double)(osd_alt * convertd), dst_sym);
                            break;
                        case OSD_ITEM_HAlt:
                            printf_P(PSTR("\xe7%5.0f%c"),
                                (double)((osd_alt - osd_home_alt) * convertd), dst_sym);
                            break;
                        case OSD_ITEM_Vel:
                            printf_P(PSTR("\xe9%3.0f%c"), (double)osd_groundspeed * converts, spd_sym);
                            break;
                        case OSD_ITEM_AS:
                            printf_P(PSTR("\xe8%3.0f%c"), (double)osd_airspeed * converts, spd_sym);
                            break;
                        case OSD_ITEM_Thr:
                            printf_P(PSTR("\x87%3i%%"), osd_throttle);
                            break;
                        case OSD_ITEM_FMod:
                            {
                                if (apm_mav_type == 1 && osd_mode <= 12 || osd_mode <= 10)
                                {
                                    write('\xE0');
                                    print_P(apm_mav_type == 1 ? FM_APM[osd_mode] : FM_ACM[osd_mode]);
                                }
                            }
                            break;
                        case OSD_ITEM_Hor:
                            for (uint8_t j = 0; j < 5; j++)
                            {
                                char c = (j == 2 ? '\xd8' : '\xc8');
                                write(c);
                                for (uint8_t k = 0; k < 12; k++)
                                    write('\x20');
                                write(++c);
                                write('|');
                            }
                            ClosePanel();
                            DrawHorizon(setting.coord[OSD_ITEM_Hor, 0] + 1, setting.coord[OSD_ITEM_Hor, 1]);
                            break;
                        case OSD_ITEM_SYS:
                            if (osd_sys_status < 3)
                                print_P(PSTR("APM Init"));
                            else if (osd_sys_status != 4)
                                print_P(PSTR("Disarmed"));
                            else
                                print_P(PSTR("        "));
                            break;
                    }

                    if (i != OSD_ITEM_Hor)
                        ClosePanel();
                }
            }
            base.Draw();
            mod = false;
        }

        uint _BV(int bit)
        {
            return (uint)(1UL << bit);
        }

        string PSTR(string input)
        {
            return input;
        }


        void DrawArrow()
        {
            char c = '\x90';
            if (osd_home_direction > 1)
                c = (char)(c + ((osd_home_direction - 1) << 1));

            write(c);
            write(++c);
        }

        void DrawHorizon(int start_col, int start_row)
        {

            int x, nose, row, minval, hit, subval = 0;
            int cols = 12;
            int rows = 5;
            int[] col_hit = new int[cols];
            double pitch, roll;

            pitch = (abs(osd_pitch) == 90) ? 89.99 * (90 / osd_pitch) * -0.017453293 : osd_pitch * -0.017453293;
            roll = (abs(osd_roll) == 90) ? 89.99 * (90 / osd_roll) * 0.017453293 : osd_roll * 0.017453293;

            nose = round(tan(pitch) * (rows * 9));
            for (int col = 1; col <= cols; col++)
            {
                x = (col * 12) - (cols * 6) - 6;//center X point at middle of each col
                col_hit[col - 1] = (int)((tan(roll) * x) + nose + (rows * 9) - 1);//calculating hit point on Y plus offset to eliminate negative values
                //col_hit[(col-1)] = nose + (rows * 9);
            }

            for (int col = 0; col < cols; col++)
            {
                hit = col_hit[col];
                if (hit > 0 && hit < (rows * 18))
                {
                    row = rows - ((hit - 1) / 18);
                    minval = rows * 18 - row * 18 + 1;
                    subval = hit - minval;
                    subval = round((subval * 9) / 18);
                    if (subval == 0) subval = 1;
                    OpenSingle(start_col + col, start_row + row - 1);
                    write((char)('\x05' + subval));
                    //write('0' + start_row + row - 1);
                }
            }
        }

        string getHeadingPatern(float heading)
        {
            int start;
            start = round((heading * 36) / 360);
            start -= 5;
            if (start < 0) start += 36;
            for (int x = 0; x <= 10; x++)
            {
                buf_show[x] = (char)buf_Rule[start];
                if (++start > 35) start = 0;
            }
            //buf_show[11] = '\0';
            return new String(buf_show);
        }

        /* **************************************************************** */
        // Panel  : panAlt
        // Needs  : X, Y locations
        // Output : Alt symbol and altitude value in meters from MAVLink
        // Size   : 1 x 7Hea  (rows x chars)
        // Staus  : done

        void panAlt(/*int first_col, int first_line*/)
        {
            ////setPanel(first_col, first_line);
            //openPanel();
            //printf("%c%5.0f%c",0x85, (double)(osd_alt - osd_home_alt), 0x8D);
            printf("%c%5.0f%c", 0x85, (double)(osd_alt), 0x8D);
            //closePanel();
        }

        void panAlt_R(/*int first_col, int first_line*/)
        {
            ////setPanel(first_col, first_line);
            //openPanel();
            //printf("%c%5.0f%c",0x85, (double)(osd_alt - osd_home_alt), 0x8D);
            printf("%c%5.0f%c", 0x85, (double)(osd_alt - 50), 0x8D);
            //closePanel();
        }

        /* **************************************************************** */
        // Panel  : panVel
        // Needs  : X, Y locations
        // Output : Velocity value from MAVlink with symbols
        // Size   : 1 x 7  (rows x chars)
        // Staus  : done

        void panVel(/*int first_col, int first_line*/)
        {
            ////setPanel(first_col, first_line);
            //openPanel();
            printf("%c%3.0f%c", 0x86, (double)osd_groundspeed, 0x88);
            //closePanel();
        }

        /* **************************************************************** */
        // Panel  : panThr
        // Needs  : X, Y locations
        // Output : Throttle value from MAVlink with symbols
        // Size   : 1 x 7  (rows x chars)
        // Staus  : done

        void panThr(/*int first_col, int first_line*/)
        {
            ////setPanel(first_col, first_line);
            //openPanel();
            printf("%c%3.0i%%", 0x87, osd_throttle);
            //closePanel();
        }

        /* **************************************************************** */
        // Panel  : panHomeDis
        // Needs  : X, Y locations
        // Output : Home Symbol with distance to home in meters
        // Size   : 1 x 7  (rows x chars)
        // Staus  : done

        void panHomeDis(/*int first_col, int first_line*/)
        {
            ////setPanel(first_col, first_line);
            //openPanel();
            printf("%c%5.0f%c", 0x1F, (double)osd_home_distance, 0x8D);
            //closePanel();
        }

        /* **************************************************************** */
        // Panel  : panCenter
        // Needs  : X, Y locations
        // Output : 2 row croshair symbol created by 2 x 4 chars
        // Size   : 2 x 4  (rows x chars)
        // Staus  : done

        void panCenter(/*int first_col, int first_line*/)
        {
            ////setPanel(first_col, first_line);
            //openPanel();
            print_P(PSTR("\x05\x03\x04\x05|\x15\x13\x14\x15"));
            //closePanel();
        }

        /* **************************************************************** */
        // Panel  : panHorizon
        // Needs  : X, Y locations
        // Output : 12 x 4 Horizon line surrounded by 2 cols (left/right rules)
        // Size   : 14 x 4  (rows x chars)
        // Staus  : done

        void panHorizon(int first_col, int first_line)
        {
            ////setPanel(first_col, first_line);
            //openPanel();


            print_P(PSTR("\xc8\x20\x20\x20\x20\x20\x20\x20\x20\x20\x20\x20\x20\xc9|"));
            print_P(PSTR("\xc8\x20\x20\x20\x20\x20\x20\x20\x20\x20\x20\x20\x20\xc9|"));
            print_P(PSTR("\xd8\x20\x20\x20\x20\x20\x20\x20\x20\x20\x20\x20\x20\xd9|"));
            print_P(PSTR("\xc8\x20\x20\x20\x20\x20\x20\x20\x20\x20\x20\x20\x20\xc9|"));
            print_P(PSTR("\xc8\x20\x20\x20\x20\x20\x20\x20\x20\x20\x20\x20\x20\xc9"));

            showHorizon((first_col + 1), first_line);
        }

        /* **************************************************************** */
        // Panel  : panPitch
        // Needs  : X, Y locations
        // Output : -+ value of current Pitch from vehicle with degree symbols and pitch symbol
        // Size   : 1 x 6  (rows x chars)
        // Staus  : done

        void panPitch(/*int first_col, int first_line*/)
        {
            ////setPanel(first_col, first_line);
            //openPanel();
            printf("%4i%c%c", osd_pitch, 0xb0, 0xb1);
            //closePanel();
        }

        /* **************************************************************** */
        // Panel  : panRoll
        // Needs  : X, Y locations
        // Output : -+ value of current Roll from vehicle with degree symbols and roll symbol
        // Size   : 1 x 6  (rows x chars)
        // Staus  : done

        void panRoll(/*int first_col, int first_line*/)
        {
            ////setPanel(first_col, first_line);
            //openPanel();
            printf("%4i%c%c", osd_roll, 0xb0, 0xb2);
            //closePanel();
        }

        /* **************************************************************** */
        // Panel  : panBattery A (Voltage 1)
        // Needs  : X, Y locations
        // Output : Voltage value as in XX.X and symbol of over all battery status
        // Size   : 1 x 8  (rows x chars)
        // Staus  : done

        void panBatt_A(/*int first_col, int first_line*/)
        {
            //setPanel(first_col, first_line);
            //openPanel();
            /*************** This commented code is for the next ArduPlane Version
            #ifdef MAVLINK10
              if(osd_battery_remaining_A > 100){
                printf(" %c%5.2f%c", 0xE2, (double)osd_vbat_A, 0x8E);
              }
            #else
              if(osd_battery_remaining_A > 1000){
                printf(" %c%5.2f%c", 0xE2, (double)osd_vbat_A, 0x8E);
              }
            #endif //MAVLINK10
              else printf("%c%5.2f%c%c", 0xE2, (double)osd_vbat_A, 0x8E, osd_battery_pic_A);
            */
            printf(" %c%5.2f%c", 0xE2, (double)osd_vbat_A, 0x8E);
            //closePanel();
        }

        void panBatt_B(/*int first_col, int first_line*/)
        {
            //setPanel(first_col, first_line);
            //openPanel();
            /*************** This commented code is for the next ArduPlane Version
            #ifdef MAVLINK10
              if(osd_battery_remaining_A > 100){
                printf(" %c%5.2f%c", 0xE2, (double)osd_vbat_A, 0x8E);
              }
            #else
              if(osd_battery_remaining_A > 1000){
                printf(" %c%5.2f%c", 0xE2, (double)osd_vbat_A, 0x8E);
              }
            #endif //MAVLINK10
              else printf("%c%5.2f%c%c", 0xE2, (double)osd_vbat_A, 0x8E, osd_battery_pic_A);
            */
            printf(" %c%5.2f%c", 0xE3, (double)osd_vbat_A, 0x8E);
            //closePanel();
        }

        void panCur_A()
        {
            printf(" \xE4%5.2f\x8F", (double)osd_curr_A);
        }

        void panCur_B()
        {
            printf(" \xE5%5.2f\x8F", (double)osd_curr_B);
        }

        void panRSSI()
        {
            printf("\xE1%3i%%", osd_rssi);
        }

        //------------------ Panel: Startup ArduCam OSD LOGO -------------------------------

        void panLogo(/*int first_col, int first_line*/)
        {
            SetPanel(10, 5);
            OpenPanel();
            print_P(PSTR("\x20\x20\x20\x20\xba\xbb\xbc\xbd\xbe|\x20\x20\x20\x20\xca\xcb\xcc\xcd\xce|ArduCam OSD"));
            ClosePanel();
        }

        //------------------ Panel: Waiting for MAVLink HeartBeats -------------------------------

        void panWaitMAVBeats(int first_col, int first_line)
        {
            panLogo();
            SetPanel(first_col, first_line);
            OpenPanel();
            print_P(PSTR("Waiting for|MAVLink heartbeats..."));
            ClosePanel();
        }

        /* **************************************************************** */
        // Panel  : panGPL
        // Needs  : X, Y locations
        // Output : 1 static symbol with changing FIX symbol
        // Size   : 1 x 2  (rows x chars)
        // Staus  : done

        void panGPL(/*int first_col, int first_line*/)
        {
            ////setPanel(first_col, first_line);
            //openPanel();
            switch (osd_fix_type)
            {
                case 0:
                //    print_P(PSTR("\x10\x20"));
                //    break;
                case 1:
                    print_P(PSTR("\x10\x20"));
                    break;
                case 2:
                //    print_P(PSTR("\x11\x20"));//If not APM, x01 would show 2D fix
                //    break;
                case 3:
                    print_P(PSTR("\x11\x20"));//If not APM, x02 would show 3D fix
                    break;
            }

            /*  if(osd_fix_type <= 1) {
            print_P(PSTR("\x10"));
            } else {
            print_P(PSTR("\x11"));
            }  */
            //closePanel();
        }

        /* **************************************************************** */
        // Panel  : panGPSats
        // Needs  : X, Y locations
        // Output : 1 symbol and number of locked satellites
        // Size   : 1 x 5  (rows x chars)
        // Staus  : done

        void panGPSats(/*int first_col, int first_line*/)
        {
            //setPanel(first_col, first_line);
            //openPanel();
            printf("%c%2i", 0x0f, osd_satellites_visible);
            //closePanel();
        }

        /* **************************************************************** */
        // Panel  : panGPS
        // Needs  : X, Y locations
        // Output : two row numeric value of current GPS location with LAT/LON symbols as on first char
        // Size   : 2 x 12  (rows x chars)
        // Staus  : done

        void panGPS(/*int first_col, int first_line*/)
        {
            ////setPanel(first_col, first_line);
            //openPanel();
            printf("%c%11.6f|%c%11.6f", 0x83, (double)osd_lat, 0x84, (double)osd_lon);
            //closePanel();
        }

        /* **************************************************************** */
        // Panel  : panHeading
        // Needs  : X, Y locations
        // Output : Symbols with numeric compass heading value
        // Size   : 1 x 5  (rows x chars)
        // Staus  : not ready

        void panHeading(/*int first_col, int first_line*/)
        {
            ////setPanel(first_col, first_line);
            //openPanel();
            printf("%4.0f%c", (double)osd_heading, 0xb0);
            //closePanel();
        }

        /* **************************************************************** */
        // Panel  : panRose
        // Needs  : X, Y locations
        // Output : a dynamic compass rose that changes along the heading information
        // Size   : 2 x 13  (rows x chars)
        // Staus  : done

        void panRose(/*int first_col, int first_line*/)
        {
            ////setPanel(first_col, first_line);
            //openPanel();
            //osd_heading  = osd_yaw;
            //if(osd_yaw < 0) osd_heading = 360 + osd_yaw;
            printf("%s|%c%s%c", "\x20\xc0\xc0\xc0\xc0\xc0\xc7\xc0\xc0\xc0\xc0\xc0\x20", 0xd0, rose, 0xd1);
            //closePanel();
        }


        /* **************************************************************** */
        // Panel  : panBoot
        // Needs  : X, Y locations
        // Output : Booting up text and empty bar after that
        // Size   : 1 x 21  (rows x chars)
        // Staus  : done

        void panBoot(int first_col, int first_line)
        {
            SetPanel(first_col, first_line);
            OpenPanel();
            print_P(PSTR("Booting up:\xed\xf2\xf2\xf2\xf2\xf2\xf2\xf2\xf3"));
            ClosePanel();
        }

        /* **************************************************************** */
        // Panel  : panMavBeat
        // Needs  : X, Y locations
        // Output : 2 symbols, one static and one that blinks on every 50th received
        //          mavlink packet.
        // Size   : 1 x 2  (rows x chars)
        // Staus  : done



        /* **************************************************************** */
        // Panel  : panWPDir
        // Needs  : X, Y locations
        // Output : 2 symbols that are combined as one arrow, shows direction to next waypoint
        // Size   : 1 x 2  (rows x chars)
        // Staus  : not ready

        void panWPDir(/*int first_col, int first_line*/)
        {
            //setPanel(first_col, first_line);
            //openPanel();
            showArrow();
            //closePanel();
        }

        /* **************************************************************** */
        // Panel  : panHomeDir
        // Needs  : X, Y locations
        // Output : 2 symbols that are combined as one arrow, shows direction to home
        // Size   : 1 x 2  (rows x chars)
        // Status : not tested

        void panHomeDir(/*int first_col, int first_line*/)
        {
            //setPanel(first_col, first_line);
            // openPanel();
            showArrow();
            // closePanel();
        }

        /* **************************************************************** */
        // Panel  : panFlightMode
        // Needs  : X, Y locations
        // Output : 2 symbols, one static name symbol and another that changes by flight modes
        // Size   : 1 x 2  (rows x chars)
        // Status : done

        void panFlightMode(/*int first_col, int first_line*/)
        {
            //setPanel(first_col, first_line);
            // openPanel();
            //write('\xE0');
            /*
        #ifndef MAVLINK10
            if(apm_mav_type == 2) //ArduCopter MultiRotor or ArduCopter Heli
            {
                if(osd_mode == 100) print_P(FM_STAB);//Stabilize
                if(osd_mode == 101) print_P(FM_ACRO);//Acrobatic
                if(osd_mode == 102) print_P(FM_ALTH);//Alt Hold
                if(osd_mode == MAV_MODE_AUTO && osd_nav_mode == MAV_NAV_WAYPOINT) print_P(FM_AUTO);//Auto
                if(osd_mode == MAV_MODE_GUIDED && osd_nav_mode == MAV_NAV_WAYPOINT) print_P(FM_GUID);//Guided
                if(osd_mode == MAV_MODE_AUTO && osd_nav_mode == MAV_NAV_HOLD) print_P(FM_LOIT);//Loiter
                if(osd_mode == MAV_MODE_AUTO && osd_nav_mode == MAV_NAV_RETURNING) print_P(FM_RETL);//Return to Launch
                if(osd_mode == 107) print_P(FM_CIRC); // Circle
                if(osd_mode == 108) print_P(FM_POSI); // Position
                if(osd_mode == 109) print_P(FM_LAND); // Land
                if(osd_mode == 110) print_P(FM_OFLO); // OF_Loiter
            }
            else if(apm_mav_type == 1) //ArduPlane
            {
                if(osd_mode == MAV_MODE_TEST1 && osd_nav_mode == MAV_NAV_VECTOR) print_P(FM_STAB);//Stabilize
                if(osd_mode == MAV_MODE_MANUAL && osd_nav_mode == MAV_NAV_VECTOR) print_P(FM_MANU);//Manual
                if(osd_mode == MAV_MODE_AUTO && osd_nav_mode == MAV_NAV_LOITER) print_P(FM_LOIT);//Loiter
                if(osd_mode == MAV_MODE_AUTO && osd_nav_mode == MAV_NAV_RETURNING) print_P(FM_RETL);//Return to Launch
                if(osd_mode == MAV_MODE_TEST2 && osd_nav_mode == 1) print_P(FM_FBWA);//FLY_BY_WIRE_A
                if(osd_mode == MAV_MODE_TEST2 && osd_nav_mode == 2) print_P(FM_FBWB);//FLY_BY_WIRE_B
                if(osd_mode == MAV_MODE_GUIDED) print_P(FM_GUID);//GUIDED
                if(osd_mode == MAV_MODE_AUTO && osd_nav_mode == MAV_NAV_WAYPOINT) print_P(FM_AUTO);//AUTO
                if(osd_mode == MAV_MODE_TEST3) print_P(FM_CIRC);//CIRCLE
            }
        #else*/
            if (apm_mav_type == 2) //ArduCopter MultiRotor or ArduCopter Heli
            {
                switch (osd_mode)
                {
                    case 0:
                        print_P(FM_STAB);//Stabilize
                        break;
                    case 1:
                        print_P(FM_STAB);//Stabilize
                        break;
                    case 2:
                        print_P(FM_ALTH);//Alt Hold;
                        break;
                    case 3:
                        print_P(FM_AUTO);//Auto
                        break;
                    case 4:
                        print_P(FM_GUID);//Guided
                        break;
                    case 5:
                        print_P(FM_LOIT);//Loiter
                        break;
                    case 6:
                        print_P(FM_RETL);//Return to Launch
                        break;
                    case 7:
                        print_P(FM_CIRC); // Circle
                        break;
                    case 8:
                        print_P(FM_POSI); // Position
                        break;
                    case 9:
                        print_P(FM_LAND); // Land
                        break;
                    case 10:
                        print_P(FM_OFLO); // OF_Loiter
                        break;
                }
            }
            else if (apm_mav_type == 1) //ArduPlane
            {
                if (osd_mode == 2) print_P(FM_STAB);//Stabilize
                if (osd_mode == 0) print_P(FM_MANU);//Manual
                if (osd_mode == 12) print_P(FM_LOIT);//Loiter
                if (osd_mode == 11) print_P(FM_RETL);//Return to Launch
                if (osd_mode == 5) print_P(FM_FBWA);//FLY_BY_WIRE_A
                if (osd_mode == 6) print_P(FM_FBWB);//FLY_BY_WIRE_B
                if (osd_mode == 15) print_P(FM_GUID);//GUIDED
                if (osd_mode == 10) print_P(FM_AUTO);//AUTO
                if (osd_mode == 1) print_P(FM_CIRC);//CIRCLE
            }
            //#endif
            // closePanel();
        }


        // ---------------- EXTRA FUNCTIONS ----------------------
        // Show those fancy 2 char arrows
        void showArrow()
        {
            char c = '\x90';
            if (osd_home_direction > 1)
                c += (char)(2 * (osd_home_direction - 1));

            printf("%c%c", c, c + 1);
            //write(c);
            //write(++c);

        }

        void printHit(byte col, byte row, byte subval)
        {
            //openSingle(col, row);
            SetPanel(col, row);
            OpenPanel();
            //write((char)('\x05' + subval));

            switch (subval)
            {
                case 1:
                    print_P(PSTR("\x06"));
                    break;
                case 2:
                    print_P(PSTR("\x07"));
                    break;
                case 3:
                    print_P(PSTR("\x08"));
                    break;
                case 4:
                    print_P(PSTR("\x09"));
                    break;
                case 5:
                    print_P(PSTR("\x0a"));
                    break;
                case 6:
                    print_P(PSTR("\x0b"));
                    break;
                case 7:
                    print_P(PSTR("\x0c"));
                    break;
                case 8:
                    print_P(PSTR("\x0d"));
                    break;
                case 9:
                    print_P(PSTR("\x0e"));
                    break;
            }
        }

        // Calculate and shows Artificial Horizon
        void showHorizon(int start_col, int start_row)
        {

            int x, nose, row, minval, hit, subval = 0;
            int cols = 12;
            int rows = 5;
            int[] col_hit = new int[cols];
            double pitch, roll;

            pitch = (abs(osd_pitch) == 90) ? 89.99 * (90 / osd_pitch) * -0.017453293 : osd_pitch * -0.017453293;
            roll = (abs(osd_roll) == 90) ? 89.99 * (90 / osd_roll) * 0.017453293 : osd_roll * 0.017453293;

            nose = round(tan(pitch) * (rows * 9));
            for (int col = 1; col <= cols; col++)
            {
                x = (col * 12) - (cols * 6) - 6;//center X point at middle of each col
                col_hit[col - 1] = (int)((tan(roll) * x) + nose + (rows * 9) - 1);//calculating hit point on Y plus offset to eliminate negative values
                //col_hit[(col-1)] = nose + (rows * 9);
            }

            for (int col = 0; col < cols; col++)
            {
                hit = col_hit[col];
                if (hit > 0 && hit < (rows * 18))
                {
                    row = rows - ((hit - 1) / 18);
                    minval = rows * 18 - row * 18 + 1;
                    subval = hit - minval;
                    subval = round((subval * 9) / 18);
                    if (subval == 0) subval = 1;
                    printHit((byte)(start_col + col), (byte)(start_row + row - 1), (byte)subval);
                }
            }
        }



        //public void


        double abs(double input)
        {
            return Math.Abs(input);
        }

        int round(double input)
        {
            return (int)Math.Round(input, 0);
        }

        double tan(double input)
        {
            return Math.Tan(input);
        }

        void setHeadingPatern()
        {
            int start;
            start = round((osd_heading * 36) / 360);
            start -= 5;
            if (start < 0) start += 36;
            for (int x = 0; x <= 10; x++)
            {
                buf_show[x] = (char)buf_Rule[start];
                if (++start > 35) start = 0;
            }
            //buf_show[11] = '\0';
            rose = new string(buf_show);
        }

        const string FM_STAB = "stab";
        const string FM_ACRO = "acro";
        const string FM_ALTH = "alth";
        const string FM_AUTO = "auto";
        const string FM_GUID = "guid";
        const string FM_LOIT = "loit";
        const string FM_RETL = "retl";
        const string FM_CIRC = "circ";
        const string FM_POSI = "posi";
        const string FM_OFLO = "oflo";
        const string FM_MANU = "manu";
        const string FM_FBWA = "fbwa";
        const string FM_FBWB = "fbwb";
        const string FM_LAND = "land";



        string[] FM_ACM = { FM_STAB, FM_STAB, FM_ALTH, FM_AUTO, FM_GUID, FM_LOIT, FM_RETL, FM_CIRC, FM_POSI, FM_LAND, FM_OFLO };
        string[] FM_APM = { FM_MANU, FM_CIRC, FM_STAB, FM_STAB, FM_STAB, FM_FBWA, FM_FBWB, FM_STAB, FM_STAB, FM_STAB, FM_AUTO, FM_RETL, FM_LOIT };

        string rose;
        char[] buf_show = new char[11];

        byte[] buf_Rule = new byte[]{0xc2,0xc0,0xc0,0xc1,0xc0,0xc0,0xc1,0xc0,0xc0,
                           0xc4,0xc0,0xc0,0xc1,0xc0,0xc0,0xc1,0xc0,0xc0,
                           0xc3,0xc0,0xc0,0xc1,0xc0,0xc0,0xc1,0xc0,0xc0,
                           0xc5,0xc0,0xc0,0xc1,0xc0,0xc0,0xc1,0xc0,0xc0
                          };


        const int OSD_ITEM_Cen = 0;
        const int OSD_ITEM_Pit = 1;
        const int OSD_ITEM_Rol = 2;
        const int OSD_ITEM_VBatA = 3;
        const int OSD_ITEM_VBatB = 4;
        const int OSD_ITEM_GPSats = 5;
        const int OSD_ITEM_GPL = 6;
        const int OSD_ITEM_GPS = 7;
        // panB_REG Byte has:
        const int OSD_ITEM_Rose = 8;
        const int OSD_ITEM_Head = 9;
        const int OSD_ITEM_MavB = 10;
        const int OSD_ITEM_HDir = 11;
        const int OSD_ITEM_HDis = 12;
        const int OSD_ITEM_WDir = 13 /*(!Not implemented)*/;
        const int OSD_ITEM_WDis = 14;
        const int OSD_ITEM_RSSI = 15;
        // panC_REG Byte has:
        const int OSD_ITEM_CurrA = 16;
        const int OSD_ITEM_CurrB = 17;
        const int OSD_ITEM_Alt = 18;
        const int OSD_ITEM_HAlt = 19;
        const int OSD_ITEM_Vel = 20;
        const int OSD_ITEM_AS = 21;
        const int OSD_ITEM_Thr = 22;
        const int OSD_ITEM_FMod = 23;
        const int OSD_ITEM_Hor = 24;
        const int OSD_ITEM_SYS = 25;


        const int OSD_OPT_Alt_R = 7;
        //OSD_ITEM_VBatA_ADC, OSD_ITEM_VBatB_ADC, OSD_ITEM_CurrA_ADC, OSD_ITEM_CurrB_ADC, OSD_ITEM_RSSI_ADC, OSD_ITEM_Alt_R


    }
}
