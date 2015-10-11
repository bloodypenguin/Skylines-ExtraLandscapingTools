using System;
using System.Linq;
using System.Reflection;
using ColossalFramework;
using ColossalFramework.UI;
using UnityEngine;

namespace NaturalResourcesBrush
{
    public class ResourcePanelDetour : GeneratedScrollPanel
    {
        private static readonly MethodInfo SpawnEntryMethod = typeof (ResourcePanel).GetMethod("SpawnEntry",
            BindingFlags.NonPublic | BindingFlags.Instance,
            null, new Type[] {typeof (string), typeof (int)}, null);

        private static readonly FieldInfo OptionsBrushPanelField = typeof (ResourcePanel).GetField(
            "m_OptionsBrushPanel",
            BindingFlags.NonPublic | BindingFlags.Instance);
        private static readonly PositionData<NaturalResourceManager.Resource>[] kResources =
            Utils.GetOrderedEnumData<NaturalResourceManager.Resource>("Resource").Concat(
            Utils.GetOrderedEnumData<NaturalResourceManager.Resource>("Decorative")).ToArray();

        private static RedirectCallsState _state1;
        private static RedirectCallsState _state2;
        private static bool _deployed;

        public static void Deploy()
        {
            if (_deployed)
            {
                return;
            }
            try
            {
                _state1 = RedirectionHelper.RedirectCalls
                    (
                        typeof(ResourcePanel).GetMethod("OnButtonClicked",
                            BindingFlags.Instance | BindingFlags.NonPublic),
                        typeof(ResourcePanelDetour).GetMethod("OnButtonClicked",
                            BindingFlags.Instance | BindingFlags.NonPublic)
                    );
                _state2 = RedirectionHelper.RedirectCalls
                    (
                        typeof(ResourcePanel).GetMethod("RefreshPanel",
                            BindingFlags.Instance | BindingFlags.Public),
                        typeof(ResourcePanelDetour).GetMethod("RefreshPanel",
                            BindingFlags.Instance | BindingFlags.Public)
                    );
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogException(e);
            }
            _deployed = true;
        }

        public static void Revert()
        {
            if (!_deployed)
            {
                return;
            }
            try
            {
                RedirectionHelper.RevertRedirect(
                    typeof(ResourcePanel).GetMethod("OnButtonClicked",
                        BindingFlags.Instance | BindingFlags.NonPublic),
                    _state1
                    );
                RedirectionHelper.RevertRedirect(
                        typeof(ResourcePanel).GetMethod("RefreshPanel",
                            BindingFlags.Instance | BindingFlags.Public),
                    _state2
                    );

            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogException(e);
            }
            _deployed = false;
        }



        public override void RefreshPanel()
        {
            base.RefreshPanel();
            for (int index = 0; index < kResources.Length; ++index)
            {
                SpawnEntryMethod.Invoke(this, new object[] {kResources[index].enumName, index});
            }
        }

        protected override void OnButtonClicked(UIComponent comp)
        {
            ResourceTool resourceTool = ToolsModifierControl.SetTool<ResourceTool>();
            if (resourceTool == null)
                return;
            var m_OptionsBrushPanel = (UIPanel)OptionsBrushPanelField.GetValue(this);
            if (m_OptionsBrushPanel != null)
                m_OptionsBrushPanel.isVisible = true;
            resourceTool.m_resource = kResources[comp.zOrder].enumValue;
        }

        public override ItemClass.Service service
        {
            get { throw new System.NotImplementedException(); }
        }
    }
}