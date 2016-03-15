using System;
using ICities;
using NaturalResourcesBrush.Options;

namespace NaturalResourcesBrush
{
 
    public class NaturalResourcesBrush : IUserMod
    {
        private static bool _optionsLoaded;

        public string Name
        {
            get
            {
                if (!_optionsLoaded)
                {
                    OptionsLoader.LoadOptions();
                    _optionsLoaded = true;
                }
                return "Extra Landscaping Tools";
            }
        }

        public string Description => "Provides some Map Editor tools in-game";

        public void OnSettingsUI(UIHelperBase helper)
        {
            Options.Util.AddOptionsGroup(helper, "Extra Landscaping Tools Options");
        }
    }
}