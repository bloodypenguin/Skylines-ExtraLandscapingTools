using System;
using System.Reflection;
using NaturalResourcesBrush.RedirectionFramework;
using NaturalResourcesBrush.RedirectionFramework.Attributes;
using NaturalResourcesBrush.Utils;
using UnityEngine;

namespace NaturalResourcesBrush.Detours
{
    [TargetType(typeof(WaterTool))]
    public class WaterToolDetour : WaterTool
    {
        [RedirectMethod]
        protected override void Awake()
        {
            this.m_levelMaterial = new Material(Shader.Find("Custom/Overlay/WaterLevel"));
            this.m_sourceMaterial = new Material(Shader.Find("Custom/Tools/WaterSource"));
            m_sourceMaterial.color = new Color(48.0f / 255.0f, 140.0f / 255, 1.0f, 54.0f / 255.0f);
            this.m_sourceMesh = Util.Load<Mesh>("Cylinder01");
            Redirector<WaterToolDetour>.Revert();
            base.Awake();
            Redirector<WaterToolDetour>.Deploy();
        }
    }
}