cmake_minimum_required(VERSION 3.13)

include(pico-sdk/pico_sdk_init.cmake)

project(harp-treadmill)

# Enable try/catch exceptions. (Required for string parsing implementation.)
set(PICO_CXX_ENABLE_EXCEPTIONS 1)

# Use modern conventions like std::invoke
set(CMAKE_CXX_STANDARD 17)

# initialize the Raspberry Pi Pico SDK
pico_sdk_init()


add_executable(${PROJECT_NAME}
    src/main.cpp
)

add_library(pio_encoder
src/pio_encoder
)

pico_generate_pio_header(${PROJECT_NAME} ${CMAKE_CURRENT_LIST_DIR}/inc/quadrature_encoder.pio)

# Where to look for header files.
include_directories(inc)

# Link libraries to the targets that need them.
target_link_libraries(${PROJECT_NAME} pico_stdlib hardware_pio)

# create map/bin/hex/uf2 file in addition to ELF.
pico_add_extra_outputs(${PROJECT_NAME})

# USB serial. Eventually implement this natively with cdc calls from tinyusb.
pico_enable_stdio_usb(${PROJECT_NAME} 1)
pico_enable_stdio_uart(${PROJECT_NAME} 0)

# Additional libraries need to have stdio init also.
