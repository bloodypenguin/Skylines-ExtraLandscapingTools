using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using ColossalFramework;
using ColossalFramework.Globalization;
using ColossalFramework.UI;
using ICities;
using UnityEngine;
using Object = UnityEngine.Object;

namespace NaturalResourcesBrush
{
    public static class Util
    {
        public static Texture2D LoadTextureFromAssembly(string path, string textureName, bool readOnly = true)
        {
            try
            {
                var assembly = System.Reflection.Assembly.GetExecutingAssembly();
                using (var textureStream = assembly.GetManifestResourceStream(path))
                {
                    return LoadTextureFromStream(readOnly, textureStream, textureName);
                }
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogException(e);
                return null;
            }
        }

        public static UITextureAtlas CreateAtlasFromResources(List<string> baseIconNames)
        {
            var names = GetIconNames(baseIconNames);
            var sprites = ResourceUtils.Load<Texture2D>(names);
            return CreateAtlas(sprites);
        }

        public static UITextureAtlas CreateAtlasFromEmbeddedResources(List<string> baseIconNames)
        {
            var names = GetIconNames(baseIconNames);
            var sprites = new Texture2D[names.Length];
            for (var i = 0; i < names.Length; i++)
            {
                sprites[i] = LoadTextureFromAssembly($"NaturalResourcesBrush.resources.{names[i]}.png", names[i], false);
            }
            return CreateAtlas(sprites);
        }

        private static string[] GetIconNames(List<string> baseIconNames)
        {
            var names = new string[baseIconNames.Count*5];
            var i = 0;
            foreach (var baseIconName in baseIconNames)
            {
                names[i*5] = baseIconName;
                names[i*5 + 1] = baseIconName + "Focused";
                names[i*5 + 2] = baseIconName + "Hovered";
                names[i*5 + 3] = baseIconName + "Pressed";
                names[i*5 + 4] = baseIconName + "Disabled";
                i++;
            }
            return names;
        }

        public static UITextureAtlas CreateAtlas(Texture2D[] sprites)
        {
            UITextureAtlas atlas = new UITextureAtlas();
            atlas.material = new Material(GetUIAtlasShader());

            Texture2D texture = new Texture2D(0, 0);
            Rect[] rects = texture.PackTextures(sprites, 0);

            for (int i = 0; i < rects.Length; ++i)
            {
                Texture2D sprite = sprites[i];
                Rect rect = rects[i];

                UITextureAtlas.SpriteInfo spriteInfo = new UITextureAtlas.SpriteInfo();
                spriteInfo.name = sprite.name;
                spriteInfo.texture = sprite;
                spriteInfo.region = rect;
                spriteInfo.border = new RectOffset();

                atlas.AddSprite(spriteInfo);
            }
            atlas.material.mainTexture = texture;
            return atlas;
        }

        private static Shader GetUIAtlasShader()
        {
            return UIView.GetAView().defaultAtlas.material.shader;
        }

        private static Texture2D LoadTextureFromStream(bool readOnly, Stream textureStream, string textureName = null)
        {
            var buf = new byte[textureStream.Length]; //declare arraysize
            textureStream.Read(buf, 0, buf.Length); // read from stream to byte array
            textureStream.Close();
            var tex = new Texture2D(2, 2, TextureFormat.ARGB32, false);
            tex.LoadImage(buf);
            tex.Apply(false, readOnly);
            tex.name = textureName ?? Guid.NewGuid().ToString();
            return tex;
        }

        public static void AddLocale(string idBase, string key, string title, string description)
        {
            var localeField = typeof(LocaleManager).GetField("m_Locale", BindingFlags.NonPublic | BindingFlags.Instance);
            var locale = (Locale)localeField.GetValue(SingletonLite<LocaleManager>.instance);
            var localeKey = new Locale.Key() { m_Identifier = $"{idBase}_TITLE", m_Key = key };
            if (!locale.Exists(localeKey))
            {
                locale.AddLocalizedString(localeKey, title);
            }
            localeKey = new Locale.Key() { m_Identifier = $"{idBase}_DESC", m_Key = key };
            if (!locale.Exists(localeKey))
            {
                locale.AddLocalizedString(localeKey, description);
            }
            localeKey = new Locale.Key() { m_Identifier = $"{idBase}", m_Key = key };
            if (!locale.Exists(localeKey))
            {
                locale.AddLocalizedString(localeKey, description);
            }
        }

        public static Type FindType(string className)
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                try
                {
                    var types = assembly.GetTypes();
                    foreach (var type in types.Where(type => type.Name == className))
                    {
                        return type;
                    }
                }
                catch
                {
                    // ignored
                }
            }
            return null;
        }

        public static void AddExtraToolsToController(ref ToolController toolController, List<ToolBase> extraTools)
        {
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
        public static bool SetUpExtraTools(LoadMode mode, ref ToolController toolController, out List<ToolBase> extraTools)
        {
            extraTools = new List<ToolBase>();
            if (mode == LoadMode.LoadGame | mode == LoadMode.NewGame)
            {
                LoadResources();
                if (SetUpResourcesToolbar())
                {
                    if (NaturalResourcesBrush.Options.IsFlagSet(ModOptions.WaterTool))
                    {
                        SetUpWaterTool(ref toolController, ref extraTools);
                    }
                    var optionsPanel = SetupBrushOptionsPanel(NaturalResourcesBrush.Options.IsFlagSet(ModOptions.TreeBrush));
                    if (optionsPanel != null)
                    {
                        optionsPanel.m_BuiltinBrushes = toolController.m_brushes;
                        if (NaturalResourcesBrush.Options.IsFlagSet(ModOptions.ResourcesTool))
                        {
                            SetUpNaturalResourcesTool(ref toolController, ref extraTools, ref optionsPanel);
                        }
                        if (NaturalResourcesBrush.Options.IsFlagSet(ModOptions.TerrainTool))
                        {
                            SetUpTerrainTool(ref toolController, ref extraTools, ref optionsPanel);
                        }
                    }

                }
            }
            return extraTools.Count > 0;
        }

        private static void SetUpNaturalResourcesTool(ref ToolController toolController, ref List<ToolBase> extraTools, ref BrushOptionPanel optionsPanel)
        {
            var resourceTool = toolController.gameObject.GetComponent<ResourceTool>();
            if (resourceTool == null)
            {
                resourceTool = toolController.gameObject.AddComponent<ResourceTool>();
                extraTools.Add(resourceTool);
            }
            resourceTool.m_brush = toolController.m_brushes[0];
        }

        private static void SetUpWaterTool(ref ToolController toolController, ref List<ToolBase> extraTools)
        {
            var optionsPanel = SetupWaterPanel();
            if (optionsPanel == null)
            {
                return;
            }
            var waterTool = toolController.gameObject.GetComponent<WaterTool>();
            if (waterTool == null)
            {
                waterTool = toolController.gameObject.AddComponent<WaterTool>();
                extraTools.Add(waterTool);
            }
        }

        private static void SetUpTerrainTool(ref ToolController toolController, ref List<ToolBase> extraTools, ref BrushOptionPanel optionsPanel)
        {
            var terrainTool = toolController.gameObject.GetComponent<TerrainTool>();
            if (terrainTool == null)
            {
                terrainTool = toolController.gameObject.AddComponent<TerrainTool>();
                extraTools.Add(terrainTool);
            }
            terrainTool.m_brush = toolController.m_brushes[0];
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


        public static BrushOptionPanel SetupBrushOptionsPanel(bool treeBrushEnabled)
        {
            var optionsBar = UIView.Find<UIPanel>("OptionsBar");
            if (optionsBar == null)
            {
                Debug.LogError("ExtraTools#SetupBrushOptionsPanel(): options bar not found");
                return null;
            }

            var brushOptionsPanel = optionsBar.AddUIComponent<UIPanel>();
            brushOptionsPanel.name = "BrushPanel";
            brushOptionsPanel.backgroundSprite = "MenuPanel2";
            brushOptionsPanel.size = new Vector2(231, 506);
            brushOptionsPanel.isVisible = false;
            brushOptionsPanel.relativePosition = new Vector3(-256, -488);

            UIUtil.SetupTitle("Brush Options", brushOptionsPanel);
            UIUtil.SetupBrushSizePanel(brushOptionsPanel);
            UIUtil.SetupBrushStrengthPanel(brushOptionsPanel);
            UIUtil.SetupBrushSelectPanel(brushOptionsPanel);

            if (treeBrushEnabled)
            {
                var beauPanel = Object.FindObjectOfType<BeautificationPanel>();
                if (beauPanel == null)
                {
                    Debug.LogWarning("ExtraTools#SetupBrushOptionsPanel(): beautification panel not found.");
                }
                else
                {
                    beauPanel.component.eventVisibilityChanged += (comp, visible) =>
                    {
                        brushOptionsPanel.isVisible = visible;
                    };
                }
            }
            return brushOptionsPanel.gameObject.AddComponent<BrushOptionPanel>();
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

            UIUtil.SetupTitle("Water Options", waterPanel);
            UIUtil.SetupWaterCapacityPanel(waterPanel);
            return waterPanel.gameObject.AddComponent<WaterOptionPanel>();
        }

        public static bool SetUpResourcesToolbar()
        {
            var mainToolbar = ToolsModifierControl.mainToolbar as GameMainToolbar;
            if (mainToolbar == null)
            {
                Debug.LogError("ExtraTools#SetUpResourcesToolbar(): main toolbar is null");
                return false;
            }
            var strip = mainToolbar.component as UITabstrip;
            if (strip == null)
            {
                Debug.LogError("ExtraTools#SetUpResourcesToolbar(): strip is null");
                return false;
            }
            try
            {
                var defaultAtlas = UIView.GetAView().defaultAtlas;
                if (NaturalResourcesBrush.Options.IsFlagSet(ModOptions.ResourcesTool))
                {
                    ToolbarButtonSpawner.SpawnSubEntry(strip, "Resource", "MAPEDITOR_TOOL", null, "ToolbarIcon", true,
                        mainToolbar.m_OptionsBar, mainToolbar.m_DefaultInfoTooltipAtlas);
                    ((UIButton) UIView.FindObjectOfType<ResourcePanel>().Find("Ore")).atlas = defaultAtlas;
                    ((UIButton) UIView.FindObjectOfType<ResourcePanel>().Find("Oil")).atlas = defaultAtlas;
                    ((UIButton) UIView.FindObjectOfType<ResourcePanel>().Find("Fertility")).atlas = defaultAtlas;
                    ((UIButton) UIView.FindObjectOfType<ResourcePanel>().Find("Sand")).atlas = defaultAtlas;
                }
                if (NaturalResourcesBrush.Options.IsFlagSet(ModOptions.WaterTool))
                {
                    ToolbarButtonSpawner.SpawnSubEntry(strip, "Water", "MAPEDITOR_TOOL", null, "ToolbarIcon", true,
                        mainToolbar.m_OptionsBar, mainToolbar.m_DefaultInfoTooltipAtlas);
                    ((UIButton) UIView.FindObjectOfType<WaterPanel>().Find("PlaceWater")).atlas =
                        CreateAtlasFromResources(new List<string> {"WaterPlaceWater"});
                    ((UIButton) UIView.FindObjectOfType<WaterPanel>().Find("MoveSeaLevel")).atlas =
                        CreateAtlasFromResources(new List<string> {"WaterMoveSeaLevel"});
                    ((UIButton) UIView.FindObjectOfType<GameMainToolbar>().Find("Water")).atlas =
                        CreateAtlasFromResources(new List<string> {"ToolbarIconWater", "ToolbarIconBase"});
                }
                if (NaturalResourcesBrush.Options.IsFlagSet(ModOptions.TerrainTool))
                {


                    ToolbarButtonSpawner.SpawnSubEntry(strip, "Terrain", "MAPEDITOR_TOOL", null, "ToolbarIcon", true,
                        mainToolbar.m_OptionsBar, mainToolbar.m_DefaultInfoTooltipAtlas);
                    ((UIButton) UIView.FindObjectOfType<TerrainPanel>().Find("Shift")).atlas =
                        CreateAtlasFromResources(new List<string> {"TerrainShift"});
                    ((UIButton) UIView.FindObjectOfType<TerrainPanel>().Find("Slope")).atlas =
                        CreateAtlasFromResources(new List<string> {"TerrainSlope"});
                    ((UIButton) UIView.FindObjectOfType<TerrainPanel>().Find("Level")).atlas =
                        CreateAtlasFromResources(new List<string> {"TerrainLevel"});
                    ((UIButton) UIView.FindObjectOfType<TerrainPanel>().Find("Soften")).atlas =
                        CreateAtlasFromResources(new List<string> {"TerrainSoften"});
                    ((UIButton) UIView.FindObjectOfType<GameMainToolbar>().Find("Terrain")).atlas =
                        CreateAtlasFromResources(new List<string> {"ToolbarIconTerrain", "ToolbarIconBase"});
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