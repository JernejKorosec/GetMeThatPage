namespace GetMeThatPage.Parser
{
    public class PagesList
    {
        private static readonly object lockObject = new object();  // Lock object for synchronization
        public string? UrlRoot { get; set; }
        public string? SavepathRoot { get; set; }
        public List<Page> Pages { get; } = new List<Page>();  // Initialize the list
        public void AddPage(Page page)
        {
            lock (lockObject) // if threads try to add the page and another thread is adding it wait.
            {
                Pages.Add(page);
            }
        }
        public void RemovePage(Page page)
        {
            lock (lockObject) // if threads try to remove the page and another thread is adding it wait.
            {
                Pages.Remove(page);
            }
        }
    }
}