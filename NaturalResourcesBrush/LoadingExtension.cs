
using System;
using System.Collections.Generic;
using System.Reflection;
using ColossalFramework;
using ColossalFramework.Globalization;
using ICities;
using UnityEngine;
using Object = UnityEngine.Object;

namespace NaturalResourcesBrush
{
    public class LoadingExtension : LoadingExtensionBase
    {
        public override void OnCreated(ILoading lodaing)
        {
            //to allow to work in MapEditor
            if (NaturalResourcesBrush.Options.IsFlagSet(ModOptions.TreePencil))
            {
                TreeToolDetour.Deploy();
            }
            if (NaturalResourcesBrush.Options.IsFlagSet(ModOptions.TerrainTool))
            {
                TerrainToolDetour.Deploy();
                TerrainPanelDetour.Deploy();
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
        }

        public override void OnLevelLoaded(LoadMode mode)
        {
            if (mode == LoadMode.LoadGame || mode == LoadMode.NewGame)
            {
                if (NaturalResourcesBrush.Options.IsFlagSet(ModOptions.TreeBrush))
                {
                    BeautificationPanelDetour.Deploy();
                }
                if (NaturalResourcesBrush.Options.IsFlagSet(ModOptions.WaterTool))
                {
                    WaterToolDetour.Deploy();
                }
                if (NaturalResourcesBrush.Options.IsFlagSet(ModOptions.ResourcesTool))
                {
                    Util.AddLocale("RESOURCE", "Sand", "Sand",
                        "Use the primary mouse button to place decorative sand to the area under the brush\n" +
                            "Use secondary mouse button to remove decorative sand from the area under the brush");
                    ResourcePanelDetour.Deploy();
                }
                Util.AddLocale("TUTORIAL_ADVISER", "Terrain", "Terrain Tool", "");
                Util.AddLocale("TUTORIAL_ADVISER", "Water", "Water Tool", "");
                Util.AddLocale("TUTORIAL_ADVISER", "Resource", "Ground Resources Tool", "");
            }
            if (NaturalResourcesBrush.Options.IsFlagSet(ModOptions.TerrainTool))
            {
                Util.AddLocale("TERRAIN", "Ditch", "Ditch tool",
                    "");
            }
            var toolController = Object.FindObjectOfType<ToolController>();
            if (toolController == null)
            {
                Debug.LogError("ExtraTools#OnLevelLoaded(): ToolContoller not found");
                return;
            }
            try
            {
                List<ToolBase> extraTools;
                if (!Util.SetUpExtraTools(mode, ref toolController, out extraTools))
                {
                    return;
                }
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
        }
    }

}
