using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace RoboDog
{

    public class Serializer
    {
        //TODO: This is probably not the best place to put the file, but it depends on the file system on the EV3
        private static string filePath = Path.Combine(Environment.CurrentDirectory, "test.dat");

        public static void Serialize(IDictionary<string, Action> combos)
        {
            var formatter = new BinaryFormatter();
            var stream = new MemoryStream();

            formatter.Serialize(stream, combos);
            stream.Position = 0;
            var bytes = stream.GetBuffer();

            using (var streamer = File.Create(filePath))
            {
                streamer.Write(bytes, 0, bytes.Length);
            }
        }

        public static IDictionary<string, Action> Deserialize()
        {
            var bytes = File.ReadAllBytes(filePath);
            var formatter = new BinaryFormatter();
            var stream = new MemoryStream(bytes);
            var action = (IDictionary<string, Action>)formatter.Deserialize(stream);
            return action;
        }


    }

}
