namespace GraphEditor
{
    [Title("Common", "Output")]
    public class OutputNode : NodeData
    {
        private const int InputPortId = 0;
        private const string kInputPortName = "In";

        public OutputNode() : base()
        {
            Name = "Output";
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
                    Feature = SlotFeatureType.Action,
                }
            });
        }
    }
}