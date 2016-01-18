using System;
using System.Collections.Generic;
using System.Reflection;
using ColossalFramework;
using ColossalFramework.UI;
using NaturalResourcesBrush.Redirection;

namespace NaturalResourcesBrush
{
    [TargetType(typeof(TerrainPanel))]
    public class TerrainPanelDetour : GeneratedScrollPanel
    {
        private static readonly PositionData<TerrainTool.Mode>[] kTools = Utils.GetOrderedEnumData<TerrainTool.Mode>();
        private static readonly MethodInfo SpawnEntryMethod = typeof(TerrainPanel).GetMethod("SpawnEntry",
    BindingFlags.NonPublic | BindingFlags.Instance,
    null, new Type[] { typeof(string), typeof(int) }, null);

        private static readonly MethodInfo ShowBrushOptionsPanelMethod =
            typeof(TerrainPanel).GetMethod("ShowBrushOptionsPanel", BindingFlags.Instance | BindingFlags.NonPublic);
        private static readonly MethodInfo ShowLevelHeightPanelMethod =
    typeof(TerrainPanel).GetMethod("ShowLevelHeightPanel", BindingFlags.Instance | BindingFlags.NonPublic);

        private static readonly FieldInfo OptionsBrushPanelField = typeof(TerrainPanel).GetField(
            "m_OptionsBrushPanel",
            BindingFlags.NonPublic | BindingFlags.Instance);

        private static Dictionary<MethodInfo, RedirectCallsState> _redirects;

        public static void Deploy()
        {
            if (_redirects != null)
            {
                return;
            }
            _redirects = RedirectionUtil.RedirectType(typeof(TerrainPanelDetour));
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
            for (index = 0; index < TerrainPanelDetour.kTools.Length; ++index)
                SpawnEntryMethod.Invoke(this, new object[] { TerrainPanelDetour.kTools[index].enumName, index });
            var ditchButton = (UIButton)SpawnEntryMethod.Invoke(this, new object[] { "Ditch", index });
            ditchButton.atlas = Util.CreateAtlasFromEmbeddedResources(new List<string> { "TerrainDitch" });
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
                    terrainTool.m_mode = TerrainPanelDetour.kTools[zOrder].enumValue;
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
            if (zOrder == 1 || zOrder == 3)
                ShowLevelHeightPanelMethod.Invoke(this, new object[] { true });
            else
                ShowLevelHeightPanelMethod.Invoke(this, new object[] { false });
        }

        public override ItemClass.Service service { get; }
    }
}