QtChungus is an opinionated .NET wrapper around the [Qt Widgets](https://doc.qt.io/qt-6/qtwidgets-index.html) library.

Technical overview:
- Designed to be distributed as a NuGet package.
- NativeAOT-compatible, designed for performance and reasonably minimal allocations.
- [QtChungus.Cxx](src/QtChungus.Cxx/) is a C++ project that exposes Qt to C# code, distributed as source code inside the NuGet package.
- [QtChungus.targets](src/QtChungus/QtChungus.targets) (imported via the NuGet package) builds `QtChungus.Cxx` directly on the consumer's machine, and statically links to it during NativeAOT publish (or emits a dynamic library for non-NativeAOT).  
The build process is designed to be seamless for the package consumer. While it's a good practice to know what happens under the hood, the typical workflow is to import the package and have it "just work". However:
    - Currently, only the Linux build target is implemented,
    - The build environment must have CMake and Qt (headers, libraries and CMake packages) globally available (it should be sufficient to install both from the distro's package manager),
    - `QtChungus` as a repository/library/package only contains original source code and is not susceptible to Qt's licensing restrictions. However, since the imported MSBuild target builds C++ code against Qt headers, and then dynamically links the final executable to Qt libraries, the consuming application will most likely be bound by LGPL. See: [Qt Licensing](https://doc.qt.io/qt-6/licensing.html).
- [QtChungus](src/QtChungus/) exposes Qt widgets in a "builder-like"/"MVVM-like" pattern. See [Program.cs](src/QtChungus.Demo/Program.cs) for an example of consuming code.

Development snapshot available (mind the limitations above): https://git.theleftexit.net/TheLeftExit/-/packages/nuget/qtchungus/

This codebase does not contain any AI-generated/co-authored/autocompleted code.