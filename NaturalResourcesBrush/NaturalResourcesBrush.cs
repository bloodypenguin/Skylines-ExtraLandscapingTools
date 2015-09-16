using System;
using ICities;

namespace NaturalResourcesBrush
{
    [Flags]
    public enum ModOptions : long
    {
        None = 0,
        ResourcesTool = 1,
        WaterTool = 2,
        TerrainTool = 4,
        TreeBrush = 8,
        TreePencil = 16
    }
    
    public class NaturalResourcesBrush : IUserMod
    {
        public static ModOptions Options = ModOptions.None;

        public string Name
        {
            get
            {
                OptionsLoader.LoadOptions();
                return "In-game Natural Resources Tool + Tree Brush & Pencil + Water Tool + Terrain Tool";
            }
        }

        public string Description
        {
            get { return "Provides some Map Editor tools in-game"; }
        }

        public void OnSettingsUI(UIHelperBase helper)
        {
            UIHelperBase group = helper.AddGroup("Extra Tools Options");
            group.AddCheckbox("Resources Tool", (Options & ModOptions.ResourcesTool) != 0,
                (b) =>
                {
                    if (b)
                    {
                        Options |= ModOptions.ResourcesTool;
                    }
                    else
                    {
                        Options &= ~ModOptions.ResourcesTool;
                    }
                    OptionsLoader.SaveOptions();
                });
            group.AddCheckbox("Water Tool", (Options & ModOptions.WaterTool) != 0,
                (b) =>
                {
                    if (b)
                    {
                        Options |= ModOptions.WaterTool;
                    }
                    else
                    {
                        Options &= ~ModOptions.WaterTool;
                    }
                    OptionsLoader.SaveOptions();
                });
            group.AddCheckbox("Terrain Tool", (Options & ModOptions.TerrainTool) != 0,
                (b) =>
                {
                    if (b)
                    {
                        Options |= ModOptions.TerrainTool;
                    }
                    else
                    {
                        Options &= ~ModOptions.TerrainTool;
                    }
                    OptionsLoader.SaveOptions();
                });
            group.AddCheckbox("Tree Brush", (Options & ModOptions.TreeBrush) != 0,
                (b) =>
                {
                    if (b)
                    {
                        Options |= ModOptions.TreeBrush;
                    }
                    else
                    {
                        Options &= ~ModOptions.TreeBrush;
                    }
                    OptionsLoader.SaveOptions();
                });
            group.AddCheckbox("Tree Pencil", (Options & ModOptions.TreePencil) != 0,
                (b) =>
                {
                    if (b)
                    {
                        Options |= ModOptions.TreePencil;
                    }
                    else
                    {
                        Options &= ~ModOptions.TreePencil;
                    }
                    OptionsLoader.SaveOptions();
                });

        }
    }
}