cmake_minimum_required(VERSION 3.13)

#set(DEBUG 1)

include(pico-sdk/pico_sdk_init.cmake)

project(harp-treadmill)

set(CMAKE_CXX_STANDARD 17)

# initialize the Raspberry Pi Pico SDK
pico_sdk_init()

# include the harp core
add_subdirectory(lib/harp-core-rp2040/firmware)
# Handle DEBUG option after adding the core subdirectory.
if (DEBUG)
    add_definitions(-DDEBUG)
endif()

add_executable(${PROJECT_NAME}
    src/main.cpp
)

pico_generate_pio_header(${PROJECT_NAME}
    ${CMAKE_CURRENT_LIST_DIR}/inc/quadrature_encoder.pio
)

#add_library(pio_encoder
#src/pio_encoder
#)


# Where to look for header files.
include_directories(inc)

# Link libraries to the targets that need them.
target_link_libraries(${PROJECT_NAME} pico_stdlib hardware_pio harp_core harp_sync)

# create map/bin/hex/uf2 file in addition to ELF.
pico_add_extra_outputs(${PROJECT_NAME})

if (DEBUG)
    pico_enable_stdio_uart(${PROJECT_NAME} 1)
    # Additional libraries need to have stdio init also.
endif()

