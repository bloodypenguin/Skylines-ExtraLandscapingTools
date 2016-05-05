using System;
using System.Reflection;
using ColossalFramework;
using NaturalResourcesBrush.Redirection;
using UnityEngine;

namespace NaturalResourcesBrush
{
    public class WaterToolDetour : WaterTool
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
                    typeof(WaterTool).GetMethod("Awake",
                    BindingFlags.Instance | BindingFlags.NonPublic),
                    typeof(WaterToolDetour).GetMethod("Awake",
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
                RedirectionHelper.RevertRedirect(typeof(WaterTool).GetMethod("Awake",
                        BindingFlags.Instance | BindingFlags.NonPublic), _state);
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogException(e);
            }
            _deployed = false;
        }

        protected override void Awake()
        {
            this.m_levelMaterial = new Material(Shader.Find("Custom/Overlay/WaterLevel"));
            this.m_sourceMaterial = new Material(Shader.Find("Custom/Tools/WaterSource"));
            m_sourceMaterial.color = new Color(48.0f / 255.0f, 140.0f / 255, 1.0f, 54.0f / 255.0f);
            this.m_sourceMesh = Util.Load<Mesh>("Cylinder01");
            RedirectionHelper.RevertRedirect(
                    typeof(WaterTool).GetMethod("Awake",
                        BindingFlags.Instance | BindingFlags.NonPublic),
                    _state
                );
            base.Awake();
            WaterToolDetour._state = RedirectionHelper.RedirectCalls
                (
                    typeof(WaterTool).GetMethod("Awake",
                        BindingFlags.Instance | BindingFlags.NonPublic),
                    typeof(WaterToolDetour).GetMethod("Awake",
                        BindingFlags.Instance | BindingFlags.NonPublic)
                );
        }
    }
}