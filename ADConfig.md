# Analog Digital Conversion Configuration #

This OSD firmware support using Analog/Digital (AD) Conversion for reading useful information. Currently supported functions including Voltage A/B, Current A/B and RSSI.

However, the AD conversion need additional configuration, including channels, actual value per digital bit, and actual value at zero reading, as explained in the following text.

![http://hazys-osd.googlecode.com/svn/wiki/image/adc.jpg](http://hazys-osd.googlecode.com/svn/wiki/image/adc.jpg)

  1. **Function**: Choose the function that needs to config from the drop down list.
  1. **Channel**: Choose which pin is connected to corresponding signal. Refer to the electrical schematic from your hardware manufacturer for more detail.
  1. **A/D conversion parameters**: these parameters controls how a internal reading value represents a actual value. (e.g. reading 1 represents 0.1v, 1v, or 5v?). There are two ways to input these parameters:
    * 3 a) **Raw parameter**:
      1. **Value / bit**: how much does the actual value (i.e. voltage, current, etc) increase while ADC reading increase by 1. Refer to your electrical schematic for exact parameter value.
      1. **Value @ 0**: how much is the actual value when ADC reading is 0.
    * 3 b) **Sampling**: It is quite difficult for users to calculate such complex parameters. Instead, the config tool can calculate those parameters by two sampling:
      1. Measure the actual value using a instrument (e.g. a multimeter), input the value in **Real Value 1**.
      1. Click the **AD Reading 1**, the config tool will ask OSD to sample the signal. The internal reading will be shown beside AD Reading 1.
      1. Change the actual value, measure it and input in **Real Value 2**.
      1. Click the **AD Reading 2** to sample the new signal again.
      1. The Value / bit and Value @ 0 will calculated automatically.
  1. Click "Write to OSD", save settings.


---

## Example ##
### RSSI ###
The RSSI value in this OSD is displayed as parentage, rather than the raw value. in order to show 100%, you should enter 100 as the real value, but not the voltage of the RSSI pin. Here are a simple instruction of setting RSSI:

  1. Open the transmitter, place it at a proper distance from the receiver, let's say about 1m or 2m. That's because some same receivers get bad signals when they are too close to the transmitter.
  1. Input 100 in the Real Value 1, click AD Reading 1 to get ADC value.
  1. Turn off the transmitter, input 0 in the Real Value 2, and click AD Reading 2 to get another value.
  1. Click "Save to OSD"

**Note**: I assume that the radio signal strength is linear to the RSSI voltage. If it is not true, the display value will not be true, either.