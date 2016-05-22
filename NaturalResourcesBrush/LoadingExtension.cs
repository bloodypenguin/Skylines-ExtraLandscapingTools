
using System;
using System.Collections.Generic;
using System.Reflection;
using ColossalFramework.UI;
using ICities;
using NaturalResourcesBrush.API;
using NaturalResourcesBrush.Detours;
using NaturalResourcesBrush.OptionsFramework;
using NaturalResourcesBrush.Redirection;
using UnityEngine;
using Object = UnityEngine.Object;

namespace NaturalResourcesBrush
{
    public class LoadingExtension : LoadingExtensionBase
    {
        public override void OnCreated(ILoading lodaing)
        {
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
            Util.AddLocale("LANDSCAPING", "Ditch", "Ditch tool", "");
            Util.AddLocale("TERRAIN", "Ditch", "Ditch tool", "");
            Util.AddLocale("LANDSCAPING", "Sand", "Sand",
                "Use the primary mouse button to place decorative sand to the area under the brush\n" +
                "Use secondary mouse button to remove decorative sand from the area under the brush");
            Util.AddLocale("TUTORIAL_ADVISER", "Resource", "Ground Resources Tool", "");
            Util.AddLocale("TUTORIAL_ADVISER", "Water", "Water Tool", "");
            Plugins.Initialize();
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
            Plugins.Dispose();
        }

        public override void OnLevelLoaded(LoadMode mode)
        {
            if (mode == LoadMode.LoadGame || mode == LoadMode.NewGame)
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
            if (OptionsWrapper<Options>.Options.terrainTool)
            {
                var panels =
    (Dictionary<string, UIDynamicPanels.DynamicPanelInfo>)
    typeof(UIDynamicPanels).GetField("m_CachedPanels", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(UIView.library);
                panels.Remove("LandscapingInfoPanel");
                LandscapingPanelDetour.Initialize();
            }
        }
    }

}
