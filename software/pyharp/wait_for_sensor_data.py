#!/usr/bin/env python3
from pyharp.device import Device, DeviceMode
from pyharp.messages import HarpMessage
from pyharp.messages import MessageType
from pyharp.messages import CommonRegisters as Regs
from struct import *
import os
from time import sleep, perf_counter


# Open the device and print the info on screen
# Open serial connection and save communication to a file
if os.name == 'posix': # check for Linux.
    #device = Device("/dev/harp_device_00", "ibl.bin")
    device = Device("/dev/ttyACM0", "ibl.bin")
else: # assume Windows.
    device = Device("COM95", "ibl.bin")


print(f"Device mode is: {device.read_device_mode()}.")
print("Waiting for events.")
#reply = device.set_mode(DeviceMode.Active)
start_time = perf_counter()
# Enable sensor message events at 1 Hz
reply = device.send(HarpMessage.WriteU16(36, 1).frame) # >0 = enable dispatch
try:
    while True:
        event_response = device._read()
        if event_response is not None:
            print()
            print(event_response)
except KeyboardInterrupt:
    print("Disabling events")
    reply = device.send(HarpMessage.WriteU16(36, 0).frame) # >0 = enable dispatch
    # Close connection
    device.disconnect()
