cmake_minimum_required(VERSION 2.8)
project( corals )

# includes
include_directories(
  include
)


# find header & source
file(GLOB SOURCES
    src/*.h
    src/*.cpp
)

# remove cmake files

add_executable(${PROJECT_NAME}
  ${SOURCES}
)

find_package(Qt5 COMPONENTS Core Gui REQUIRED)
target_link_libraries(corals PRIVATE Qt5::Core Qt5::Gui)