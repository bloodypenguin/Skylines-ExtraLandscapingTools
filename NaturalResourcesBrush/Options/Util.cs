using System;
using System.Linq;
using ICities;

namespace NaturalResourcesBrush.Options
{
    public static class Util
    {
        public static string GetPropertyDescription<T>(this T value, string propertyName)
        {
            var fi = value.GetType().GetProperty(propertyName);
            var attributes =
         (CheckboxAttribute[])fi.GetCustomAttributes(typeof(CheckboxAttribute), false);

            if (attributes != null && attributes.Length > 0)
                return attributes[0].Description;

            return propertyName;
        }

        public static Action<bool> GetPropertyAction<T>(this T value, string propertyName)
        {
            var fi = value.GetType().GetProperty(propertyName);
            var attributes =
         (CheckboxAttribute[])fi.GetCustomAttributes(typeof(CheckboxAttribute), false);

            if (attributes == null || attributes.Length != 1 || attributes[0].action == null)
                return b => { };

            var action = attributes[0].action;
            return b =>
            {
               action.Invoke(b);
            };


        }

        public static void AddOptionsGroup(UIHelperBase helper, string groupName)
        {
            var group = helper.AddGroup(groupName);
            var properties = typeof(Options).GetProperties();
            foreach (var name in from property in properties select property.Name)
            {
                var description = OptionsHolder.Options.GetPropertyDescription(name);
                group.AddCheckbox(description, name, OptionsHolder.Options.GetPropertyAction(name));

            }
        }
    }
}