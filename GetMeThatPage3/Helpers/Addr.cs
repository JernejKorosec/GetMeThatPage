namespace GetMeThatPage3.Helpers
{
    public static class Addr
    {
        public static string? baseUri { get; set; }
        public static bool IsRelativeDirectoryAddress(this string? address)
        {
            bool startAsRelativeDirectoryAddress = false;
            bool endsAsRelativeDirectoryAddress = false;
            if (address == null) return false;
            if (address.StartsWith(".") || address.StartsWith("/"))
                startAsRelativeDirectoryAddress = true;
            if (address.EndsWith(".") || address.EndsWith("/"))
                endsAsRelativeDirectoryAddress = true;
            return startAsRelativeDirectoryAddress && endsAsRelativeDirectoryAddress;
        }
        public static bool IsRelativeFileAddress(this string? address)
        {
            bool startAsRelativeFileAddress = false;
            bool endsAsRelativeFileAddress = false;
            if (address == null) return false;
            if (address.StartsWith(".") || address.StartsWith("/"))
                startAsRelativeFileAddress = true;
            if (!address.EndsWith(".") || !address.EndsWith("/"))
                endsAsRelativeFileAddress = true;
            return startAsRelativeFileAddress && endsAsRelativeFileAddress;
        }
        public static bool IsRelativeAdress(this string? address)
        {
            if (IsRelativeDirectoryAddress(address)) return true;
            if (IsRelativeFileAddress(address)) return true;
            return false;
        }
        public static string Join(params string[] strings)
        { 
            return string.Join("/", strings);
        }
    }
}