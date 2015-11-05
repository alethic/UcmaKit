using System;
using System.Net.Mail;
using System.Web;

namespace ISI.Rtc.VoiceXml.Web.Site.Office
{

    public class Message : IHttpHandler
    {

        public bool IsReusable
        {
            get { return false; }
        }

        public void ProcessRequest(HttpContext context)
        {
            using (var msg = new MailMessage())
            {
                msg.To.Add("office@isillc.com");
                msg.Subject = "Voice Message (" + DateTime.Now.ToString() + ")";

                // apppend caller id if available
                if (!string.IsNullOrWhiteSpace(context.Request.Form["callerid"]))
                    msg.Subject += " [" + context.Request["callerid"] + "]";

                // extract file data
                if (context.Request.Files.Count > 0)
                {
                    var f = context.Request.Files[0];
                    if (f != null)
                    {
                        var l = f.ContentLength;
                        var s = f.InputStream;
                        var n = context.Request["n"];
                        var t = context.Request["t"];

                        if (l > 0)
                        {
                            msg.Attachments.Add(new Attachment(s, n, t));

                            // send message
                            using (var smtp = new SmtpClient())
                                smtp.Send(msg);
                        }
                    }
                }
            }

            // direct to target specified by caller
            if (context.Request["r"] != null)
                context.Response.Redirect(context.Request["r"]);
        }

    }

}