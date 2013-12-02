using System;
using MonoBrickFirmware.IO;
using MonoBrickFirmware.Graphics;
using System.Threading;
using System.Net; using System.Net.Mail; 
namespace MailExample
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			const string to = "anders@soborg.net";
			const string from = "anders.soborg@gmail.com";
			const string password = "builttospill";
			
			ManualResetEvent terminateProgram = new ManualResetEvent(false);
			var colorSensor = new ColorSensor(SensorPort.In1);
			ButtonEvents buts = new ButtonEvents ();
			SmtpClient smptpClient = new SmtpClient("smtp.gmail.com", 587); 			smptpClient.EnableSsl = true; 			smptpClient.UseDefaultCredentials = false; 			smptpClient.Credentials = new NetworkCredential(from, password); 			MailMessage message = new MailMessage(); 			message.To.Add(to); 			message.From = new MailAddress(from); 			message.Subject = "Color message from EV3"; 			message.Body = "EV3 test"; 			LcdConsole.Clear();
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
