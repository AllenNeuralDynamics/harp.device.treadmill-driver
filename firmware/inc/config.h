#ifndef CONFIG_H
#define CONFIG_H

#define HW_VERSION_MAJOR (0)
#define HW_VERSION_MINOR (2)
#define HW_VERSION_PATCH (1)

#define FW_VERSION_MAJOR (0)
#define FW_VERSION_MINOR (2)
#define FW_VERSION_PATCH (0)

#define UART_TX_PIN (0) // for printf-style debugging.
#define HARP_SYNC_RX_PIN (5)
#define LED1 (25)

#define ENCODER_BASE_PIN (16) // 16 = A, 17 = B

#define MAX_EVENT_FREQUENCY_HZ (1000)


#define TREADMILL_HARP_DEVICE_ID (0x057A)

#endif // CONFIG_H
