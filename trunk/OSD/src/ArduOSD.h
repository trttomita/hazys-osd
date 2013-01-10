#ifndef OSD_Config_Func_h
#define OSD_Config_Func_h

#include "Max7456.h"
#include "MavlinkClient.h"

typedef unsigned char byte;

enum OSD_ITEM
{
    OSD_ITEM_Cen, OSD_ITEM_Pit, OSD_ITEM_Rol, OSD_ITEM_VBatA, OSD_ITEM_VBatB, OSD_ITEM_GPSats, OSD_ITEM_GPL, OSD_ITEM_GPS,
    // panB_REG Byte has:
    OSD_ITEM_Rose, OSD_ITEM_Head, OSD_ITEM_MavB, OSD_ITEM_HDir, OSD_ITEM_HDis, OSD_ITEM_WDir, OSD_ITEM_WDis, OSD_ITEM_RSSI,
    // panC_REG Byte has:
    OSD_ITEM_CurrA, OSD_ITEM_CurrB, OSD_ITEM_Alt, OSD_ITEM_HAlt, OSD_ITEM_Vel, OSD_ITEM_AS, OSD_ITEM_Thr, OSD_ITEM_FMod, 
    
    OSD_ITEM_Hor, OSD_ITEM_SYS,

    //OSD_ITEM_VBatA_ADC, OSD_ITEM_VBatB_ADC, OSD_ITEM_CurrA_ADC, OSD_ITEM_CurrB_ADC, OSD_ITEM_RSSI_ADC, OSD_ITEM_Alt_R
};

enum OSD_OPTION
{
		OSD_OPT_VBatA_ADC, OSD_OPT_VBatB_ADC, OSD_OPT_CurrA_ADC, OSD_OPT_CurrB_ADC, OSD_OPT_RSSI_ADC, OSD_OPT_M_ISO = 7,	
};

enum AD_ITEM
{
    AD_VBatA,
    AD_VBatB,
    AD_CurrA,
    AD_CurrB,
    AD_RSSI,
    AD_COUNT
};
/*
enum OSD_ALT_CONF
{
    OSD_ALT_BAT_A_ADC = (1UL << 24),
    OSD_ALT_BAT_B_ADC = (1UL << 25),
    OSD_ALT_CUR_A_ADC = (1UL << 26),
    OSD_ALT_CUR_B_ADC = (1UL << 27),
    OSD_ALT_RSSI_ADC = (1UL << 28),
    OSD_ALT_REL_ALT = (1UL << 31)
}*/

struct ad_setting_t
{
    uint8_t channel;
    float	k;
    float b;
};

struct osd_setting_t
{
    uint8_t ver;
    uint8_t option;
    uint32_t enable;
    uint8_t coord[26][2];
    ad_setting_t ad_setting[(int)AD_COUNT];
};



class ArduOSD : public OSD, MavlinkClient
{
public:
    static void Init();
    static void Run();

private:
    // config functions
    static void LoadSetting();
    static void UploadFont();
    static void UploadSetting();
    static void GetSetting();
    static void GetAnalog();
    static void Reboot();

    // data functions
    //static void RequestMavlinkRates();
    //static void ReadMavlink();
    static void ReadMessage();
    static inline void SetHomeVars();

    // draw functions
    static void Draw();
    static void DrawLoadBar();
    static void DrawLogo();
    static void DrawArrow(uint8_t dir);
    static void DrawWaitingHB();
    static inline void DrawHorizon(uint8_t start_col, uint8_t start_row);
    static float analog_read(ad_setting_t& ad_setting);
private:
    
    static volatile uint8_t 	crlf_count;
    static osd_setting_t 			setting;
    static char dst_sym;
    static char spd_sym;
    static float converts;
    static float convertd;
};

#endif // OSD_Config_Func_h

