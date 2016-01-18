
using System;
using System.Collections.Generic;
using ColossalFramework;
using ICities;
using UnityEngine;
using Object = UnityEngine.Object;

namespace NaturalResourcesBrush
{
    public class LoadingExtension : LoadingExtensionBase
    {
        public override void OnCreated(ILoading lodaing)
        {
            if (NaturalResourcesBrush.Options.IsFlagSet(ModOptions.TreePencil))
            {
                TreeToolDetour.Deploy();
            }
            TerrainToolDetour.Deploy();
        }

        public override void OnReleased()
        {
            TreeToolDetour.Revert();
            BeautificationPanelDetour.Revert();
            WaterToolDetour.Revert();
            ResourcePanelDetour.Revert();
            TerrainToolDetour.Revert();
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
                    ResourcePanelDetour.Deploy();
                }
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
