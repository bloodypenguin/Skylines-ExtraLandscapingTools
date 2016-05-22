using System.Collections.Generic;
using System.Reflection;
using NaturalResourcesBrush.Redirection;

namespace NaturalResourcesBrush.Detours
{
    [TargetType(typeof(UndoTerrainOptionPanel))]
    public class UndoTerrainOptionPanelDetour
    {
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