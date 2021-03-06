﻿namespace PaprikaFunctionsApp.Common.Models
{
    public class Status<T>
    {
        public bool Success { get; set; }
        public T Attachment { get; set; }

        public Status(bool success, T attachment)
        {
            Attachment = attachment;
            Success = success;
        }

        public Status(bool success)
        {
            Attachment = default(T); //Set it to null-ish value
            Success = success;
        }
    }
}
