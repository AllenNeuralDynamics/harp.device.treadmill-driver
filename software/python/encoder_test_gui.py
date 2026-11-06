# /// script
# requires-python = ">=3.11"
# dependencies = ["harp-serial>=0.5", "harp-device>=0.5", "harp-protocol>=0.5"]
# ///
"""Bench-test GUI for the treadmill board: live encoder position and Harp timestamp.

Usage:
    uv run software/python/encoder_test_gui.py [--port /dev/cu.usbmodemXXXX]
"""

import argparse
import glob
import queue
import sys
import time
import tkinter as tk
from pathlib import Path

from harp.device.schema import create_device_module
from harp.serial import open_device

DEVICE_YML = Path(__file__).resolve().parents[2] / "device.yml"
# AMT102-V default is 2048 PPR; the firmware counts all 4 quadrature edges.
DEFAULT_TICKS_PER_REV = 2048 * 4


def default_port() -> str | None:
    ports = sorted(glob.glob("/dev/cu.usbmodem*") + glob.glob("/dev/ttyACM*"))
    return ports[0] if ports else None


class EncoderGui:
    def __init__(self, root: tk.Tk, dev, treadmill, ticks_per_rev: int, rate: int):
        self.root = root
        self.dev = dev
        self.treadmill = treadmill
        self.ticks_per_rev = ticks_per_rev
        self.rate = rate
        self.samples: queue.SimpleQueue[tuple[int, float]] = queue.SimpleQueue()
        self.event_times: list[float] = []
        self.last_event = time.monotonic()

        root.title("Treadmill Encoder Test")
        root.protocol("WM_DELETE_WINDOW", self.close)

        frame = tk.Frame(root, padx=16, pady=16)
        frame.pack(fill="both", expand=True)

        tk.Label(frame, text="Position (rev)").grid(row=0, column=0, sticky="w")
        self.rev_var = tk.StringVar(value="—")
        tk.Label(frame, textvariable=self.rev_var, font=("Menlo", 36, "bold"), width=12,
                 anchor="e").grid(row=1, column=0, columnspan=2, sticky="ew")
        self.ticks_var = tk.StringVar(value="ticks: —")
        tk.Label(frame, textvariable=self.ticks_var, font=("Menlo", 12)).grid(
            row=2, column=0, columnspan=2, sticky="e")

        tk.Label(frame, text="Last timestamp (s)").grid(row=3, column=0, sticky="w", pady=(12, 0))
        self.ts_var = tk.StringVar(value="")
        tk.Entry(frame, textvariable=self.ts_var, state="readonly", font=("Menlo", 14),
                 width=20, justify="right").grid(row=4, column=0, columnspan=2, sticky="ew")

        self.status_var = tk.StringVar(value="waiting for events…")
        tk.Label(frame, textvariable=self.status_var, fg="gray").grid(
            row=5, column=0, sticky="w", pady=(12, 0))
        tk.Button(frame, text="Tare", command=self.tare).grid(
            row=5, column=1, sticky="e", pady=(12, 0))
        frame.columnconfigure(0, weight=1)

        dev.subscribe(treadmill.SensorData, self.on_event)
        dev.write(treadmill.SensorDataDispatchRate, rate)
        root.after(30, self.poll)

    def on_event(self, msg) -> None:
        # Runs on the harp event thread; hand off to the Tk thread.
        self.samples.put((int(msg.payload.encoder), msg.timestamp))

    def poll(self) -> None:
        latest = None
        now = time.monotonic()
        while True:
            try:
                latest = self.samples.get_nowait()
            except queue.Empty:
                break
            self.event_times.append(now)
            self.last_event = now
        if latest is not None:
            ticks, ts = latest
            self.rev_var.set(f"{ticks / self.ticks_per_rev:+.3f}")
            self.ticks_var.set(f"ticks: {ticks}")
            self.ts_var.set(f"{ts:.6f}")
        self.event_times = [t for t in self.event_times if now - t <= 1.0]
        if now - self.last_event > 1.0:
            self.status_var.set("NO EVENTS: board disconnected or stopped?")
        else:
            self.status_var.set(f"{len(self.event_times)} events/s (set: {self.rate} Hz)")
        self.root.after(30, self.poll)

    def tare(self) -> None:
        try:
            self.dev.write(self.treadmill.TareSensors, self.treadmill.Sensors.ENCODER)
        except Exception as exc:
            self.status_var.set(f"tare failed: {exc}")

    def close(self) -> None:
        try:
            self.dev.write(self.treadmill.SensorDataDispatchRate, 0)
        except Exception:
            pass  # board already gone; nothing to stop
        self.dev.close()
        self.root.destroy()


def main() -> None:
    parser = argparse.ArgumentParser(description=__doc__.splitlines()[0])
    parser.add_argument("--port", default=default_port(), help="serial port of the board")
    parser.add_argument("--ticks-per-rev", type=int, default=DEFAULT_TICKS_PER_REV,
                        help=f"encoder ticks per revolution (default {DEFAULT_TICKS_PER_REV})")
    parser.add_argument("--rate", type=int, default=100, help="SensorData event rate in Hz")
    args = parser.parse_args()
    if args.port is None:
        sys.exit("No serial port found; pass --port.")

    treadmill = create_device_module(DEVICE_YML.read_bytes())
    dev = open_device(treadmill, port=args.port)  # validates WhoAmI
    root = tk.Tk()
    EncoderGui(root, dev, treadmill, args.ticks_per_rev, args.rate)
    root.mainloop()


if __name__ == "__main__":
    main()
