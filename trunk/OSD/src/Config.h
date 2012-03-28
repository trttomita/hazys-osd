
// Versio number, incrementing this will erase/upload factory settings.
// Only devs should increment this
#define VER 1

// EEPROM Stepping, be careful not to overstep.
// We reserved floats for just to be sure if some values needs to be
// changed in future.
// byte  = 1
// int   = 4
// float = 8

// Panel 8bit REGISTER with BIT positions
// panA_REG Byte has:
/*
#define Cen_BIT        0
#define Pit_BIT        1
#define Rol_BIT        2
#define BatA_BIT       3
#define BatB_BIT       4  //(!Not implemented)
#define GPSats_BIT     5
#define GPL_BIT        6
#define GPS_BIT        7

// panB_REG Byte has:
#define Rose_BIT       (8+0)
#define Head_BIT       (8+1)
#define MavB_BIT       (8+2)
#define HDir_BIT       (8+3)
#define HDis_BIT       (8+4)
#define WDir_BIT       (8+5) //(!Not implemented)
#define WDis_BIT       (8+6) //(!Not implemented)
#define RSSI_BIT       (8+7) //(!Not implemented)

// panC_REG Byte has:
#define CurA_BIT       (16+0) //(!Not implemented)
#define CurB_BIT       (16+1) //(!Not implemented)
#define Alt_BIT        (16+2)
#define Vel_BIT        (16+3)
#define Thr_BIT        (16+4)
#define FMod_BIT       (16+5)
#define Hor_BIT        (16+6)
//#define XXC_BIT      7 //Free
*/


#define TELEMETRY_SPEED  57600  // How fast our MAVLink telemetry is coming to Serial port
#define BOOTTIME         2000   // Time in milliseconds that we show boot loading bar and wait user input

#define MinimOSD