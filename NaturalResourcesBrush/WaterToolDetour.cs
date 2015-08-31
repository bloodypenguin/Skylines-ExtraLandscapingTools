using System.Reflection;
using UnityEngine;

namespace NaturalResourcesBrush
{
    public class WaterToolDetour : WaterTool
    {

        public static RedirectCallsState _state;

        protected override void Awake()
        {
            this.m_levelMaterial = new Material(Shader.Find("Custom/Overlay/WaterLevel"));
            this.m_sourceMaterial = new Material(Shader.Find("Custom/Tools/WaterSource"));
            this.m_sourceMesh = ResourceUtils.Load<Mesh>("Cylinder01");
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