using System;
using System.Collections;
using System.Reflection;
using ColossalFramework;
using NaturalResourcesBrush.Redirection;
using UnityEngine;

namespace NaturalResourcesBrush.Detours
{
    [TargetType(typeof(TreeTool))]
    public class TreeToolDetour : TreeTool
    {
        private static FieldInfo mouseLeftDownField = typeof(TreeTool).GetField("m_mouseLeftDown",
            BindingFlags.NonPublic | BindingFlags.Instance);
        private static FieldInfo mouseRightDownField = typeof(TreeTool).GetField("m_mouseRightDown",
    BindingFlags.NonPublic | BindingFlags.Instance);
        private static FieldInfo mousePositionField = typeof(TreeTool).GetField("m_mousePosition",
BindingFlags.NonPublic | BindingFlags.Instance);
        private static MethodInfo createTreeMethodInfo = typeof(TreeTool).GetMethod("CreateTree",
            BindingFlags.NonPublic | BindingFlags.Instance);

        private static Vector3 lastAllowedMousePosition = Vector3.zero;

        [RedirectMethod]
        protected override void OnToolGUI(Event e)
        {
            if (!this.m_toolController.IsInsideUI &&
                e.type == EventType.MouseDown || (e.type == EventType.MouseDrag && this.m_mode == TreeTool.Mode.Single))
            {
                if (e.button == 0)
                {

                    mouseLeftDownField.SetValue(this, true);
                    if (this.m_mode != TreeTool.Mode.Single)
                        return;
                    var mousePosition = (Vector3)mousePositionField.GetValue(this);
                    if (!lastAllowedMousePosition.Equals(Vector3.zero))
                    {
                        if (m_strength < 1.0)
                        {
                            var distance = 25;
                            if (Math.Pow(mousePosition.x - lastAllowedMousePosition.x, 2) +
                                Math.Pow(mousePosition.z - lastAllowedMousePosition.z, 2) < Math.Pow(distance - distance * m_strength, 2))
                            {
                                return;
                            }
                        }
                    }
                    lastAllowedMousePosition = mousePosition;
                    Singleton<SimulationManager>.instance.AddAction(
                        (IEnumerator)createTreeMethodInfo.Invoke(this, new object[] { }));
                }
                else
                {
                    if (e.button != 1)
                        return;
                    mouseRightDownField.SetValue(this, true);
                }
            }
            else
            {
                if (e.type != EventType.MouseUp)
                    return;
                if (e.button == 0)
                {
                    mouseLeftDownField.SetValue(this, false);
                    lastAllowedMousePosition = Vector3.zero;
                }
                else
                {
                    if (e.button != 1)
                        return;
                    mouseRightDownField.SetValue(this, false);
                }
            }
        }
    }
}