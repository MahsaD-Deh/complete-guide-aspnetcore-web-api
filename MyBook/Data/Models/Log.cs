﻿namespace MyBook.Data.Models
{
    public class Log
    {
        public int Id { get; set; }

        public string Message { get; set; }

        public string MessageTemplate { get; set; }

        public string MessageLevel { get; set; }

        public DateTime TimeStamp { get; set; }

        public string Exception { get; set; }

        public string Properties { get; set; }

        public string LogEvent { get; set; }

    }
}
