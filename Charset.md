# Update MAX7456 Character Set #

The hazy's OSD (Minim OSD) base code has an optimized way to draw panels fast on screen. This is possible due a special technique to write panels with reduced number of SPI transactions and also by the replacement of the original MAX7456 character set to a new one which is indexed as the ASCII table.

If you got a .mcm file from our Download Section you can use our configuration tool as explained below.

## ArduCAM OSD Config (Update CharSet) ##

  1. Connect your FTDI Serial adapter (Like this one);
  1. Select the proper Serial COM Port (on the example bellow is COM12. Your adapter may use another one);
  1. Go to menu Options and select Update CharSet.
  1. Select the desired .mcm file from your PC and press Open;

![http://hazys-osd.googlecode.com/svn/wiki/image/Serial.jpg](http://hazys-osd.googlecode.com/svn/wiki/image/Serial.jpg)

When the process starts, you board go to the upload process in few seconds.
If there is a video monitor attached to the OSD, the screen will turn to black.
You have to observe the loading bar on the bottom of ArduCAM OSD Config screen.

![http://hazys-osd.googlecode.com/svn/wiki/image/charset.jpg](http://hazys-osd.googlecode.com/svn/wiki/image/charset.jpg)