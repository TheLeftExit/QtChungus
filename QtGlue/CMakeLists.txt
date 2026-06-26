cmake_minimum_required(VERSION 3.16)

project(QtGlue LANGUAGES CXX)

set(CMAKE_CXX_STANDARD 17)
set(CMAKE_CXX_STANDARD_REQUIRED ON)

find_package(Qt6 REQUIRED COMPONENTS Widgets)

qt_standard_project_setup()

qt_add_library(QtGlue STATIC
    Classes.cxx
    Properties.cxx
    SignalRouter.cxx
    Model.cxx
)

target_link_libraries(QtGlue PRIVATE Qt6::Widgets)