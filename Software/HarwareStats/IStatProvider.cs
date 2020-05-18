using System;
using System.Collections.Generic;
using System.Text;

namespace HardwareInfo
{
    interface IStatProvider
    {
        bool CheckAvailability();

        IList<Stat> GetStats();
    }
}
