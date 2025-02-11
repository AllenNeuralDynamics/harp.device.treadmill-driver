#ifndef CONFIG_H
#define CONFIG_H

#define HW_VERSION_MAJOR (0)
#define HW_VERSION_MINOR (2)
#define HW_VERSION_PATCH (1)

#define FW_VERSION_MAJOR (0)
#define FW_VERSION_MINOR (1)
#define FW_VERSION_PATCH (1)

#define UART_TX_PIN (0) // for printf-style debugging.
#define HARP_SYNC_RX_PIN (5)
#define LED0 (24)
#define LED1 (25)

#define ENCODER_BASE_PIN (16) // 16 = A, 17 = B

// Brake Current ADC
#define BRAKE_CURRENT_CS_PIN (18)
#define BRAKE_CURRENT_POCI_PIN (19)
#define BRAKE_CURRENT_SCK_PIN (20)

// Torque Transducer ADC
#define TORQUE_TRANSDUCER_CS_PIN (9)
#define TORQUE_TRANSDUCER_POCI_PIN (10)
#define TORQUE_TRANSDUCER_SCK_PIN (11)

// Torque Transducer safety limits.
// Note: not all transducers can physcially reach 0 and 4095 extremes.
#define RAW_TORQUE_SENSOR_MIN (100)
#define RAW_TORQUE_SENSOR_MAX (3995) // 12-bit.

// Brake Setpoint DAC
#define BRAKE_SETPOINT_CS_PIN (23)
#define BRAKE_SETPOINT_PICO_PIN (21)
#define BRAKE_SETPOINT_SCK_PIN (22)

#define MAX_EVENT_FREQUENCY_HZ (1000)


#define TREADMILL_HARP_DEVICE_ID (0x057A)

#endif // CONFIG_H
