namespace GraphEditor
{
    [Title("Condition", "If (Variable)")]
    public class IfVariableNode : NodeData
    {
        private const int Input1PortId = 0;
        private const int Input2PortId = 1;
        private const int Input3PortId = 2;
        private const int Input4PortId = 3;
        private const int Input5PortId = 4;
        private const int Output1PortId = 5;
        private const int Output2PortId = 6;

        private const string kInput1PortName = "In";
        private const string kInput2PortName = "Target";
        private const string kInput3PortName = "Constant";
        private const string kInput4PortName = "Variable";
        private const string kInput5PortName = "V";
        private const string kOutput1PortName = "True";
        private const string kOutput2PortName = "False";

        public IfVariableNode() : base()
        {
            Name = "IfVariable";
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
                    ValueType = SlotValueType.IntProperty,
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
                    ValueType = SlotValueType.Int,
                },
            });

            AddSlot(new SlotData()
            {
                Id = Input4PortId,
                Name = kInput4PortName,
                Types = new SlotData.SlotType()
                {
                    Direction = SlotDirection.Input,
                    Feature = SlotFeatureType.Basic,
                    ValueType = SlotValueType.IntProperty,
                },
            });

            AddSlot(new SlotData()
            {
                Id = Input5PortId,
                Name = kInput5PortName,
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
