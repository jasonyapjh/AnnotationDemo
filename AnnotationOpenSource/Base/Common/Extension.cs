using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Base.Common
{
    public static class Extension
    {
        public static string GetDetailMessage(this Exception exp)
        {
            string sDump = $"Exception Message: {exp.Message} in ";
            if (exp.TargetSite != null) sDump += $"{exp.TargetSite} at ";
            if (exp.StackTrace != null) sDump += $"{exp.StackTrace}\n";
            while (exp.InnerException != null)
            {
                sDump += $"->{exp.Message} in ";
                if (exp.TargetSite != null) sDump += $"{exp.TargetSite} at ";
                if (exp.StackTrace != null) sDump += $"{exp.StackTrace}\n";
                exp = exp.InnerException;
            }
            return sDump;
        }
        public static T Between<T>(this T val, T min, T max) where T : IComparable<T>
        {
            if (val.CompareTo(min) < 0) return min;
            else if (val.CompareTo(max) > 0) return max;
            else return val;
        }
        public static short Between(this short val, short min, short max)
        {
            if (val.CompareTo(min) < 0) return min;
            else if (val.CompareTo(max) > 0) return max;
            else return val;
        }
        public static ushort Between(this ushort val, ushort min, ushort max)
        {
            if (val.CompareTo(min) < 0) return min;
            else if (val.CompareTo(max) > 0) return max;
            else return val;
        }
        public static void FileCleanUp(string path, int day)
        {
            string[] files = Directory.GetFiles(path);

            foreach (string file in files)
            {
                FileInfo m_FI = new FileInfo(file);
                if (m_FI.CreationTime < DateTime.Now.AddDays(-(day)))
                {
                    m_FI.Delete();
                }

            }
        }
        public static void FolderCleanUp(string path, int day)
        {
            string[] files = Directory.GetDirectories(path);

            foreach (string file in files)
            {
                DateTime dt = Directory.GetCreationTime(file);
                if (DateTime.Now.Subtract(dt).TotalDays > day)
                {
                    try
                    {
                        Directory.Delete(file, true);
                    }
                    catch (Exception ex)
                    {
                        var str = ex.GetDetailMessage();
                    }
                }
            }
        }
        public static string GetHalconErrMessage(this Exception exp)
        {
            string temp = exp.Message;

            temp = temp.Substring(7);

            string sDump = $"Exception Message: Vision {temp}.";

            return sDump;
        }
        public static double ConvertRadiansToDegrees(double radians)
        {
            double degrees = (180 / Math.PI) * radians;
            return (degrees);
        }
        public static double ConvertDegreesToRadians(double degrees)
        {
            double radians = (Math.PI / 180) * degrees;
            return (radians);
        }
        public static double ConvertDegreesToRadians(this int degrees)
        {
            double radians = (Math.PI / 180) * degrees;
            return (radians);
        }
        public enum Output
        {
            MessageBox = 0,
            Debug,
            EventLog,
        }
        public static string GetDetailMessage(this Exception exp, Output outputtype)
        {
            string sDump = $"Exception Message: {exp.Message} in ";
            if (exp.TargetSite != null) sDump += $"{exp.TargetSite} at ";
            if (exp.StackTrace != null) sDump += $"{exp.StackTrace}\n";
            while (exp.InnerException != null)
            {
                sDump += $"->{exp.Message} in ";
                if (exp.TargetSite != null) sDump += $"{exp.TargetSite} at ";
                if (exp.StackTrace != null) sDump += $"{exp.StackTrace}\n";
                exp = exp.InnerException;
            }
            return sDump;
        }
        public static bool CheckFileExist(string path)
        {
            if (File.Exists(path))
            {
                return true;
            }
            else
                return false;
        }
        public static void CreateFolder(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }
        public static bool CheckFolderExist(string path)
        {
            if (!Directory.Exists(path))
            {
                return false;
            }
            return true;
        }
        public static void CreateFile(string path)
        {
            File.Create(path);
        }
        public static bool ChangeLogFileName(string appenderName, string newFilename)
        {
            var rootRepository = log4net.LogManager.GetRepository();
            foreach (var appender in rootRepository.GetAppenders())
            {
                if (appender.Name.Equals(appenderName) && appender is log4net.Appender.RollingFileAppender)
                {
                    var fileAppender = appender as log4net.Appender.FileAppender;
                    fileAppender.File = newFilename;
                    fileAppender.ActivateOptions();
                    return true;  // Appender found and name changed to NewFilename
                }
            }
            return false; // appender not found
        }
    }
}
