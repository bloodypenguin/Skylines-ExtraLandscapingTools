using ColossalFramework.UI;
using UnityEngine;

namespace ExtraLanscapingToolsCommon
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

        public static UIScrollbar SetUpScrollbar(UIComponent comp)
        {
            var scrollbar = comp.AddUIComponent<UIScrollbar>();
            scrollbar.name = "Scrollbar";
            scrollbar.width = 20f;
            scrollbar.height = comp.height;
            scrollbar.orientation = UIOrientation.Vertical;
            scrollbar.pivot = UIPivotPoint.BottomLeft;
            scrollbar.AlignTo(comp, UIAlignAnchor.TopRight);
            scrollbar.minValue = 0;
            scrollbar.value = 0;
            scrollbar.incrementAmount = 50;

            UISlicedSprite tracSprite = scrollbar.AddUIComponent<UISlicedSprite>();
            tracSprite.relativePosition = Vector2.zero;
            tracSprite.autoSize = true;
            tracSprite.size = tracSprite.parent.size;
            tracSprite.fillDirection = UIFillDirection.Vertical;
            tracSprite.spriteName = "ScrollbarTrack";
            tracSprite.name = "Track";
            scrollbar.trackObject = tracSprite;
            scrollbar.trackObject.height = scrollbar.height;

            UISlicedSprite thumbSprite = tracSprite.AddUIComponent<UISlicedSprite>();
            thumbSprite.relativePosition = Vector2.zero;
            thumbSprite.fillDirection = UIFillDirection.Vertical;
            thumbSprite.autoSize = true;
            thumbSprite.width = thumbSprite.parent.width - 8;
            thumbSprite.spriteName = "ScrollbarThumb";
            thumbSprite.name = "Thumb";

            scrollbar.thumbObject = thumbSprite;
            scrollbar.isVisible = true;
            scrollbar.isEnabled = true;
            return scrollbar;
        }
    }
}