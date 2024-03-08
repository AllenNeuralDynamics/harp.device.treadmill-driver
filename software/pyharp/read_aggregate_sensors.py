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

# Read encoder and torque raw measurements.
try:
    while True:
        bulk_meas = device.send(HarpMessage.ReadU32(35).frame)
        # Data comes in 3 U32s, but U32[1] and U32[2] are actually S16 values.
        vals = unpack("<Lhxxhxx", bulk_meas._raw_payload)
        print(vals)
        sleep(0.015);
finally:
    # Close connection
    device.disconnect()
