using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;

namespace PersistentClipboard
{
    public static class JsonHelper
    {
        public static string Serialize<T>(T obj)
        {
            try
            {
                var serializer = new DataContractJsonSerializer(typeof(T));
                using (var stream = new MemoryStream())
                {
                    serializer.WriteObject(stream, obj);
                    return Encoding.UTF8.GetString(stream.ToArray());
                }
            }
            catch
            {
                return null;
            }
        }

        public static T Deserialize<T>(string json)
        {
            try
            {
                var serializer = new DataContractJsonSerializer(typeof(T));
                using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(json)))
                {
                    return (T)serializer.ReadObject(stream);
                }
            }
            catch
            {
                return default;
            }
        }
    }
}
