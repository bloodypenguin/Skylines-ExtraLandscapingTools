using System.Collections.Generic;
using System.Reflection;
using ColossalFramework;
using ColossalFramework.DataBinding;
using ColossalFramework.Globalization;
using ColossalFramework.UI;
using NaturalResourcesBrush.Redirection;
using Object = System.Object;

namespace NaturalResourcesBrush.Detours
{
    [TargetType(typeof(LandscapingPanel))]
    public class LandscapingPanelDetour : GeneratedScrollPanel
    {
        private static readonly PositionData<TerrainTool.Mode>[] kTools = Utils.GetOrderedEnumData<TerrainTool.Mode>();
        private static UIPanel m_OptionsUndoTerrainPanel;
        private static UIPanel m_OptionsBrushPanel;
        private static UIPanel m_OptionsLevelHeightPanel;

        public static void Dispose()
        {
            m_OptionsBrushPanel = null;
            m_OptionsLevelHeightPanel = null;
            m_OptionsUndoTerrainPanel = null;
        }

        public static void Initialize()
        {
            var optionsBar = UIView.Find<UIPanel>("OptionsBar");
            if ((Object)optionsBar != (Object)null)
            {
                if ((Object)m_OptionsBrushPanel == (Object)null)
                    m_OptionsBrushPanel = optionsBar.Find<UIPanel>("BrushPanel");
                if ((Object)m_OptionsLevelHeightPanel == (Object)null)
                    m_OptionsLevelHeightPanel = optionsBar.Find<UIPanel>("LevelHeightPanel");
                if ((Object)m_OptionsUndoTerrainPanel == (Object)null)
                    m_OptionsUndoTerrainPanel = optionsBar.Find<UIPanel>("UndoTerrainPanel");
            }
        }

        private static void ShowUndoTerrainOptionsPanel(bool show)
        {
            if (!((Object)m_OptionsUndoTerrainPanel != (Object)null))
                return;
            m_OptionsUndoTerrainPanel.isVisible = show;
            m_OptionsUndoTerrainPanel.zOrder = 2;
        }

        private static void ShowBrushOptionsPanel(bool show)
        {
            if (!((Object)m_OptionsBrushPanel != (Object)null))
                return;
            m_OptionsBrushPanel.isVisible = show;
            m_OptionsBrushPanel.zOrder = 1;
        }

        private static void ShowLevelHeightPanel(bool show)
        {
            if (!((Object)m_OptionsLevelHeightPanel != (Object)null))
                return;
            m_OptionsLevelHeightPanel.isVisible = show;
            m_OptionsLevelHeightPanel.zOrder = 0;
        }

        [RedirectMethod]
        protected override void OnHideOptionBars()
        {
            //begin mod
            ShowBrushOptionsPanel(false);
            ShowUndoTerrainOptionsPanel(false);
            ShowLevelHeightPanel(false);
            //end mod
            UIView.library.Hide("LandscapingInfoPanel");
        }

        [RedirectMethod]
        public override void RefreshPanel()
        {
            base.RefreshPanel();
            int index;
            for (index = 0; index < LandscapingPanelDetour.kTools.Length; ++index)
                SpawnEntry(LandscapingPanelDetour.kTools[index].enumName, true, null);
            //begin mod
            SpawnEntry("Ditch", true, null);
            SpawnEntry("Sand", true, null);
            //end mod
        }

        [RedirectMethod]
        protected override void OnButtonClicked(UIComponent comp)
        {
            int zOrder = comp.zOrder;
            TerrainTool terrainTool = null;
            ResourceTool resourceTool = null;
            if (zOrder < kTools.Length + 1)
            {
                terrainTool = ToolsModifierControl.SetTool<TerrainTool>();
                if (terrainTool == null)
                {
                    return;
                }
                ShowUndoTerrainOptionsPanel(true);
                UIView.library.Show("LandscapingInfoPanel");
            }
            else
            {
                resourceTool = ToolsModifierControl.SetTool<ResourceTool>();
                if (resourceTool == null)
                    return;
                UIView.library.Hide("LandscapingInfoPanel");
                ShowUndoTerrainOptionsPanel(false);
            }
            ShowBrushOptionsPanel(true);

            if (zOrder == 1 || zOrder == 3)
                ShowLevelHeightPanel(true);
            else
                ShowLevelHeightPanel(false);
            //begin mod
            if (zOrder < kTools.Length)
            {
                terrainTool.m_mode = LandscapingPanelDetour.kTools[zOrder].enumValue;
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

        [RedirectMethod]
        private void SpawnEntry(string name, bool enabled, MilestoneInfo info)
        {
            var landscapingInfo = (LandscapingPanel.LandscapingInfo)
                typeof(LandscapingPanel).GetProperty("landscapingInfo", BindingFlags.NonPublic | BindingFlags.Instance)
                    .GetValue(this, null);

            landscapingInfo.m_DirtPrice = Singleton<TerrainManager>.instance.m_properties.m_dirtPrice;
            float cost = (float)((double)landscapingInfo.m_DirtPrice * 65536.0 / 262144.0 * 512.0 / 100.0);
            string str = TooltipHelper.Format(LocaleFormatter.Title, Locale.Get("LANDSCAPING_TITLE", name), LocaleFormatter.Sprite, name, LocaleFormatter.Text, Locale.Get("LANDSCAPING_DESC", name), LocaleFormatter.Locked, (!enabled).ToString(), LocaleFormatter.Cost, LocaleFormatter.FormatCubicCost(cost));
            if (Singleton<UnlockManager>.exists)
            {
                string unlockDesc;
                string currentValue;
                string targetValue;
                string progress;
                string locked;
                ToolsModifierControl.GetUnlockingInfo(info, out unlockDesc, out currentValue, out targetValue, out progress, out locked);
                string addTooltip = TooltipHelper.Format(LocaleFormatter.LockedInfo, locked, LocaleFormatter.UnlockDesc, unlockDesc, LocaleFormatter.UnlockPopulationProgressText, progress, LocaleFormatter.UnlockPopulationTarget, targetValue, LocaleFormatter.UnlockPopulationCurrent, currentValue);
                str = TooltipHelper.Append(str, addTooltip);
            }
            //begin mod
            string buttonName;
            UITextureAtlas buttonAtlas;
            UIComponent tooltipBox;
            if (name == "Ditch")
            {
                buttonName = "TerrainDitch";
                buttonAtlas = Util.CreateAtlasFromEmbeddedResources("NaturalResourcesBrush.resources", new List<string> { "TerrainDitch" });
                tooltipBox = GeneratedPanel.landscapingTooltipBox;
            }
            else if (name == "Sand")
            {
                buttonName = "ResourceSand";
                tooltipBox = GeneratedPanel.tooltipBox;
                buttonAtlas = UIView.GetAView().defaultAtlas;
            }
            else
            {
                buttonName = "Landscaping" + name;
                buttonAtlas = null;
                tooltipBox = GeneratedPanel.landscapingTooltipBox;
            }
            var button = (UIButton)this.SpawnEntry(name, str, buttonName, (UITextureAtlas)buttonAtlas, tooltipBox, enabled);
            button.objectUserData = (object)landscapingInfo;
            //end mod
        }

        public override ItemClass.Service service { get; }
    }
}