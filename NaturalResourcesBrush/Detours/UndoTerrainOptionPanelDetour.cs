using System;
using System.Collections.Generic;
using System.Reflection;
using NaturalResourcesBrush.Redirection;

namespace NaturalResourcesBrush
{
    [TargetType(typeof(UndoTerrainOptionPanel))]
    public class UndoTerrainOptionPanelDetour
    {
        private static Dictionary<MethodInfo, RedirectCallsState> _redirects;

        public static void Deploy()
        {
            if (_redirects != null)
            {
                return;
            }
            _redirects = RedirectionUtil.RedirectType(typeof(TerrainToolDetour));
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
        public void UndoTerrain()
        {
            var terrainTool = ToolsModifierControl.GetTool<TerrainTool>();
            if (terrainTool == null || !terrainTool.IsUndoAvailable())
                return;
            TerrainToolDetour.Undo(terrainTool);
        }
    }
}