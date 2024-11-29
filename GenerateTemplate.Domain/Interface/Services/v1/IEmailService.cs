namespace GenerateTemplate.Domain.Interface.Services.v1;

public interface IEmailService
{
    Task SendMail(string from, string email, string subject, string message);
}
