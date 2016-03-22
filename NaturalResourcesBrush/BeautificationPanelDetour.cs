using System;
using System.Collections.Generic;
using System.Reflection;
using ColossalFramework.UI;
using NaturalResourcesBrush.Redirection;
using UnityEngine;

namespace NaturalResourcesBrush
{
    [TargetType(typeof(BeautificationPanel))]
    public class BeautificationPanelDetour : GeneratedScrollPanel
    {
        private static Dictionary<MethodInfo, RedirectCallsState> _redirects;

        private static UIPanel m_OptionsBrushPanel;

        public static void Deploy()
        {
            if (_redirects != null)
            {
                return;
            }
            _redirects = RedirectionUtil.RedirectType(typeof(BeautificationPanelDetour));
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

            m_OptionsBrushPanel = null;
        }


        public override ItemClass.Service service
        {
            get { throw new NotImplementedException(); }
        }

        [RedirectMethod]
        protected override void OnButtonClicked(UIComponent comp)
        {
            object objectUserData = comp.objectUserData;
            BuildingInfo buildingInfo = objectUserData as BuildingInfo;
            NetInfo netInfo = objectUserData as NetInfo;
            TreeInfo treeInfo = objectUserData as TreeInfo;
            PropInfo propInfo = objectUserData as PropInfo;
            m_OptionsBrushPanel?.Hide();
            if (buildingInfo != null)
            {
                BuildingTool buildingTool = SetTool<BuildingTool>();
                if (buildingTool != null)
                {
                    this.HideAllOptionPanels();
                    buildingTool.m_prefab = buildingInfo;
                    buildingTool.m_relocate = 0;
                }
            }
            if (netInfo != null)
            {
                NetTool netTool = SetTool<NetTool>();
                if (netTool != null)
                {
                    if (netInfo.GetClassLevel() == ItemClass.Level.Level3)
                        this.ShowFloodwallsOptionPanel();
                    else if (netInfo.GetClassLevel() == ItemClass.Level.Level4)
                        this.ShowQuaysOptionPanel();
                    else if (netInfo.GetClassLevel() == ItemClass.Level.Level5)
                        this.ShowCanalsOptionPanel();
                    else
                        this.ShowPathsOptionPanel();
                    netTool.m_prefab = netInfo;
                }
            }
            if (treeInfo != null)
            {
                var prevTreeTool = GetCurrentTool<TreeTool>();
                TreeTool treeTool = SetTool<TreeTool>();
                if (treeTool != null)
                {
                    this.HideAllOptionPanels();
                    treeTool.m_prefab = treeInfo;
                    if (prevTreeTool == null)
                    {
                        treeTool.m_brush = toolController.m_brushes[3];
                        treeTool.m_brushSize = 30;
                        treeTool.m_mode = TreeTool.Mode.Single;
                    }

                    if (this.m_OptionsBar != null && m_OptionsBrushPanel == null)
                    {
                        m_OptionsBrushPanel = this.m_OptionsBar.Find<UIPanel>("BrushPanel");
                    }
                    m_OptionsBrushPanel.zOrder = 1;
                    m_OptionsBrushPanel.Show();
                }
            }
            if (!(propInfo != null))
                return;
            var prevPropTool = GetCurrentTool<PropTool>();
            PropTool propTool = SetTool<PropTool>();
            if (!(propTool != null))
                return;
            this.HideAllOptionPanels();
            propTool.m_prefab = propInfo;
            if (prevPropTool == null)
            {
                propTool.m_mode = PropTool.Mode.Single;
            }
        }
    }
}