namespace GetMeThatPage3.Helpers.State
{
    public class State
    {
        /// <summary>
        /// Is stream loaded or not
        /// </summary>
        public bool IsLoaded { get; set; }
        /// <summary>
        /// was file processed, parsed
        /// </summary>
        public bool IsParsed { get; set; }
        /// <summary>
        /// is file saved locally
        /// </summary>
        public bool IsSaved { get; set; }
    }
}