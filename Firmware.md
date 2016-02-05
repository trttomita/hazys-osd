# Uploading Firmware #

The basic Remzibi's OSD comes with a MegaLoad bootloader, which allows us to upload firmware through serial port.

### Upload firmware for the first time ###
With original Remzibi's software, the OSD Config tool is unable to reboot the OSD and communicate with the bootloader. Thus you have to select the firmware file first, and then reboot the OSD manually.

  1. Connect your USB-to-TTL adapter (e.g. FTDI table) to your computer
  1. Connect the TX, RX and GND wire to the OSD's serial port, leaving the Power unconnected.
  1. Open OSDConfig tool, select the proper CommPort.
  1. Open the OSD firmware file (.hex)
  1. Connect the Power wire to the OSD. The OSDConfig tool should upload the firmware now.
  1. Wait till the firmware has been uploaded.

![http://hazys-osd.googlecode.com/svn/wiki/image/Serial.jpg](http://hazys-osd.googlecode.com/svn/wiki/image/Serial.jpg)

### Update existing firmware ###

If previous version of hazys-osd software has been uploaded, the OSDConfig tool can  automatically reboot the OSD itself and then talk to the bootloader.

  1. Connect your USB-to-TTL adapter (e.g. FTDI table) to your computer
  1. Connect the TX, RX, GND and Power wire to the OSD's serial port.
  1. Open OSDConfig tool, select the proper CommPort.
  1. Open the OSD firmware file (.hex)
  1. Wait till the firmware has been uploaded.

![http://hazys-osd.googlecode.com/svn/wiki/image/firmware.jpg](http://hazys-osd.googlecode.com/svn/wiki/image/firmware.jpg)