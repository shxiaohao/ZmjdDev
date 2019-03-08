using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HJD.AccessServiceTask.Helper
{
    public class MailHelper
    {
        public static bool SendMail(string from, string toStr, string subject, string body, bool isHtml)
        {
            var toList = new List<string>();
            if (!string.IsNullOrEmpty(toStr))
            {
                toList = Regex.Split(toStr, ",").ToList();
            }
            return SendMail(from, toList, subject, body, isHtml, new List<string>());
        }

        public static bool SendMail(string from, List<string> toList, string subject, string body, bool isHtml)
        {
            return SendMail(from, toList, subject, body, isHtml, new List<string>());
        }

        public static bool SendMail(string from, string toStr, string subject, string body, bool isHtml, List<string> files)
        {
            var toList = new List<string>();
            if (!string.IsNullOrEmpty(toStr))
            {
                toList = Regex.Split(toStr, ",").ToList();
            }
            return SendMail(from, toList, subject, body, isHtml, files);
        }

        public static bool SendMail(string from, List<string> toList, string subject, string body, bool isHtml, List<string> files)
        {
            try
            {
                //
                var bSendAsHtml = isHtml;

                //
                var vSmtpAddress = "smtp.ym.163.com";
                var vSendMailUserName = "haoy@zmjiudian.com";
                var vSendMailPassword = "zmjiudian01!";
                var smtp = new SmtpClient(vSmtpAddress) { Credentials = new System.Net.NetworkCredential(vSendMailUserName, vSendMailPassword) };
                var mes = new MailMessage { From = new MailAddress(from) };

                //To User
                foreach (var item in toList)
                {
                    mes.To.Add(item);
                }

                //增加附件
                if (files != null)
                {
                    foreach (var item in files)
                    {
                        Attachment mailAttach = new Attachment(item);
                        mes.Attachments.Add(mailAttach);
                    }
                }

                mes.Subject = subject;
                mes.IsBodyHtml = bSendAsHtml;
                mes.Body = body;

                smtp.Send(mes);
                mes.Attachments.Dispose();

                Console.WriteLine("Send Mail OK!");
                //Log("Send Mail OK!");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Send Mail Error:" + ex.Message);
                //Log("Send Mail Error:" + ex.Message);

                return false;
            }

            return true;
        }
    }
}
