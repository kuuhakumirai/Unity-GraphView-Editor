namespace GraphEditor
{
    [Title("Common", "Entry")]
    public class EntryNode : NodeData
    {
        private const int OutputPortId = 0;
        private const string kOutputPortName = "Out";

        public EntryNode() : base()
        {
            Name = "Entry";
            UpdateNodeAfterDeserialization();
        }

        public override void UpdateNodeAfterDeserialization()
        {
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

