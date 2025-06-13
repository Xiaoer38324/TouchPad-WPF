using Newtonsoft.Json;
using System.IO;
using System.Text;

namespace WpfApp1
{
    internal static class SaveALoad
    {
        public static bool Save(object obj, string path)
        {
            try
            {
                string json = JsonConvert.SerializeObject(obj);
                File.WriteAllText(path, json);
                return true;
            }
            catch (IOException e)
            {
                return false;
            }

        }
        public static string CovertoJson(object obj)
        {
            try
            {
                string json = JsonConvert.SerializeObject(obj);
                return json;
            }
            catch (IOException e)
            {
                return null;
            }

        }
        public static T Load<T>(string path)
        {
            try
            {
                string json="";
                if (File.Exists(path))
                 json = File.ReadAllText(path);
                T obj = JsonConvert.DeserializeObject<T>(json);
                if (obj != null)
                    return obj;
                else
                    return default(T);
            }
            catch (IOException e)
            {
                return default(T);
            }

        }
        public static T Load<T>(Stream stream)
        {
            try
            {
                string json = "";
                if (stream != null&&stream.CanRead)
                {

                    StreamReader streamR = new StreamReader(stream, Encoding.Default);
                    json = streamR.ReadToEnd();
                }
                T obj = JsonConvert.DeserializeObject<T>(json);
                if (obj != null)
                    return obj;
                else
                    return default(T);
            }
            catch (IOException e)
            {
                return default(T);
            }

        }
        public static T LoadByCode<T>(string json)
        {
            try
            {
                
                T obj = JsonConvert.DeserializeObject<T>(json);
                if (obj != null)
                    return obj;
                else
                    return default(T);
            }
            catch (IOException e)
            {
                return default(T);
            }

        }
    }
}
