namespace GraphEditor 
{
    [Title("Condition", "If (SelfSwitch)")]
    public class IfSelfSwitchNode : NodeData
    {
        private const int Input1PortId = 0;
        private const int Input2PortId = 1;
        private const int Input3PortId = 2;
        private const int Output1PortId = 3;
        private const int Output2PortId = 4;

        private const string kInput1PortName = "In";
        private const string kInput2PortName = "SelfSwitch";
        private const string kInput3PortName = "V";
        private const string kOutput1PortName = "True";
        private const string kOutput2PortName = "False";

        public IfSelfSwitchNode() : base()
        {
            Name = "IfSelfSwitch";
            UpdateNodeAfterDeserialization();
        }

        public override void UpdateNodeAfterDeserialization()
        {
            AddSlot(new SlotData()
            {
                Id = Input1PortId,
                Name = kInput1PortName,
                Types = new SlotData.SlotType()
                {
                    Direction = SlotDirection.Input,
                    Feature = SlotFeatureType.Action,
                },
            });

            AddSlot(new SlotData()
            {
                Id = Input2PortId,
                Name = kInput2PortName,
                Types = new SlotData.SlotType()
                {
                    Direction = SlotDirection.Input,
                    Feature = SlotFeatureType.Basic,
                    ValueType = SlotValueType.SelfSwitch,
                },
            });


            AddSlot(new SlotData()
            {
                Id = Input3PortId,
                Name = kInput3PortName,
                Types = new SlotData.SlotType()
                {
                    Direction = SlotDirection.Input,
                    Feature = SlotFeatureType.Basic,
                    ValueType = SlotValueType.Enum,
                },
                Value = "0",
            });

            AddSlot(new SlotData()
            {
                Id = Output1PortId,
                Name = kOutput1PortName,
                Types = new SlotData.SlotType()
                {
                    Direction = SlotDirection.Output,
                    Feature = SlotFeatureType.Action,
                }
            });

            AddSlot(new SlotData()
            {
                Id = Output2PortId,
                Name = kOutput2PortName,
                Types = new SlotData.SlotType()
                {
                    Direction = SlotDirection.Output,
                    Feature = SlotFeatureType.Action,
                }
            });
        }
    }
}
