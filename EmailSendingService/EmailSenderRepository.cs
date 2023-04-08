using InternetBank.Db.Db.Entities;
using Microsoft.EntityFrameworkCore;

namespace InternetBank.Db.Db.Repositories;

public interface IEmailSenderRepository
{
    Task<List<EmailSenderEntity>?> FindPendedEmails();
    Task<EmailSenderEntity> AddInSenderDb(string receiverEmail, string subject, string text);
    Task<List<EmailSenderEntity>> GetBlockedEmails();
    Task<EmailSenderEntity> ChangeEmailStatusToSent(EmailSenderEntity emailSenderEntity);
    Task ChangeEmailStatusToBlocked(EmailSenderEntity emailSenderEntity);
}

public class EmailSenderRepository : IEmailSenderRepository
{
    private readonly AppDbContext _db;

    public EmailSenderRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<EmailSenderEntity> AddInSenderDb(string receiverEmail, string subject, string text)
    {
        var user = await _db.User.FirstOrDefaultAsync(u => u.Email == receiverEmail);

        if (user != null)
        {
            var emailSenderEntity = CreateEmailSenderEntity(user.Id, receiverEmail);
            var emailLoggerEntity = CreateEmailLoggerEntity(subject, text, emailSenderEntity, user);
            await _db.EmailSender.AddAsync(emailSenderEntity);
            await _db.EmailLogger.AddAsync(emailLoggerEntity);
            await _db.SaveChangesAsync();
            return emailSenderEntity;
        }

        var loggerEntity = CreateLoggerEntity(receiverEmail);
        await _db.Logger.AddAsync(loggerEntity);
        await _db.SaveChangesAsync();
        throw new Exception("User not found");
    }

    private static EmailLoggerEntity CreateEmailLoggerEntity(string subject, string text, EmailSenderEntity emailSenderEntity,
        UserEntity user)
    {
        var emailLoggerEntity = new EmailLoggerEntity
        {
            EmailId = emailSenderEntity.Id,
            SenderEmail = "testproject1100@gmail.com",
            ReceiverEmail = emailSenderEntity.Email,
            ReceiverName = $"{user.FirstName} {user.LastName}",
            Subject = subject,
            Text = text,
            SendDate = DateTimeOffset.Now
        };
        
        return emailLoggerEntity;
    }

    public async Task<List<EmailSenderEntity>?> FindPendedEmails()
    {
        var pendedEmails = await _db.EmailSender.Where(e => e.Type == 0).ToListAsync();
  
        return pendedEmails;
    }

    private EmailSenderEntity CreateEmailSenderEntity(int userId, string email)
    {
        var emailSenderEntity = new EmailSenderEntity
        {
            UserId = userId,
            Email = email,
            Type = 0,
            CreationDate = DateTimeOffset.Now
        };

        return emailSenderEntity;
    }

    public async Task<EmailSenderEntity> ChangeEmailStatusToSent(EmailSenderEntity emailSenderEntity)
    {
        emailSenderEntity.Type = (EmailType)1;
        
        return emailSenderEntity;
    }
    
    public async Task ChangeEmailStatusToBlocked(EmailSenderEntity emailSenderEntity)
    {
        emailSenderEntity.Type = (EmailType)2;
        await _db.EmailSender.AddAsync(emailSenderEntity);
    }
    
    public async Task<List<EmailSenderEntity>> GetBlockedEmails()
    {
        var needsBlockEmails = await _db.EmailSender
            .Where(e => e.CreationDate < DateTimeOffset.Now.AddDays(-1) && e.Type == 0)
            .ToListAsync();

        if (needsBlockEmails == null)
        {
            throw new Exception("There is not any emails which needs blocking");
        }
        
        return needsBlockEmails;
    }
    
    private LoggerEntity CreateLoggerEntity(string email)
    {
        var loggerEntity = new LoggerEntity
        {
            ProjectName = "Email Sender Service",
            Exception = "Could not find user with provided email",
            Data = email,
            ThrowTime = DateTimeOffset.Now
        };

        return loggerEntity;
    }
}