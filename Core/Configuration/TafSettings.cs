namespace Core.Configuration
{
    public class TafSettings
    {
        public string Environment { get; set; } = "QA";
        public string Browser { get; set; } = "Chrome";
        public bool Headless { get; set; } = false;
        public string BaseUrl { get; set; } = string.Empty;
        public string MinLogLevel { get; set; } = "Info";
        public int DefaultWaitTimeoutSeconds { get; set; } = 10;
    }
}
