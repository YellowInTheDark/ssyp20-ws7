namespace QR_testing_tools
{
    enum SegmentMode
    {
        ECI                = 0b0111,
        Numeric            = 0b0001,
        Alphanumeric       = 0b0010,
        Byte               = 0b0100,
        Kanji              = 0b1000,
        StructuredAppend   = 0b0011,
        FNC1FirstPosition  = 0b0101,
        FNC1SecondPosition = 0b1001,
        Terminator         = 0b0000
    }
}
