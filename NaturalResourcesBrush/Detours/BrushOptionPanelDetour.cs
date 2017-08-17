using System;
using System.Reflection;
using ColossalFramework;
using ColossalFramework.UI;
using NaturalResourcesBrush.API;
using NaturalResourcesBrush.RedirectionFramework.Attributes;
using NaturalResourcesBrush.Utils;
using UnityEngine;

namespace NaturalResourcesBrush.Detours
{
    [TargetType(typeof(BrushOptionPanel))]
    public class BrushOptionPanelDetour : BrushOptionPanel
    {

        [RedirectMethod]
        private void SetBrushStrength(float val)
        {
            PropTool currentTool1 = ToolsModifierControl.GetCurrentTool<PropTool>();
            if ((UnityEngine.Object)currentTool1 != (UnityEngine.Object)null)
                currentTool1.m_strength = val;
            TerrainTool currentTool2 = ToolsModifierControl.GetCurrentTool<TerrainTool>();
            if ((UnityEngine.Object)currentTool2 != (UnityEngine.Object)null)
                currentTool2.m_strength = val;
            TreeTool currentTool3 = ToolsModifierControl.GetCurrentTool<TreeTool>();
            if ((UnityEngine.Object)currentTool3 != (UnityEngine.Object)null)
                currentTool3.m_strength = val;
            ResourceTool currentTool4 = ToolsModifierControl.GetCurrentTool<ResourceTool>();
            if ((UnityEngine.Object)currentTool4 != (UnityEngine.Object)null)
                currentTool4.m_strength = val;
            //begin mod
            try
            {
                Plugins.SetStrength(val);
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogException(e);
            }
            //end mod
        }

        [RedirectMethod]
        private void SetBrushSize(float val)
        {
            var brushSizeSlider =
                (UISlider)typeof(BrushOptionPanel).GetField("m_BrushSizeSlider", BindingFlags.NonPublic | BindingFlags.Instance)
                    .GetValue(this);

            PropTool currentTool1 = ToolsModifierControl.GetCurrentTool<PropTool>();
            if ((UnityEngine.Object)currentTool1 != (UnityEngine.Object)null)
            {
                currentTool1.m_brushSize = val;
                currentTool1.m_mode = (double)currentTool1.m_brushSize != (double)brushSizeSlider.minValue ? PropTool.Mode.Brush : PropTool.Mode.Single;
            }
            TerrainTool currentTool2 = ToolsModifierControl.GetCurrentTool<TerrainTool>();
            //begin mod
            if ((UnityEngine.Object)currentTool2 != (UnityEngine.Object)null)
            {
                currentTool2.m_brushSize = val;
                TerrainToolDetour.m_sizeMode = (double)currentTool2.m_brushSize != (double)brushSizeSlider.minValue ? TerrainToolDetour.SizeMode.Brush : TerrainToolDetour.SizeMode.Single;
            }
            //end mod
            TreeTool currentTool3 = ToolsModifierControl.GetCurrentTool<TreeTool>();
            if ((UnityEngine.Object)currentTool3 != (UnityEngine.Object)null)
            {
                currentTool3.m_brushSize = val;
                currentTool3.m_mode = (double)currentTool3.m_brushSize != (double)brushSizeSlider.minValue ? TreeTool.Mode.Brush : TreeTool.Mode.Single;
            }
            ResourceTool currentTool4 = ToolsModifierControl.GetCurrentTool<ResourceTool>();
            if ((UnityEngine.Object)currentTool4 != (UnityEngine.Object)null)
            {
                currentTool4.m_brushSize = val;
            }
            //begin mod
            try
            {
                Plugins.SetSize(val, val == (double)brushSizeSlider.minValue);
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogException(e);
            }
            //end mod
        }

        [RedirectMethod]
        public new void OnMouseDown(UIComponent comp, UIMouseEventParameter p)
        {
            UIButton uiButton = p.source as UIButton;
            int byIndex = this.GetByIndex((UIComponent)uiButton);
            if (byIndex == -1)
                return;
            this.SelectByIndex(byIndex);
            Texture2D texture2D = uiButton.objectUserData as Texture2D;
            var brushesContainer =
    (UIScrollablePanel)typeof(BrushOptionPanel).GetField("m_BrushesContainer", BindingFlags.NonPublic | BindingFlags.Instance)
        .GetValue(this);
            if (!((UnityEngine.Object)uiButton.parent == (UnityEngine.Object)brushesContainer) || !((UnityEngine.Object)texture2D != (UnityEngine.Object)null))
                return;
            TerrainTool tool1 = ToolsModifierControl.GetTool<TerrainTool>();
            if ((UnityEngine.Object)tool1 != (UnityEngine.Object)null)
                tool1.m_brush = texture2D;
            TreeTool tool2 = ToolsModifierControl.GetTool<TreeTool>();
            if ((UnityEngine.Object)tool2 != (UnityEngine.Object)null)
                tool2.m_brush = texture2D;
            ResourceTool tool3 = ToolsModifierControl.GetTool<ResourceTool>();
            if ((UnityEngine.Object)tool3 != (UnityEngine.Object)null)
                tool3.m_brush = texture2D;
            PropTool tool4 = ToolsModifierControl.GetTool<PropTool>();
            if ((UnityEngine.Object)tool4 == (UnityEngine.Object)null)
            {
                tool4.m_brush = texture2D;
            }
            //begin mod
            try
            {
                Plugins.SetBrush(texture2D);
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogException(e);
            }
            //end mod
        }

        [RedirectMethod]
        private bool SupportsSingle()
        {
            //begin mod
            try
            {
                if (Plugins.SupportsSingle(ToolsModifierControl.GetCurrentTool<ToolBase>()))
                {
                    return true;
                }
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogException(e);
            }
            return ToolsModifierControl.GetCurrentTool<PropTool>() != null || ToolsModifierControl.GetCurrentTool<TreeTool>() != null || ToolsModifierControl.GetCurrentTool<TerrainTool>() != null;
            //end mod
        }

        [RedirectMethod]
        private static void ProcessKeyEvent(BrushOptionPanel panel, UnityEngine.EventType eventType, KeyCode keyCode, EventModifiers modifiers)
        {
            if (eventType != UnityEngine.EventType.KeyDown)
                return;
            var mBrushSizeSlider = Util.GetPrivate<UISlider>(panel, "m_BrushSizeSlider");
            var sizeInterval = GetSliderValue(mBrushSizeSlider) < 50f ? 1f : 50f;
            if (Util.GetPrivate<SavedInputKey>(panel, "m_IncreaseBrushSize").IsPressed(eventType, keyCode, modifiers))
                SetSliderValue(mBrushSizeSlider, GetSliderValue(mBrushSizeSlider) + sizeInterval);
            else if (Util.GetPrivate<SavedInputKey>(panel, "m_DecreaseBrushSize").IsPressed(eventType, keyCode, modifiers))
                SetSliderValue(mBrushSizeSlider, GetSliderValue(mBrushSizeSlider) - sizeInterval);
            else
            {
                var mBrushStrengthSlider = Util.GetPrivate<UISlider>(panel, "m_BrushStrengthSlider");
                var strengthInterval = 0.1f;
                if (Util.GetPrivate<SavedInputKey>(panel, "m_IncreaseBrushStrength").IsPressed(eventType, keyCode, modifiers))
                {
                    mBrushStrengthSlider.value = mBrushStrengthSlider.value + strengthInterval;
                }
                else
                {
                    if (!Util.GetPrivate<SavedInputKey>(panel, "m_DecreaseBrushStrength").IsPressed(eventType, keyCode, modifiers))
                        return;
                    mBrushStrengthSlider.value = mBrushStrengthSlider.value - strengthInterval;
                }
            }
        }

        [RedirectReverse]
        private static void SetSliderValue(UISlider slider, float value)
        {
            UnityEngine.Debug.Log("Failed to detour SetSliderValue()");
        }

        [RedirectReverse]
        private static float GetSliderValue(UISlider slider)
        {
            UnityEngine.Debug.Log("Failed to detour SelectByIndex()");
            return 0f;
        }


        [RedirectReverse]
        private void SelectByIndex(int value)
        {
            UnityEngine.Debug.Log("Failed to detour SelectByIndex()");
        }
    }
}