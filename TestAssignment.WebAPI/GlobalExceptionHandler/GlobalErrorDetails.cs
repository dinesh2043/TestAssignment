using Newtonsoft.Json;

namespace TestAssignment.WebAPI.GlobalExceptionHandler
{
    internal class GlobalErrorDetails
    {
        public int StatusCode { get; set; }
        public string Message { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}