using System.Net;

namespace API_Practice.Model
{
    public class APIResponse
    {
        public APIResponse()
        {
            ErrorMessage = new List<string>();
        }

        public bool IsSuucess { get; set; }

        public object Result { get; set; }

        public HttpStatusCode StatusCode { get; set; }

        public List<string> ErrorMessage { get;set; }
    }
}
