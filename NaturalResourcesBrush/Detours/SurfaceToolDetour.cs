using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using ColossalFramework;
using NaturalResourcesBrush.Redirection;
using UnityEngine;

namespace NaturalResourcesBrush.Detours
{
    [TargetType(typeof(SurfaceTool))]
    public class SurfaceToolDetour : SurfaceTool
    {

        private static Dictionary<MethodInfo, RedirectCallsState> _redirects;

        public static void Deploy()
        {
            if (_redirects != null)
            {
                return;
            }
            _redirects = RedirectionUtil.RedirectType(typeof(SurfaceToolDetour));
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
        protected override void OnToolGUI(UnityEngine.Event e)
        {
            // UnityEngine.Debug.Log("OnToolGUI");
            if (this.m_toolController.IsInsideUI)
                return;
            if (e.type == UnityEngine.EventType.MouseDown)
            {
                if (e.button == 0)
                {
                    Singleton<SimulationManager>.instance.AddAction(this.BeginDrawing());
                }
                else
                {
                    if (e.button != 1)
                        return;
                    //Singleton<SimulationManager>.instance.AddAction(this.BeginErasing());
                }
            }
            else
            {
                if (e.type != UnityEngine.EventType.MouseUp)
                    return;
                if (e.button == 0)
                {
                    //Singleton<SimulationManager>.instance.AddAction(this.EndDrawing());
                }
                else
                {
                    if (e.button != 1)
                        return;
                    // Singleton<SimulationManager>.instance.AddAction(this.EndErasing());
                }
            }
        }


        [RedirectMethod]
        IEnumerator BeginDrawing()
        {

            UnityEngine.Debug.Log("ApplyQuad");
            var mousePos =
                (Vector3)
                    typeof(SurfaceTool).GetField("m_mousePosition", BindingFlags.NonPublic | BindingFlags.Instance)
                        .GetValue(this);
            var m_brushSize = 10;
            var surfaceManager = SurfaceManager.instance;
            surfaceManager.elements.Add(new SurfaceManager.SurfaceElement()
            {
                x1 = mousePos.x - m_brushSize,
                x2 = mousePos.x + m_brushSize,
                x3 = mousePos.x + m_brushSize,
                x4 = mousePos.x - m_brushSize,

                z1 = mousePos.z + m_brushSize,
                z2 = mousePos.z + m_brushSize,
                z3 = mousePos.z - m_brushSize,
                z4 = mousePos.z - m_brushSize,

                surface = m_surface
            });


            TerrainModify.UpdateArea(mousePos.x - m_brushSize, mousePos.z - m_brushSize, mousePos.x + m_brushSize, mousePos.z + m_brushSize, false, true, false);

            yield return null;
        }
    }
}