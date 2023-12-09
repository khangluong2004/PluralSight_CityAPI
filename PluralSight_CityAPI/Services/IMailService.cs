namespace PluralSight_CityAPI.Services
{
    public interface IMailService
    {
        void SendMail(string subject, string body);
    }
}