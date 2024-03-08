#!/usr/bin/env python3
from pyharp.device import Device, DeviceMode
from pyharp.messages import HarpMessage
from struct import unpack
import logging
from time import sleep

#logger = logging.getLogger()
#logger.setLevel(logging.DEBUG)
#logger.addHandler(logging.StreamHandler())
#logger.handlers[-1].setFormatter(
#    logging.Formatter(fmt='%(asctime)s:%(name)s:%(levelname)s: %(message)s'))


COM_PORT = '/dev/ttyACM0' # Windows will be 'COMx'
device = Device("/dev/ttyACM0", "ibl.bin")

try:
    print("Starting sensor values: ", end="")
    bulk_meas = device.send(HarpMessage.ReadS32(35).frame)
    vals = unpack("<lll", bulk_meas._raw_payload)
    print(vals)
    sleep(0.5)

# apply tare values.
    print("Zeroing encoder.")
    device.send(HarpMessage.WriteU8(38, 1 << 0).frame)
    sleep(0.5)

    print("New sensor values: ", end="")
    bulk_meas = device.send(HarpMessage.ReadS32(35).frame)
    vals = unpack("<lll", bulk_meas._raw_payload)
    print(vals)
    sleep(0.5)

    print("Taring reaction torque sensor.")
    device.send(HarpMessage.WriteU8(38, 1 << 1).frame)
    sleep(0.5)

    print("New sensor values: ", end="")
    bulk_meas = device.send(HarpMessage.ReadS32(35).frame)
    vals = unpack("<lll", bulk_meas._raw_payload)
    print(vals)
    sleep(0.5)

    print("Taring brake current sensor.")
    device.send(HarpMessage.WriteU8(38, 1 << 2).frame)
    sleep(0.5)

    print("New sensor values: ", end="")
    bulk_meas = device.send(HarpMessage.ReadS32(35).frame)
    vals = unpack("<lll", bulk_meas._raw_payload)
    print(vals)
    sleep(0.5)

# clear tare values.
    print("Unzeroing encoder.")
    device.send(HarpMessage.WriteU8(38, 1 << 4).frame)
    sleep(0.5)

    print("New sensor values: ", end="")
    bulk_meas = device.send(HarpMessage.ReadS32(35).frame)
    vals = unpack("<lll", bulk_meas._raw_payload)
    print(vals)
    sleep(0.5)

    print("Clearing reaction torque sensor tare.")
    device.send(HarpMessage.WriteU8(38, 1 << 5).frame)
    sleep(0.5)

    print("New sensor values: ", end="")
    bulk_meas = device.send(HarpMessage.ReadS32(35).frame)
    vals = unpack("<lll", bulk_meas._raw_payload)
    print(vals)
    sleep(0.5)

    print("Clearing brake current sensor tare.")
    device.send(HarpMessage.WriteU8(38, 1 << 6).frame)
    sleep(0.5)

    print("New sensor values: ", end="")
    bulk_meas = device.send(HarpMessage.ReadS32(35).frame)
    vals = unpack("<lll", bulk_meas._raw_payload)
    print(vals)
    sleep(0.5)


finally:
    # Close connection
    device.disconnect()
