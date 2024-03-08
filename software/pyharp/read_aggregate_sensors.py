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

# Read all measurements from the aggregate register.
try:
    while True:
        bulk_meas = device.send(HarpMessage.ReadS32(35).frame)
        # Data comes in 3 S32s, but S32[1] and S32[2] have smaller full scale range.
        vals = unpack("<lll", bulk_meas._raw_payload)
        print(vals)
        sleep(0.015);
finally:
    # Close connection
    device.disconnect()
