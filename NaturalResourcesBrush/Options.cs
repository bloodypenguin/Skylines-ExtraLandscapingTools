using System;
using System.IO;
using System.Xml.Serialization;
using Debug = UnityEngine.Debug;

namespace NaturalResourcesBrush
{

    public struct Options
    {
        public bool resourcesTool;
        public bool waterTool;
        public bool terrainTool;
        public bool treeBrush;
        public bool treePencil;
    }

    public static class OptionsLoader
    {
        public static void LoadOptions()
        {
            NaturalResourcesBrush.Options = ModOptions.None;
            Options options;
            try
            {
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(Options));
                using (StreamReader streamReader = new StreamReader("CSL-ExtraTools.xml"))
                {
                    options = (Options)xmlSerializer.Deserialize(streamReader);
                }
            }
            catch (FileNotFoundException)
            {
                options = new Options
                {
                    resourcesTool = true,
                    waterTool = true,
                    terrainTool = true,
                    treeBrush = true,
                    treePencil = true,
                };
                SaveOptions(options);
                // No options file yet
            }
            catch (Exception e)
            {
                Debug.LogError("Unexpected " + e.GetType().Name + " loading options: " + e.Message + "\n" + e.StackTrace);
                return;
            }
            if (options.resourcesTool)
                NaturalResourcesBrush.Options |= ModOptions.ResourcesTool;

            if (options.waterTool)
                NaturalResourcesBrush.Options |= ModOptions.WaterTool;

            if (options.terrainTool)
                NaturalResourcesBrush.Options |= ModOptions.TerrainTool;
            if (options.treeBrush)
                NaturalResourcesBrush.Options |= ModOptions.TreeBrush;
            if (options.treePencil)
                NaturalResourcesBrush.Options |= ModOptions.TreePencil;
        }

        public static void SaveOptions()
        {
            Options options = new Options();
            if ((NaturalResourcesBrush.Options & ModOptions.ResourcesTool) != 0)
            {
                options.resourcesTool = true;
            }
            if ((NaturalResourcesBrush.Options & ModOptions.WaterTool) != 0)
            {
                options.waterTool = true;
            }
            if ((NaturalResourcesBrush.Options & ModOptions.TerrainTool) != 0)
            {
                options.terrainTool = true;
            }
            if ((NaturalResourcesBrush.Options & ModOptions.TreeBrush) != 0)
            {
                options.treeBrush = true;
            }
            if ((NaturalResourcesBrush.Options & ModOptions.TreePencil) != 0)
            {
                options.treePencil = true;
            }
            SaveOptions(options);
        }

        public static void SaveOptions(Options options)
        {
            try
            {
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(Options));
                using (StreamWriter streamWriter = new StreamWriter("CSL-ExtraTools.xml"))
                {
                    xmlSerializer.Serialize(streamWriter, options);
                }
            }
            catch (Exception e)
            {
                Debug.LogError("Unexpected " + e.GetType().Name + " saving options: " + e.Message + "\n" + e.StackTrace);
            }
        }
    }
}