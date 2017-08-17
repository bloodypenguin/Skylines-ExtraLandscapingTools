using System.Reflection;
using ColossalFramework;
using ColossalFramework.UI;
using NaturalResourcesBrush.Utils;
using UnityEngine;

namespace NaturalResourcesBrush
{
    public static class UI
    {

        public static void SetupBrushSizePanel(UIComponent brushOptionsPanel)
        {
            var brushSizePanel = brushOptionsPanel.AddUIComponent<UIPanel>();
            brushSizePanel.size = new Vector2(197, 49);
            brushSizePanel.relativePosition = new Vector2(17, 57);
            brushSizePanel.name = "Size";
            var brushSizeLabel = brushSizePanel.AddUIComponent<UILabel>();
            brushSizeLabel.localeID = "MAPEDITOR_BRUSHSIZE";
            brushSizeLabel.size = new Vector2(126, 18);
            brushSizeLabel.relativePosition = new Vector3(-3, 8);

            var brushSizeText = brushSizePanel.AddUIComponent<UITextField>();
            brushSizeText.name = "BrushSize";
            brushSizeText.size = new Vector2(60, 18);
            brushSizeText.normalBgSprite = "TextFieldPanel";
            brushSizeText.relativePosition = new Vector3(125, 7, 0);
            brushSizeText.builtinKeyNavigation = true;
            brushSizeText.isInteractive = true;
            brushSizeText.readOnly = false;
            brushSizeText.selectionSprite = "EmptySprite";
            brushSizeText.selectionBackgroundColor = new Color32(0, 172, 234, 255);
            var brushSizeSlider = brushSizePanel.AddUIComponent<UISlider>();
            brushSizeSlider.name = "BrushSize";
            brushSizeSlider.relativePosition = new Vector3(13, 30, 0);
            brushSizeSlider.backgroundSprite = "ScrollbarTrack";
            brushSizeSlider.size = new Vector2(171, 12);
            brushSizeSlider.minValue = 14;
            brushSizeSlider.maxValue = 2000;
            brushSizeSlider.stepSize = 1;
            var brushSizeSliderThumb = brushSizeSlider.AddUIComponent<UISlicedSprite>();
            brushSizeSliderThumb.spriteName = "ScrollbarThumb";
            brushSizeSliderThumb.size = new Vector2(10, 20);
            brushSizeSlider.thumbObject = brushSizeSliderThumb;
        }

        public static void SetupBrushStrengthPanel(UIComponent brushOptionsPanel)
        {
            var brushStrengthPanel = brushOptionsPanel.AddUIComponent<UIPanel>();
            brushStrengthPanel.size = new Vector2(197, 49);
            brushStrengthPanel.relativePosition = new Vector2(17, 110);
            brushStrengthPanel.name = "Strength";
            var brushStrengthLabel = brushStrengthPanel.AddUIComponent<UILabel>();
            brushStrengthLabel.localeID = "MAPEDITOR_BRUSHSTRENGTH";
            brushStrengthLabel.size = new Vector2(131, 19);
            brushStrengthLabel.relativePosition = new Vector3(-5, 7);
            var brushStrengthText = brushStrengthPanel.AddUIComponent<UITextField>();
            brushStrengthText.name = "BrushStrength";
            brushStrengthText.size = new Vector2(60, 18);
            brushStrengthText.normalBgSprite = "TextFieldPanel";
            brushStrengthText.relativePosition = new Vector3(125, 7, 0);
            brushStrengthText.builtinKeyNavigation = true;
            brushStrengthText.isInteractive = true;
            brushStrengthText.readOnly = false;
            brushStrengthText.selectionSprite = "EmptySprite";
            brushStrengthText.selectionBackgroundColor = new Color32(0, 172, 234, 255);


            var brushStrengthSlider = brushStrengthPanel.AddUIComponent<UISlider>();
            brushStrengthSlider.name = "BrushStrength";
            brushStrengthSlider.relativePosition = new Vector3(13, 30, 0);
            brushStrengthSlider.backgroundSprite = "ScrollbarTrack";
            brushStrengthSlider.size = new Vector2(171, 12);
            brushStrengthSlider.minValue = 0;
            brushStrengthSlider.maxValue = 1;
            brushStrengthSlider.stepSize = 0.01f;
            var brushStrengthSliderThumb = brushStrengthSlider.AddUIComponent<UISlicedSprite>();
            brushStrengthSliderThumb.spriteName = "ScrollbarThumb";
            brushStrengthSliderThumb.size = new Vector2(10, 20);
            brushStrengthSlider.thumbObject = brushStrengthSliderThumb;
        }

        public static void SetupBrushSelectPanel(UIComponent brushOptionsPanel)
        {
            var brushSelectPanel = brushOptionsPanel.AddUIComponent<UIPanel>();
            brushSelectPanel.size = new Vector2(255, 321);
            brushSelectPanel.relativePosition = new Vector2(3, 180);
            brushSelectPanel.name = "Brushes";
            var scrollablePanel = brushSelectPanel.AddUIComponent<UIScrollablePanel>();
            scrollablePanel.name = "BrushesContainer";
            scrollablePanel.size = new Vector2(219, 321);
            scrollablePanel.relativePosition = new Vector2(3, 0);
            scrollablePanel.backgroundSprite = "GenericPanel";
            scrollablePanel.autoLayout = true;
            scrollablePanel.autoLayoutDirection = LayoutDirection.Horizontal;
            scrollablePanel.autoLayoutStart = LayoutStart.TopLeft;
            scrollablePanel.autoLayoutPadding = new RectOffset(5, 5, 5, 5);
            scrollablePanel.scrollPadding = new RectOffset(10, 10, 10, 10);
            scrollablePanel.wrapLayout = true;
            scrollablePanel.clipChildren = true;
            scrollablePanel.freeScroll = false;
            var verticalScrollbar = UIUtil.SetUpScrollbar(brushSelectPanel);
            scrollablePanel.verticalScrollbar = verticalScrollbar;
            verticalScrollbar.relativePosition = new Vector2(206, 0);
        }

        public static void SetupWaterCapacityPanel(UIComponent waterOptionsPanel)
        {
            var waterCapacityPanel = waterOptionsPanel.AddUIComponent<UIPanel>();
            waterCapacityPanel.size = new Vector2(231, 78);
            waterCapacityPanel.relativePosition = new Vector2(0, 40);
            waterCapacityPanel.name = "Settings";
            var waterCapacityLabel = waterCapacityPanel.AddUIComponent<UILabel>();
            waterCapacityLabel.localeID = "MAPEDITOR_WATERCAPACITY";
            waterCapacityLabel.size = new Vector2(137, 16);
            waterCapacityLabel.relativePosition = new Vector3(10, 16);
            var waterCapacityText = waterCapacityPanel.AddUIComponent<UITextField>();
            waterCapacityText.name = "Capacity";
            waterCapacityText.size = new Vector2(64, 18);
            waterCapacityText.normalBgSprite = "TextFieldPanel";
            waterCapacityText.builtinKeyNavigation = true;
            waterCapacityText.isInteractive = true;
            waterCapacityText.readOnly = false;
            waterCapacityText.selectionSprite = "EmptySprite";
            waterCapacityText.selectionBackgroundColor = new Color32(0, 172, 234, 255);

            waterCapacityText.relativePosition = new Vector3(150, 16, 0);
            var waterCapacitySlider = waterCapacityPanel.AddUIComponent<UISlider>();
            waterCapacitySlider.name = "Capacity";
            waterCapacitySlider.relativePosition = new Vector3(28, 39, 0);
            waterCapacitySlider.backgroundSprite = "ScrollbarTrack";
            waterCapacitySlider.size = new Vector2(174, 12);
            waterCapacitySlider.minValue = 0.0001f;
            waterCapacitySlider.maxValue = 1;
            waterCapacitySlider.stepSize = 0.0001f;
            var waterCapacitySliderThumb = waterCapacitySlider.AddUIComponent<UISlicedSprite>();
            waterCapacitySliderThumb.spriteName = "ScrollbarThumb";
            waterCapacitySliderThumb.size = new Vector2(10, 20);
            waterCapacitySlider.thumbObject = waterCapacitySliderThumb;

            var resetButton = waterOptionsPanel.AddUIComponent<UIButton>();
            resetButton.name = "Apply";
            resetButton.localeID = "MAPEDITOR_RESET_WATER";
            resetButton.size = new Vector2(191, 38);
            resetButton.relativePosition = new Vector3(20, 132);
            resetButton.eventClick += (component, eventParam) => { Singleton<TerrainManager>.instance.WaterSimulation.m_resetWater = true; };
            resetButton.normalBgSprite = "ButtonMenu";
            resetButton.hoveredBgSprite = "ButtonMenuHovered";
            resetButton.pressedBgSprite = "ButtonMenuPressed";
            resetButton.disabledBgSprite = "ButtonMenuDisabled";
            resetButton.canFocus = false;
        }




        public static void SetUpUndoModififcationPanel(UIComponent optionsBar)
        {
            if (GameObject.Find("UndoTerrainPanel") != null)
            {
                return;
            }
            var undoPanel = optionsBar.AddUIComponent<UIPanel>();
            undoPanel.name = "UndoTerrainPanel";
            undoPanel.backgroundSprite = "MenuPanel2";
            undoPanel.size = new Vector2(231, 106);
            undoPanel.isVisible = false;
            undoPanel.relativePosition = new Vector3(-256, -594);
            UIUtil.SetupTitle("", undoPanel);
            var applyButton = undoPanel.AddUIComponent<UIButton>();
            applyButton.name = "Apply";
            applyButton.localeID = "MAPEDITOR_UNDO_TERRAIN";
            applyButton.size = new Vector2(191, 38);
            applyButton.relativePosition = new Vector3(20, 54);
            applyButton.normalBgSprite = "ButtonMenu";
            applyButton.hoveredBgSprite = "ButtonMenuHovered";
            applyButton.pressedBgSprite = "ButtonMenuPressed";
            applyButton.disabledBgSprite = "ButtonMenuDisabled";
            applyButton.canFocus = false;
            var utoPanel = undoPanel.gameObject.AddComponent<UndoTerrainOptionPanel>();
            applyButton.eventClick += (component, eventParam) => { utoPanel.UndoTerrain(); };

            var toolField = typeof(UndoTerrainOptionPanel).GetField("m_TerrainTool", BindingFlags.Instance | BindingFlags.NonPublic);
            toolField.SetValue(utoPanel, ToolsModifierControl.GetTool<TerrainTool>());
        }

        public static void SetupLevelHeightPanel(UIComponent optionsBar)
        {
            if (GameObject.Find("LevelHeightPanel") != null)
            {
                return;
            }
            var levelHeightPanel = optionsBar.AddUIComponent<UIPanel>();
            levelHeightPanel.backgroundSprite = "MenuPanel2";
            levelHeightPanel.isVisible = false;
            levelHeightPanel.size = new Vector2(231, 108);
            levelHeightPanel.relativePosition = new Vector2(-256, -702);
            levelHeightPanel.name = "LevelHeightPanel";
            UIUtil.SetupTitle("", levelHeightPanel);
            var heightLabel = levelHeightPanel.AddUIComponent<UILabel>();
            heightLabel.name = "HeightLabel";
            heightLabel.localeID = "MAPEDITOR_TERRAINLEVEL";
            heightLabel.size = new Vector2(134, 18);
            heightLabel.relativePosition = new Vector3(13, 56);
            var heightText = levelHeightPanel.AddUIComponent<UITextField>();
            heightText.name = "Height";
            heightText.size = new Vector2(52, 18);
            heightText.normalBgSprite = "TextFieldPanel";
            heightText.relativePosition = new Vector3(150, 56);
            heightText.builtinKeyNavigation = true;
            heightText.isInteractive = true;
            heightText.readOnly = false;
            heightText.selectionSprite = "EmptySprite";
            heightText.selectionBackgroundColor = new Color32(0, 172, 234, 255);

            var heightSlider = levelHeightPanel.AddUIComponent<UISlider>();
            heightSlider.name = "Height";
            heightSlider.relativePosition = new Vector3(28, 79);
            heightSlider.backgroundSprite = "ScrollbarTrack";
            heightSlider.size = new Vector2(174, 12);
            heightSlider.minValue = 0.0f;
            heightSlider.maxValue = 1024.0f;
            heightSlider.stepSize = 0.01f;
            var heightSliderThumb = heightSlider.AddUIComponent<UISlicedSprite>();
            heightSliderThumb.spriteName = "ScrollbarThumb";
            heightSliderThumb.size = new Vector2(10, 20);
            heightSlider.thumbObject = heightSliderThumb;

            levelHeightPanel.gameObject.AddComponent<LevelHeightOptionPanel>();
        }
    }
}