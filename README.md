A Harp Device for controlling an instrumented variable-friction mouse treadmill

![overview](./assets/pics/overview_diagram.png)

## Features
* linear (no switching!) current control of 12V magnetic brake, Placid Industries B5ZD, [datasheet](https://placidindustries.com/d/?h=a03be4b).
* reads measurements from an [RTS-10 torque transducer](https://www.transducertechniques.com/rts-torque-sensor.aspx#)
* wheatstone bridge input for torque transducer
* quadrature encoder input for position/speed measurements
* Harp-protocol compliant over full-speed USB
* accepts Harp time synchronization input.
* reverse-polarity protected power supply.

## Register Map
[List of Registers](./firmware/docs/register_map.csv)

## Credits
This board design was created in partnership with the team at [Second Order Effects](https://soeffects.com/).
