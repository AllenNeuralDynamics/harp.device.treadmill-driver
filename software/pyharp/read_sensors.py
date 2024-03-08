#!/usr/bin/env python3
from pyharp.device import Device, DeviceMode
from pyharp.messages import HarpMessage
import logging
import os
from time import sleep, perf_counter

#logger = logging.getLogger()
#logger.setLevel(logging.DEBUG)
#logger.addHandler(logging.StreamHandler())
#logger.handlers[-1].setFormatter(
#    logging.Formatter(fmt='%(asctime)s:%(name)s:%(levelname)s: %(message)s'))

COM_PORT = "/dev/ttyACM0" # Windows will be 'COMx'
device = Device(COM_PORT, "ibl.bin")

# Read encoder and torque raw measurements.
try:
    while True:
        encoder_meas = device.send(HarpMessage.ReadU32(32).frame)
        brake_current_meas = device.send(HarpMessage.ReadS16(33).frame)
        torque_meas = device.send(HarpMessage.ReadS16(34).frame)
        print("Encoder:")
        print(encoder_meas.payload)
        print("Brake current:")
        print(brake_current_meas.payload)
        print("Torque:")
        print(torque_meas.payload)
        print()
        sleep(0.015);
finally:
    # Close connection
    device.disconnect()
