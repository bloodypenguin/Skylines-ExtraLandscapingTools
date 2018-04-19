using System;
using System.Collections.Generic;
using System.Reflection;
using ColossalFramework;
using ColossalFramework.UI;
using NaturalResourcesBrush.RedirectionFramework.Attributes;
using NaturalResourcesBrush.Utils;

namespace NaturalResourcesBrush.Detours
{
    [TargetType(typeof(TerrainPanel))]
    public class TerrainPanelDetour : GeneratedScrollPanel
    {
        private static readonly PositionData<TerrainTool.Mode>[] kTools = ColossalFramework.Utils.GetOrderedEnumData<TerrainTool.Mode>();

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
            var ditchButton = (UIButton)SpawnEntry(panel, "Ditch", ++index );
            ditchButton.atlas = Util.CreateAtlasFromEmbeddedResources(Assembly.GetExecutingAssembly(), "NaturalResourcesBrush.resources", new List<string> { "TerrainDitch" });
            if(ToolsModifierControl.toolController.m_mode == ItemClass.Availability.ThemeEditor)
            {
                var sandButton = (UIButton)SpawnEntry(panel, "Sand", index);
                sandButton.atlas = UIView.GetAView().defaultAtlas;
                sandButton.normalFgSprite = "ResourceSand";
                sandButton.hoveredFgSprite = "ResourceSandHovered";
                sandButton.pressedFgSprite = "ResourceSandPressed";
                sandButton.focusedFgSprite = "ResourceSandFocused";
            }
        }

        [RedirectMethod]        
        protected override void OnButtonClicked(UIComponent comp)
        {
            if (ToolsModifierControl.toolController.m_mode == ItemClass.Availability.ThemeEditor)
            {
                int zOrder = comp.zOrder;
                TerrainTool terrainTool = null;
                ResourceTool resourceTool = null;
                var panel = (TerrainPanel)Convert.ChangeType(this, typeof(TerrainPanel));
                if (zOrder < kTools.Length + 1)
                {
                    terrainTool = ToolsModifierControl.SetTool<TerrainTool>();
                    if (terrainTool == null)
                    {
                        return;
                    }
                    ShowUndoTerrainOptionsPanel(panel, true);
                    UIView.library.Show("LandscapingInfoPanel");
                }
                else
                {
                    resourceTool = ToolsModifierControl.SetTool<ResourceTool>();
                    if (resourceTool == null)
                        return;
                    UIView.library.Hide("LandscapingInfoPanel");
                    ShowUndoTerrainOptionsPanel(panel, false);
                }
                ShowBrushOptionsPanel(panel, true);

                if (zOrder == 1 || zOrder == 3)
                    ShowLevelHeightPanel(panel, true);
                else
                    ShowLevelHeightPanel(panel, false);
                //begin mod
                if (zOrder < kTools.Length)
                {
                    terrainTool.m_mode = kTools[zOrder].enumValue;
                    TerrainToolDetour.isDitch = false;
                }
                else
                {
                    if (zOrder < kTools.Length + 1)
                    {
                        terrainTool.m_mode = TerrainTool.Mode.Shift;
                        TerrainToolDetour.isDitch = true;
                    }
                    else
                    {
                        resourceTool.m_resource = NaturalResourceManager.Resource.Sand;
                    }
                }
                //end mod
            }
            else
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
        }
        public override ItemClass.Service service { get; }
    }
}