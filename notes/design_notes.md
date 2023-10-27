## RP2040

### Oscillator
The RP2040 accepts either a crystal or CMOS-level clock input
I opted for a precision oscillator instead of a vanilla crystal.
To preserve USB Boot functionality (useful!), I kept the frequency at 12MHz (datasheet pg 12).

This oscillator requires a 0.01-0.1uF bypass capacitor placed very close to the pins.
An example layout is provided in the datasheet.


## USB Isolator
To disconnect the host PC's ground from this device's ground, I added a Full-Speed capable USB isolator.

The downside is that this device cannot be powered via USB from the host pc.


## Analog Front-End

# PCB Layout

## Layers

## References
* [All About USBC: Example Circuits](https://hackaday.com/2023/08/07/all-about-usb-c-example-circuits/)
  * contains schematic for a device to display itself as a full speed USB device.
* [Raspberry Pi Pico Datasheet](https://datasheets.raspberrypi.com/pico/pico-datasheet.pdf)
  * contains schematic for minimal board design.
* [AD9833 Signal Generator First Steps Part 1](https://daumemo.com/diy-ad9833-signal-generator-first-steps-part-1/)
  * Contains effect of load resistance on output voltage.
* [Hardware Design with the RP2040](https://datasheets.raspberrypi.com/rp2040/hardware-design-with-rp2040.pdf)
* [SPI: High Speed Design](https://practicalee.com/spi/)
  * source termination resistor placement.
* [Reduced Printed Circuit Board Emissions with Low-Noise Design Practices](https://ntrs.nasa.gov/api/citations/20120009353/downloads/20120009353.pdf)
* [Stack Exchange: How to Connect USB Connector Shield](https://electronics.stackexchange.com/questions/4515/how-to-connect-usb-connector-shield)
* [Driving Inductive Loads with Power Switches](https://www.ti.com/video/6018730150001#transcript-tab)
* [B5ZD Magnetic Particle Brake datasheet](https://placidindustries.com/d/?h=a03be4b)
  * this design is intended for the 12V variant.
* [Signal Chain Basics 78: How to avoid common mode limitations on instrumentation amplifiers](https://www.planetanalog.com/signal-chain-basics-78-how-to-avoid-common-mode-limitations-on-instrumentation-amplifiers/)
* [Hammond Manufacturing 1455L801 Case Drawing](https://www.hammfg.com/files/parts/pdf/1455L801.pdf)

### Reference Designs
