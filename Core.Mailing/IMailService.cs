namespace Core.Mailing
{
    public interface IMailService
    {
        Task SendMailAsync(Mail mail);
    }
}
