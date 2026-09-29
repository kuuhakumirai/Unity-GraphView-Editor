namespace GraphEditor
{
    public class IntProperty : PropertyData
    {
        public IntProperty() : base()
        {
            Name = "Int";
            UpdateNodeAfterDeserialization();
        }

        public override void UpdateNodeAfterDeserialization()
        {
            AddSlot(new SlotData()
            {
                Types = new SlotData.SlotType()
                {
                    Direction = SlotDirection.Output,
                    Feature = SlotFeatureType.Basic,
                    ValueType = SlotValueType.IntProperty,
                },
            });
        }
    }
}
