public static partial class QTypes
{
    // Default types
    public static QBasicType Bool = new("bool", "bool");
    public static QBasicType Int = new("int", "qint32");
    public static QBasicType LongLong = new("long", "qint64");
    public static QBasicType QString = new("string", "QString");
    public static QBasicType QObjectStar = new("QObjectStar", "QObject *");
    public static QBasicType VoidStar = new("nint", "void*");

    public static QEnumType QtAlignment = new("Qt::AlignmentFlag");
    public static QEnumType QBoxLayoutDirection = new("QBoxLayout::Direction");

    // Special default types
    public static readonly QSpecialType IntAmpersand = new("int*", "int&",
        null!,
        x => $"*static_cast<int *>(static_cast<void *>({x}))",
        x => $"(nint){x}",
        null!
    );
    public static readonly QSpecialType CharStarStar = new("byte**", "char**",
        null!,
        x => $"static_cast<char **>(static_cast<void *>({x}))",
        x => $"(nint){x}",
        null!
    );
    public static readonly QSpecialType CharStar = new("byte*", "char*",
        null!,
        x => $"static_cast<char *>(static_cast<void *>({x}))",
        x => $"(nint){x}",
        null!
    );

    public static readonly QSmuggledType CModelIndex = new("CModelIndex", "CModelIndex");
    public static readonly QFunctionPointerType QcSignalRouter_RouteDelegate = new(
        "delegate* unmanaged<QObjectStar, byte*, void>",
        "void (*)(QObject *, const char *)"
    );
    public static readonly QFunctionPointerType QcSynchronizationContext_InvokeDelegate = new(
        "delegate* unmanaged<nint, nint, void>",
        "void (*)(void *, void *)"
    );
}
