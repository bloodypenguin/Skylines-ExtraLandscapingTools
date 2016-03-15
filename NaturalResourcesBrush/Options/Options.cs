using System;
using System.IO;
using System.Xml.Serialization;
using Debug = UnityEngine.Debug;

namespace NaturalResourcesBrush.Options
{
    public class Options
    {
        public Options()
        {
            resourcesTool = true;
            waterTool = true;
            terrainTool = true;
            treeBrush = true;
            treePencil = true;
            dirtLimits = false;
        }

        [Checkbox("Natural Resources Tool")]
        public bool resourcesTool { set; get; }
        [Checkbox("Water Tool")]
        public bool waterTool { set; get; }
        [Checkbox("Terrain Tool Extensions")]
        public bool terrainTool { set; get; }
        [Checkbox("Tree Brush")]
        public bool treeBrush { set; get; }
        [Checkbox("Tree Pencil")]
        public bool treePencil { set; get; }
        [Checkbox("Dirt Limits")]
        public bool dirtLimits { set; get; }
    }

    public static class OptionsHolder
    {
        public static Options Options = new Options();
    }

    public static class OptionsLoader
    {
        private const string FileName = "CSL-ExtraTools.xml";

        public static void LoadOptions()
        {
            try
            {
                try
                {
                    var xmlSerializer = new XmlSerializer(typeof(Options));
                    using (var streamReader = new StreamReader(FileName))
                    {
                        OptionsHolder.Options = (Options)xmlSerializer.Deserialize(streamReader);
                    }
                }
                catch (FileNotFoundException)
                {
                    // No options file yet
                }
            }
            catch (Exception e)
            {
                Debug.LogErrorFormat("Unexpected {0} while loading options: {1}\n{2}",
                    e.GetType().Name, e.Message, e.StackTrace);
            }
        }

        public static void SaveOptions()
        {
            try
            {
                var xmlSerializer = new XmlSerializer(typeof(Options));
                using (var streamWriter = new StreamWriter(FileName))
                {
                    xmlSerializer.Serialize(streamWriter, OptionsHolder.Options);
                }
            }
            catch (Exception e)
            {
                Debug.LogErrorFormat("Unexpected {0} while saving options: {1}\n{2}",
                    e.GetType().Name, e.Message, e.StackTrace);
            }
        }
    }
}