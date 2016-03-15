using System;
using System.Reflection;

namespace NaturalResourcesBrush.Options
{
    [AttributeUsage(AttributeTargets.Property)]
    public class CheckboxAttribute : Attribute {
        public CheckboxAttribute(string description, Action<bool> action)
        {
            Description = description;
            this.action = action;
        }

        public CheckboxAttribute(string description)
        {
            Description = description;
            action = null;
        }

        public string Description { get; }
        public Action<bool> action { get; }
    }
}