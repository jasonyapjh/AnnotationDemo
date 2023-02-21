using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Base.Common
{
    public class ParametersObject : ParametersBase
    {
        public ParametersObject() : base() { }

        public ParametersObject(string query) : base(query) { }
    }
}
