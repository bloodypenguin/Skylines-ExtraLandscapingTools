using System.Xml.Serialization;

namespace NaturalResourcesBrush.OptionsFramework
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