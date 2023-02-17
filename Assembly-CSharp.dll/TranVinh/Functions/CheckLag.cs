using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TranVinh.Models;
using TranVinh.Xmap;

namespace TranVinh.Functions
{
    internal static class CheckLag
    {
        static bool isBlackScreen()
        {
            return Char.isLoadingMap || LoginScr.isContinueToLogin || ServerListScreen.waitToLogin || ServerListScreen.isWait;
        }

        internal static void update()
        {
            var time = mSystem.currentTimeMillis();
            if(time - Variables.lastTimeCheckLag >= 1000)
            {
                if(isBlackScreen())
                    Variables.countBlackScreen--;
                else
                    Variables.countBlackScreen = 30;

                if (Variables.countBlackScreen < 25)
                    Pk9rXmap.FixBlackScreen();

                if (Pk9rXmap.IsXmapRunning)
                    Variables.countXmap--;
                else
                    Variables.countXmap = byte.MaxValue;

                Variables.countConnect--;
                if(Variables.countConnect <= 0 || Variables.countBlackScreen <= 0 || Variables.countXmap <= 0)
                {
                    DragonClient.sendMessage(new
                    {
                        action = "close-socket"
                    });
                }
                


                Variables.lastTimeCheckLag = time;
            }
        }
    }
}
