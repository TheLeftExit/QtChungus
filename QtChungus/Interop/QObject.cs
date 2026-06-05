using System.Drawing;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;

public readonly unsafe partial record struct QObject(nint Handle)
{
    [LibraryImport(Qt.libraryName)]
    public static partial QObject CreateWidget([MarshalAs(UnmanagedType.LPUTF8Str)] string name, QObject parent);

    [LibraryImport(Qt.libraryName)]
    private static partial char* GetProperty_string(QObject target, [MarshalAs(UnmanagedType.LPUTF8Str)] string name);
    [LibraryImport(Qt.libraryName)]
    private static partial void SetProperty_string(QObject target, [MarshalAs(UnmanagedType.LPUTF8Str)] string name, ReadOnlySpan<char> value);

    public string GetString(string propertyName) => new(GetProperty_string(this, propertyName));
    public void SetString(string propertyName, ReadOnlySpan<char> value) => SetProperty_string(this, propertyName, value);

    [LibraryImport(Qt.libraryName)]
    private static partial long GetProperty_int64(QObject target, [MarshalAs(UnmanagedType.LPUTF8Str)] string name);
    [LibraryImport(Qt.libraryName)]
    private static partial void SetProperty_int64(QObject target, [MarshalAs(UnmanagedType.LPUTF8Str)] string name, long value);

    public T GetInteger<T>(string propertyName) where T : unmanaged, IBinaryInteger<T> => T.CreateChecked(GetProperty_int64(this, propertyName));
    public void SetInteger<T>(string propertyName, T value) where T : unmanaged, IBinaryInteger<T> => SetProperty_int64(this, propertyName, long.CreateChecked(value));

    public bool GetBool(string propertyName) => GetProperty_int64(this, propertyName) != 0;
    public void SetBool(string propertyName, bool value) => SetProperty_int64(this, propertyName, value ? 1 : 0);

    [LibraryImport(Qt.libraryName)]
    private static partial void GetProperty_rect(QObject target, [MarshalAs(UnmanagedType.LPUTF8Str)] string name, Span<int> buffer);
    [LibraryImport(Qt.libraryName)]
    private static partial void SetProperty_rect(QObject target, [MarshalAs(UnmanagedType.LPUTF8Str)] string name, ReadOnlySpan<int> buffer);

    public Rectangle GetRectangle(string propertyName)
    {
        Span<int> buffer = stackalloc int[4];
        GetProperty_rect(this, propertyName, buffer);
        return new(buffer[0], buffer[1], buffer[2], buffer[3]);
    }
    public void SetRectangle(string propertyName, Rectangle rectangle)
    {
        Span<int> buffer = stackalloc int[4] { rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height };
        SetProperty_rect(this, propertyName, buffer);
    }
}