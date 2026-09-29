namespace GraphEditor
{
    [Title("Action", "Image", "Display")]
    public class DisplayImageNode : NodeData
    {
        private const int Input1PortId = 0;
        private const int Input2PortId = 1;
        private const int OutputPortId = 2;

        private const string kInput1PortName = "In";
        private const string kInput2PortName = "Image";
        private const string kOutputPortName = "Out";

        public DisplayImageNode() : base()
        {
            Name = "DisplayImage";
            UpdateNodeAfterDeserialization();
        }

        public override void UpdateNodeAfterDeserialization()
        {
            AddSlot(new SlotData() { Id = Input1PortId, Name = kInput1PortName, Types = new SlotData.SlotType() { Direction = SlotDirection.Input, Feature = SlotFeatureType.Action } });
            AddSlot(new SlotData() { Id = Input2PortId, Name = kInput2PortName, Types = new SlotData.SlotType() { Direction = SlotDirection.Input, Feature = SlotFeatureType.Basic, ValueType = SlotValueType.Sprite } });
            AddSlot(new SlotData() { Id = OutputPortId, Name = kOutputPortName, Types = new SlotData.SlotType() { Direction = SlotDirection.Output, Feature = SlotFeatureType.Action } });
        }
    }
}

