
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace KiotVietTimeSheet.Utilities
{
    public static class EmailHelper
    {
        private static Regex CreateRegexInitial()
        {
            return new Regex("<img[^>]+src\\s*=\\s*['\"]([^'\"]+)['\"][^>]*>"); //NOSONAR
        }

        public static async Task SendMailToServerAsync(string smtpHost, int port, bool useSsl, string userCertify,
           string passwordCertify, string senderEmail, string sendTo, string bccEmail, string replyTo,
           string senderName, string replyToName, string subject, string body, Attachment attachment = null)
        {
            try
            {

                using (var smtpClient = new SmtpClient
                {
                    Host = smtpHost,
                    Port = port,
                    EnableSsl = useSsl,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(userCertify, passwordCertify)
                })
                using (var mailMessage = new MailMessage(senderEmail, sendTo, subject, body)
                {
                    SubjectEncoding = Encoding.UTF8,
                    BodyEncoding = Encoding.UTF8,
                    IsBodyHtml = true
                })
                {
                    if (body != null && body.Contains("base64"))
                    {
                        var rx = EmailHelper.CreateRegexInitial();

                        var matches = rx.Matches(body);
                        
                        var viewCollections = new List<LinkedResource>();

                        body = GenBody(body, matches, viewCollections);

                        var view = AlternateView.CreateAlternateViewFromString(body, null, MediaTypeNames.Text.Html);
                        foreach (var tempLinkedReSource in viewCollections)
                        {
                            view.LinkedResources.Add(tempLinkedReSource);
                        }
                        mailMessage.AlternateViews.Add(view);
                    }
                    mailMessage.Body = body;
                    if (attachment != null) mailMessage.Attachments.Add(attachment);

                    if (CheckExistString(replyTo))
                        mailMessage.ReplyToList.Add(new MailAddress(replyTo, replyToName, Encoding.UTF8));

                    if (CheckExistString(bccEmail))
                        mailMessage.Bcc.Add(new MailAddress(bccEmail));

                    smtpClient.SendCompleted += (result, @event) =>
                    {
                        Console.WriteLine(@event);
                    };
                    try
                    {
                        await smtpClient.SendMailAsync(mailMessage).ConfigureAwait(false);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        throw;
                    }
                }
            }

            catch (SmtpException ex)
            {
                switch (ex.StatusCode)
                {
                    case SmtpStatusCode.MustIssueStartTlsFirst:
                    case SmtpStatusCode.ClientNotPermitted:
                        var exNew = new Exception("Tài khoản / mật khẩu không chính xác không xác thực được với server");
                        throw  exNew;
                    default:
                        var stmEx = new Exception(ex.Message);
                        throw stmEx;
                }
            }
        }

        private static bool CheckExistString(string text)
        {
            return !string.IsNullOrEmpty(text) && !string.IsNullOrWhiteSpace(text);
        }

        private static string GenBody(string body, IEnumerable matches, ICollection<LinkedResource> viewCollections)
        {
            var i = 0;
            foreach (Match match in matches)
            {
                var groupCollection = match.Groups;
                Console.WriteLine("serial " + groupCollection[1]);
                var indexSrcInCurrentCultureIgnore = body.IndexOf("src", StringComparison.CurrentCultureIgnoreCase);

                if (indexSrcInCurrentCultureIgnore == -1) continue;
                i++;
                var bitmapData = Convert.FromBase64String(Regex.Split(groupCollection[1].ToString(), ",")[1]);
                var streamBitmap = new System.IO.MemoryStream(bitmapData);
                var imageToInline = new LinkedResource(streamBitmap, MediaTypeNames.Image.Jpeg)
                {
                    ContentId = "MyImage" + i
                };
                var srcToken = "cid:" + imageToInline.ContentId;
                viewCollections.Add(imageToInline);
                body = body.Replace(groupCollection[1].ToString(), srcToken);
            }

            return body;
        }
    }
}
