using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsyncHttp
{
    [ServerPath(Method.ALL, "")]
    public sealed class NotFoundPath : IPath
    {
        public override void HandleRequest(Request request)
        {
            Response.Body = new byte[0];
            Response.Code = 404;
        }
    }
}
