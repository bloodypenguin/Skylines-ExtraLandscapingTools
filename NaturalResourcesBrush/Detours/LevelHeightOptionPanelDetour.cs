using ColossalFramework.UI;
using NaturalResourcesBrush.Redirection;

namespace NaturalResourcesBrush.Detours
{
    [TargetType(typeof(LevelHeightOptionPanel))]
    public class LevelHeightOptionPanelDetour : LevelHeightOptionPanel
    {

        private static UISlider m_HeightSlider;

        public static void Dispose()
        {
            m_HeightSlider = null;
        }

        [RedirectMethod]
        private void SetHeight(float height)
        {
            TerrainToolDetour.m_startPosition.y = height;
        }

        [RedirectMethod]
        private void Update()
        {
            if (!this.component.isVisible)
                return;
            if (m_HeightSlider == null)
            {
                m_HeightSlider = this.Find<UISlider>("Height");
            }
            m_HeightSlider.value = TerrainToolDetour.m_startPosition.y;
        }
    }
}