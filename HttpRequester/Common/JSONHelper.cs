using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

namespace HttpRequester.Common
{
    public class JSONHelper
    {
        public static string Serialize<T>(IEnumerable<T> objList)
        {
            if (objList == null)
            {
                throw new ArgumentException("obj must not be null");
            }

            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(IEnumerable<T>), new[] { objList.GetType() });
            using (var ms = new MemoryStream())
            {
                serializer.WriteObject(ms, objList);
                ms.Seek(0, SeekOrigin.Begin);
                StreamReader sr = new StreamReader(ms);
                return sr.ReadToEnd();
            }
        }
    }
}
