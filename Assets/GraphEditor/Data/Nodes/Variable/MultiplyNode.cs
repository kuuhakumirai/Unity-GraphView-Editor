namespace GraphEditor
{
    [Title("Variable", "Multiply")]
    public class MultiplyNode : NodeData
    {
        private const int Input1PortId = 0;
        private const int Input2PortId = 1;
        private const int Input3PortId = 2;
        private const int OutputPortId = 3;

        private const string kInput1PortName = "In";
        private const string kInput2PortName = "A";
        private const string kInput3PortName = "B";
        private const string kOutputPortName = "Out";

        public MultiplyNode() : base()
        {
            Name = "Multiply";
            UpdateNodeAfterDeserialization();
        }

        public override void UpdateNodeAfterDeserialization()
        {
            AddSlot(new SlotData() { Id = Input1PortId, Name = kInput1PortName, Types = new SlotData.SlotType() { Direction = SlotDirection.Input, Feature = SlotFeatureType.Action } });
            AddSlot(new SlotData() { Id = Input2PortId, Name = kInput2PortName, Types = new SlotData.SlotType() { Direction = SlotDirection.Input, Feature = SlotFeatureType.Basic, ValueType = SlotValueType.IntProperty } });
            AddSlot(new SlotData() { Id = Input3PortId, Name = kInput3PortName, Types = new SlotData.SlotType() { Direction = SlotDirection.Input, Feature = SlotFeatureType.Basic, ValueType = SlotValueType.Int } });
            AddSlot(new SlotData() { Id = OutputPortId, Name = kOutputPortName, Types = new SlotData.SlotType() { Direction = SlotDirection.Output, Feature = SlotFeatureType.Action } });
        }
    }
}