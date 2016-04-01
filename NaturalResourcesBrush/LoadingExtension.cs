
using System;
using System.Collections.Generic;
using System.Reflection;
using ColossalFramework.UI;
using ICities;
using NaturalResourcesBrush.OptionsFramework;
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
                TreeToolDetour.Deploy();
            }
            if (OptionsWrapper<Options>.Options.terrainTool)
            {
                TerrainToolDetour.Deploy();
                TerrainPanelDetour.Deploy();
                LandscapingPanelDetour.Deploy();
                LevelHeightOptionPanelDetour.Deploy();
                UndoTerrainOptionPanelDetour.Deploy();
            }
            Util.AddLocale("LANDSCAPING", "Ditch", "Ditch tool", "");
            Util.AddLocale("TERRAIN", "Ditch", "Ditch tool", "");
            Util.AddLocale("RESOURCE", "Sand", "Sand",
                "Use the primary mouse button to place decorative sand to the area under the brush\n" +
                "Use secondary mouse button to remove decorative sand from the area under the brush");
            Util.AddLocale("TUTORIAL_ADVISER", "Resource", "Ground Resources Tool", "");
            Util.AddLocale("TUTORIAL_ADVISER", "Water", "Water Tool", "");
        }

        public override void OnReleased()
        {
            TreeToolDetour.Revert();
            BeautificationPanelDetour.Revert();
            WaterToolDetour.Revert();
            TerrainToolDetour.Revert();
            TerrainPanelDetour.Revert();
            LandscapingPanelDetour.Revert();
            LevelHeightOptionPanelDetour.Revert();
            UndoTerrainOptionPanelDetour.Revert();

        }

        public override void OnLevelLoaded(LoadMode mode)
        {
            if (mode == LoadMode.LoadGame || mode == LoadMode.NewGame)
            {
                if (OptionsWrapper<Options>.Options.treeBrush)
                {
                    BeautificationPanelDetour.Deploy();
                }
                if (OptionsWrapper<Options>.Options.waterTool)
                {
                    WaterToolDetour.Deploy();
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
                var extraTools = NaturalResourcesBrush.SetUpExtraTools(mode, ref toolController);
                NaturalResourcesBrush.AddExtraToolsToController(ref toolController, extraTools);
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
