namespace PluralSight_CityAPI.Services
{
    public class LocalMailService : IMailService
    {
        private readonly string _to = string.Empty;
        private readonly string _from = string.Empty;

        public LocalMailService(IConfiguration config) {
            _to = config["mailSettings:mailTo"];
            _from = config["mailSettings:mailFrom"];
        }

        public void SendMail(string subject, string body)
        {
            Console.WriteLine($"Send mail from {_from} to {_to} with LocalMail");
            Console.WriteLine($"Subject: {subject}");
            Console.WriteLine($"Body: {body}");
        }
    }
}
