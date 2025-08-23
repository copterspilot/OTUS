// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using Microsoft.AspNetCore.Identity.UI.Services;
using StrongGrid;


namespace CleanAspire.Api.Identity;

public class EmailSender : IEmailSender
{

    private readonly string _sendGridApiKey;
    private readonly string _defaultFromEmail;
    public EmailSender(IConfiguration configuration)
    {
        _sendGridApiKey = configuration["SendGrid:ApiKey"] ?? "";
        _defaultFromEmail = configuration["SendGrid:DefaultFromEmail"] ?? "noreply@blazorserver.com";
    }
    public async Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        if (string.IsNullOrEmpty(_sendGridApiKey))
        {
            throw new InvalidOperationException("SendGrid API Key is not configured.");
        }

        var client = new Client(_sendGridApiKey);
        var from = new StrongGrid.Models.MailAddress(_defaultFromEmail, "noreply");
        var to = new StrongGrid.Models.MailAddress(email, email);
        var messageId = await client.Mail.SendToSingleRecipientAsync(to: to, from: from, subject: subject, htmlContent: htmlMessage, textContent: htmlMessage);
    }
}


