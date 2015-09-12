using System;
using System.Collections;
using System.Reflection;
using ColossalFramework;
using UnityEngine;

namespace NaturalResourcesBrush
{
    public class TreeToolDetour : TreeTool
    {

        private static RedirectCallsState _state;

        private static FieldInfo mouseLeftDownField = typeof(TreeTool).GetField("m_mouseLeftDown",
            BindingFlags.NonPublic | BindingFlags.Instance);
        private static FieldInfo mouseRightDownField = typeof(TreeTool).GetField("m_mouseRightDown",
    BindingFlags.NonPublic | BindingFlags.Instance);

        private static MethodInfo createTreeMethodInfo = typeof(TreeTool).GetMethod("CreateTree",
            BindingFlags.NonPublic | BindingFlags.Instance);

        public static void Deploy()
        {
            try
            {
                _state = RedirectionHelper.RedirectCalls(
                    typeof(TreeTool).GetMethod("OnToolGUI", BindingFlags.NonPublic | BindingFlags.Instance),
                    typeof(TreeToolDetour).GetMethod("OnToolGUI", BindingFlags.NonPublic | BindingFlags.Instance));

            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogException(e);
            }
        }

        public static void Revert()
        {
            RedirectionHelper.RevertRedirect(typeof(TreeTool).GetMethod("OnToolGUI", BindingFlags.NonPublic | BindingFlags.Instance), _state);
        }

        protected override void OnToolGUI()
        {

            Event current = Event.current;
            if (!this.m_toolController.IsInsideUI &&
                current.type == EventType.MouseDown || (current.type == EventType.MouseDrag && this.m_mode == TreeTool.Mode.Single))
            {
                if (current.button == 0)
                {

                    mouseLeftDownField.SetValue(this, true);
                    if (this.m_mode != TreeTool.Mode.Single)
                        return;
                    Singleton<SimulationManager>.instance.AddAction((IEnumerator)createTreeMethodInfo.Invoke(this, new object[] { }));

                }
                else
                {
                    if (current.button != 1)
                        return;
                    mouseRightDownField.SetValue(this, true);
                }
            }
            else
            {
                if (current.type != EventType.MouseUp)
                    return;
                if (current.button == 0)
                {
                    mouseLeftDownField.SetValue(this, false);
                }
                else
                {
                    if (current.button != 1)
                        return;
                    mouseRightDownField.SetValue(this, false);
                }
            }
        }
    }
}