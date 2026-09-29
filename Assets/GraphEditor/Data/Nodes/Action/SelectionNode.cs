namespace GraphEditor
{
    [Title("Action", "Common", "Selection")]
    public class SelectionNode : NodeData
    {
        private const int Input1PortId = 0;
        private const int Input2PortId = 1;
        private const int Input3PortId = 3;
        private const int Input4PortId = 4;
        private const int Input5PortId = 5;
        private const int Input6PortId = 6;

        private const int Output1PortId = 7;
        private const int Output2PortId = 8;
        private const int Output3PortId = 9;
        private const int Output4PortId = 10;
        private const int Output5PortId = 11;
        private const int Output6PortId = 12;

        private const string kInput1PortName = "In";
        private const string kInput2PortName = "In1";
        private const string kInput3PortName = "In2";
        private const string kInput4PortName = "In3";
        private const string kInput5PortName = "In4";
        private const string kInput6PortName = "In5";

        private const string kOutput1PortName = "Default";
        private const string kOutput2PortName = "Out1";
        private const string kOutput3PortName = "Out2";
        private const string kOutput4PortName = "Out3";
        private const string kOutput5PortName = "Out4";
        private const string kOutput6PortName = "Out5";

        public SelectionNode() : base()
        {
            Name = "Selection";
            UpdateNodeAfterDeserialization();
        }

        public override void UpdateNodeAfterDeserialization()
        {
            AddSlot(new SlotData() { Id = Input1PortId, Name = kInput1PortName, Types = new SlotData.SlotType() { Direction = SlotDirection.Input, Feature = SlotFeatureType.Action} });
            AddSlot(new SlotData() { Id = Input2PortId, Name = kInput2PortName, Types = new SlotData.SlotType() { Direction = SlotDirection.Input, Feature = SlotFeatureType.Basic, ValueType = SlotValueType.String } });
            AddSlot(new SlotData() { Id = Input3PortId, Name = kInput3PortName, Types = new SlotData.SlotType() { Direction = SlotDirection.Input, Feature = SlotFeatureType.Basic, ValueType = SlotValueType.String } });
            AddSlot(new SlotData() { Id = Input4PortId, Name = kInput4PortName, Types = new SlotData.SlotType() { Direction = SlotDirection.Input, Feature = SlotFeatureType.Basic, ValueType = SlotValueType.String } });
            AddSlot(new SlotData() { Id = Input5PortId, Name = kInput5PortName, Types = new SlotData.SlotType() { Direction = SlotDirection.Input, Feature = SlotFeatureType.Basic, ValueType = SlotValueType.String } });
            AddSlot(new SlotData() { Id = Input6PortId, Name = kInput6PortName, Types = new SlotData.SlotType() { Direction = SlotDirection.Input, Feature = SlotFeatureType.Basic, ValueType = SlotValueType.String } });

            AddSlot(new SlotData() { Id = Output1PortId, Name = kOutput1PortName, Types = new SlotData.SlotType() { Direction = SlotDirection.Output, Feature = SlotFeatureType.Action } });
            AddSlot(new SlotData() { Id = Output2PortId, Name = kOutput2PortName, Types = new SlotData.SlotType() { Direction = SlotDirection.Output, Feature = SlotFeatureType.Action } });
            AddSlot(new SlotData() { Id = Output3PortId, Name = kOutput3PortName, Types = new SlotData.SlotType() { Direction = SlotDirection.Output, Feature = SlotFeatureType.Action } });
            AddSlot(new SlotData() { Id = Output4PortId, Name = kOutput4PortName, Types = new SlotData.SlotType() { Direction = SlotDirection.Output, Feature = SlotFeatureType.Action } });
            AddSlot(new SlotData() { Id = Output5PortId, Name = kOutput5PortName, Types = new SlotData.SlotType() { Direction = SlotDirection.Output, Feature = SlotFeatureType.Action } });
            AddSlot(new SlotData() { Id = Output6PortId, Name = kOutput6PortName, Types = new SlotData.SlotType() { Direction = SlotDirection.Output, Feature = SlotFeatureType.Action } });
        }
    }
}


