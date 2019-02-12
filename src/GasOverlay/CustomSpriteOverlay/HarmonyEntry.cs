using Harmony;
using Klei;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace CustomSpriteOverlay
{
    public static class HarmonyEntry
    {
        [HarmonyPatch(typeof(Door))]
        [HarmonyPatch("OnSpawn")]
        public static class DrawSpriteOnWorldCanvas
        {
            public static void Postfix(Door __instance)
            {
                var go = new GameObject("sprite_test");
                var renderer = go.AddComponent<SpriteRenderer>();

                renderer.sprite = GetSprite();
                renderer.material.renderQueue = 5000;

                Util.KInstantiate(renderer, GameScreenManager.Instance.worldSpaceCanvas);

                var pos = Grid.PosToXY(__instance.transform.position);

                Debug.Log($"x: {pos.X}, y: {pos.Y}");

                //for (int i = 0; i < Enum.GetValues(typeof(GameScreenManager.UIRenderTarget)).Length; i++)
                //{
                //    try
                //    {
                //        Debug.Log("camera z: "
                //            + GameScreenManager.Instance
                //            .GetCamera((GameScreenManager.UIRenderTarget)i)
                //            .transform
                //            .position
                //            .z);
                //    }
                //    catch { }
                //}

                // TODO: horizontal door needs different offset
                // AND rotated icon
                go.transform.position = new Vector3
                (
                    pos.X + 0.5f,
                    pos.Y + 0.4f,
                    Grid.GetLayerZ(Grid.SceneLayer.SceneMAX)
                );

                go.transform.localScale = new Vector3
                (
                    0.005f,
                    0.005f,
                    1
                );

                //LogRectTransformOfWorldSpaceCanvasComponents();
            }

            private static void LogRectTransformOfWorldSpaceCanvasComponents()
            {
                var rectTransform = GameScreenManager.Instance.worldSpaceCanvas.GetComponent<RectTransform>();
                var comps = rectTransform.GetComponentsInChildren<Component>();

                for (int i = 0; i < comps.Length; i++)
                {
                    Component comp = comps[i];
                    Debug.Log($"component:{i}-{comp}-{comp.name}-{comp.gameObject.name}");
                }
            }

            private static Sprite GetSprite()
            {
                var width = 256;
                var height = 256;

                byte[] bytes = File.ReadAllBytes("restricted.png");

                Texture2D texture = new Texture2D(width, height, TextureFormat.RGB24, false)
                {
                    filterMode = FilterMode.Trilinear
                };

                texture.LoadImage(bytes);

                return Sprite.Create(texture, new Rect(0, 0, width, height), new Vector2(0.5f, 0.0f), 1.0f);
            }
        }
    }
}
