using System.Xml.Serialization;
using ExtraLanscapingToolsCommon.OptionsFramework;

namespace NaturalResourcesBrush
{
    public class Options : IModOptions
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
        [Checkbox("Render terrain topography")]
        public bool terrainTopography { set; get; }

        [XmlIgnore]
        public string FileName => "CSL-ExtraTools.xml";
    }
}