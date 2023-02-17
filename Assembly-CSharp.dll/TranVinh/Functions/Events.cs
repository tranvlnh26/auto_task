using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TranVinh.Helper;
using TranVinh.Models;
using TranVinh.Xmap;

namespace TranVinh.Functions
{
    internal static class Events
    {
        internal static void UpdateInMain()
        {
            MainThreadDispatcher.update();
            Account.doLogin();
        }
        internal static void UpdateInGameScr()
        {
            Pk9rXmap.Update();

            var time = mSystem.currentTimeMillis();
            if (time - Variables.lastTimeUpdateZone >= 500)
            {
                Service.gI().openUIZone();
                Variables.lastTimeUpdateZone = time;
            }
            CheckLag.update();
            AutoTask.update();
        }
        internal static void OnStarted()
        {
            DragonClient.Connect();
        }
        internal static bool OnClosing()
        {
            return DragonClient.Close();
        }
    }
}
