using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoboDog
{
    public interface IMemory
    {
        IDictionary<string, Action> GetTricksFromMemory();
        void AddTrickToMemory(IDictionary<string, Action> oneTrick);
        void AddAllTricksToMemory(IDictionary<string, Action> combos);
    }
}
