using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TranVinh.Helper
{
    internal static class Utils
    {
        internal static bool IsGetInfoChat<T>(string text, string s)
        {
            if (!text.StartsWith(s))
            {
                return false;
            }
            try
            {
                Convert.ChangeType(text.Substring(s.Length), typeof(T));
            }
            catch
            {
                return false;
            }
            return true;
        }

        internal static T GetInfoChat<T>(string text, string s)
        {
            return (T)Convert.ChangeType(text.Substring(s.Length), typeof(T));
        }

        internal static int getYGround(int x)
        {
            int num = 50;
            int i = 0;
            while (i < 30)
            {
                i++;
                num += 24;
                bool flag = TileMap.tileTypeAt(x, num, 2);
                bool flag2 = flag;
                if (flag2)
                {
                    bool flag3 = num % 24 != 0;
                    bool flag4 = flag3;
                    if (flag4)
                    {
                        num -= num % 24;
                    }
                    return num;
                }
            }
            return num;
        }
    }
}
