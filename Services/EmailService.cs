namespace Exercise4.Services;

using System.Net;
using System.Net.Mail;

public class EmailService
{
    private readonly IConfiguration _config; 

    // Passing appsettings.json vars from Program.cs
    public EmailService(IConfiguration config)
    {
        _config = config;
    }
    
    public void SendEmail(string to, string subject, string body)
    {
        // Getting smtp object from appsettings.json
        var smtp = _config.GetSection("Smtp");

        // Client SMTP
        var client = new SmtpClient(smtp["Host"]) // Server Host Provider
        {
            Port = int.Parse(smtp["Port"]), // Port configured
            Credentials = new NetworkCredential( 
                smtp["User"], smtp["Pass"] // Email account credentials
            ),
            EnableSsl = bool.Parse(smtp["EnableSsl"]) // SSL Config
        };

        // HTML Layout
        string layout = @$"
            <div>
                <h1>Hello. </h1>
                <p>This message was sent from Library App</p>
                <p style='color: white; border-bottom: 3px solid #000; margin-bottom: 1rem;'>.</p>
                <p>{body}</p>
                <p style='color: white; border-bottom: 3px solid #000; margin-bottom: 1rem;'>.</p>
                <p>You can see more information about doing click <a href='https://bookstore.jerogallego.andrescortes.dev'>here</a></p>
            </div>
        ";
        
        // Creating Email 
        var mail = new MailMessage
        {
            From = new MailAddress(smtp["User"]),
            Subject = subject,
            Body = layout,
            IsBodyHtml = true
        };
        
        mail.To.Add(to); // Add recipients
        client.SendMailAsync(mail); // Sending
    }
}