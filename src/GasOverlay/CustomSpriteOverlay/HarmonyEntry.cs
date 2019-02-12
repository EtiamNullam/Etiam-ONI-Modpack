using Harmony;
using Klei;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using static Grid;

namespace CustomSpriteOverlay
{
    public static class HarmonyEntry
    {
        //[HarmonyPatch(typeof(CameraController), "Update")]
        public static class CameraController_Update
        {
            public static void Postfix(CameraController __instance)
            {
                //__instance.
            }
        }

        //[HarmonyPatch(typeof(Door))]
        //[HarmonyPatch("OnSpawn")]
        [HarmonyPatch(typeof(AttackTool))]
        [HarmonyPatch("OnActivateTool")]
        public static class DrawSpriteOnWorldCanvas
        {
            public static void Postfix()
            {
                // TODO: get components
                // TODO: draw sprite somewhere around there

                var go = new GameObject("sprite_test");
                var renderer = go.AddComponent<SpriteRenderer>();
                renderer.sprite = GetSprite();
                //go.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);

                Util.KInstantiate(renderer, GameScreenManager.Instance.worldSpaceCanvas);

                go.transform.position = new Vector3
                (
                    go.transform.position.x,
                    go.transform.position.y,
                    Grid.GetLayerZ(SceneLayer.SceneMAX)
                );

                //var canvas = GameScreenManager.Instance.worldSpaceCanvas.GetComponent<canvas>();
                //var canvasScaler = GameScreenManager.Instance.worldSpaceCanvas.GetComponent<UnityEngine.UI.CanvasScaler>();

                //rectTransform.Translate(1, 0, 0);

                LogRectTransformOfWorldSpaceCanvasComponents();

                //var comps = GameScreenManager.Instance.worldSpaceCanvas.GetComponents<Component>();

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

                //Debug.Log("jpg bytes: " + JsonConvert.SerializeObject(bytes));

                Texture2D texture = new Texture2D(width, height, TextureFormat.RGB24, false)
                {
                    filterMode = FilterMode.Trilinear
                };


                //Debug.Log("texture: " + JsonConvert.SerializeObject(texture));

                //Debug.Log("loadimage result: " +
                ImageConversion.LoadImage(texture, bytes);
                    //);

                return Sprite.Create(texture, new Rect(0, 0, width, height), new Vector2(0.5f, 0.0f), 1.0f);
            }
        }

        //private static 

        //[HarmonyPatch(typeof(PopFXManager), "SpawnFX")]
        //public static class PopFXManager_SpawnFX
        //{
        //    public static void Postfix(PopFXManager __instance)
        //    {
        //        GameObject gameObject = Util.KInstantiate(__instance.Prefab_PopFX, __instance.gameObject, "Pooled_PopFX");
        //        //gameObject.transform.localScale = Vector3.one;

        //        foreach (var comp in gameObject.GetComponents<Component>())
        //        {
        //            Debug.Log(comp.name);
        //        }

        //        Debug.Log("===============\n");
        //    }
        //}
    }
}
