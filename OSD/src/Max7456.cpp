#include "Max7456.h"
//#include "WProgram.h"
#include "spi.h"
#include "time.h"

//volatile int x;
//volatile int font_count;
//volatile byte character_bitmap[0x40];

uint8_t OSD::row;
uint8_t OSD::col;
uint8_t OSD::video_center;
uint8_t OSD::video_mode;

//------------------ init ---------------------------------------------------

void OSD::init()
{
    //PIN_MODE(PORTB, PB3,OUTPUT);
    //PIN_MODE(MAX7456_VSYNC, INPUT);
    //DIGITAL_WRITE(MAX7456_VSYNC,HIGH); //enabling pull-up resistor
    DDRB |= _BV(PB3);

    detectMode();

    //DIGITAL_WRITE(PORTB, PB3,LOW);
    select();;
    //read black level register
    spi_transfer(MAX7456_OSDBL_reg_read);//black level read register
    byte osdbl_r = spi_transfer(0xff);
    spi_transfer(MAX7456_VM0_reg);
    spi_transfer(MAX7456_RESET | video_mode);
    delay(50);
    //set black level
    byte osdbl_w = (osdbl_r & 0xef); //Set bit 4 to zero 11101111
    spi_transfer(MAX7456_OSDBL_reg); //black level write register
    spi_transfer(osdbl_w);

    // set all rows to same charactor white level, 90%
    for (uint8_t x = 0; x < MAX7456_screen_rows; x++)
    {
        spi_transfer(x + 0x10);
        spi_transfer(MAX7456_WHITE_level_120);
    }
    // define sync (auto,int,ext) and
    // making sure the Max7456 is enabled
    control(1);
}

//------------------ Detect Mode (PAL/NTSC) ---------------------------------

void OSD::detectMode()
{
    select();;

    //read STAT and auto detect Mode PAL/NTSC
    spi_transfer(MAX7456_STAT_reg_read);//status register
    byte osdstat_r = spi_transfer(0xff);

    setMode((0x02 & osdstat_r)?MAX7456_MODE_NTCS:MAX7456_MODE_PAL);
    deSelect();;
}

//------------------ Set Mode (PAL/NTSC) ------------------------------------

void OSD::setMode(int themode)
{
    video_mode = themode;
    video_center = themode == MAX7456_MODE_NTCS? MAX7456_CENTER_NTSC: MAX7456_CENTER_PAL;
}


//------------------ clear ---------------------------------------------------

void OSD::clear()
{
    // clear the screen
    //DIGITAL_WRITE(PORTB, PB3,LOW);
    select();;
    spi_transfer(MAX7456_DMM_reg);
    spi_transfer(MAX7456_CLEAR_display);
    deSelect();;
    //DIGITAL_WRITE(PORTB, PB3,HIGH);
}

//------------------ set panel -----------------------------------------------

void OSD::setPanel(uint8_t st_col, uint8_t st_row)
{
    col = st_col;
    row = st_row;
}

//------------------ open panel ----------------------------------------------

void OSD::openPanel(void)
{
    unsigned int linepos;
    byte settings, char_address_hi, char_address_lo;

    //find [start address] position
    linepos = row*30+col;

    // divide 16 bits into hi & lo byte
    char_address_hi = linepos >> 8;
    char_address_lo = linepos;

    //Auto increment turn writing fast (less SPI commands).
    //No need to set next char address. Just send them
    settings = MAX7456_INCREMENT_auto; //To Enable DMM Auto Increment
    //DIGITAL_WRITE(PORTB, PB3,LOW);
    select();;
    spi_transfer(MAX7456_DMM_reg); //dmm
    spi_transfer(settings);

    spi_transfer(MAX7456_DMAH_reg); // set start address high
    spi_transfer(char_address_hi);

    spi_transfer(MAX7456_DMAL_reg); // set start address low
    spi_transfer(char_address_lo);
    //Serial.printf("setPos -> %d %d\n", col, row);
}

//------------------ close panel ---------------------------------------------

void OSD::closePanel(void)
{
    spi_transfer(MAX7456_DMDI_reg);
    spi_transfer(MAX7456_END_string); //This is needed "trick" to finish auto increment
    deSelect();;
    row++; //only after finish the auto increment the new row will really act as desired
}

//------------------ write single char ---------------------------------------------

void OSD::openSingle(uint8_t x, uint8_t y)
{
    unsigned int linepos;
    byte char_address_hi, char_address_lo;

    //find [start address] position
    linepos = y*30+x;

    // divide 16 bits into hi & lo byte
    char_address_hi = linepos >> 8;
    char_address_lo = linepos;

    //DIGITAL_WRITE(PORTB, PB3,LOW);
    select();;

    spi_transfer(MAX7456_DMAH_reg); // set start address high
    spi_transfer(char_address_hi);

    spi_transfer(MAX7456_DMAL_reg); // set start address low
    spi_transfer(char_address_lo);
    //Serial.printf("setPos -> %d %d\n", col, row);
}

//------------------ write ---------------------------------------------------

void OSD::write(uint8_t c)
{
    if(c == '|')
    {
        closePanel(); //It does all needed to finish auto increment and change current row
        openPanel(); //It does all needed to re-enable auto increment
    }
    else
    {
        spi_transfer(MAX7456_DMDI_reg);
        spi_transfer(c);
    }
}

//---------------------------------

void OSD::control(uint8_t ctrl)
{
    select();;
    spi_transfer(MAX7456_VM0_reg);
    switch(ctrl)
    {
    case 0:
        spi_transfer(MAX7456_DISABLE_display | video_mode);
        break;
    case 1:
        //spi_transfer((MAX7456_ENABLE_display_vert | video_mode) | MAX7456_SYNC_internal);
        //spi_transfer((MAX7456_ENABLE_display_vert | video_mode) | MAX7456_SYNC_external);
        spi_transfer((MAX7456_ENABLE_display_vert | video_mode) | MAX7456_SYNC_autosync);
        break;
    }
    //DIGITAL_WRITE(PORTB, PB3,HIGH);
    deSelect();;
}

void OSD::write_NVM(int font_count, uint8_t *character_bitmap)
{
    byte x;
    byte char_address_hi, char_address_lo;
    byte screen_char;

    char_address_hi = font_count;
    char_address_lo = 0;
//Serial.println("write_new_screen");

    // disable display
    //DIGITAL_WRITE(PORTB, PB3,LOW);
    select();;
    spi_transfer(MAX7456_VM0_reg);
    spi_transfer(MAX7456_DISABLE_display);

    spi_transfer(MAX7456_CMAH_reg); // set start address high
    spi_transfer(char_address_hi);

    for(x = 0; x < NVM_ram_size; x++) // write out 54 (out of 64) bytes of character to shadow ram
    {
        screen_char = character_bitmap[x];
        spi_transfer(MAX7456_CMAL_reg); // set start address low
        spi_transfer(x);
        spi_transfer(MAX7456_CMDI_reg);
        spi_transfer(screen_char);
    }

    // transfer a 54 bytes from shadow ram to NVM
    spi_transfer(MAX7456_CMM_reg);
    spi_transfer(WRITE_nvr);

    // wait until bit 5 in the status register returns to 0 (12ms)
    while ((spi_transfer(MAX7456_STAT_reg_read) & STATUS_reg_nvr_busy) != 0x00);

    spi_transfer(MAX7456_VM0_reg); // turn on screen next vertical
    spi_transfer(MAX7456_ENABLE_display_vert);
    //DIGITAL_WRITE(PORTB, PB3,HIGH);
    deSelect();;
}

void OSD::print_P(const prog_char *s)
{
    char    c;

    while ('\0' != (c = pgm_read_byte((const prog_char *)s++)))
        write(c);
}

/*
void
BetterStream::println_P(const prog_char *s)
{
    print_P(s);
    //println();
    write('\n');
}*/

void OSD::printf(const char *fmt, ...)
{
    va_list ap;

    va_start(ap, fmt);
    _vprintf(0, fmt, ap);
    va_end(ap);
}

void OSD::printf_P(const prog_char *fmt, ...)
{
    va_list ap;

    va_start(ap, fmt);
    _vprintf(1, fmt, ap);
    va_end(ap);
}

//------------------ pure virtual ones (just overriding) ---------------------
/*
int  OSD::available(void)
{
    return 0;
}

int  OSD::read(void)
{
    return 0;
}
int  OSD::peek(void)
{
    return 0;
}
void OSD::flush(void)
{
}*/

//OSD osd;