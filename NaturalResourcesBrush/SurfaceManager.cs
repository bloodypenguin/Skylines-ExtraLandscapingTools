using System;
using System.Collections.Generic;
using ColossalFramework;
using UnityEngine;

namespace NaturalResourcesBrush
{
    public class SurfaceManager : Singleton<SurfaceManager>, ITerrainManager
    {
        public List<SurfaceElement> elements = new List<SurfaceElement>();

        public void TerrainUpdated(TerrainArea heightArea, TerrainArea surfaceArea, TerrainArea zoneArea)
        {
            foreach (var surfaceElement in elements)
            {
//                if (!surfaceElement.BelongsTo(surfaceArea.m_minX, surfaceArea.m_maxX, surfaceArea.m_minZ,
//                    surfaceArea.m_maxZ))
//                {
//                    continue;
//                }
                var a1 = new Vector3(surfaceElement.x1, 0, surfaceElement.z1);
                var a2 = new Vector3(surfaceElement.x2, 0, surfaceElement.z2);
                var a3 = new Vector3(surfaceElement.x3, 0, surfaceElement.z3);
                var a4 = new Vector3(surfaceElement.x4, 0, surfaceElement.z4);
                TerrainModify.ApplyQuad(a1, a2, a3, a4, TerrainModify.Edges.All, TerrainModify.Heights.None, surfaceElement.surface);
            }
        }

        public void AfterTerrainUpdate(TerrainArea heightArea, TerrainArea surfaceArea, TerrainArea zoneArea)
        {
        }

        public struct SurfaceElement
        {
            public float x1, x2, x3, x4;
            public float z1, z2, z3, z4;
            public TerrainModify.Surface surface;

            public bool BelongsTo(int minX, int maxX, int minZ, int maxZ)
            {
                return ((x1 >= minX && x1 <= maxX) || (x2 >= minX && x2 <= maxX) || (x3 >= minX && x3 <= maxX) ||
                        (x4 >= minX && x4 <= maxX)) &&
                       ((z1 >= minZ && z1 <= maxZ) || (z2 >= minZ && z2 <= maxZ) || (z3 >= minZ && z3 <= maxZ) ||
                        (z4 >= minZ && z4 <= maxZ));
            }
        }
    }
}