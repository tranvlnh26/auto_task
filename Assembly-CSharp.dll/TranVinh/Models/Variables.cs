using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TranVinh.Models
{
    internal static class Variables
    {
        internal static long lastTimeUpdateZone;

        //check lag
        internal static long lastTimeCheckLag = 0;
        internal static byte countConnect = 30;
        internal static byte countBlackScreen = 30;
        internal static byte countXmap = byte.MaxValue;
    }
}
