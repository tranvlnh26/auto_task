using System.Collections.Generic;

namespace TranVinh.Xmap
{
    internal struct MapNext
    {
        internal int MapID;

        internal TypeMapNext Type;

        internal int[] Info;

        internal MapNext(int mapID, TypeMapNext type, int[] info)
        {
            MapID = mapID;
            Type = type;
            Info = info;
        }
    }

    internal enum TypeMapNext
    {
        AutoWaypoint,
        NpcMenu,
        NpcPanel,
        Position,
        Capsule
    }
}