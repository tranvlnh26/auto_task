using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QLTK.Functions
{
    internal class TimeHelper
    {

        #region
        static TimeHelper instance;
        public static TimeHelper gI()
        {
            if (instance == null)
            {
                instance = new TimeHelper();
                instance.timezone_vietnam = TimeZoneInfo.GetSystemTimeZones().ToList().Find(x => x.DisplayName.ToLower().Contains("hanoi"));
            }
            return instance;
        }
        #endregion
        TimeZoneInfo timezone_vietnam = null;
        public DateTime DateTimeFromString(string time)
        {
            return DateTime.ParseExact(time, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
        }
        public DateTime DateTimeNow()
        {
            return TimeZoneInfo.ConvertTime(DateTime.Now, timezone_vietnam);
        }
        public long CheckTimeOut(string time)
        {
            return (DateTimeNow().Ticks - DateTimeFromString(time).Ticks) / 10000L;
        }
        public TimeSpan calculator(DateTime expired) {
            return expired - DateTimeNow();
        }
    }
}
