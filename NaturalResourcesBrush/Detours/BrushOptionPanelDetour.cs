using System.Reflection;
using ColossalFramework.UI;
using NaturalResourcesBrush.API;
using NaturalResourcesBrush.Redirection;
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
            Plugins.SetStrength(val);
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
            if ((UnityEngine.Object)currentTool2 != (UnityEngine.Object)null)
                currentTool2.m_brushSize = val;
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
            Plugins.SetSize(val, val == (double)brushSizeSlider.minValue);
            //end mod
        }

        [RedirectMethod]
        public void OnClick(UIComponent comp, UIMouseEventParameter p)
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
            Plugins.SetBrush(texture2D);
            //end mod
        }

        [RedirectMethod]
        private bool SupportsSingle()
        {
            //begin mod
            if (Plugins.SupportsSingle(ToolsModifierControl.GetCurrentTool<ToolBase>()))
            {
                return true;
            }
            //end mod
            return ToolsModifierControl.GetCurrentTool<PropTool>() != null || ToolsModifierControl.GetCurrentTool<TreeTool>() != null;
        }


        [RedirectReverse]
        private void SelectByIndex(int value)
        {
            UnityEngine.Debug.Log("Failed to detour SelectByIndex()");
        }
    }
}