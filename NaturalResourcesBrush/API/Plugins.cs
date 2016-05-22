using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ColossalFramework.Plugins;
using ColossalFramework.UI;
using ICities;
using UnityEngine;
using UnityEngine.VR;

namespace NaturalResourcesBrush.API
{
    public class Plugins
    {

        private static IEltPlugin[] plugins = { };

        public static void Initialize()
        {
            var pluginsList = new List<IEltPlugin>();
            var enabledMods = PluginManager.instance.GetPluginsInfo().Where(p => p.isEnabled);
            foreach (var mod in enabledMods)
            {
                var assemblies = Util.GetPrivate<List<Assembly>>(mod, "m_Assemblies");
                if (assemblies == null)
                {
                    continue;
                }
                foreach (var assembly in assemblies)
                {
                    try
                    {
                        var types = assembly.GetTypes();
                        pluginsList.AddRange(types
                            .Where(type => typeof(IEltPlugin).IsAssignableFrom(type))
                            .Select(type => (IEltPlugin)Activator.CreateInstance(type))
                            );
                    }
                    catch
                    {
                        // ignored
                    }
                }
            }
            plugins = pluginsList.ToArray();
            foreach (var plugin in plugins)
            {
                plugin.Initialize();
            }
        }

        public static void SetStrength(float strength)
        {
            foreach (var plugin in plugins)
            {
                plugin.SetStrength(strength);
            }
        }

        public static void SetSize(float size, bool minSliderValue)
        {
            foreach (var plugin in plugins)
            {
                plugin.SetSize(size, minSliderValue);
            }
        }

        public static void SetBrush(Texture2D brush)
        {
            foreach (var plugin in plugins)
            {
                plugin.SetBrush(brush);
            }
        }

        public static ToolBase[] SetupTools(LoadMode mode)
        {
            var tools = new List<ToolBase>();
            foreach (var plugin in plugins)
            {
                var pluginTools = plugin.SetupTools(mode);
                var toolBases = pluginTools as ToolBase[] ?? pluginTools.ToArray();
                if (pluginTools == null || !toolBases.Any())
                {
                    continue;
                }
                tools.AddRange(toolBases);
            }

            return tools.ToArray();
        }

        public static void Dispose()
        {
            foreach (var plugin in plugins)
            {
                plugin.Dispose();
            }
        }

        public static void CreateToolbars(LoadMode loadMode)
        {
            foreach (var plugin in plugins)
            {
                plugin.CreateToolbars(loadMode);
            }
        }

        public static bool SupportsSingle(ToolBase tool)
        {
            foreach (var plugin in plugins)
            {
                if (plugin.SupportsSingle(tool))
                {
                    return true;
                }
            }
            return false;
        }
    }
}