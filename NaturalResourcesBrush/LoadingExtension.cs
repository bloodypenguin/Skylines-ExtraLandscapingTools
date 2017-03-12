
using System;
using System.Collections.Generic;
using System.Reflection;
using ColossalFramework.UI;
using ExtraLanscapingToolsCommon.API;
using ExtraLanscapingToolsCommon.OptionsFramework;
using ExtraLanscapingToolsCommon.Redirection;
using ICities;
using NaturalResourcesBrush.Detours;
using UnityEngine;
using Object = UnityEngine.Object;
using Util = ExtraLanscapingToolsCommon.Util;

namespace NaturalResourcesBrush
{
    public class LoadingExtension : LoadingExtensionBase
    {
        private const string LandscapingInfoPanel = "LandscapingInfoPanel";
        private static UIDynamicPanels.DynamicPanelInfo landscapingPanel;

        public override void OnCreated(ILoading loading)
        {
            base.OnCreated(loading);
            if (loading.currentMode == AppMode.Game)
            {
                if (OptionsWrapper<Options>.Options.treeBrush)
                {
                    Redirector<BeautificationPanelDetour>.Deploy();
                }
                if (OptionsWrapper<Options>.Options.waterTool)
                {
                    Redirector<WaterToolDetour>.Deploy();
                }
            }

            //to allow to work in MapEditor
            if (OptionsWrapper<Options>.Options.treePencil)
            {
                Redirector<TreeToolDetour>.Deploy();
            }
            if (OptionsWrapper<Options>.Options.terrainTool)
            {
                Redirector<TerrainToolDetour>.Deploy();
                Redirector<TerrainPanelDetour>.Deploy();
                Redirector<LandscapingPanelDetour>.Deploy();
                Redirector<LevelHeightOptionPanelDetour>.Deploy();
                Redirector<UndoTerrainOptionPanelDetour>.Deploy();
            }
            Redirector<BrushOptionPanelDetour>.Deploy();
            Util.AddLocale("LANDSCAPING", "Ditch", Mod.translation.GetTranslation("ELT_DITCH_BUTTON"), "");
            Util.AddLocale("TERRAIN", "Ditch", Mod.translation.GetTranslation("ELT_DITCH_BUTTON"), "");
            Util.AddLocale("LANDSCAPING", "Sand", Mod.translation.GetTranslation("ELT_SAND_BUTTON"),
                Mod.translation.GetTranslation("ELT_SAND_DESCRIPTION"));
            Util.AddLocale("TUTORIAL_ADVISER", "Resource", "Ground Resources Tool", "");
            Util.AddLocale("TUTORIAL_ADVISER", "Water", "Water Tool", "");
            try
            {
                Plugins.Initialize();
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogException(e);
            }
        }

        public override void OnReleased()
        {
            Redirector<TreeToolDetour>.Revert();
            Redirector<BeautificationPanelDetour>.Revert();
            BeautificationPanelDetour.Dispose();
            Redirector<WaterToolDetour>.Revert();
            Redirector<TerrainToolDetour>.Revert();
            TerrainToolDetour.Dispose();
            Redirector<TerrainPanelDetour>.Revert();
            Redirector<LandscapingPanelDetour>.Revert();
            LandscapingPanelDetour.Dispose();
            Redirector<LevelHeightOptionPanelDetour>.Revert();
            LevelHeightOptionPanelDetour.Dispose();
            Redirector<UndoTerrainOptionPanelDetour>.Revert();
            Redirector<BrushOptionPanelDetour>.Revert();
            try
            {
                Plugins.Dispose();
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogException(e);
            }
        }

        public override void OnLevelLoaded(LoadMode mode)
        {
            var toolController = ToolsModifierControl.toolController;
            if (toolController == null)
            {
                Debug.LogError("ExtraTools#OnLevelLoaded(): ToolContoller not found");
                return;
            }
            try
            {
                var extraTools = NaturalResourcesBrush.SetUpExtraTools(mode, toolController);
                NaturalResourcesBrush.AddExtraToolsToController(toolController, extraTools);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
            finally
            {
                if (toolController.Tools.Length > 0)
                {
                    toolController.Tools[0].enabled = true;
                }
            }
            if (OptionsWrapper<Options>.Options.terrainTool && GetPanels().ContainsKey(LandscapingInfoPanel))
            {
                landscapingPanel = GetPanels()[LandscapingInfoPanel];
                if (landscapingPanel != null)
                {
                    GetPanels().Remove(LandscapingInfoPanel);
                }
                LandscapingPanelDetour.Initialize();
            }
        }

        public override void OnLevelUnloading()
        {
            base.OnLevelUnloading();
            if (landscapingPanel != null)
            {
                GetPanels().Add(LandscapingInfoPanel, landscapingPanel);
            }
            landscapingPanel = null;

        }

        private static Dictionary<string, UIDynamicPanels.DynamicPanelInfo> GetPanels()
        {
            return (Dictionary<string, UIDynamicPanels.DynamicPanelInfo>)
                typeof(UIDynamicPanels).GetField("m_CachedPanels", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(UIView.library);
        }
    }

}
