using System;
using System.Diagnostics;

namespace Base.Common
{
    public enum Units
    {
        Second,
        Millisecond,
        Microsecond,
        Nanosecond,
    }
    public static class TimerBenchmark
    {
        public static double ResolveTicks(Units uom, long ticks, int decimals = 0)
        {
            // Stopwatch.Frequency gives the number of ticks per second. This value is hardware-
            // and system-dependent.
            var time =
                uom == Units.Millisecond ? ticks * 1e3 / Stopwatch.Frequency :
                uom == Units.Microsecond ? ticks * 1e6 / Stopwatch.Frequency :
                uom == Units.Nanosecond ? ticks * 1e9 / Stopwatch.Frequency :
                ticks * 1 / Stopwatch.Frequency;

            return decimals != 0 ? Math.Round(time, (int)decimals) : time;
        }
        public static void ExecuteAction(Action action, Units uom, out double elapsed)
        {
            var stopwatch = Stopwatch.StartNew();
            action();
            stopwatch.Stop();

            elapsed = ResolveTicks(uom, stopwatch.ElapsedTicks);
        }
        public static void ExecuteAction<T>(Action<T> action, Units uom, out double elapsed, T arg)
        {
            var stopwatch = Stopwatch.StartNew();
            action(arg);
            stopwatch.Stop();

            elapsed = ResolveTicks(uom, stopwatch.ElapsedTicks);
        }
        public static void ExecuteAction<T1, T2>(Action<T1, T2> action, Units uom, out double elapsed, T1 arg1, T2 arg2)
        {
            var stopwatch = Stopwatch.StartNew();
            action(arg1, arg2);
            stopwatch.Stop();

            elapsed = ResolveTicks(uom, stopwatch.ElapsedTicks);
        }
        public static void ExecuteAction<T1, T2, T3>(Action<T1, T2, T3> action, Units uom, out double elapsed, T1 arg1, T2 arg2, T3 arg3)
        {
            var stopwatch = Stopwatch.StartNew();
            action(arg1, arg2, arg3);
            stopwatch.Stop();

            elapsed = ResolveTicks(uom, stopwatch.ElapsedTicks);
        }
        public static TResult ExecuteFunc<TResult>(Func<TResult> func, Units uom, out double elapsed)
        {
            var stopwatch = Stopwatch.StartNew();
            TResult ret = func();
            stopwatch.Stop();

            elapsed = ResolveTicks(uom, stopwatch.ElapsedTicks);
            return ret;
        }
        public static TResult ExecuteFunc<T, TResult>(Func<T, TResult> func, Units uom, out double elapsed, T arg)
        {
            var stopwatch = Stopwatch.StartNew();
            TResult ret = func(arg);
            stopwatch.Stop();

            elapsed = ResolveTicks(uom, stopwatch.ElapsedTicks);
            return ret;
        }
        public static TResult ExecuteFunc<T1, T2, TResult>(Func<T1, T2, TResult> func, Units uom, out double elapsed, T1 arg1, T2 arg2)
        {
            var stopwatch = Stopwatch.StartNew();
            TResult ret = func(arg1, arg2);
            stopwatch.Stop();

            elapsed = ResolveTicks(uom, stopwatch.ElapsedTicks);
            return ret;
        }
        public static TResult ExecuteFunc<T1, T2, T3, TResult>(Func<T1, T2, T3, TResult> func, Units uom, out double elapsed, T1 arg1, T2 arg2, T3 arg3)
        {
            var stopwatch = Stopwatch.StartNew();
            TResult ret = func(arg1, arg2, arg3);
            stopwatch.Stop();

            elapsed = ResolveTicks(uom, stopwatch.ElapsedTicks);
            return ret;
        }
    }
}
