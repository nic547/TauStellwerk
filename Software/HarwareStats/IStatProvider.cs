using System.Collections.Generic;

namespace HardwareInfo
{
    interface IStatProvider
    {
        bool CheckAvailability();

        IList<Stat> GetStats();
    }
}
