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
    }
}
