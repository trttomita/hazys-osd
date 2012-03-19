using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace OSDConfig
{
    class ArduOSD : OSD
    {
        /*Panels variables*/
        //Will come from APM telem port


        static float osd_vbat = 11.61f;                   // voltage in milivolt
        //static UInt16 osd_battery_remaining = 50;      // 0 to 100 <=> 0 to 1000
        static UInt16 osd_vbat_A = 50;
        static byte osd_battery_pic = 0xb4;         // picture to show battery remaining

        static UInt16 osd_mode = 3;                   // Navigation mode from RC AC2 = CH5, APM = CH8
        static byte osd_nav_mode = 4;               // Navigation mode from RC AC2 = CH5, APM = CH8

        static float osd_lat = -35.020938f;                    // latidude
        static float osd_lon = 117.883419f;                    // longitude
        static byte osd_satellites_visible = 7;     // number of satelites
        static byte osd_fix_type = 3;               // GPS lock 0-1=no fix, 2=2D, 3=3D

        static byte osd_got_home = 1;               // tels if got home position or not
        //static float osd_home_lat = 0;               // home latidude
        //static float osd_home_lon = 0;               // home longitude
        //static float osd_home_alt = 0;
        static long osd_home_distance = 0;          // distance from home
        static byte osd_home_direction = 0;             // Arrow direction pointing to home (1-16 to CW loop)

        static SByte osd_pitch = 0;                  // pitch form DCM
        static SByte osd_roll = 0;                   // roll form DCM
        //static byte osd_yaw = 0;                    // relative heading form DCM
        static float osd_heading = 0;                // ground course heading from GPS
        static float osd_alt = 200;                    // altitude
        static float osd_groundspeed = 12;            // ground speed
        static UInt16 osd_throttle = 52;               // throtle

        //MAVLink session control
        static bool mavbeat = true;
        //static float lastMAVBeat = 0;
        //static boolean waitingMAVBeats = 1;
        static byte apm_mav_type = 2;
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


                        //if(osd_got_home == 1){
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


                    }
                    ClosePanel();
                }
            }

            base.Draw();
            mod = false;
        }

        string PSTR(string input)
        {
            return input;
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
            printf("%c%3.0i%c", 0x87, osd_throttle, 0x25);
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

        void panMavBeat(/*int first_col, int first_line*/)
        {
            //setPanel(first_col, first_line);
            //openPanel();
            if (mavbeat)
            {
                print_P(PSTR("\xEA\xEC"));
                mavbeat = false;
            }
            else
            {
                print_P(PSTR("\xEA\xEB"));
            }
            //closePanel();
        }


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
            
            switch (subval){
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

        const string FM_STAB = "\x00E0stab";
        const string FM_ACRO = "\x00E0acro";
        const string FM_ALTH = "\x00E0alth";
        const string FM_AUTO = "\x00E0auto";
        const string FM_GUID = "\x00E0guid";
        const string FM_LOIT = "\x00E0loit";
        const string FM_RETL = "\x00E0retl";
        const string FM_CIRC = "\x00E0circ";
        const string FM_POSI = "\x00E0posi";
        const string FM_OFLO = "\x00E0oflo";
        const string FM_MANU = "\x00E0manu";
        const string FM_FBWA = "\x00E0fbwa";
        const string FM_FBWB = "\x00E0fbwb";
        const string FM_LAND = "\x00E0land";

        string rose;
        char[] buf_show = new char[11];

        byte[] buf_Rule = new byte[]{0xc2,0xc0,0xc0,0xc1,0xc0,0xc0,0xc1,0xc0,0xc0,
                           0xc4,0xc0,0xc0,0xc1,0xc0,0xc0,0xc1,0xc0,0xc0,
                           0xc3,0xc0,0xc0,0xc1,0xc0,0xc0,0xc1,0xc0,0xc0,
                           0xc5,0xc0,0xc0,0xc1,0xc0,0xc0,0xc1,0xc0,0xc0
                          };
    }
}
