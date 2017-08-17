
using NaturalResourcesBrush.OptionsFramework.Attibutes;

namespace NaturalResourcesBrush
{
    [Options("ExtraLanscapingTools", "CSL-ExtraTools.xml")]
    public class Options
    {
        public Options()
        {
            resourcesTool = true;
            waterTool = true;
            terrainTool = true;
            treeBrush = true;
            treePencil = true;
            terrainTopography = true;
        }

        [Checkbox("ELT_OPTION_RESOURCES_TOOL")]
        public bool resourcesTool { set; get; }
        [Checkbox("ELT_OPTION_WATER_TOOL")]
        public bool waterTool { set; get; }
        [Checkbox("ELT_OPTION_TERRAIN_TOOL")]
        public bool terrainTool { set; get; }
        [Checkbox("ELT_OPTION_TREE_BRUSH")]
        public bool treeBrush { set; get; }
        [Checkbox("ELT_OPTION_TREE_PENCIL")]
        public bool treePencil { set; get; }
        [Checkbox("ELT_OPTION_TOPOGRAPHY")]
        public bool terrainTopography { set; get; }
    }
}