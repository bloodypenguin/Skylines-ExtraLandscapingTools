using ColossalFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using NaturalResourcesBrush.Redirection;
using UnityEngine;
using ColossalFramework.Globalization;
using NaturalResourcesBrush.Options;

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
    private ushort[] m_tempBuffer;
    private ToolBase.ToolErrors m_toolErrors;
    private int m_currentCost;
    private static int m_strokeXmin;
    private static int m_strokeXmax;
    private static int m_strokeZmin;
    private static int m_strokeZmax;
    private static int m_undoBufferFreePointer;
    private static List<TerrainToolDetour.UndoStroke> m_undoList;
    private static bool m_strokeInProgress;

    private static Dictionary<MethodInfo, RedirectCallsState> _redirects;
    public static bool isDitch = false;
    private static ushort targetHeightStroke;
    private static ushort[] ditchHeights;

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

        m_UndoKey = null;
    }


    [RedirectMethod]
    private IEnumerator StrokeEnded()
    {
        if (m_strokeInProgress)
        {
            EndStroke();
            m_strokeInProgress = false;
            //begin mod
            ditchHeights = null;
            //end mod
        }
        yield return null;
    }


    [RedirectMethod]
    private IEnumerator DisableTool()
    {
        m_mouseLeftDown = false;
        m_mouseRightDown = false;
        if (m_strokeInProgress)
        {
            EndStroke();
            m_strokeInProgress = false;
        }
        yield return null;
    }

    [RedirectMethod]
    private IEnumerator UndoRequest()
    {
        if (!m_strokeInProgress)
        {
            ApplyUndo();
        }
        yield return null;
    }

    [RedirectMethod]
    public bool IsUndoAvailable()
    {
        if (m_undoList != null)
            return m_undoList.Count > 0;
        return false;
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
                    Singleton<SimulationManager>.instance.AddAction(StrokeEnded());
            }
            else if (e.button == 1)
            {
                m_mouseRightDown = false;
                if (!m_mouseLeftDown)
                    Singleton<SimulationManager>.instance.AddAction(StrokeEnded());
            }
        }
        if (m_UndoKey == null)
        {
            m_UndoKey = (SavedInputKey)typeof(TerrainTool).GetField("m_UndoKey", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(this);
        }
        if (!m_UndoKey.IsPressed(e) || m_mouseLeftDown || (m_mouseRightDown || !this.IsUndoAvailable()))
            return;
        this.Undo();
    }

    [RedirectMethod]
    protected override void OnEnable()
    {
        BaseOnEnable();
        this.m_toolErrors = ToolBase.ToolErrors.Pending;
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
        Singleton<TerrainManager>.instance.TransparentWater = true;
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
        //begin mod
        TerrainManager.instance.TransparentWater = false;
        //end mod
        m_toolController.SetBrush((Texture2D)null, Vector3.zero, 1f);
        m_mouseRayValid = false;
        this.m_toolErrors = ToolBase.ToolErrors.Pending;
        Singleton<SimulationManager>.instance.AddAction(DisableTool());
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
        if (!this.m_toolController.IsInsideUI && Cursor.visible && this.m_toolErrors != ToolBase.ToolErrors.Pending)
        {
            int num = this.m_currentCost;
            if (m_strokeInProgress)
            {
                string text = (string)null;
                if (num > 0)
                    text = string.Format(Locale.Get("TOOL_LANDSCAPING_COST"), (object)(num / 100));
                else if (num < 0)
                    text = string.Format(Locale.Get("TOOL_REFUND_AMOUNT"), (object)(-num / 100));
                this.ShowToolInfo(true, text, m_mousePosition);
            }
            else
                this.ShowToolInfo(false, (string)null, m_mousePosition);
        }
        else
            this.ShowToolInfo(false, (string)null, m_mousePosition);


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
        ToolBase.RaycastOutput output;
        if (m_mouseRayValid && ToolBase.RayCast(new ToolBase.RaycastInput(m_mouseRay, m_mouseRayLength), out output))
        {
            m_mousePosition = output.m_hitPos;
            if (m_mouseLeftDown != m_mouseRightDown)
            {
                if (!m_strokeInProgress)
                {
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
                    this.m_currentCost = 0;
                }
                this.ApplyBrush();
            }
            else
            {
                this.m_toolErrors = ToolBase.ToolErrors.Pending;
            }
        }
        else
            this.m_toolErrors = ToolBase.ToolErrors.RaycastFailed;
        GuideController guideController = Singleton<GuideManager>.instance.m_properties;
        if (guideController == null)
            return;
        Singleton<TerrainManager>.instance.m_terrainToolNotUsed.Activate(guideController.m_terrainToolNotUsed);
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
        if ((this.m_toolController.m_mode & ItemClass.Availability.Game) != ItemClass.Availability.None)
        {
            if (this.m_currentCost > 0)
                Singleton<EconomyManager>.instance.FetchResource(EconomyManager.Resource.Landscaping, this.m_currentCost, ItemClass.Service.Beautification, ItemClass.SubService.None, ItemClass.Level.None);
            else if (this.m_currentCost < 0)
                Singleton<EconomyManager>.instance.AddResource(EconomyManager.Resource.RefundAmount, -this.m_currentCost, ItemClass.Service.Beautification, ItemClass.SubService.None, ItemClass.Level.None);
            this.m_currentCost = 0;
        }
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
        TerrainManager instance1 = Singleton<TerrainManager>.instance;
        GameAreaManager instance2 = Singleton<GameAreaManager>.instance;
        SimulationManager instance3 = Singleton<SimulationManager>.instance;
        float[] brushData = this.m_toolController.BrushData;
        float num1 = this.m_brushSize * 0.5f;
        float num2 = 16f;
        int b = 1080;
        ushort[] rawHeights = instance1.RawHeights;
        ushort[] finalHeights = instance1.FinalHeights;
        ushort[] backupHeights = instance1.BackupHeights;
        float num3 = this.m_strength;
        int num4 = 3;
        float num5 = (float)(1.0 / 64.0);
        float num6 = 64f;
        Vector3 p = m_mousePosition;
        Vector3 vector3 = m_endPosition - m_startPosition;
        vector3.y = 0.0f;
        float num7 = vector3.sqrMagnitude;
        if ((double)num7 != 0.0)
            num7 = 1f / num7;
        float num8 = 20f;
        bool flag = (this.m_toolController.m_mode & ItemClass.Availability.Game) != ItemClass.Availability.None;
        int a1 = 0;
        int a2 = 0;
        int amount = this.m_currentCost;
        int dirtBuffer = instance1.DirtBuffer;
        int num9 = 524288;
        int num10 = 0;
        if (flag)
        {
            if (instance2.PointOutOfArea(p))
            {
                this.m_toolErrors = ToolBase.ToolErrors.OutOfArea;
                return;
            }
            TerrainProperties terrainProperties = instance1.m_properties;
            if (terrainProperties != null)
                num10 = terrainProperties.m_dirtPrice;
        }
        int num11 = Mathf.Max((int)(((double)p.x - (double)num1) / (double)num2 + (double)b * 0.5), 0);
        int num12 = Mathf.Max((int)(((double)p.z - (double)num1) / (double)num2 + (double)b * 0.5), 0);
        int num13 = Mathf.Min((int)(((double)p.x + (double)num1) / (double)num2 + (double)b * 0.5) + 1, b);
        int num14 = Mathf.Min((int)(((double)p.z + (double)num1) / (double)num2 + (double)b * 0.5) + 1, b);
        if (this.m_mode == TerrainTool.Mode.Shift)
        {
            if (m_mouseRightDown)
                num8 = -num8;
        }
        else if (this.m_mode == TerrainTool.Mode.Soften && m_mouseRightDown)
            num4 = 10;
        if (this.m_tempBuffer == null || this.m_tempBuffer.Length < (num14 - num12 + 1) * (num13 - num11 + 1))
            this.m_tempBuffer = new ushort[(num14 - num12 + 1) * (num13 - num11 + 1)];
        for (int index1 = num12; index1 <= num14; ++index1)
        {
            float z = ((float)index1 - (float)b * 0.5f) * num2;
            float f1 = (float)(((double)z - (double)p.z + (double)num1) / (double)this.m_brushSize * 64.0 - 0.5);
            int num15 = Mathf.Clamp(Mathf.FloorToInt(f1), 0, 63);
            int num16 = Mathf.Clamp(Mathf.CeilToInt(f1), 0, 63);
            for (int index2 = num11; index2 <= num13; ++index2)
            {
                float x = ((float)index2 - (float)b * 0.5f) * num2;
                float f2 = (float)(((double)x - (double)p.x + (double)num1) / (double)this.m_brushSize * 64.0 - 0.5);
                int num17 = Mathf.Clamp(Mathf.FloorToInt(f2), 0, 63);
                int num18 = Mathf.Clamp(Mathf.CeilToInt(f2), 0, 63);
                int num19 = (int)rawHeights[index1 * (b + 1) + index2];
                float from = (float)num19 * num5;
                float to = 0.0f;
                if (flag && instance2.PointOutOfArea(new Vector3(x, p.y, z), num2 * 0.5f))
                {
                    this.m_tempBuffer[(index1 - num12) * (num13 - num11 + 1) + index2 - num11] = (ushort)num19;
                }
                else
                {
                    float num20 = brushData[num15 * 64 + num17];
                    float num21 = brushData[num15 * 64 + num18];
                    float num22 = brushData[num16 * 64 + num17];
                    float num23 = brushData[num16 * 64 + num18];
                    float num24 = num20 + (float)(((double)num21 - (double)num20) * ((double)f2 - (double)num17));
                    float num25 = num22 + (float)(((double)num23 - (double)num22) * ((double)f2 - (double)num17));
                    float t = (num24 + (float)(((double)num25 - (double)num24) * ((double)f1 - (double)num15))) * num3;
                    if ((double)t <= 0.0)
                    {
                        this.m_tempBuffer[(index1 - num12) * (num13 - num11 + 1) + index2 - num11] = (ushort)num19;
                    }
                    else
                    {
                        //begin mod
                        if (isDitch)
                        {
                            to = ditchHeights[index1 * (b + 1) + index2];
                        }
                        else
                        //end mod
                        if (this.m_mode == TerrainTool.Mode.Shift)
                            to = (float)finalHeights[index1 * (b + 1) + index2] * num5 + num8;
                        else if (this.m_mode == TerrainTool.Mode.Level)
                            to = m_startPosition.y;
                        else if (this.m_mode == TerrainTool.Mode.Soften)
                        {
                            int num26 = Mathf.Max(index2 - num4, 0);
                            int num27 = Mathf.Max(index1 - num4, 0);
                            int num28 = Mathf.Min(index2 + num4, b);
                            int num29 = Mathf.Min(index1 + num4, b);
                            float num30 = 0.0f;
                            for (int index3 = num27; index3 <= num29; ++index3)
                            {
                                for (int index4 = num26; index4 <= num28; ++index4)
                                {
                                    float num31 = (float)(1.0 - (double)((index4 - index2) * (index4 - index2) + (index3 - index1) * (index3 - index1)) / (double)(num4 * num4));
                                    if ((double)num31 > 0.0)
                                    {
                                        to += (float)finalHeights[index3 * (b + 1) + index4] * (num5 * num31);
                                        num30 += num31;
                                    }
                                }
                            }
                            if ((double)num30 > 1.0 / 1000.0)
                                to /= num30;
                            else
                                to = (float)finalHeights[index1 * (b + 1) + index2];
                        }
                        else if (this.m_mode == TerrainTool.Mode.Slope)
                        {
                            float num26 = ((float)index2 - (float)b * 0.5f) * num2;
                            float num27 = ((float)index1 - (float)b * 0.5f) * num2;
                            to = Mathf.Lerp(m_startPosition.y, m_endPosition.y, (float)(((double)num26 - (double)m_startPosition.x) * (double)vector3.x + ((double)num27 - (double)m_startPosition.z) * (double)vector3.z) * num7);
                        }
                        float num32 = to;
                        float num33 = Mathf.Lerp(from, to, t);
                        int num34 = Mathf.Clamp(Mathf.RoundToInt(num33 * num6), 0, (int)ushort.MaxValue);
                        if (num34 == num19)
                        {
                            int num26 = Mathf.Clamp(Mathf.RoundToInt(num32 * num6), 0, (int)ushort.MaxValue);
                            if (num26 > num19)
                            {
                                if (((double)num33 - (double)from) * (double)num6 > (double)instance3.m_randomizer.Int32(0, 10000) * 9.99999974737875E-05)
                                    ++num34;
                            }
                            else if (num26 < num19 && ((double)from - (double)num33) * (double)num6 > (double)instance3.m_randomizer.Int32(0, 10000) * 9.99999974737875E-05)
                                --num34;
                        }
                        this.m_tempBuffer[(index1 - num12) * (num13 - num11 + 1) + index2 - num11] = (ushort)num34;
                        if (flag)
                        {
                            if (num34 > num19)
                                a1 += num34 - num19;
                            else if (num34 < num19)
                                a2 += num19 - num34;
                            int num26 = (int)backupHeights[index1 * (b + 1) + index2];
                            int num27 = Mathf.Abs(num34 - num26) - Mathf.Abs(num19 - num26);
                            amount += num27 * num10;
                        }
                    }
                }
            }
        }
        int num35 = a1;
        int num36 = a2;
        ToolBase.ToolErrors toolErrors = ToolBase.ToolErrors.None;
        if (flag)
        {
            if (OptionsHolder.Options.dirtLimits)
            {
                if (a1 > a2)
                {
                    num35 = Mathf.Min(a1, dirtBuffer + a2);
                    if (num35 < a1)
                    {
                        toolErrors |= ToolBase.ToolErrors.NotEnoughDirt;
                        GuideController guideController = Singleton<GuideManager>.instance.m_properties;
                        if (guideController != null)
                            Singleton<TerrainManager>.instance.m_notEnoughDirt.Activate(guideController.m_notEnoughDirt);
                    }
                    GenericGuide genericGuide = Singleton<TerrainManager>.instance.m_tooMuchDirt;
                    if (genericGuide != null)
                        genericGuide.Deactivate();
                }
                else if (a2 > a1)
                {
                    num36 = Mathf.Min(a2, num9 - dirtBuffer + a1);
                    if (num36 < a2)
                    {
                        toolErrors |= ToolBase.ToolErrors.TooMuchDirt;
                        GuideController guideController = Singleton<GuideManager>.instance.m_properties;
                        if (guideController != null)
                            Singleton<TerrainManager>.instance.m_tooMuchDirt.Activate(guideController.m_tooMuchDirt);
                    }
                    GenericGuide genericGuide = Singleton<TerrainManager>.instance.m_notEnoughDirt;
                    if (genericGuide != null)
                        genericGuide.Deactivate();
                }
            }
            if (amount != Singleton<EconomyManager>.instance.PeekResource(EconomyManager.Resource.Landscaping, amount))
            {
                this.m_toolErrors = toolErrors | ToolBase.ToolErrors.NotEnoughMoney;
                return;
            }
            amount = this.m_currentCost;
        }
        this.m_toolErrors = toolErrors;
        if (num35 != 0 || num36 != 0)
        {
            GenericGuide genericGuide = Singleton<TerrainManager>.instance.m_terrainToolNotUsed;
            if (genericGuide != null && !genericGuide.m_disabled)
                genericGuide.Disable();
        }
        for (int val2_1 = num12; val2_1 <= num14; ++val2_1)
        {
            for (int val2_2 = num11; val2_2 <= num13; ++val2_2)
            {
                int num15 = (int)rawHeights[val2_1 * (b + 1) + val2_2];
                int num16 = (int)this.m_tempBuffer[(val2_1 - num12) * (num13 - num11 + 1) + val2_2 - num11];
                if (flag)
                {
                    int num17 = num16 - num15;
                    if (num17 > 0)
                    {
                        if (a1 > num35)
                            num17 = (a1 - 1 + num17 * num35) / a1;
                        a1 -= num16 - num15;
                        num35 -= num17;
                        num16 = num15 + num17;
                        dirtBuffer -= num17;
                    }
                    else if (num17 < 0)
                    {
                        if (a2 > num36)
                            num17 = -((a2 - 1 - num17 * num36) / a2);
                        a2 -= num15 - num16;
                        num36 += num17;
                        num16 = num15 + num17;
                        dirtBuffer -= num17;
                    }
                    int num18 = (int)backupHeights[val2_1 * (b + 1) + val2_2];
                    int num19 = Mathf.Abs(num16 - num18) - Mathf.Abs(num15 - num18);
                    amount += num19 * num10;
                }
                if (num16 != num15)
                {
                    rawHeights[val2_1 * (b + 1) + val2_2] = (ushort)num16;
                    m_strokeXmin = Math.Min(m_strokeXmin, val2_2);
                    m_strokeXmax = Math.Max(m_strokeXmax, val2_2);
                    m_strokeZmin = Math.Min(m_strokeZmin, val2_1);
                    m_strokeZmax = Math.Max(m_strokeZmax, val2_1);
                }
            }
        }
        if (flag)
        {
            if (OptionsHolder.Options.dirtLimits)
            {
                instance1.DirtBuffer = dirtBuffer;
            }
            this.m_currentCost = amount;
        }
        TerrainModify.UpdateArea(num11 - 2, num12 - 2, num13 + 2, num14 + 2, true, false, false);
    }

    [RedirectMethod]
    public override ToolBase.ToolErrors GetErrors()
    {
        return this.m_toolErrors;
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
