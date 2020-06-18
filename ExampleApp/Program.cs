using AsyncHttp;
using System.IO;
using System.Text;

namespace ExampleApp
{
    class Program
    {
        [ServerPath(Method.GET, "/")]
        public class IndexPath : IPath
        {
            public override void HandleRequest(Request request)
            {
                Response.Body = Encoding.UTF8.GetBytes("Hello Async World");
            }
        }

        static void Main(string[] args)
        {
            Server server = new Server();
            //server.CertificatePath = "cert.pfx";
            //server.CertificatePassword = "1234";
            server.StartAsync();

            while (true)
            {
                // do whatever you want in the meanwhile
            }
        }
    }
}
