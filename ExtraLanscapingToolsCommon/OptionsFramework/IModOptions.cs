using System.Xml.Serialization;

namespace ExtraLanscapingToolsCommon.OptionsFramework
{
    public interface IModOptions
    {
        [XmlIgnore]
        string FileName
        {
            get;
        }
    }
}