using Newtonsoft.Json;

namespace DeliveryServiceApp.Helpers.Exceptions
{
    public class ErrorDetails
    {
        public string Message { get; set; }
        public string Source { get; set; }

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
