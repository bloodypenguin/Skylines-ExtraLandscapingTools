using System;
using System.Collections.Generic;
using System.Linq;
using ColossalFramework.UI;
using UnityEngine;

namespace NaturalResourcesBrush
{
    public class ResourceUtils
    {
        public static T Load<T>(string name) where T : UnityEngine.Object
        {
            T[] ts = Load<T>(new string[] { name });
            if (ts.Length >= 1)
            {
                return ts[0];
            }
            else
            {
                return null;
            }
        }

        public static T[] Load<T>(string[] names) where T : UnityEngine.Object
        {
            List<T> ts = new List<T>();
            foreach (T t in Resources.FindObjectsOfTypeAll<T>())
            {
                if (Array.Exists(names, n => n == t.name))
                {
                    ts.Add(t);
                }
            }
            return ts.ToArray();
        }
    }
}