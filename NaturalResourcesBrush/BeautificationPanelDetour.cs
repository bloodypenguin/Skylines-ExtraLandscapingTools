using System;
using System.Reflection;
using ColossalFramework.UI;
using NaturalResourcesBrush.Redirection;
using UnityEngine;

namespace NaturalResourcesBrush
{
    public class BeautificationPanelDetour : GeneratedScrollPanel
    {
        private static RedirectCallsState _state;
        private static bool _deployed;

        public static void Deploy()
        {
            if (_deployed)
            {
                return;
            }
            try
            {
                _state = RedirectionHelper.RedirectCalls
                    (
                        typeof(BeautificationPanel).GetMethod("OnButtonClicked",
                            BindingFlags.Instance | BindingFlags.NonPublic),
                        typeof(BeautificationPanelDetour).GetMethod("OnButtonClicked",
                            BindingFlags.Instance | BindingFlags.NonPublic)
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
                    typeof(BeautificationPanel).GetMethod("OnButtonClicked",
                        BindingFlags.Instance | BindingFlags.NonPublic),
                    _state
                    );

            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogException(e);
            }
            _deployed = false;
        }

        public override ItemClass.Service service
        {
            get { throw new NotImplementedException(); }
        }

        protected override void OnButtonClicked(UIComponent comp)
        {
            object objectUserData = comp.objectUserData;
            BuildingInfo buildingInfo = objectUserData as BuildingInfo;
            NetInfo netInfo = objectUserData as NetInfo;
            TreeInfo treeInfo = objectUserData as TreeInfo;
            PropInfo propInfo = objectUserData as PropInfo;
            if (buildingInfo != null)
            {
                BuildingTool buildingTool = SetTool<BuildingTool>();
                if (buildingTool != null)
                {
                    if (pathsOptionPanel != null)
                        pathsOptionPanel.Hide();
                    buildingTool.m_prefab = buildingInfo;
                    buildingTool.m_relocate = 0;
                }
            }
            if (netInfo != null)
            {
                NetTool netTool = SetTool<NetTool>();
                if (netTool != null)
                {
                    if (pathsOptionPanel != null)
                        pathsOptionPanel.Show();
                    netTool.m_prefab = netInfo;
                }
            }
            if (treeInfo != null)
            {
                var prevTreeTool = GetCurrentTool<TreeTool>();
                TreeTool treeTool = SetTool<TreeTool>();
                if (treeTool != null)
                {
                    if (pathsOptionPanel != null)
                        pathsOptionPanel.Hide();
                    treeTool.m_prefab = treeInfo;
                    if (prevTreeTool == null)
                    {
                        var toolController = GameObject.FindObjectOfType<ToolController>();
                        treeTool.m_brush = toolController.m_brushes[3];
                        treeTool.m_brushSize = 30;
                        treeTool.m_mode = TreeTool.Mode.Single;
                    }
                }
            }
            if (!(propInfo != null))
                return;
            var prevPropTool = GetCurrentTool<PropTool>();
            PropTool propTool = SetTool<PropTool>();
            if (!(propTool != null))
                return;
            if (pathsOptionPanel != null)
                pathsOptionPanel.Hide();
            propTool.m_prefab = propInfo;
            if (prevPropTool == null)
            {
                propTool.m_mode = PropTool.Mode.Single;
            }
        }
    }
}