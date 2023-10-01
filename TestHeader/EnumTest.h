enum SimpleEnum
{
    SimpleEnum_EnumValue1,
    SimpleEnum_EnumValue2,
    SimpleEnum_EnumValue3
};

enum SimpleEnumWithValue
{
    SimpleEnumWithValue_EnumValue1 = 1,
    SimpleEnumWithValue_EnumValue2 = 3,
    SimpleEnumWithValue_EnumValue3 = 9
};

enum ExpresionEnum
{
    ExpresionEnum_EnumValue1 = 1 * 4,
    ExpresionEnum_EnumValue2 = 1 << 3,
    ExpresionEnum_EnumValue3 = (10 + 3) * 3
};

enum ExpresionEnum
{
    ExpresionEnum_EnumValue1 = 1 * 4,
    ExpresionEnum_EnumValue2 = 1 << 3,
    ExpresionEnum_EnumValue3 = (10 + 3) * 3
};

enum RefEnum
{
    ExpresionEnum_EnumValue1 = SimpleEnum_EnumValue1,
    ExpresionEnum_EnumValue2 = ExpresionEnum_EnumValue2,
    ExpresionEnum_EnumValue3 = ExpresionEnum_EnumValue1
};


enum RefEnum
{
    ExpresionEnum_EnumValue1 = SimpleEnum_EnumValue1,
    ExpresionEnum_EnumValue2 = ExpresionEnum_EnumValue2,
    ExpresionEnum_EnumValue3 = ExpresionEnum_EnumValue1
};



