#!/usr/bin/env python3
from pyharp.device import Device, DeviceMode
from pyharp.messages import HarpMessage
from pyharp.messages import MessageType
from pyharp.messages import CommonRegisters as Regs
from struct import *
import logging
import os
from time import sleep, perf_counter

#logger = logging.getLogger()
#logger.setLevel(logging.DEBUG)
#logger.addHandler(logging.StreamHandler())
#logger.handlers[-1].setFormatter(
#    logging.Formatter(fmt='%(asctime)s:%(name)s:%(levelname)s: %(message)s'))


# Open the device and print the info on screen
# Open serial connection and save communication to a file
if os.name == 'posix': # check for Linux.
    #device = Device("/dev/harp_device_00", "ibl.bin")
    device = Device("/dev/ttyACM0", "ibl.bin")
else: # assume Windows.
    device = Device("COM95", "ibl.bin")

# Read encoder and torque raw measurements.
try:
    while True:
        #encoder_meas = device.send(HarpMessage.ReadU16(32).frame)
        torque_meas = device.send(HarpMessage.ReadU32(36).frame)
        print("Torque:")
        print(torque_meas.payload)
        #print()
        sleep(0.015);
finally:
    # Close connection
    device.disconnect()
