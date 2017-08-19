using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using ColossalFramework;
using ColossalFramework.Globalization;
using ColossalFramework.UI;
using ICities;
using NaturalResourcesBrush.API;
using NaturalResourcesBrush.OptionsFramework;
using NaturalResourcesBrush.Utils;
using UnityEngine;
using Object = UnityEngine.Object;
using Util = NaturalResourcesBrush.Utils.Util;

namespace NaturalResourcesBrush
{
    public static class NaturalResourcesBrush
    {
        public static Dictionary<UIComponent, bool> beautificationPanelsCachedVisible = new Dictionary<UIComponent, bool>();

        public static void AddExtraToolsToController(ToolController toolController, List<ToolBase> extraTools)
        {
            if (extraTools.Count < 1)
            {
                return;
            }
            var fieldInfo = typeof(ToolController).GetField("m_tools", BindingFlags.Instance | BindingFlags.NonPublic);
            var tools = (ToolBase[])fieldInfo.GetValue(toolController);
            var initialLength = tools.Length;
            Array.Resize(ref tools, initialLength + extraTools.Count);
            var i = 0;
            var dictionary =
                (Dictionary<Type, ToolBase>)
                    typeof(ToolsModifierControl).GetField("m_Tools", BindingFlags.Static | BindingFlags.NonPublic)
                        .GetValue(null);
            foreach (var tool in extraTools)
            {
                dictionary.Add(tool.GetType(), tool);
                tools[initialLength + i] = tool;
                i++;
            }
            fieldInfo.SetValue(toolController, tools);
        }

        //returns false in no extra tools were set up
        public static List<ToolBase> SetUpExtraTools(LoadMode mode, ToolController toolController)
        {
            var extraTools = new List<ToolBase>();
            if (mode == LoadMode.LoadGame || mode == LoadMode.NewGame || mode == LoadMode.NewGameFromScenario)
            {
                LoadResources();
                if (SetUpToolbars(mode))
                {
                    if (OptionsWrapper<Options>.Options.waterTool)
                    {
                        SetUpWaterTool(extraTools);
                    }
                    SetupBrushOptionsPanel(OptionsWrapper<Options>.Options.treeBrush);
                    var optionsPanel = Object.FindObjectOfType<BrushOptionPanel>();
                    if (optionsPanel != null)
                    {
                        optionsPanel.m_BuiltinBrushes = toolController.m_brushes;
                        if (OptionsWrapper<Options>.Options.resourcesTool || OptionsWrapper<Options>.Options.terrainTool)
                        {
                            SetUpNaturalResourcesTool(extraTools);
                        }
                        if (OptionsWrapper<Options>.Options.terrainTool)
                        {
                            SetUpTerrainToolExtensionss();
                        }
                    }
                    var beautificationPanels = Object.FindObjectsOfType<BeautificationPanel>();
                    beautificationPanels.ForEach(p =>
                    {
                        p.component.eventVisibilityChanged -= HideBrushOptionsPanel();
                        p.component.eventVisibilityChanged += HideBrushOptionsPanel();
                    });
                }
            }
            try
            {
                var pluginTools = Plugins.SetupTools(mode);
                extraTools.AddRange(pluginTools);
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogException(e);
            }
            return extraTools;
        }

        private static PropertyChangedEventHandler<bool> HideBrushOptionsPanel()
        {
            return (sender, visible) =>
            {
                
                if (beautificationPanelsCachedVisible.TryGetValue(sender, out bool cached) && cached && !visible)
                {
                    var optionsPanel = Object.FindObjectOfType<BrushOptionPanel>();
                    optionsPanel?.Hide();
                }
                beautificationPanelsCachedVisible[sender] = visible;
            };
        }

        private static void SetUpNaturalResourcesTool(ICollection<ToolBase> extraTools)
        {
            var resourceTool = ToolsModifierControl.GetTool<ResourceTool>();
            if (resourceTool == null)
            {
                resourceTool = ToolsModifierControl.toolController.gameObject.AddComponent<ResourceTool>();
                extraTools.Add(resourceTool);
            }
            resourceTool.m_brush = ToolsModifierControl.toolController.m_brushes[0];
        }

        private static void SetUpWaterTool(ICollection<ToolBase> extraTools)
        {
            var optionsPanel = SetupWaterPanel();
            if (optionsPanel == null)
            {
                return;
            }
            var waterTool = ToolsModifierControl.GetTool<WaterTool>();
            if (waterTool != null)
            {
                return;
            }
            waterTool = ToolsModifierControl.toolController.gameObject.AddComponent<WaterTool>();
            extraTools.Add(waterTool);
        }

        private static void SetUpTerrainToolExtensionss()
        {
            var terrainTool = ToolsModifierControl.GetTool<TerrainTool>();
            if (terrainTool == null)
            {
                Debug.LogError("ExtraTools#SetupBrushOptionsPanel(): terrain tool not found");
                return;
            }
            var optionsBar = UIView.Find<UIPanel>("OptionsBar");
            if (optionsBar == null)
            {
                Debug.LogError("ExtraTools#SetupBrushOptionsPanel(): options bar not found");
                return;
            }
            UI.SetUpUndoModififcationPanel(optionsBar);
            UI.SetupLevelHeightPanel(optionsBar);
        }
        public static void LoadResources()
        {
            var defaultAtlas = UIView.GetAView().defaultAtlas;

            CopySprite("InfoIconResources", "ToolbarIconResource", defaultAtlas);
            CopySprite("InfoIconResourcesDisabled", "ToolbarIconResourceDisabled", defaultAtlas);
            CopySprite("InfoIconResourcesFocused", "ToolbarIconResourceFocused", defaultAtlas);
            CopySprite("InfoIconResourcesHovered", "ToolbarIconResourceHovered", defaultAtlas);
            CopySprite("InfoIconResourcesPressed", "ToolbarIconResourcePressed", defaultAtlas);

            CopySprite("ToolbarIconGroup6Normal", "ToolbarIconBaseNormal", defaultAtlas);
            CopySprite("ToolbarIconGroup6Disabled", "ToolbarIconBaseDisabled", defaultAtlas);
            CopySprite("ToolbarIconGroup6Focused", "ToolbarIconBaseFocused", defaultAtlas);
            CopySprite("ToolbarIconGroup6Hovered", "ToolbarIconBaseHovered", defaultAtlas);
            CopySprite("ToolbarIconGroup6Pressed", "ToolbarIconBasePressed", defaultAtlas);
        }

        public static void CopySprite(string originalName, string newName, UITextureAtlas destAtlas)
        {
            try
            {
                var spriteInfo = UIView.GetAView().defaultAtlas[originalName];
                destAtlas.AddSprite(new UITextureAtlas.SpriteInfo
                {
                    border = spriteInfo.border,
                    name = newName,
                    region = spriteInfo.region,
                    texture = spriteInfo.texture
                });
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }

        }


        public static void SetupBrushOptionsPanel(bool treeBrushEnabled)
        {
            var optionsBar = UIView.Find<UIPanel>("OptionsBar");
            if (optionsBar == null)
            {
                Debug.LogError("ExtraTools#SetupBrushOptionsPanel(): options bar not found");
                return;
            }
            if (GameObject.Find("BrushPanel") != null)
            {
                return;
            }
            var brushOptionsPanel = optionsBar.AddUIComponent<UIPanel>();
            brushOptionsPanel.name = "BrushPanel";
            brushOptionsPanel.backgroundSprite = "MenuPanel2";
            brushOptionsPanel.size = new Vector2(231, 506);
            brushOptionsPanel.isVisible = false;
            brushOptionsPanel.relativePosition = new Vector3(-256, -488);
            UIUtil.SetupTitle(Mod.translation.GetTranslation("ELT_BRUSH_OPTIONS"), brushOptionsPanel);
            UI.SetupBrushSizePanel(brushOptionsPanel);
            UI.SetupBrushStrengthPanel(brushOptionsPanel);
            UI.SetupBrushSelectPanel(brushOptionsPanel);

            brushOptionsPanel.gameObject.AddComponent<BrushOptionPanel>();
        }

        public static WaterOptionPanel SetupWaterPanel()
        {
            var optionsBar = UIView.Find<UIPanel>("OptionsBar");
            if (optionsBar == null)
            {
                Debug.LogError("SetupWaterPanel(): options bar not found");
                return null;
            }

            var waterPanel = optionsBar.AddUIComponent<UIPanel>();
            waterPanel.name = "WaterPanel";
            waterPanel.backgroundSprite = "MenuPanel2";
            waterPanel.size = new Vector2(231, 184);
            waterPanel.isVisible = false;
            waterPanel.relativePosition = new Vector3(-256, -166);

            UIUtil.SetupTitle(Mod.translation.GetTranslation("ELT_WATER_OPTIONS"), waterPanel);
            UI.SetupWaterCapacityPanel(waterPanel);
            return waterPanel.gameObject.AddComponent<WaterOptionPanel>();
        }

        public static bool SetUpToolbars(LoadMode mode)
        {
            var mainToolbar = ToolsModifierControl.mainToolbar;
            if (mainToolbar == null)
            {
                Debug.LogError("ExtraTools#SetUpToolbars(): main toolbar is null");
                return false;
            }
            var strip = mainToolbar.component as UITabstrip;
            if (strip == null)
            {
                Debug.LogError("ExtraTools#SetUpToolbars(): strip is null");
                return false;
            }
            try
            {
                if (mode == LoadMode.NewGame || mode == LoadMode.LoadGame || mode == LoadMode.NewGameFromScenario)
                {
                    var defaultAtlas = UIView.GetAView().defaultAtlas;
                    if (OptionsWrapper<Options>.Options.resourcesTool)
                    {
                        ToolbarButtonSpawner.SpawnSubEntry(strip, "Resource", "MAPEDITOR_TOOL", null, "ToolbarIcon",
                            true,
                            mainToolbar.m_OptionsBar, mainToolbar.m_DefaultInfoTooltipAtlas);
                        ((UIButton)UIView.FindObjectOfType<ResourcePanel>().Find("Ore")).atlas = defaultAtlas;
                        ((UIButton)UIView.FindObjectOfType<ResourcePanel>().Find("Oil")).atlas = defaultAtlas;
                        ((UIButton)UIView.FindObjectOfType<ResourcePanel>().Find("Fertility")).atlas = defaultAtlas;
                    }
                    if (OptionsWrapper<Options>.Options.waterTool)
                    {
                        ToolbarButtonSpawner.SpawnSubEntry(strip, "Water", "MAPEDITOR_TOOL", null, "ToolbarIcon", true,
                            mainToolbar.m_OptionsBar, mainToolbar.m_DefaultInfoTooltipAtlas);
                        ((UIButton)UIView.FindObjectOfType<WaterPanel>().Find("PlaceWater")).atlas =
                            Util.CreateAtlasFromResources(new List<string> { "WaterPlaceWater" });
                        ((UIButton)UIView.FindObjectOfType<WaterPanel>().Find("MoveSeaLevel")).atlas =
                           Util.CreateAtlasFromResources(new List<string> { "WaterMoveSeaLevel" });
                        ((UIButton)UIView.FindObjectOfType<GameMainToolbar>().Find("Water")).atlas =
                            Util.CreateAtlasFromResources(new List<string> { "ToolbarIconWater", "ToolbarIconBase" });
                    }
                }
                else if (mode == LoadMode.NewAsset || mode == LoadMode.LoadAsset)
                {
                    if (OptionsWrapper<Options>.Options.terrainTool)
                    {
                        ToolbarButtonSpawner.SpawnSubEntry(strip, "Terrain", "MAPEDITOR_TOOL", null, "ToolbarIcon", true,
                            mainToolbar.m_OptionsBar, mainToolbar.m_DefaultInfoTooltipAtlas);
                        ((UIButton)UIView.FindObjectOfType<TerrainPanel>().Find("Shift")).atlas =
                            Util.CreateAtlasFromResources(new List<string> { "TerrainShift" });
                        ((UIButton)UIView.FindObjectOfType<TerrainPanel>().Find("Slope")).atlas =
                            Util.CreateAtlasFromResources(new List<string> { "TerrainSlope" });
                        ((UIButton)UIView.FindObjectOfType<TerrainPanel>().Find("Level")).atlas =
                            Util.CreateAtlasFromResources(new List<string> { "TerrainLevel" });
                        ((UIButton)UIView.FindObjectOfType<TerrainPanel>().Find("Soften")).atlas =
                            Util.CreateAtlasFromResources(new List<string> { "TerrainSoften" });
                        ((UIButton)UIView.FindObjectOfType<GameMainToolbar>().Find("Terrain")).atlas =
                            Util.CreateAtlasFromResources(new List<string> { "ToolbarIconTerrain", "ToolbarIconBase" });
                    }
                }
                try
                {
                    Plugins.CreateToolbars(mode);
                }
                catch (Exception e)
                {
                    UnityEngine.Debug.LogException(e);
                }
                return true;
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogException(e);
            }
            return false;
        }
    }
}