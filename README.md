QtChungus is a .NET wrapper around the [Qt Widgets](https://doc.qt.io/qt-6/qtwidgets-index.html) library.

Technical overview:
- Designed to be distributed as a NuGet package.
- NativeAOT-compatible, designed for performance and reasonably minimal allocations.
- [QtChungus.Cxx](src/QtChungus.Cxx/) is a C++ project that exposes Qt to C# code, distributed as source code inside the NuGet package.
- [QtChungus.targets](src/QtChungus/QtChungus.targets) (imported via the NuGet package) builds `QtChungus.Cxx` directly on the consumer's machine, and statically links to it during NativeAOT publish (or emits a dynamic library for non-NativeAOT).
- [QtChungus](src/QtChungus/) exposes Qt widgets in a "builder-like"/"MVVM-like" pattern. See [Program.cs](src/QtChungus.Demo/Program.cs) for an example of consuming code.