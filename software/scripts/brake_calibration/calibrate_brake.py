#!/usr/bin/env python3
"""Calibration Script for extacting raw data to linearize magnetic brake."""

from pyharp.device import Device
from pyharp.messages import HarpMessage
from time import sleep, perf_counter
from serial import SerialException
from struct import unpack
from tqdm import tqdm
import matplotlib.pyplot as plt
import numpy as np
import argparse

ALL_SENSOR_REG = 35

device = None

def set_motor_speed_signed(percent: float):
    """Set the motor speed in the percent range [-100: 100]."""
    # Map values to signed range 0 - 4095 where 2048 is midscale.
    pass

def set_brake_current_raw(raw_current):
    """Set the brake current in raw uint16 format."""
    pass

def get_measurements() -> list:
    """Read the [position, torque, current] measurements from the board."""
    #bulk_meas = device.send(HarpMessage.ReadS32(35).frame)
    #vals = unpack("<lll", bulk_meas._raw_payload)
    #return vals # TODO: return a dict?
    return [0, int(perf_counter()/1000), 0]


if __name__ == "__main__":
    # Collect input params.
    parser = argparse.ArgumentParser()
    parser.add_argument("--port", type=str, default="/dev/ttyACM0",
                        help="Harp Treadmill Driver com port")
    parser.add_argument("--min_current", type=float, default=0,
                        help="minimum current value in [%]")
    parser.add_argument("--max_current", type=float, default=50,
                        help="maximum current value in [%]")
    parser.add_argument("--sample_average_count", type=int, default=10,
                        help="number of samples to average together "
                             "*per* data point (default: 10)")
    parser.add_argument("--reverse", type=bool, default=False)
    args = parser.parse_args()

    #try:
    #    device = Device(args.port, "/dev/null")
    #except SerialException as e:
    #    print()
    #    print("Cannot connect to Harp Treadmill device! Is it plugged in and "
    #          "powered on? Is the com port correct?")
    #    print()
    #    raise e

    max_uint16 = np.iinfo(np.uint16).max
    # Range should be in steps of 4 since the value is 12-bit upscaled to 16-bit
    input_current = np.arange(0, round(max_uint16/args.max_current), 4)
    output_torque = np.zeros(len(input_current))

    for i, current in enumerate(tqdm(input_current)):
        set_brake_current_raw(current)
        raw_pts = []
        for _ in range(args.sample_average_count):
            sleep(0.001)
            raw_pts.append(get_measurements()[1])
        output_torque[i] = float(sum(raw_pts))/args.sample_average_count

    # Plot Output current vs input current!
    plt.plot(input_current, output_torque, 'go--', linewidth=2, markersize=12)
    plt.show()
