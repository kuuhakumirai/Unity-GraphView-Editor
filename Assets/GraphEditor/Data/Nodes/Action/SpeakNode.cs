namespace GraphEditor
{
    [Title("Action", "Common", "Speak")]
    public class SpeakNode : NodeData
    {
        private const int Input1PortId = 0;
        private const int Input2PortId = 1;
        private const int Input3PortId = 2;
        private const int Input4PortId = 3;

        private const int OutputPortId = 4;

        private const string kInput1PortName = "In";
        private const string kInput2PortName = "Head";
        private const string kInput3PortName = "Name";
        private const string kInput4PortName = "Text";

        private const string kOutputPortName = "Out";


        public SpeakNode() : base()
        {
            Name = "Speak";
            UpdateNodeAfterDeserialization();
        }

        public override void UpdateNodeAfterDeserialization()
        {
            base.UpdateNodeAfterDeserialization();
            AddSlot(new SlotData() { Id = Input1PortId, Name = kInput1PortName, Types = new SlotData.SlotType() { Direction = SlotDirection.Input, Feature = SlotFeatureType.Action } });
            AddSlot(new SlotData() { Id = Input2PortId, Name = kInput2PortName, Types = new SlotData.SlotType() { Direction = SlotDirection.Input, Feature = SlotFeatureType.Basic, ValueType = SlotValueType.Sprite, } });
            AddSlot(new SlotData() { Id = Input3PortId, Name = kInput3PortName, Types = new SlotData.SlotType() { Direction = SlotDirection.Input, Feature = SlotFeatureType.Basic , ValueType = SlotValueType.String, } });
            AddSlot(new SlotData() { Id = Input4PortId, Name = kInput4PortName, Types = new SlotData.SlotType() { Direction = SlotDirection.Input, Feature = SlotFeatureType.Basic , ValueType = SlotValueType.String, } });
            AddSlot(new SlotData() { Id = OutputPortId, Name = kOutputPortName, Types = new SlotData.SlotType() { Direction = SlotDirection.Output, Feature = SlotFeatureType.Action } });
        }
    }
}

