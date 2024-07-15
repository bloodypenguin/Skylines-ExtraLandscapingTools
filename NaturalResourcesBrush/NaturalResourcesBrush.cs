using System;
using System.Collections.Generic;
using System.Reflection;
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
            if (mode == LoadMode.LoadGame || mode == LoadMode.NewGame || mode == LoadMode.NewGameFromScenario || mode == LoadMode.NewTheme || mode == LoadMode.LoadTheme)
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
                if (spriteInfo == null) return;
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
                if (mode == LoadMode.NewGame || mode == LoadMode.LoadGame || mode == LoadMode.NewGameFromScenario || mode == LoadMode.NewTheme || mode == LoadMode.LoadTheme)
                {
                    var defaultAtlas = UIView.GetAView().defaultAtlas;
                    if (OptionsWrapper<Options>.Options.resourcesTool)
                    {
                        ToolbarButtonSpawner.SpawnSubEntry(strip, "Resource", "MAPEDITOR_TOOL", null, "ToolbarIcon",
                            true,
                            mainToolbar.m_OptionsBar, mainToolbar.m_DefaultInfoTooltipAtlas);
                        var ResourcePanel = UIView.FindObjectOfType<ResourcePanel>();
                        var buttons = ResourcePanel.GetComponentsInChildren<UIButton>();
                        foreach (var button in buttons)
                        {
                            if (button.name == "Ore" || button.name == "Oil" || button.name == "Fertility")
                            {
                                button.atlas = defaultAtlas;
                            }
                        }
                    }
                    if (OptionsWrapper<Options>.Options.waterTool)
                    {
                        ToolbarButtonSpawner.SpawnSubEntry(strip, "Water", "MAPEDITOR_TOOL", null, "ToolbarIcon", true,
                            mainToolbar.m_OptionsBar, mainToolbar.m_DefaultInfoTooltipAtlas);
                        var WaterPanel = UIView.FindObjectOfType<WaterPanel>();
                        var buttons = WaterPanel.GetComponentsInChildren<UIButton>();
                        foreach (var button in buttons)
                        {
                            if (button.name == "PlaceWater")
                            {
                                button.atlas = Util.CreateAtlasFromResources(new List<string> { "WaterPlaceWater" });
                            }
                            if (button.name == "MoveSeaLevel")
                            {
                                button.atlas = Util.CreateAtlasFromResources(new List<string> { "WaterMoveSeaLevel" });
                            }
                        }


                        if (mode == LoadMode.NewTheme || mode == LoadMode.LoadTheme)
                        {
                            var ThemeEditorMainToolbar = UIView.FindObjectOfType<ThemeEditorMainToolbar>();
                            var Themebuttons = ThemeEditorMainToolbar.GetComponentsInChildren<UIButton>();
                            foreach (var button in Themebuttons)
                            {
                                if (button.name == "Water")
                                {
                                    button.atlas = Util.CreateAtlasFromResources(new List<string> { "ToolbarIconWater", "ToolbarIconBase" });
                                }
                            }
                        }
                        else
                        {
                            var GameMainToolbar = UIView.FindObjectOfType<GameMainToolbar>();
                            var Gamebuttons = GameMainToolbar.GetComponentsInChildren<UIButton>();
                            foreach (var button in Gamebuttons)
                            {
                                if (button.name == "Water")
                                {
                                    button.atlas = Util.CreateAtlasFromResources(new List<string> { "ToolbarIconWater", "ToolbarIconBase" });
                                }
                            }
                        }
                    }
                }
                if (mode == LoadMode.NewAsset || mode == LoadMode.LoadAsset || mode == LoadMode.NewTheme || mode == LoadMode.LoadTheme)
                {
                    if (OptionsWrapper<Options>.Options.terrainTool)
                    {
                        ToolbarButtonSpawner.SpawnSubEntry(strip, "Terrain", "MAPEDITOR_TOOL", null, "ToolbarIcon", true,
                            mainToolbar.m_OptionsBar, mainToolbar.m_DefaultInfoTooltipAtlas);

                        var TerrainPanel = UIView.FindObjectOfType<TerrainPanel>();
                        var buttons = TerrainPanel.GetComponentsInChildren<UIButton>();
                        foreach (var button in buttons)
                        {
                            if (button.name == "Shift")
                            {
                                button.atlas = Util.CreateAtlasFromResources(new List<string> { "TerrainShift" });
                            }
                            if (button.name == "Slope")
                            {
                                button.atlas = Util.CreateAtlasFromResources(new List<string> { "TerrainSlope" });
                            }
                            if (button.name == "Level")
                            {
                                button.atlas = Util.CreateAtlasFromResources(new List<string> { "TerrainLevel" });
                            }
                            if (button.name == "Soften")
                            {
                                button.atlas = Util.CreateAtlasFromResources(new List<string> { "TerrainSoften" });
                            }
                        }

                        if (mode == LoadMode.NewTheme || mode == LoadMode.LoadTheme)
                        {
                            var ThemeEditorMainToolbar = UIView.FindObjectOfType<ThemeEditorMainToolbar>();
                            var Themebuttons = ThemeEditorMainToolbar.GetComponentsInChildren<UIButton>();
                            foreach (var button in Themebuttons)
                            {
                                if (button.name == "Terrain")
                                {
                                    button.atlas = Util.CreateAtlasFromResources(new List<string> { "ToolbarIconTerrain", "ToolbarIconBase" });
                                }
                            }
                        }
                        else
                        {
                            var AssetEditorMainToolbar = UIView.FindObjectOfType<AssetEditorMainToolbar>();
                            var AssetEditorbuttons = AssetEditorMainToolbar.GetComponentsInChildren<UIButton>();
                            foreach (var button in AssetEditorbuttons)
                            {
                                if (button.name == "Terrain")
                                {
                                    button.atlas = Util.CreateAtlasFromResources(new List<string> { "ToolbarIconTerrain", "ToolbarIconBase" });
                                }
                            }
                        }
                    }
                }
                if (mode == LoadMode.NewTheme || mode == LoadMode.LoadTheme)
                {
                    ToolbarButtonSpawner.SpawnSubEntry(strip, "Forest", "MAPEDITOR_TOOL", null, "ToolbarIcon", true,
                            mainToolbar.m_OptionsBar, mainToolbar.m_DefaultInfoTooltipAtlas);
                }
                try
                {
                    Plugins.CreateToolbars(mode);
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
                return true;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
            return false;
        }
    }
}