/*
namespace GraphEditor
{
    [Title("Basic", "ScriptableObject", "Armor")]
    public class ArmorNode : NodeData
    {
        private const int InputPortId = 0;
        private const string kInputPortName = "V";
        private const int OutputPortId = 1;
        private const string kOutputPortName = "Out";

        public ArmorNode() : base()
        {
            Name = "Armor";
            UpdateNodeAfterDeserialization();
        }
        public override void UpdateNodeAfterDeserialization()
        {
            AddSlot(new SlotData()
            {
                Id = InputPortId,
                Name = kInputPortName,
                Types = new SlotData.SlotType()
                {
                    Direction = SlotDirection.Input,
                    Feature = SlotFeatureType.Basic,
                    ValueType = SlotValueType.ScriptableObject,
                },
                Value = "",
            });

            AddSlot(new SlotData()
            {
                Id = OutputPortId,
                Name = kOutputPortName,
                Types = new SlotData.SlotType()
                {
                    Direction = SlotDirection.Output,
                    Feature = SlotFeatureType.Basic,
                    ValueType = SlotValueType.ScriptableObject,
                }
            });
        }
    }
}
*/
