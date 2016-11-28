using BestFor.Dto;
using Newtonsoft.Json;
using System;
using System.Text;

namespace BestFor
{
    /// <summary>
    /// Help converting data into a string back and force.
    /// Needed for navigation between controllers.
    /// </summary>
    public class NavigationHelper
    {
        public static string Encode(NavigationDataDto data)
        {
            if (data == null)
                throw new Exception("Null data passed to NavigationHelper.Encode");
            // Convert object to json.
            var serial = JsonConvert.SerializeObject(data);
            // Base 64 encode to that is can be passed in the URL
            var anotherSerial = Convert.ToBase64String(Encoding.Unicode.GetBytes(serial));

            return anotherSerial;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns>Null if can not decode or nothing to decode</returns>
        public static NavigationDataDto Decode(string data)
        {
            if (string.IsNullOrEmpty(data) || string.IsNullOrWhiteSpace(data))
                return null;
            // Try to decode from base 64 
            byte[] byteData = Convert.FromBase64String(data);
            string decodedString = Encoding.Unicode.GetString(byteData);

            // Convert json to object
            var result = JsonConvert.DeserializeObject<NavigationDataDto>(decodedString);

            return result;
        }
    }
}
