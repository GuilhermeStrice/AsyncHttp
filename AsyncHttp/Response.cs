using System.IO;
using System.Net;

namespace AsyncHttp
{
    public class Response
    {
        public Response()
        {
            Cookies = new CookieCollection();
        }

        public byte[] Body
        {
            get; set;
        }

        public int Code
        {
            get; set;
        }

        public string ContentType
        {
            get; set;
        }

        public CookieCollection Cookies
        {
            get; set;
        }
    }
}
