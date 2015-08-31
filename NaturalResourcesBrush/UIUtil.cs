using ColossalFramework;
using ColossalFramework.UI;
using UnityEngine;

namespace NaturalResourcesBrush
{
    public static class UIUtil
    {

        public static void SetupTitle(string text, UIComponent parentPanel)
        {
            var title = parentPanel.AddUIComponent<UIPanel>();
            title.size = new Vector2(parentPanel.width, 40);
            title.canFocus = true;
            title.isInteractive = true;
            title.relativePosition = Vector3.zero;

            var dragHandle = title.AddUIComponent<UIDragHandle>();
            dragHandle.size = title.size;
            dragHandle.relativePosition = Vector3.zero;
            dragHandle.target = parentPanel;

            var windowName = dragHandle.AddUIComponent<UILabel>();
            windowName.relativePosition = new Vector3(60, 13);
            windowName.text = text;
        }

        public static void SetupBrushSizePanel(UIComponent brushOptionsPanel)
        {
            var brushSizePanel = brushOptionsPanel.AddUIComponent<UIPanel>();
            brushSizePanel.size = new Vector2(197, 49);
            brushSizePanel.relativePosition = new Vector2(17, 57);
            brushSizePanel.name = "Size";
            var brushSizeLabel = brushSizePanel.AddUIComponent<UILabel>();
            brushSizeLabel.text = "Size";
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
            brushSizeSlider.minValue = 29;
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
            brushStrengthLabel.text = "Strength";
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
        }

        public static void SetupWaterCapacityPanel(UIComponent waterOptionsPanel)
        {
            var waterCapacityPanel = waterOptionsPanel.AddUIComponent<UIPanel>();
            waterCapacityPanel.size = new Vector2(231, 78);
            waterCapacityPanel.relativePosition = new Vector2(0, 40);
            waterCapacityPanel.name = "Settings";
            var waterCapacityLabel = waterCapacityPanel.AddUIComponent<UILabel>();
            waterCapacityLabel.text = "Water Capacity";
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
            resetButton.text = "Reset Water To Sea Level";
            resetButton.size = new Vector2(191, 38);
            resetButton.relativePosition = new Vector3(20, 132);
            resetButton.eventClick += (component, eventParam) => { Singleton<TerrainManager>.instance.WaterSimulation.m_resetWater = true; };
            resetButton.normalBgSprite = "ButtonMenu";
            resetButton.hoveredBgSprite = "ButtonMenuHovered";
            resetButton.pressedBgSprite = "ButtonMenuPressed";
            resetButton.disabledBgSprite = "ButtonMenuDisabled";
            resetButton.canFocus = false;
        }
    }
}