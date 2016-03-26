using ICities;
using NaturalResourcesBrush.OptionsFramework;

namespace NaturalResourcesBrush
{
 
    public class NaturalResourcesBrush : IUserMod
    {

        public string Name => "Extra Landscaping Tools";

        public string Description => "Provides some Map Editor tools in-game";

        public void OnSettingsUI(UIHelperBase helper)
        {
            helper.AddOptionsGroup<Options>();
        }
    }
}