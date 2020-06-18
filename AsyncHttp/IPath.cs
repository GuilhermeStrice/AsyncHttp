namespace AsyncHttp
{
    public abstract class IPath
    {
        private ServerPath attr;

        public Response Response
        { get; set; }

        public IPath()
        {
            Response = new Response();
            attr = (ServerPath)GetType().GetCustomAttributes(typeof(ServerPath), false)[0];
        }

        public string GetPathString()
        {
            return attr.Path;
        }

        public string GetMethodString()
        {
            return attr.Method;
        }

        public abstract void HandleRequest(Request request);
    }
}
