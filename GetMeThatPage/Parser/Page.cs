namespace GetMeThatPage.Parser
{
    public class Page
    {
        private static readonly object lockObject = new object();
        private string? urlRoot;
        private string? localRoot;
        public string? UrlRoot
        {
            get
            {
                lock (lockObject)
                {
                    return urlRoot;
                }
            }
            set
            {
                lock (lockObject)
                {
                    urlRoot = value;
                }
            }
        }
        public string? LocalRoot
        {
            get
            {
                lock (lockObject)
                {
                    return localRoot;
                }
            }
            set
            {
                lock (lockObject)
                {
                    localRoot = value;
                }
            }
        }
        public bool? IsSaved { get; set; }
        public Page SetUrlAndLocalRoot(string url, string local)
        {
            lock (lockObject)
            {
                urlRoot = url;
                localRoot = local;
            }
            return this;
        }
    }

}