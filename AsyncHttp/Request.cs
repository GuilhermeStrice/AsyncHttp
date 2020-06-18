using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace AsyncHttp
{
    public class Request
    {
        public Request(HttpListenerRequest Request = null)
        {
            if (Request == null)
                throw new ArgumentNullException();

            BaseUrl = Request.Url.GetLeftPart(UriPartial.Authority);
            BodyStream = Request.InputStream;
            ContentType = Request.ContentType;

            Cookies = Request.Cookies;
            Queries = Request.QueryString;
        }

        public string GetBodyString()
        {
            using (BodyStream)
            {
                using (StreamReader readStream = new StreamReader(BodyStream, Encoding.UTF8))
                {
                    return readStream.ReadToEnd();
                }
            }
        }

        public string BaseUrl
        {
            get; private set;
        }

        public Stream BodyStream
        {
            get; private set;
        }

        public CookieCollection Cookies
        {
            get; private set;
        }

        public string ContentType
        {
            get; private set;
        }

        public NameValueCollection Queries
        {
            get; private set;
        }
    }
}
