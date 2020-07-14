namespace QR_testing_tools
{
    enum SegmentMode
    {
        ECI                = 0b0111,//0x10101,
        Numeric            = 0b0001, //0x1000000,
        Alphanumeric       = 0b0010, //0x10000,
        Byte               = 0b0100, //0x100,
        Kanji              = 0b1000, //0x1,
        StructuredAppend   = 0b0011, // 0x101,
        FNC1FirstPosition  = 0b0101, // 0x10001,
        FNC1SecondPosition = 0b1001, // 0x1000001,
        Terminator         = 0b0000 //0x0
    }
}
