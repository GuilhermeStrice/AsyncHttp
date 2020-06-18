using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace AsyncHttp
{
    public class Server
    {
        private object stop_lock = new object();
        private bool stop = false;
        private HttpListener listener = new HttpListener();
        private List<IPath> paths = new List<IPath>();

        public int Port
        {
            get; set;
        }

        public string CertificatePath
        {
            get; set;
        }

        public string CertificatePassword
        {
            get; set;
        }

        public int SSLPort
        {
            get; set;
        }

        public Server(int Port = 8500, int SSLPort = 8443)
        {
            this.Port = Port;
            this.SSLPort = SSLPort;
        }

        public async Task StartAsync()
        {
            listener.Prefixes.Add("http://*:" + Port.ToString() + "/");

            if (!string.IsNullOrEmpty(CertificatePath))
            {
                try
                {
                    var cert_path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, CertificatePath);
                    X509Certificate2 cert = new X509Certificate2(cert_path, CertificatePassword);
                    X509Store store = new X509Store(StoreName.Root, StoreLocation.LocalMachine);
                    store.Open(OpenFlags.ReadWrite);
                    if (!store.Certificates.Contains(cert))
                    {
                        store.Add(cert);
                    }
                    store.Close();
                    listener.Prefixes.Add("https://*:" + SSLPort.ToString() + "/");
                }
                catch (Exception ex)
                {
                    // notify in console
                }
            }

            try
            {
                var paths = from a in AppDomain.CurrentDomain.GetAssemblies()
                            from t in a.GetTypes()
                            let attributes = t.GetCustomAttributes(typeof(ServerPath), true)
                            where attributes != null && attributes.Length > 0
                            select new { Type = t, Attributes = attributes.Cast<ServerPath>() };

                foreach (var t in paths)
                {
                    if (t.Type.BaseType == typeof(IPath))
                    {
                        var obj = (IPath)Activator.CreateInstance(t.Type);
                        this.paths.Add(obj);
                    }
                }

                listener.Start();
                stop = false;

                while (listener.IsListening)
                {
                    var context = await listener.GetContextAsync();

                    try
                    {
                        Console.WriteLine(context.Request.RawUrl);
                        await ProcessRequestAsync(context);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("# EXCEPTION #   " + ex.StackTrace);
                    }

                    lock (stop_lock)
                    {
                        if ((bool)stop == true) listener.Stop();
                    }
                }

                listener.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public IPath GetPathForUrl(HttpListenerContext context)
        {
            for (var i = 0; i < paths.Count; i++)
            {
                var current_path = paths[i];
                if (context.Request.RawUrl == current_path.GetPathString())
                {
                    if (current_path.GetMethodString() != "ALL")
                    {
                        if (context.Request.HttpMethod == current_path.GetMethodString())
                        {
                            return paths[i];
                        }
                    }
                    else
                        return paths[i];
                }
            }

            return new NotFoundPath();
        }

        private async Task ProcessRequestAsync(HttpListenerContext context)
        {
            Request req = new Request(context.Request);

            IPath path = GetPathForUrl(context);

            path.HandleRequest(req);

            var Response = path.Response;
            context.Response.StatusCode = Response.Code == 0 ? 200 : Response.Code;
            
            var output_stream = context.Response.OutputStream;

            context.Response.ContentType = (!string.IsNullOrEmpty(Response.ContentType) ? 
                Response.ContentType: MimeTypes.Text.Plain);

            context.Response.Cookies = Response.Cookies;

            if (Response.Body != null)
            {
                context.Response.ContentLength64 = Response.Body.Length;
                await output_stream.WriteAsync(Response.Body, 0, Response.Body.Length);
            }
            context.Response.Close();
        }

        public void Stop()
        {
            lock (stop_lock)
            {
                stop = true;
            }
        }

        public bool isRunning()
        {
            bool ret_val;
            lock (stop_lock)
            {
                ret_val = !stop;
            }
            return ret_val;
        }
    }
}
