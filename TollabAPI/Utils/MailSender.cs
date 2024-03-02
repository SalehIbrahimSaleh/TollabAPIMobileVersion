using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;

namespace TollabAPI.Utils
{
    public class MailSender
    {
        //public static Task<bool> SendVerificationLinkEmail(HttpRequestMessage request, string EmailId, string ActivationCode)
        //{
        //    try
        //    {
        //        var verifyUrl = "/api/AcountVerification/VerifyAccount?verificationToken=" + ActivationCode;
        //        var Link = request.RequestUri.AbsoluteUri.Replace(request.RequestUri.PathAndQuery, verifyUrl);
        //        var FromEmail = new MailAddress(ConfigurationManager.AppSettings["ContactEmail"], "Forrerra");
        //        var ToEmail = new MailAddress(EmailId);
        //        var FromEmailPassword = ConfigurationManager.AppSettings["ContactEmailPassword"];
        //        string MySubject = "Email Confirmation";

        //        string MyBody = "welcome to Forrerra service , click this link to confirm registration , <br/> its valid through next 7 hours" +
        //                         "<a href='" + Link + "'>" + Link + "</a>";
        //        var smtp = new SmtpClient
        //        {
        //            Host = "smtp.gmail.com",
        //            Port = 587,
        //            EnableSsl = true,
        //            DeliveryMethod = SmtpDeliveryMethod.Network,
        //            //SmtpUseDefaultCredentials = true,
        //            UseDefaultCredentials = false,
        //            Credentials = new NetworkCredential(FromEmail.Address, FromEmailPassword)


        //        };

        //        var message = new MailMessage(FromEmail, ToEmail)
        //        {
        //            Subject = MySubject,
        //            Body = MyBody,
        //            IsBodyHtml = true
        //        };

        //        smtp.Send(message);
        //        return Task.Run(() => true);
        //    }
        //    catch (Exception e)
        //    {
        //        return Task.Run(() => false);
        //    }



        //}



        ////to call it from mvc controller
        //public static Task<bool> SendPasswordResitingEmail(HttpRequestBase request, string email, string code)
        //{

        //    try
        //    {
        //        var verifyUrl = "/PasswordResetter/confirmPasswordReseting?userToken=" + code;
        //        var Link = request.Url.AbsoluteUri.Replace(request.Url.PathAndQuery, verifyUrl);
        //        var FromEmail = new MailAddress(ConfigurationManager.AppSettings["ContactEmail"], "Forrerra");
        //        var ToEmail = new MailAddress(email);
        //        var FromEmailPassword = ConfigurationManager.AppSettings["ContactEmailPassword"];
        //        string MySubject = "Password Resetting";

        //        string MyBody = "welcome to Forrerra service , click this link to confirm Password Resetting <br/> " +
        //                         "<br/> <br/> <br/>  <br/>" +
        //                         "<a href='" + Link + "'>" + Link + "</a>";
        //        var smtp = new SmtpClient
        //        {
        //            Host = "smtp.gmail.com",
        //            Port = 587,
        //            EnableSsl = true,
        //            DeliveryMethod = SmtpDeliveryMethod.Network,

        //            UseDefaultCredentials = false,
        //            Credentials = new NetworkCredential(FromEmail.Address, FromEmailPassword)


        //        };

        //        var message = new MailMessage(FromEmail, ToEmail)
        //        {
        //            Subject = MySubject,
        //            Body = MyBody,
        //            IsBodyHtml = true
        //        };

        //        smtp.Send(message);
        //        return Task.Run(() => true);
        //    }
        //    catch (Exception e)
        //    {
        //        return Task.Run(() => false);
        //    }

        //}

        //to call it from web api controller



   
        public static Task<bool> SendverificationEmailBycode(HttpRequestMessage request, string email, string code)
        {
            try
            {
                 
                var FromEmail = new MailAddress(ConfigurationManager.AppSettings["ContactEmail"], "Rehla");
                var ToEmail = new MailAddress(email);
                var FromEmailPassword = ConfigurationManager.AppSettings["ContactEmailPassword"];
                string MySubject = "Password Resetting";

                string MyBody = "welcome to Rehla service , use the following token to validate your email address <br/> " +
                                 "<br/>   <br/>" +
                                 "<b>"+code+"</b>";
                var smtp = new SmtpClient
                {
                    Host = "smtp.gmail.com",
                    Port = 587,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,

                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(FromEmail.Address, FromEmailPassword)


                };

                var message = new MailMessage(FromEmail, ToEmail)
                {
                    Subject = MySubject,
                    Body = MyBody,
                    IsBodyHtml = true
                };

                smtp.Send(message);
                return Task.Run(() => true);
            }
            catch (Exception e)
            {
                return Task.Run(() => false);
            }
        }
    }
}