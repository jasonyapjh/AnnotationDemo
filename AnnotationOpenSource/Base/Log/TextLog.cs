using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Base.Log
{
    public class LogEventArgs : EventArgs
    {
        public string Message { get; set; }
    }
    public class TextLog
    {
        private static readonly ILog SystemLog = LogManager.GetLogger("System");
        private static readonly ILog ErrorLog = LogManager.GetLogger("Error");
        private static readonly ILog CommLog = LogManager.GetLogger("Communication");
        public event EventHandler<LogEventArgs> SendSyslog;
        public event EventHandler<LogEventArgs> SendErrorlog;
        public event EventHandler<LogEventArgs> SendCommlog;
        public LogEventArgs SyslogEventArgs = new LogEventArgs();
        public TextLog()
        {

        }
        protected virtual void OnSendSysLog(LogEventArgs e)
        {
            SendSyslog?.Invoke(this, e);
        }
        protected virtual void OnSendErrorLog(LogEventArgs e)
        {
            SendErrorlog?.Invoke(this, e);
        }
        protected virtual void OnSendCommLog(LogEventArgs e)
        {
            SendCommlog?.Invoke(this, e);
        }
        public void Info(string msg)
        {
            SystemLog.Info(msg);
            SyslogEventArgs.Message = msg;
            OnSendSysLog(SyslogEventArgs);
        }

        public void Error(string msg)
        {
            ErrorLog.Error(msg);
            SyslogEventArgs.Message = msg;
            OnSendErrorLog(SyslogEventArgs);
        }

        public void Comm(string msg)
        {
            CommLog.Info(msg);
            SyslogEventArgs.Message = msg;
            OnSendCommLog(SyslogEventArgs);
        }

    }
}
