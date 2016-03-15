
using System;
using System.Collections.Generic;
using System.Reflection;
using ColossalFramework;
using ICities;
using NaturalResourcesBrush.Options;
using UnityEngine;
using Object = UnityEngine.Object;

namespace NaturalResourcesBrush
{
    public class LoadingExtension : LoadingExtensionBase
    {
        public override void OnCreated(ILoading lodaing)
        {
            //to allow to work in MapEditor
            if (OptionsHolder.Options.treePencil)
            {
                TreeToolDetour.Deploy();
            }
            if (OptionsHolder.Options.terrainTool)
            {
                TerrainToolDetour.Deploy();
                TerrainPanelDetour.Deploy();
                LandscapingPanelDetour.Deploy();
                LevelHeightOptionPanelDetour.Deploy();
                Util.AddLocale("LANDSCAPING", "Ditch", "Ditch tool", "");
                Util.AddLocale("TERRAIN", "Ditch", "Ditch tool", "");
            }
        }

        public override void OnReleased()
        {
            TreeToolDetour.Revert();
            BeautificationPanelDetour.Revert();
            WaterToolDetour.Revert();
            ResourcePanelDetour.Revert();
            TerrainToolDetour.Revert();
            TerrainPanelDetour.Revert();
            LandscapingPanelDetour.Revert();
            LevelHeightOptionPanelDetour.Revert();
        }

        public override void OnLevelLoaded(LoadMode mode)
        {
            if (mode == LoadMode.LoadGame || mode == LoadMode.NewGame)
            {
                if (OptionsHolder.Options.treeBrush)
                {
                    BeautificationPanelDetour.Deploy();
                }
                if (OptionsHolder.Options.waterTool)
                {
                    WaterToolDetour.Deploy();
                    Util.AddLocale("TUTORIAL_ADVISER", "Water", "Water Tool", "");
                }
                if (OptionsHolder.Options.resourcesTool)
                {
                    Util.AddLocale("RESOURCE", "Sand", "Sand",
                        "Use the primary mouse button to place decorative sand to the area under the brush\n" +
                        "Use secondary mouse button to remove decorative sand from the area under the brush");
                    Util.AddLocale("TUTORIAL_ADVISER", "Resource", "Ground Resources Tool", "");
                    ResourcePanelDetour.Deploy();
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
                var extraTools = Util.SetUpExtraTools(mode, ref toolController);
                Util.AddExtraToolsToController(ref toolController, extraTools);
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
            if (OptionsHolder.Options.terrainTool)
            {
                LandscapingPanelDetour.Initialize();
            }
        }
    }

}
