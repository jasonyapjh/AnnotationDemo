using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Base.Sequence.ThreadServer
{
    public interface IThreadEngine
    {
        bool Start(string secCode);

        bool Stop();

        bool Stop(int timeOut);

        bool IsAlive(int moduleIdx);

        string Module_Name { set; }

        int Interval { set; }

        string Err_Msg { get; }

        EventHandler Loop_Entry { set; }

        ThreadPriority Priority_Level { set; }
    }
}
