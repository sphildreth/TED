namespace TED.Models.MetaData
{
    public sealed class ProcessMessage
    {
        public static string OkCheckMark = "✅";

        public static string BadCheckMark = "❌";

        public static string Info = "🆗";

        public static string Warning = "⛔";

        public string? Message { get; set; }

        public bool IsOK { get; set; }

        public string? StatusIndicator { get; set; }

        public ProcessMessage()
        {
        }

        public ProcessMessage(string? message, bool isOK, string? statusIndicator)
        {
            Message = message;
            IsOK = isOK;
            StatusIndicator = statusIndicator;
        }

        public ProcessMessage(Exception ex)
        {
            Message = ex.Message;
            StatusIndicator = BadCheckMark;
        }

        public static ProcessMessage MakeInfoMessage(string message)
        {
            return new ProcessMessage
            {
                Message = message,
                IsOK = true,
                StatusIndicator = Info
            };
        }

        public static ProcessMessage MakeBadMessage(string message)
        {
            return new ProcessMessage
            {
                Message = message,
                StatusIndicator = BadCheckMark
            };
        }


        public static ProcessMessage MakeWarningMessage(string message)
        {
            return new ProcessMessage
            {
                Message = message,
                StatusIndicator = Warning
            };
        }


    }
}
