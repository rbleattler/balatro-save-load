using System;

namespace BalatroSaveToolkit.Models
{
    public class ActivityLogItem
    {
        public DateTime Timestamp { get; set; }
        public string Level { get; set; }
        public string Message { get; set; }
        public string Details { get; set; }
        
        public ActivityLogItem(DateTime timestamp, string level, string message, string details = null)
        {
            Timestamp = timestamp;
            Level = level;
            Message = message;
            Details = details;
        }
    }
}
