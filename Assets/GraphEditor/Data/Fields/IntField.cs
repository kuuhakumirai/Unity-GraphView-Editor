namespace GraphEditor
{
    public class IntField : FieldData
    {
        public IntField() : base()
        {
            Name = "Int";
            Guid = System.Guid.NewGuid().ToString();
        }
    }
}
