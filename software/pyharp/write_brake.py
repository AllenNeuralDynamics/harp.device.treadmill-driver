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
TORQUE_LIMITING_TRIGGERED = 41

print("Clearing previous torque limit trigger.")
reply = device.send(HarpMessage.WriteU8(TORQUE_LIMITING_TRIGGERED, 0).frame)
print(f"reply: {reply.message_type.name}")

try:
    # Take user input:
    while True:
        val = input("enter a brake setpoint [0, 65535]:")
        try:
            val = int(val)
        except ValueError:
            print("Error: cannot set value to the amount requested.")
            continue
        brake_reply = device.send(HarpMessage.WriteU16(BRAKE_CURRENT_SETPOINT,
                                                       val).frame)
        print(brake_reply.message_type.name)


except KeyboardInterrupt:
    brake_reply = device.send(HarpMessage.WriteU16(BRAKE_CURRENT_SETPOINT,
                                                   0).frame)
    # Close connection
    device.disconnect()
