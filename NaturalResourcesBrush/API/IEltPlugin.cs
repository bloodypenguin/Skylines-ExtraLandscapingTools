using System.Collections.Generic;
using ICities;
using UnityEngine;

namespace NaturalResourcesBrush.API
{
    public interface IEltPlugin
    {
        void Initialize();
        void Dispose();
        void SetSize(float size, bool minSliderValue);
        void SetStrength(float strength);
        void SetBrush(Texture2D brush);
        IEnumerable<ToolBase> SetupTools(LoadMode mode);
        void CreateToolbars(LoadMode mode);
        bool SupportsSingle(ToolBase tool);
    }
}