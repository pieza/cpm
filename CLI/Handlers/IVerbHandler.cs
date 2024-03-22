using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cpm.CLI.Handlers
{
    interface IVerbHandler<T>
    {
        int Handle(T opts);
    }
}
