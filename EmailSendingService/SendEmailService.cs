using InternetBank.Db.Db.Repositories;
using MimeKit;
using MailKit.Net.Smtp;

namespace EmailSendingService;

public interface ISendEmailService
{
    Task SendEmail(string emailSubject, string emailBody);
    Task ChangeEmailStatusToBlocked();
}

public class SendEmailService : ISendEmailService
{
    private readonly IEmailSenderRepository _emailSenderRepository;

    public SendEmailService(IEmailSenderRepository emailSenderRepository)
    {
        _emailSenderRepository = emailSenderRepository;
    }

    private MimeMessage CreateEmail(string receiverEmail, string emailSubject, string emailBody)
    {
        var smtpClient = new SmtpClient();
        smtpClient.Connect("smtp.gmail.com", 587, false);
        smtpClient.Authenticate("testproject1100@gmail.com", "oxuwybrsrevhufdr");

        var mailMessage = new MimeMessage();
        mailMessage.From.Add(new MailboxAddress("Credo Bank", "testproject1100@gmail.com"));
        mailMessage.To.Add(new MailboxAddress("Juba", receiverEmail));
        mailMessage.Subject = emailSubject;

        var builder = new BodyBuilder();
        builder.TextBody = emailBody;
        mailMessage.Body = builder.ToMessageBody();

        return mailMessage;
    }

    public async Task SendEmail(string emailSubject, string emailBody)
    {
        var smtpClient = new SmtpClient();
        var pendedEmails = await _emailSenderRepository.FindPendedEmails();
        if (pendedEmails == null)
        {
            throw new Exception("Emails not found");
        }
        var parallelOptions = ParallelOptionsCreation(3);
        
        await Parallel.ForEachAsync(pendedEmails, parallelOptions,async (pendedEmail, token) =>
        {
            var mailMessage = CreateEmail(pendedEmail.Email, emailSubject, emailBody);
            var senderEntity = await _emailSenderRepository.AddInSenderDb(pendedEmail.Email, emailSubject, emailBody);

            await smtpClient.SendAsync(mailMessage, token);
            await smtpClient.DisconnectAsync(true, token);
            await _emailSenderRepository.ChangeEmailStatusToSent(senderEntity);
        });
    }
    
    public async Task ChangeEmailStatusToBlocked()
    {
        var parallelOptions = ParallelOptionsCreation(3);
        var listOfEmailSenderEntity = await _emailSenderRepository.GetBlockedEmails();
        await Parallel.ForEachAsync(listOfEmailSenderEntity, parallelOptions,async (emailSenderEntity, token) =>
        {
            await _emailSenderRepository.ChangeEmailStatusToBlocked(emailSenderEntity);
        });
    }

    private static ParallelOptions ParallelOptionsCreation(int parallelCount)
    {
        var parallelOptions = new ParallelOptions
        {
            MaxDegreeOfParallelism = parallelCount
        };
        return parallelOptions;
    }
}