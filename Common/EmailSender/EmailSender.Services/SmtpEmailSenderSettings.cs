﻿namespace EmailSender.Services
{
    public class SmtpEmailSenderSettings
    {
        public string FromName { get; set; } = null!;
        public string ApiAdress { get; set; } = null!;
        public int ApiPort { get; set; }
        public string ApiLogin { get; set; } = null!;
        public string ApiPassword { get; set; } = null!;
    }
}
