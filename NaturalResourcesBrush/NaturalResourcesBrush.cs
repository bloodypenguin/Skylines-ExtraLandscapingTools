
using System;
using System.Collections.Generic;
using System.Reflection;
using ICities;
using UnityEngine;
using Object = UnityEngine.Object;

namespace NaturalResourcesBrush
{
    public class NaturalResourcesBrush : LoadingExtensionBase, IUserMod
    {
        private static RedirectCallsState _state;
        private static LoadMode loadMode;

        public string Name
        {
            get { return "In-game Natural Resources Tool + Tree Brush + Water Tool"; }
        }

        public string Description
        {
            get { return "Allows to place natural resources in-game"; }
        }

        public override void OnCreated(ILoading lodaing)
        {
            TreeToolDetour.Deploy();
            _state = RedirectionHelper.RedirectCalls
                (
                    typeof(BeautificationPanel).GetMethod("OnButtonClicked",
                        BindingFlags.Instance | BindingFlags.NonPublic),
                    typeof(BeautificationPanelDetour).GetMethod("OnButtonClicked",
                        BindingFlags.Instance | BindingFlags.NonPublic)
                );
        }

        public override void OnReleased()
        {
            TreeToolDetour.Revert();
            RedirectionHelper.RevertRedirect(
                    typeof(BeautificationPanel).GetMethod("OnButtonClicked",
                        BindingFlags.Instance | BindingFlags.NonPublic),
                    _state
                );
            if (loadMode == LoadMode.LoadGame || loadMode == LoadMode.NewGame)
            {
                RedirectionHelper.RevertRedirect
                (
                    typeof(WaterTool).GetMethod("Awake",
                        BindingFlags.Instance | BindingFlags.NonPublic),
                    WaterToolDetour._state
                );
            }
        }

        public override void OnLevelLoaded(LoadMode mode)
        {
            loadMode = mode;

            if (loadMode == LoadMode.LoadGame || loadMode == LoadMode.NewGame)
            {
                            WaterToolDetour._state = RedirectionHelper.RedirectCalls
                (
                    typeof(WaterTool).GetMethod("Awake",
                        BindingFlags.Instance | BindingFlags.NonPublic),
                    typeof(WaterToolDetour).GetMethod("Awake",
                        BindingFlags.Instance | BindingFlags.NonPublic)
                );
            }
            var toolController = Object.FindObjectOfType<ToolController>();
            if (toolController == null)
            {
                Debug.LogError("ExtraTools#OnLevelLoaded(): ToolContoller not found");
                return;
            }
            try
            {
                List<ToolBase> extraTools;
                if (!Util.SetUpExtraTools(mode, ref toolController, out extraTools))
                {
                    return;
                }
                Util.AddExtraToolsToController(ref toolController, extraTools);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
            finally
            {
                if (toolController.Tools.Length > 0)
                {
                    toolController.Tools[0].enabled = true;
                }

            }
        }
    }

}
