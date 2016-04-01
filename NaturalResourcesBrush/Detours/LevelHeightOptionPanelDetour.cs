using System;
using System.Collections.Generic;
using System.Reflection;
using ColossalFramework.UI;
using NaturalResourcesBrush.Redirection;

namespace NaturalResourcesBrush
{
    [TargetType(typeof(LevelHeightOptionPanel))]
    public class LevelHeightOptionPanelDetour : LevelHeightOptionPanel
    {

        private static Dictionary<MethodInfo, RedirectCallsState> _redirects;
        private static UISlider m_HeightSlider;

        public static void Deploy()
        {
            if (_redirects != null)
            {
                return;
            }
            _redirects = RedirectionUtil.RedirectType(typeof(LevelHeightOptionPanelDetour));
        }

        public static void Revert()
        {
            if (_redirects == null)
            {
                return;
            }
            foreach (var redirect in _redirects)
            {
                RedirectionHelper.RevertRedirect(redirect.Key, redirect.Value);
            }
            _redirects = null;

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