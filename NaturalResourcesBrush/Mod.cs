using ICities;
using NaturalResourcesBrush.OptionsFramework.Extensions;
using NaturalResourcesBrush.TranslationFramework;

namespace NaturalResourcesBrush
{

    public class Mod : IUserMod
    {
        public static Translation translation = new Translation();

        public string Name => "Extra Landscaping Tools";

        public string Description => translation.GetTranslation("ELT_DESCRIPTION");

        public void OnSettingsUI(UIHelperBase helper)
        {
            helper.AddOptionsGroup<Options>(s => translation.GetTranslation(s));
        }
    }
}