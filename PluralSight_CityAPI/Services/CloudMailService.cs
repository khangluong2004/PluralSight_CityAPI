namespace PluralSight_CityAPI.Services
{
    public class CloudMailService: IMailService
    {
        private readonly string _to = string.Empty;
        private readonly string _from = string.Empty;

        public CloudMailService(IConfiguration config)
        {
            _to = config["mailSettings:mailTo"];
            _from = config["mailSettings:mailFrom"];
        }
        public void SendMail(string subject, string body)
        {
            Console.WriteLine($"Send mail from {_from} to {_to} with CloudMail");
            Console.WriteLine($"Subject: {subject}");
            Console.WriteLine($"Body: {body}");
        }
    }
}
