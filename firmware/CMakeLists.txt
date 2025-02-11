cmake_minimum_required(VERSION 3.13)
find_package(Git REQUIRED)
execute_process(COMMAND "${GIT_EXECUTABLE}" rev-parse --short HEAD OUTPUT_VARIABLE COMMIT_ID OUTPUT_STRIP_TRAILING_WHITESPACE)
message(STATUS "Computed Git Hash: ${COMMIT_ID}")
add_definitions(-DGIT_HASH="${COMMIT_ID}") # Usable in source code.

set(DEBUG 0)

add_definitions(-DUSBD_MANUFACTURER="Allen Institute")
add_definitions(-DUSBD_PRODUCT="treadmill-driver")

include(${PICO_SDK_PATH}/pico_sdk_init.cmake)

project(treadmill)

set(CMAKE_CXX_STANDARD 17)

# Compile for profiling/debugging/etc. Default: none enabled
add_definitions(-DDEBUG)
#add_definitions(-DDEBUG_HARP_MSG_IN)
#add_definitions(-DDEBUG_HARP_MSG_OUT)

# initialize the Raspberry Pi Pico SDK
pico_sdk_init()

# include the harp core
add_subdirectory(lib/harp.core.rp2040/firmware)
add_subdirectory(lib/rp2040.pio-ads7049)
add_subdirectory(lib/rp2040.pio-ltc264x)

# Handle DEBUG option after adding the core subdirectory.
if (DEBUG)
    add_definitions(-DDEBUG)
endif()

add_executable(${PROJECT_NAME}
    src/main.cpp
)

add_library(pio_encoder
    src/pio_encoder.cpp
)

pico_generate_pio_header(pio_encoder
    ${CMAKE_CURRENT_LIST_DIR}/src/pio_encoder.pio
)

# Where to look for header files.
include_directories(inc)

# Link libraries to the targets that need them.
#target_link_libraries(analog_load_cell pico_stdlib hardware_adc)
target_link_libraries(pio_encoder pico_stdlib hardware_pio)
target_link_libraries(pio_ltc264x pico_stdlib hardware_pio hardware_dma)
target_link_libraries(${PROJECT_NAME}
    pico_stdlib pio_encoder hardware_dma pio_ads7049 pio_ltc264x
    harp_core harp_sync harp_c_app)

# create map/bin/hex/uf2 file in addition to ELF.
pico_add_extra_outputs(${PROJECT_NAME})

if (DEBUG)
    message(WARNING "Debug printf() messages enabled from harp core UART \
            with baud rate 921600.")
    pico_enable_stdio_uart(${PROJECT_NAME} 1)
    # Additional libraries need to have stdio init also.
endif()

set_property(
        TARGET ${PROJECT_NAME}
        APPEND
        PROPERTY ADDITIONAL_CLEAN_FILES ${PROJECT_NAME}.uf2 ${PROJECT_NAME}.elf
                                        ${PROJECT_NAME}.dis ${PROJECT_NAME}.bin
                                        ${PROJECT_NAME}.map ${PROJECT_NAME}.hex
                                        ${PROJECT_NAME}.elf.map)
