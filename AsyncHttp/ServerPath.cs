using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsyncHttp
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ServerPath : Attribute
    {
        public ServerPath(Method Method, string Path)
        {
            this.Path = Path;
            this.Method = Method.ToString();
        }

        public string Path { get; private set; }
        public string Method { get; private set; }
    }
}
