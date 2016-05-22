using System;
using System.Collections.Generic;
using System.Reflection;
using ColossalFramework;
using NaturalResourcesBrush.Redirection;
using UnityEngine;

namespace NaturalResourcesBrush.Detours
{
    [TargetType(typeof(TerrainTool))]
    public class TerrainToolDetour : TerrainTool
    {
        private static SavedInputKey m_UndoKey;
        private static Vector3 m_mousePosition;
        internal static Vector3 m_startPosition;
        private static Vector3 m_endPosition;
        private static Ray m_mouseRay;
        private static float m_mouseRayLength;
        private static bool m_mouseLeftDown;
        private static bool m_mouseRightDown;
        private static bool m_mouseRayValid;
        private static bool m_strokeEnded;
        private static int m_strokeXmin;
        private static int m_strokeXmax;
        private static int m_strokeZmin;
        private static int m_strokeZmax;
        private static int m_undoBufferFreePointer;
        private static List<TerrainToolDetour.UndoStroke> m_undoList;
        private static bool m_strokeInProgress;
        private static bool m_undoRequest;

        public static bool isDitch = false;
        private static ushort[] ditchHeights;

        public static void Dispose()
        {
            m_UndoKey = null;
        }


        [RedirectMethod]
        public bool IsUndoAvailable()
        {
            if (m_undoList != null)
                return m_undoList.Count > 0;
            return false;
        }

        [RedirectMethod]
        public static void Undo(TerrainTool tool)
        {
            m_undoRequest = true;
        }

        [RedirectMethod]
        public void ResetUndoBuffer()
        {
            m_undoList.Clear();
            ushort[] backupHeights = Singleton<TerrainManager>.instance.BackupHeights;
            ushort[] rawHeights = Singleton<TerrainManager>.instance.RawHeights;
            for (int index1 = 0; index1 <= 1080; ++index1)
            {
                for (int index2 = 0; index2 <= 1080; ++index2)
                {
                    int index3 = index1 * 1081 + index2;
                    backupHeights[index3] = rawHeights[index3];
                }
            }
        }

        [RedirectMethod]
        protected override void Awake()
        {
            BaseAwake();
            m_undoList = new List<TerrainToolDetour.UndoStroke>();
            if (!Singleton<LoadingManager>.exists)
                return;
            Singleton<LoadingManager>.instance.m_levelLoaded += new LoadingManager.LevelLoadedHandler(OnLevelLoaded);
        }

        protected void BaseAwake()
        {
            m_toolController = GetComponent<ToolController>();
        }

        [RedirectMethod]
        protected override void OnToolGUI(UnityEngine.Event e)
        {
            if (!m_toolController.IsInsideUI && e.type == UnityEngine.EventType.MouseDown)
            {
                if (e.button == 0)
                {
                    m_mouseLeftDown = true;
                    m_endPosition = m_mousePosition;
                }
                else if (e.button == 1)
                {
                    //begin mod
                    if (m_mode == TerrainTool.Mode.Shift || m_mode == TerrainTool.Mode.Soften || isDitch)
                        //end mod
                        m_mouseRightDown = true;
                    else if (m_mode == TerrainTool.Mode.Level || m_mode == TerrainTool.Mode.Slope)
                    {
                        m_startPosition = m_mousePosition;
                    }
                }
            }
            else if (e.type == UnityEngine.EventType.MouseUp)
            {
                if (e.button == 0)
                {
                    m_mouseLeftDown = false;
                    if (!m_mouseRightDown)
                        m_strokeEnded = true;
                }
                else if (e.button == 1)
                {
                    m_mouseRightDown = false;
                    if (!m_mouseLeftDown)
                        m_strokeEnded = true;
                }
            }
            if (m_UndoKey == null)
            {
                m_UndoKey = (SavedInputKey)typeof(TerrainTool).GetField("m_UndoKey", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(this);
            }

            if (!m_UndoKey.IsPressed(e) || m_undoRequest || (m_mouseLeftDown || m_mouseRightDown) || !IsUndoAvailable())
                return;
            Undo();
        }

        [RedirectMethod]
        protected override void OnEnable()
        {
            BaseOnEnable();
            m_toolController.SetBrush(m_brush, m_mousePosition, m_brushSize);
            m_strokeXmin = 1080;
            m_strokeXmax = 0;
            m_strokeZmin = 1080;
            m_strokeZmax = 0;
            ushort[] backupHeights = Singleton<TerrainManager>.instance.BackupHeights;
            ushort[] rawHeights = Singleton<TerrainManager>.instance.RawHeights;
            for (int index1 = 0; index1 <= 1080; ++index1)
            {
                for (int index2 = 0; index2 <= 1080; ++index2)
                {
                    int index3 = index1 * 1081 + index2;
                    backupHeights[index3] = rawHeights[index3];
                }
            }
            Singleton<TerrainManager>.instance.RenderTopography = true;
            Singleton<TransportManager>.instance.TunnelsVisible = true;
            //begin mod
            TerrainManager.instance.TransparentWater = true;
            //end mod
        }

        protected void BaseOnEnable()
        {
            if (!((UnityEngine.Object)m_toolController.CurrentTool != (UnityEngine.Object)this))
                return;
            m_toolController.CurrentTool = this;
        }

        [RedirectMethod]
        private void OnLevelLoaded(SimulationManager.UpdateMode mode)
        {
            ResetUndoBuffer();
        }

        [RedirectMethod]
        protected override void OnDisable()
        {
            BaseOnDisable();
            ToolCursor = (CursorInfo)null;
            Singleton<TransportManager>.instance.TunnelsVisible = false;
            Singleton<TerrainManager>.instance.RenderTopography = false;
            m_toolController.SetBrush((Texture2D)null, Vector3.zero, 1f);
            m_mouseLeftDown = false;
            m_mouseRightDown = false;
            m_mouseRayValid = false;
            //begin mod
            TerrainManager.instance.TransparentWater = false;
            //end mod
        }

        protected void BaseOnDisable()
        {
            if ((UnityEngine.Object)ToolBase.cursorInfoLabel != (UnityEngine.Object)null && ToolBase.cursorInfoLabel.isVisible)
                ToolBase.cursorInfoLabel.isVisible = false;
            if (!((UnityEngine.Object)m_toolController.NextTool == (UnityEngine.Object)null) && m_toolController.NextTool.GetType() == typeof(BulldozeTool))
                return;
            Singleton<InfoManager>.instance.SetCurrentMode(InfoManager.InfoMode.None, InfoManager.SubInfoMode.Default);
        }

        [RedirectMethod]
        protected override void OnDestroy()
        {
            BaseOnDestroy();
            if (!Singleton<LoadingManager>.exists)
                return;
            Singleton<LoadingManager>.instance.m_levelLoaded -= new LoadingManager.LevelLoadedHandler(OnLevelLoaded);
        }

        protected void BaseOnDestroy()
        {
        }

        [RedirectMethod]
        protected override void OnToolUpdate()
        {
            switch (m_mode)
            {
                case TerrainTool.Mode.Shift:
                    ToolCursor = m_shiftCursor;
                    break;
                case TerrainTool.Mode.Level:
                    ToolCursor = m_levelCursor;
                    break;
                case TerrainTool.Mode.Soften:
                    ToolCursor = m_softenCursor;
                    break;
                case TerrainTool.Mode.Slope:
                    ToolCursor = m_slopeCursor;
                    break;
            }
        }

        [RedirectMethod]
        protected override void OnToolLateUpdate()
        {
            m_mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            m_mouseRayLength = Camera.main.farClipPlane;
            m_mouseRayValid = !m_toolController.IsInsideUI && Cursor.visible;
            m_toolController.SetBrush(m_brush, m_mousePosition, m_brushSize);
        }

        [RedirectMethod]
        public override void SimulationStep()
        {
            ToolBase.RaycastInput input = new ToolBase.RaycastInput(m_mouseRay, m_mouseRayLength);
            if (m_undoRequest && !m_strokeInProgress)
            {
                ApplyUndo();
                m_undoRequest = false;
            }
            else if (m_strokeEnded)
            {
                EndStroke();
                m_strokeEnded = false;
                m_strokeInProgress = false;
                //begin mod
                ditchHeights = null;
                //end mod
            }
            else
            {
                ToolBase.RaycastOutput output;
                if (!m_mouseRayValid || !ToolBase.RayCast(input, out output))
                    return;
                m_mousePosition = output.m_hitPos;
                if (m_mouseLeftDown == m_mouseRightDown)
                    return;
                //begin mod
                if (ditchHeights == null && isDitch)
                {
                    ditchHeights = new ushort[1168561];
                    const ushort trenchDepth = 20;
                    var diff = m_mouseLeftDown ? trenchDepth : -trenchDepth;
                    var finalStrength = m_strength * diff;
                    var i = 0;
                    foreach (var originalHeight in TerrainManager.instance.FinalHeights)
                    {
                        var from = originalHeight * 1.0f / 64.0f;
                        ditchHeights[i++] = (ushort)Math.Max(0, from + finalStrength);
                    }
                }
                //end mod
                m_strokeInProgress = true;
                ApplyBrush();
            }
        }

        [RedirectMethod]
        private int GetFreeUndoSpace()
        {
            int length = Singleton<TerrainManager>.instance.UndoBuffer.Length;
            if (m_undoList.Count > 0)
                return (length + m_undoList[0].pointer - m_undoBufferFreePointer) % length - 1;
            return length - 1;
        }

        [RedirectMethod]
        private void EndStroke()
        {
            int length = Singleton<TerrainManager>.instance.UndoBuffer.Length;
            int num1 = Math.Max(0, 1 + m_strokeXmax - m_strokeXmin) * Math.Max(0, 1 + m_strokeZmax - m_strokeZmin);
            if (num1 < 1)
                return;
            int num2;
            for (num2 = 0; GetFreeUndoSpace() < num1 && num2 < 10000; ++num2)
                m_undoList.RemoveAt(0);
            if (num2 >= 10000)
            {
                Debug.Log((object)"TerrainTool:EndStroke: unexpectedly terminated freeing loop, might be a bug.");
            }
            else
            {
                m_undoList.Add(new TerrainToolDetour.UndoStroke()
                {
                    xmin = m_strokeXmin,
                    xmax = m_strokeXmax,
                    zmin = m_strokeZmin,
                    zmax = m_strokeZmax,
                    pointer = m_undoBufferFreePointer
                });
                ushort[] undoBuffer = Singleton<TerrainManager>.instance.UndoBuffer;
                ushort[] backupHeights = Singleton<TerrainManager>.instance.BackupHeights;
                ushort[] rawHeights = Singleton<TerrainManager>.instance.RawHeights;
                for (int index1 = m_strokeZmin; index1 <= m_strokeZmax; ++index1)
                {
                    for (int index2 = m_strokeXmin; index2 <= m_strokeXmax; ++index2)
                    {
                        int index3 = index1 * 1081 + index2;
                        undoBuffer[m_undoBufferFreePointer++] = backupHeights[index3];
                        backupHeights[index3] = rawHeights[index3];
                        m_undoBufferFreePointer %= length;
                    }
                }
                m_strokeXmin = 1080;
                m_strokeXmax = 0;
                m_strokeZmin = 1080;
                m_strokeZmax = 0;
            }
        }

        [RedirectMethod]
        public void ApplyUndo()
        {
            if (m_undoList.Count < 1)
                return;
            TerrainToolDetour.UndoStroke undoStroke = m_undoList[m_undoList.Count - 1];
            m_undoList.RemoveAt(m_undoList.Count - 1);
            ushort[] undoBuffer = Singleton<TerrainManager>.instance.UndoBuffer;
            ushort[] backupHeights = Singleton<TerrainManager>.instance.BackupHeights;
            ushort[] rawHeights = Singleton<TerrainManager>.instance.RawHeights;
            int length1 = Singleton<TerrainManager>.instance.UndoBuffer.Length;
            int length2 = Singleton<TerrainManager>.instance.RawHeights.Length;
            int index1 = undoStroke.pointer;
            for (int index2 = undoStroke.zmin; index2 <= undoStroke.zmax; ++index2)
            {
                for (int index3 = undoStroke.xmin; index3 <= undoStroke.xmax; ++index3)
                {
                    int index4 = index2 * 1081 + index3;
                    rawHeights[index4] = undoBuffer[index1];
                    backupHeights[index4] = undoBuffer[index1];
                    index1 = (index1 + 1) % length1;
                }
            }
            m_undoBufferFreePointer = undoStroke.pointer;
            for (int index2 = 0; index2 < length2; ++index2)
                backupHeights[index2] = rawHeights[index2];
            int num = 128;
            undoStroke.xmin = Math.Max(0, undoStroke.xmin - 2);
            undoStroke.xmax = Math.Min(1080, undoStroke.xmax + 2);
            undoStroke.zmin = Math.Max(0, undoStroke.zmin - 2);
            undoStroke.zmax = Math.Min(1080, undoStroke.zmax + 2);
            int minZ = undoStroke.zmin;
            while (minZ <= undoStroke.zmax)
            {
                int minX = undoStroke.xmin;
                while (minX <= undoStroke.xmax)
                {
                    TerrainModify.UpdateArea(minX, minZ, minX + num, minZ + num, true, false, false);
                    minX += num + 1;
                }
                minZ += num + 1;
            }
            m_strokeXmin = 1080;
            m_strokeXmax = 0;
            m_strokeZmin = 1080;
            m_strokeZmax = 0;
        }

        [RedirectMethod]
        private void ApplyBrush()
        {
            float[] brushData = m_toolController.BrushData;
            float num1 = m_brushSize * 0.5f;
            float num2 = 16f;
            int b = 1080;
            ushort[] rawHeights = Singleton<TerrainManager>.instance.RawHeights;
            ushort[] finalHeights = Singleton<TerrainManager>.instance.FinalHeights;
            float num3 = m_strength;
            int num4 = 3;
            float num5 = 1.0f / 64.0f;
            float num6 = 64f;
            Vector3 vector3_1 = m_mousePosition;
            Vector3 vector3_2 = m_endPosition - m_startPosition;
            vector3_2.y = 0.0f;
            float num7 = vector3_2.sqrMagnitude;
            if ((double)num7 != 0.0)
                num7 = 1f / num7;
            float num8 = 20f;
            int minX = Mathf.Max((int)(((double)vector3_1.x - (double)num1) / (double)num2 + (double)b * 0.5), 0);
            int minZ = Mathf.Max((int)(((double)vector3_1.z - (double)num1) / (double)num2 + (double)b * 0.5), 0);
            int maxX = Mathf.Min((int)(((double)vector3_1.x + (double)num1) / (double)num2 + (double)b * 0.5) + 1, b);
            int maxZ = Mathf.Min((int)(((double)vector3_1.z + (double)num1) / (double)num2 + (double)b * 0.5) + 1, b);

            if (m_mode == TerrainTool.Mode.Shift)
            {
                if (m_mouseRightDown)
                    num8 = -num8;
            }
            else if (m_mode == TerrainTool.Mode.Soften && m_mouseRightDown)
                num4 = 10;
            for (int val2_1 = minZ; val2_1 <= maxZ; ++val2_1)
            {
                float f1 = (float)((((double)val2_1 - (double)b * 0.5) * (double)num2 - (double)vector3_1.z + (double)num1) / (double)m_brushSize * 64.0 - 0.5);
                int num9 = Mathf.Clamp(Mathf.FloorToInt(f1), 0, 63);
                int num10 = Mathf.Clamp(Mathf.CeilToInt(f1), 0, 63);
                for (int val2_2 = minX; val2_2 <= maxX; ++val2_2)
                {
                    float f2 = (float)((((double)val2_2 - (double)b * 0.5) * (double)num2 - (double)vector3_1.x + (double)num1) / (double)m_brushSize * 64.0 - 0.5);
                    int num11 = Mathf.Clamp(Mathf.FloorToInt(f2), 0, 63);
                    int num12 = Mathf.Clamp(Mathf.CeilToInt(f2), 0, 63);
                    float num13 = brushData[num9 * 64 + num11];
                    float num14 = brushData[num9 * 64 + num12];
                    float num15 = brushData[num10 * 64 + num11];
                    float num16 = brushData[num10 * 64 + num12];
                    float num17 = num13 + (float)(((double)num14 - (double)num13) * ((double)f2 - (double)num11));
                    float num18 = num15 + (float)(((double)num16 - (double)num15) * ((double)f2 - (double)num11));
                    float num19 = num17 + (float)(((double)num18 - (double)num17) * ((double)f1 - (double)num9));
                    float from = (float)rawHeights[val2_1 * (b + 1) + val2_2] * num5;
                    float to = 0.0f;
                    //begin mod
                    if (isDitch)
                    {
                        var index = val2_1 * (b + 1) + val2_2;
                        to = ditchHeights[index];
                    }
                    else
                    //end mod
                        if (m_mode == TerrainTool.Mode.Shift)
                            to = from + num8;
                        else if (m_mode == TerrainTool.Mode.Level)
                            to = m_startPosition.y;
                        else if (m_mode == TerrainTool.Mode.Soften)
                        {
                            int num20 = Mathf.Max(val2_2 - num4, 0);
                            int num21 = Mathf.Max(val2_1 - num4, 0);
                            int num22 = Mathf.Min(val2_2 + num4, b);
                            int num23 = Mathf.Min(val2_1 + num4, b);
                            float num24 = 0.0f;
                            for (int index1 = num21; index1 <= num23; ++index1)
                            {
                                for (int index2 = num20; index2 <= num22; ++index2)
                                {
                                    float num25 = (float)(1.0 - (double)((index2 - val2_2) * (index2 - val2_2) + (index1 - val2_1) * (index1 - val2_1)) / (double)(num4 * num4));
                                    if ((double)num25 > 0.0)
                                    {
                                        to += (float)finalHeights[index1 * (b + 1) + index2] * (num5 * num25);
                                        num24 += num25;
                                    }
                                }
                            }
                            to /= num24;
                        }
                        else if (m_mode == TerrainTool.Mode.Slope)
                        {
                            float num20 = ((float)val2_2 - (float)b * 0.5f) * num2;
                            float num21 = ((float)val2_1 - (float)b * 0.5f) * num2;
                            to = Mathf.Lerp(m_startPosition.y, m_endPosition.y, (float)(((double)num20 - (double)m_startPosition.x) * (double)vector3_2.x + ((double)num21 - (double)m_startPosition.z) * (double)vector3_2.z) * num7);
                        }
                    float num26 = Mathf.Lerp(from, to, num3 * num19);
                    rawHeights[val2_1 * (b + 1) + val2_2] = (ushort)Mathf.Clamp(Mathf.RoundToInt(num26 * num6), 0, (int)ushort.MaxValue);
                    m_strokeXmin = Math.Min(m_strokeXmin, val2_2);
                    m_strokeXmax = Math.Max(m_strokeXmax, val2_2);
                    m_strokeZmin = Math.Min(m_strokeZmin, val2_1);
                    m_strokeZmax = Math.Max(m_strokeZmax, val2_1);
                }
            }
            TerrainModify.UpdateArea(minX, minZ, maxX, maxZ, true, false, false);
        }

        private struct UndoStroke
        {
            public int xmin;
            public int xmax;
            public int zmin;
            public int zmax;
            public int pointer;
        }
    }
}
