using System.Text;
using Newtonsoft.Json;

namespace DisprzTraining.Tests
{
    public class TestHelper
    {
        public static StringContent GetStringContent(object obj)
            => new StringContent(JsonConvert.SerializeObject(obj), Encoding.Default, "application/json");
    }
}