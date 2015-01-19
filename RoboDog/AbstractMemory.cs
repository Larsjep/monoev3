using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization.Json;
using Newtonsoft.Json;
using System.IO.Compression;

namespace RoboDog
{
    [Serializable]
    public abstract class AbstractMemory : IMemory
    {
        public AbstractMemory()
        {

        }

        public virtual IDictionary<string, Action> GetTricksFromMemory()
        {
            return Serializer.Deserialize();
        }


        public virtual void AddAllTricksToMemory(IDictionary<string, Action> combos)
        {
            Serializer.Serialize(combos);
        }

       
        public virtual void AddTrickToMemory(IDictionary<string, Action> oneTrick)
        {
            //save to file
            Serializer.Serialize(oneTrick);


        }
    }
}
