namespace GraphEditor
{
    [Title("Action", "Image", "Clear")]
    public class ClearImage : NodeData
    {
        private const int InputPortId = 0;
        private const int OutputPortId = 2;
        private const string kInputPortName = "In";
        private const string kOutputPortName = "Out";

        public ClearImage() : base()
        {
            Name = "ClearImage";
            UpdateNodeAfterDeserialization();
        }

        public override void UpdateNodeAfterDeserialization()
        {
            base.UpdateNodeAfterDeserialization();
            AddSlot(new SlotData { Id = InputPortId, Name = kInputPortName, Types = new SlotData.SlotType() { Direction = SlotDirection.Input, Feature = SlotFeatureType.Action } });
            AddSlot(new SlotData() { Id = OutputPortId, Name = kOutputPortName, Types = new SlotData.SlotType() { Direction = SlotDirection.Output, Feature = SlotFeatureType.Action } });
        }
    }
}
