using System;
using System.Collections.Generic;
using ColossalFramework;
using ColossalFramework.UI;
using NaturalResourcesBrush.Redirection;

namespace NaturalResourcesBrush.Detours
{
    [TargetType(typeof(TerrainPanel))]
    public class TerrainPanelDetour : GeneratedScrollPanel
    {
        private static readonly PositionData<TerrainTool.Mode>[] kTools = Utils.GetOrderedEnumData<TerrainTool.Mode>();

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
            ditchButton.atlas = Util.CreateAtlasFromEmbeddedResources("NaturalResourcesBrush.resources", new List<string> { "TerrainDitch" });
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