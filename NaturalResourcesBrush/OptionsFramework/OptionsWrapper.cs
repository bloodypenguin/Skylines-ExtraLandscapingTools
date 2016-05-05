using System;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;

namespace NaturalResourcesBrush.OptionsFramework
{
    public class OptionsWrapper<T> where T : IModOptions
    {
        private static T _instance;

        public static T Options
        {
            get
            {
                if (_instance == null)
                {
                    LoadOptions();
                }
                return _instance;
            }
        }

        private static void LoadOptions()
        {
            try
            {
                _instance = (T)Activator.CreateInstance(typeof(T));
                try
                {
                    var xmlSerializer = new XmlSerializer(typeof(T));
                    using (var streamReader = new StreamReader(_instance.FileName))
                    {
                        var options = (T)xmlSerializer.Deserialize(streamReader);
                        foreach (var propertyInfo in typeof(T).GetProperties())
                        {
                            if (!propertyInfo.CanWrite)
                            {
                                continue;
                            }
                            var value = propertyInfo.GetValue(options, null);
                            propertyInfo.SetValue(_instance, value, null);
                        }
                    }
                }
                catch (FileNotFoundException)
                {
                    SaveOptions();// No options file yet
                }
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        internal static void SaveOptions()
        {
            try
            {
                var xmlSerializer = new XmlSerializer(typeof(T));
                using (var streamWriter = new StreamWriter(_instance.FileName))
                {
                    xmlSerializer.Serialize(streamWriter, _instance);
                }
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }
    }
}