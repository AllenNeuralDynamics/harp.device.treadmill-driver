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


BRAKE_CURRENT_SETPOINT = 37
TORQUE_LIMIITING = 40
TORQUE_LIMIT_TRIGGERED = 41

reply = device.send(HarpMessage.ReadU8(TORQUE_LIMIT_TRIGGERED).frame)
print(f"Starting torque limit trigger value: {reply.payload}")

print("Enabling max brake torque.")
brake_reply = device.send(HarpMessage.WriteU16(BRAKE_CURRENT_SETPOINT, 65535).frame)
print(f"Reply: {brake_reply.message_type.name}")

print("Waiting for messages.")
try:
    # Take user input:
    while True:
        event_response = device._read()
        if event_response is not None:
            print()
            print(event_response)
except KeyboardInterrupt:
    # Clear brake setpoint and torque limit
    brake_reply = device.send(HarpMessage.WriteU16(BRAKE_CURRENT_SETPOINT, 0).frame)
    brake_reply = device.send(HarpMessage.WriteU8(TORQUE_LIMIT_TRIGGERED, 0).frame)
    # Close connection
    device.disconnect()
