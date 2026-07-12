cmake_minimum_required(VERSION 3.16)

project(QtGlue LANGUAGES CXX)

set(CMAKE_CXX_STANDARD 17)
set(CMAKE_CXX_STANDARD_REQUIRED ON)

find_package(Qt6 REQUIRED COMPONENTS Widgets)

qt_standard_project_setup()

include_directories(
    CStruct
)

option(QTGLUE_SHARED OFF)

if(QTGLUE_SHARED)
    set(QTGLUE_TYPE SHARED)
    set(QTGLUE_SHARED_SHIM SharedLibShim.cxx)
else()
    set(QTGLUE_TYPE STATIC)
endif()

qt_add_library(QtGlue
    ${QTGLUE_TYPE}
    Classes.cxx
    Properties.cxx
    SignalRouter.cxx
    QModel.cxx

    CStruct/Base.cxx
    CStruct/CVariant.cxx
    CStruct/CModelIndex.cxx
    
    ${QTGLUE_SHARED_SHIM}
)

target_link_libraries(QtGlue PRIVATE Qt6::Widgets)