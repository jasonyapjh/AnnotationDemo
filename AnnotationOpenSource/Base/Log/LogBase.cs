using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Base.Log
{
    public enum LogCategories
    {
        NoCategory = 0x00,
        Error = 0x01,
        Process = 0x02,
        TactTime = 0x04,
        Result = 0x08,
        Data = 0x10,
        GUI = 0x20,
        Component = 0x40,
    }
    public enum LogLevel
    {
        Critical = 0,
        Medium = 1,
        Details = 2,
    }
    public enum LogSplitTo
    {
        None = 0,
        Size,
        Duration,
        EveryMin,
        EveryHour,
        EveryDay,
        EveryMonth,
        EveryYear
    }
    public class LogBase
    {
        #region Base Functions
        public string Name = "";
        #endregion

        #region Log Functions
        public delegate void log(string text, string senderName, LogCategories catogery, LogLevel level);
        public event log LogEvent = delegate { };
        protected void DoLog(string text, LogLevel level = LogLevel.Medium)
        {
            LogEvent?.Invoke(text, Name, LogCategories.NoCategory, level);
        }
        protected void DoLog(string text, LogCategories options, LogLevel level = LogLevel.Medium)
        {
            LogEvent?.Invoke(text, Name, options, level);
        }
        protected void DoLog(string text, string senderName, LogCategories options, LogLevel level = LogLevel.Medium)
        {
            LogEvent?.Invoke(text, senderName + "-" + Name, options, level);
        }
        #endregion

    }
}
