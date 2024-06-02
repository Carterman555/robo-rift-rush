using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpeedPlatformer
{
    public static class GameLayers
    {
        public static int GroundLayer { get; private set; } = 3;
        public static int PlayerLayer { get; private set; } = 6;
        public static int TrapLayer { get; private set; } = 7;
        public static int CameraFrameLayer { get; private set; } = 8;
        public static int GrappleLayer { get; private set; } = 9;
        public static int TileLayer { get; private set; } = 11;
        public static int GrappleSurfaceLayer { get; private set; } = 13;
    }
}
