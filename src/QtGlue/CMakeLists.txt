cmake_minimum_required(VERSION 3.16)

project(QtGlue LANGUAGES CXX)

set(CMAKE_CXX_STANDARD 17)
set(CMAKE_CXX_STANDARD_REQUIRED ON)

find_package(Qt6 REQUIRED COMPONENTS Widgets)

qt_standard_project_setup()

qt_add_library(QtGlueObj OBJECT
    Classes.cxx
    Properties.cxx
    SignalRouter.cxx
    QModel.cxx

    CStruct/Base.cxx
    CStruct/CVariant.cxx
    CStruct/CModelIndex.cxx
    
    SharedLibShim.cxx
)

target_include_directories(QtGlueObj PRIVATE
    CStruct
)

target_link_libraries(QtGlueObj PRIVATE Qt6::Widgets)
set_target_properties(QtGlueObj PROPERTIES POSITION_INDEPENDENT_CODE ON)

qt_add_library(QtGlue SHARED $<TARGET_OBJECTS:QtGlueObj>)
qt_add_library(QtGlueStatic STATIC $<TARGET_OBJECTS:QtGlueObj>)

target_link_libraries(QtGlue PRIVATE Qt6::Widgets)
target_link_libraries(QtGlueStatic PRIVATE Qt6::Widgets)
set_target_properties(QtGlueStatic PROPERTIES OUTPUT_NAME QtGlue)