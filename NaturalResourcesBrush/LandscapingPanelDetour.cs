using System;
using System.Collections.Generic;
using System.Reflection;
using ColossalFramework;
using ColossalFramework.UI;
using NaturalResourcesBrush.Redirection;
using UnityEngine;
using Object = System.Object;

namespace NaturalResourcesBrush
{
    [TargetType(typeof(LandscapingPanel))]
    public class LandscapingPanelDetour : GeneratedScrollPanel
    {
        private static readonly PositionData<TerrainTool.Mode>[] kTools = Utils.GetOrderedEnumData<TerrainTool.Mode>();

        private static readonly MethodInfo ShowBrushOptionsPanelMethod =
            typeof(LandscapingPanel).GetMethod("ShowBrushOptionsPanel", BindingFlags.Instance | BindingFlags.NonPublic);
        private static readonly MethodInfo ShowLevelHeightPanelMethod =
    typeof(TerrainPanel).GetMethod("ShowLevelHeightPanel", BindingFlags.Instance | BindingFlags.NonPublic);

        private static readonly FieldInfo OptionsBrushPanelField = typeof(TerrainPanel).GetField(
            "m_OptionsBrushPanel",
            BindingFlags.NonPublic | BindingFlags.Instance);

        private static Dictionary<MethodInfo, RedirectCallsState> _redirects;

        [RedirectReverse]
        private static void SpawnEntry(LandscapingPanel panel, string name, bool enabled, MilestoneInfo info)
        {
            UnityEngine.Debug.Log($"{panel}-{name}-{enabled}-{info}");
        }

        public static void Deploy()
        {
            if (_redirects != null)
            {
                return;
            }
            _redirects = RedirectionUtil.RedirectType(typeof(LandscapingPanelDetour));
        }

        public static void Revert()
        {
            if (_redirects == null)
            {
                return;
            }
            foreach (var redirect in _redirects)
            {
                RedirectionHelper.RevertRedirect(redirect.Key, redirect.Value);
            }
            _redirects = null;
        }

        [RedirectMethod]
        public override void RefreshPanel()
        {
            base.RefreshPanel();
            int index;
            var panel = (LandscapingPanel)Convert.ChangeType(this, typeof(LandscapingPanel));;

            for (index = 0; index < LandscapingPanelDetour.kTools.Length; ++index)
                SpawnEntry(panel, LandscapingPanelDetour.kTools[index].enumName, true, null);
            SpawnEntry(panel, "Ditch", true, null);
            //TODO(earalov): restore
//            var ditchButton = gameObject.transform.FindChild("Ditch").gameObject.GetComponent<UIButton>();
//            if (ditchButton != null)
//            {
//                ditchButton.atlas = Util.CreateAtlasFromEmbeddedResources(new List<string> { "TerrainDitch" });
//            }
        }

        [RedirectMethod]
        protected override void OnButtonClicked(UIComponent comp)
        {
            int zOrder = comp.zOrder;
            TerrainTool terrainTool = ToolsModifierControl.SetTool<TerrainTool>();
            if ((Object)terrainTool != (Object)null)
            {
                if ((Object)OptionsBrushPanelField.GetValue(this) != (Object)null)
                    ((UIPanel)OptionsBrushPanelField.GetValue(this)).isVisible = true;
                if (zOrder < kTools.Length)
                {
                    terrainTool.m_mode = LandscapingPanelDetour.kTools[zOrder].enumValue;
                    TerrainToolDetour.isDitch = false;
                    TerrainToolDetour.ditchCombineMultipleStrokes = false;
                    //TODO(earalov): hide ditch properties panel
                }
                else
                {
                    terrainTool.m_mode = TerrainTool.Mode.Shift;
                    TerrainToolDetour.isDitch = true;
                    TerrainToolDetour.ditchCombineMultipleStrokes = false;
                    //TODO(earalov): show & refresh ditch properties panel
                }
            }
            ShowBrushOptionsPanelMethod.Invoke(this, new object[] { true });
//TODO(earalov): bring back
//            if (zOrder == 1 || zOrder == 3)
//                ShowLevelHeightPanelMethod.Invoke(this, new object[] { true });
//            else
//                ShowLevelHeightPanelMethod.Invoke(this, new object[] { false });
        }

        public override ItemClass.Service service { get; }
    }
}