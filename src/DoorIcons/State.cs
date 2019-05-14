using Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace DoorIcons
{
    public static class State
    {
        public static Core Common = new Core("DoorIcons", "1741162790", null, true);

        public static Dictionary<Door, GameObject> DoorIcons = new Dictionary<Door, GameObject>();

        public static Dictionary<ExtendedDoorState, Sprite> DoorSprites = new Dictionary<ExtendedDoorState, Sprite>
        {
            {ExtendedDoorState.Open, CreateSprite("open.png")},
            {ExtendedDoorState.Locked, CreateSprite("locked.png")},
            {ExtendedDoorState.Automation, CreateSprite("automation.png")},
            {ExtendedDoorState.AccessLeft, CreateSprite("access_left.png")},
            {ExtendedDoorState.AccessRight, CreateSprite("access_right.png")},
            {ExtendedDoorState.AccessRestricted, CreateSprite("access_restricted.png")},
            {ExtendedDoorState.AccessCustom, CreateSprite("access_custom.png")},
        };

        private static Sprite CreateSprite(string spriteFilename)
        {
            if (State.Common.RootPath == null)
            {
                return null;
            }

            var width = 256;
            var height = 256;

            var path = Pathfinder.MergePath(State.Common.RootPath, "Sprites", spriteFilename);

            if (File.Exists(path))
            {
                byte[] bytes = File.ReadAllBytes(path);

                Texture2D texture = new Texture2D(width, height, TextureFormat.RGB24, true)
                {
                    filterMode = FilterMode.Trilinear
                };

                texture.LoadImage(bytes);

                return Sprite.Create(texture, new Rect(0, 0, width, height), new Vector2(0.5f, 0.5f), 1.0f);
            }
            else
            {
                State.Common.Logger.Log("File doesn't exist at path: " + path);
                return null;
            }
        }
    }
}
