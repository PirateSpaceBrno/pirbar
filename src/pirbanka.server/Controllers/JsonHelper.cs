using Newtonsoft.Json;

namespace PirBanka.Server.Controllers
{
    internal static class JsonHelper
    {
        public static T DeserializeObject<T>(string request) where T : class, new()
        {
            try
            {
                return JsonConvert.DeserializeObject<T>(request);
            }
            catch
            {
                return null;
            }
        }

        public static string SerializeObject(object objectToSerialize)
        {
            try
            {
                return JsonConvert.SerializeObject(objectToSerialize);
            }
            catch
            {
                return "";
            }
        }
    }
}
