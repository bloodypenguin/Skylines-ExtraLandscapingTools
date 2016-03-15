using System;
using System.Collections.Generic;
using System.Reflection;
using ColossalFramework;
using ColossalFramework.UI;
using NaturalResourcesBrush.Redirection;

namespace NaturalResourcesBrush
{
    [TargetType(typeof(TerrainPanel))]
    public class TerrainPanelDetour : GeneratedScrollPanel
    {
        private static readonly PositionData<TerrainTool.Mode>[] kTools = Utils.GetOrderedEnumData<TerrainTool.Mode>();

        private static Dictionary<MethodInfo, RedirectCallsState> _redirects;

        public static void Deploy()
        {
            if (_redirects != null)
            {
                return;
            }
            _redirects = RedirectionUtil.RedirectType(typeof(TerrainPanelDetour));
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
        }

        [RedirectReverse]
        private static UIButton SpawnEntry(TerrainPanel panel, string name, int index)
        {
            UnityEngine.Debug.Log($"SpawnEntry-{panel}-{name}-{index}");
            return null;
        }

        [RedirectReverse]
        private static void ShowUndoTerrainOptionsPanel(TerrainPanel panel, bool show)
        {
            UnityEngine.Debug.Log($"ShowUndoTerrainOptionsPanel-{panel}-{show}");
        }

        [RedirectReverse]
        private static void ShowBrushOptionsPanel(TerrainPanel panel, bool show)
        {
            UnityEngine.Debug.Log($"ShowBrushOptionsPanel-{panel}-{show}");
        }

        [RedirectReverse]
        private static void ShowLevelHeightPanel(TerrainPanel panel, bool show)
        {
            UnityEngine.Debug.Log($"ShowLevelHeightPanel-{panel}-{show}");
        }

        [RedirectMethod]
        protected override void OnHideOptionBars()
        {
            var panel = (TerrainPanel)Convert.ChangeType(this, typeof(TerrainPanel));
            ShowBrushOptionsPanel(panel, false);
            ShowUndoTerrainOptionsPanel(panel, false);
            ShowLevelHeightPanel(panel, false);
            //begin mod
            UIView.library.Hide("LandscapingInfoPanel");
            //end mod
        }

        [RedirectMethod]
        public override void RefreshPanel()
        {
            base.RefreshPanel();
            int index;
            var panel = (TerrainPanel)Convert.ChangeType(this, typeof(TerrainPanel));
            for (index = 0; index < TerrainPanelDetour.kTools.Length; ++index)
                SpawnEntry(panel, TerrainPanelDetour.kTools[index].enumName, index );
            var ditchButton = (UIButton)SpawnEntry(panel, "Ditch", index );
            ditchButton.atlas = Util.CreateAtlasFromEmbeddedResources(new List<string> { "TerrainDitch" });
        }

        [RedirectMethod]
        protected override void OnButtonClicked(UIComponent comp)
        {
            int zOrder = comp.zOrder;
            TerrainTool terrainTool = ToolsModifierControl.SetTool<TerrainTool>();
            if (terrainTool == null)
            {
                return;
            }
            var panel = (TerrainPanel)Convert.ChangeType(this, typeof(TerrainPanel));
            ShowUndoTerrainOptionsPanel(panel, true);
            ShowBrushOptionsPanel(panel, true);
            //begin mod
            UIView.library.Show("LandscapingInfoPanel");
            //end mod
            if (zOrder == 1 || zOrder == 3)
                ShowLevelHeightPanel(panel, true);
            else
                ShowLevelHeightPanel(panel, false);
            //begin mod
            if (zOrder < kTools.Length)
            {
                terrainTool.m_mode = TerrainPanelDetour.kTools[zOrder].enumValue;
                TerrainToolDetour.isDitch = false;
            }
            else
            {
                terrainTool.m_mode = TerrainTool.Mode.Shift;
                TerrainToolDetour.isDitch = true;
            }
            //end mod
        }

        public override ItemClass.Service service { get; }
    }
}