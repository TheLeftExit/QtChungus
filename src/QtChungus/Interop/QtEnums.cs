// Enum definitions ripped from F12-ing them in the C++ editor in VS Code
// Size-fixed for usage in P/Invokes

public enum QtAlignment : int
{
    AlignLeft = 0x0001,
    AlignLeading = AlignLeft,
    AlignRight = 0x0002,
    AlignTrailing = AlignRight,
    AlignHCenter = 0x0004,
    AlignJustify = 0x0008,
    AlignAbsolute = 0x0010,
    AlignHorizontal_Mask = AlignLeft | AlignRight | AlignHCenter | AlignJustify | AlignAbsolute,

    AlignTop = 0x0020,
    AlignBottom = 0x0040,
    AlignVCenter = 0x0080,
    AlignBaseline = 0x0100,
    // Note that 0x100 will clash with Qt::TextSingleLine = 0x100 due to what the comment above
    // this enum declaration states. However, since Qt::AlignBaseline is only used by layouts,
    // it doesn't make sense to pass Qt::AlignBaseline to QPainter::drawText(), so there
    // shouldn't really be any ambiguity between the two overlapping enum values.
    AlignVertical_Mask = AlignTop | AlignBottom | AlignVCenter | AlignBaseline,

    AlignCenter = AlignVCenter | AlignHCenter
};

public enum QtItemDataRole : int
{
    DisplayRole = 0,
    DecorationRole = 1,
    EditRole = 2,
    ToolTipRole = 3,
    StatusTipRole = 4,
    WhatsThisRole = 5,
    // Metadata
    FontRole = 6,
    TextAlignmentRole = 7,
    BackgroundRole = 8,
    ForegroundRole = 9,
    CheckStateRole = 10,
    // Accessibility
    AccessibleTextRole = 11,
    AccessibleDescriptionRole = 12,
    // More general purpose
    SizeHintRole = 13,
    InitialSortOrderRole = 14,
    // Internal UiLib roles. Start worrying when public roles go that high.
    DisplayPropertyRole = 27,
    DecorationPropertyRole = 28,
    ToolTipPropertyRole = 29,
    StatusTipPropertyRole = 30,
    WhatsThisPropertyRole = 31,
    // QRangeModel support for QML's required property var modelData
    RangeModelDataRole = 40,
    // QRangeModelAdapter support for accessing entire multi-role objects
    RangeModelAdapterRole = 41,

    // Reserved
    UserRole = 0x0100,

    // Used by Qt models
    StandardItemFlagsRole = UserRole - 1,  // QStandardItemModel
    FileInfoRole = UserRole - 4,           // QFileSystemModel
    RemoteObjectsCacheRole = UserRole - 1, // QtRemoteObjects::QAbstractItemModelReplica
};

public enum QtOrientation : int
{
    Horizontal = 0x1,
    Vertical = 0x2
};

public enum QtItemFlag : int
{
    NoItemFlags = 0,
    ItemIsSelectable = 1,
    ItemIsEditable = 2,
    ItemIsDragEnabled = 4,
    ItemIsDropEnabled = 8,
    ItemIsUserCheckable = 16,
    ItemIsEnabled = 32,
    ItemIsAutoTristate = 64,
    ItemNeverHasChildren = 128,
    ItemIsUserTristate = 256,

    Default = ItemIsSelectable | ItemIsEnabled
};