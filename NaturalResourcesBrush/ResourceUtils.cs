using System;
using System.Collections.Generic;
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

        public static UITextureAtlas CreateAtlas(string type, string name)
        {
            string baseIconName = type + name;
            return ResourceUtils.CreateAtlas(new string[] {
                baseIconName,
                baseIconName + "Focused",
                baseIconName + "Hovered",
                baseIconName + "Pressed",
                baseIconName + "Disabled"
            });
        }

        public static UITextureAtlas CreateAtlas(string[] names)
        {
            Texture2D[] sprites = ResourceUtils.Load<Texture2D>(names);

            UITextureAtlas atlas = new UITextureAtlas();
            atlas.material = new Material(ResourceUtils.GetUIAtlasShader());

            Texture2D texture = new Texture2D(0, 0);
            Rect[] rects = texture.PackTextures(sprites, 0);

            for (int i = 0; i < rects.Length; ++i)
            {
                Texture2D sprite = sprites[i];
                Rect rect = rects[i];

                UITextureAtlas.SpriteInfo spriteInfo = new UITextureAtlas.SpriteInfo();
                spriteInfo.name = sprite.name;
                spriteInfo.texture = sprite;
                spriteInfo.region = rect;
                spriteInfo.border = new RectOffset();

                atlas.AddSprite(spriteInfo);
            }

            atlas.material.mainTexture = texture;
            return atlas;
        }

        private static Shader GetUIAtlasShader()
        {
            return UIView.GetAView().defaultAtlas.material.shader;
        }
    }
}