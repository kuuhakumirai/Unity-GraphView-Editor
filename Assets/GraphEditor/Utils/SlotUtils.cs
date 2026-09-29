namespace GraphEditor
{
    public enum SlotDirection
    {
        Input,
        Output
    }
    
    public enum SlotValueType
    {
        Default,
        String,
        Sprite,
        AudioClip,
        Float,
        Int,
        IntProperty,
        SelfSwitch,
        Enum,
        ScriptableObject,
    }

    public static class SlotUtils
    {
        public static string ToClassName(this SlotValueType type)
        {
            return k_SlotValueTypeClassNames[(int)type];
        }

        private static readonly string[] k_SlotValueTypeClassNames =
        {
            null,
            "type_String",
            "type_Sprite",
            "type_AudioClip",
            "type_Float",
            "type_Int",
            "type_IntProperty",
            "type_SelfSwitch",
            "type_Enum",
            "type_ScriptableObject"
        };
    }
}

