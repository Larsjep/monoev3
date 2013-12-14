using System;
using MonoBrickFirmware.Display;
using MonoBrickFirmware.Buttons;
using MonoBrickFirmware.Sensors;
using System.Threading;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;

//Do the following from a ssh connection before running this code
//mozroots --import --ask-remove
//certmgr -ssl smtps://smtp.gmail.com:465
		
namespace GmailExample
{
	
	class MainClass
	{	
		public static void Main (string[] args)
		{
			const string to = "user@mail.dk";
			const string from = "name.surname@gmail.com";
			const string password = "YourPassword";
			
			ManualResetEvent terminateProgram = new ManualResetEvent(false);
			var colorSensor = new ColorSensor(SensorPort.In1);
			ButtonEvents buts = new ButtonEvents ();
			SmtpClient smptpClient = new SmtpClient("smtp.gmail.com", 587);
			smptpClient.EnableSsl = true;
			smptpClient.UseDefaultCredentials = false;
			smptpClient.Credentials = new NetworkCredential(from, password);
			smptpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
			ServicePointManager.ServerCertificateValidationCallback = 
                delegate(object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) 
                    { return true; };
			MailMessage message = new MailMessage();
			message.To.Add(to);
			message.From = new MailAddress(from);
			message.Subject = "Color mail from my EV3";
			LcdConsole.Clear();
			buts.EscapePressed += () => { 
				terminateProgram.Set();
			};
			buts.EnterPressed += () => { 
				LcdConsole.WriteLine("Sending email");
				try{
					message.Body = "EV3 read color: " + colorSensor.ReadColor();
					smptpClient.Send(message);
					LcdConsole.WriteLine("Done sending email");
				}
				catch(Exception e)
				{
					LcdConsole.WriteLine("Failed to send email");
					Console.WriteLine(e.StackTrace);
				}
			};
			terminateProgram.WaitOne();
			message = null;
		}
	}
}
