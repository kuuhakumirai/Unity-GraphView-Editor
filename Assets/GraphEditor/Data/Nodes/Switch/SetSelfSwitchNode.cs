namespace GraphEditor
{
    [Title("Switch", "SetSelfSwitch")]
    public class SetSelfSwitchNode : NodeData
    {
        private const int Input1PortId = 0;
        private const int Input2PortId = 1;
        private const int Input3PortId = 2;
        private const int OutputPortId = 3;

        private const string kInput1PortName = "In";
        private const string kInput2PortName = "SelfSwitch";
        private const string kInput3PortName = "V";
        private const string kOutputPortName = "Out";

        public SetSelfSwitchNode() : base()
        {
            Name = "SetSelfSwitch";
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
                Id = OutputPortId,
                Name = kOutputPortName,
                Types = new SlotData.SlotType()
                {
                    Direction = SlotDirection.Output,
                    Feature = SlotFeatureType.Action,
                }
            });
        }
    }
}

