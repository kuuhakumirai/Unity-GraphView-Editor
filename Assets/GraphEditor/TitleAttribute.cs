using System;

namespace GraphEditor
{
    public class TitleAttribute : ContextFilterableAttribute
    {
        public string[] title;
        public TitleAttribute(params string[] title) { this.title = title; }
    }


    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public abstract class ContextFilterableAttribute : Attribute
    {
    }
}
