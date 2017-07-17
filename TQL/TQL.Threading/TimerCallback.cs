using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TQL.Interfaces;

namespace TQL.Threading
{
    public delegate void TimerCallback(DateTimeOffset pointInTime, IEnumerable<IKey> identifiers);
}
