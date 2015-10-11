using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using MonoBrickFirmware.Display.Menus;
using MonoBrickFirmware.Display.Dialogs;

namespace EmailClient
{
	class MainClass
	{
		private static string message = "";
		private static string emailTo = "anders@soborg.net";
		private static string subject = "Email from EV3";
		private static StepContainer SendStep = new StepContainer (SendEmail, "Sending mail", "Failed to send mail", "Mail sent successfully");
		private static EmailSettings settings;
		public static void Main (string[] args)
		{
			settings = new EmailSettings ();
			settings.Load ();
			Menu settingMenu = new Menu ("Settings");
			settingMenu.AddItem (new ItemWithCharacterInput ("User Name", "User Name", settings.UserSettings.User, OnUserChanged, false, true, false));
			settingMenu.AddItem (new ItemWithCharacterInput ("SMTP", "SMTP Server", settings.UserSettings.Smtp, OnSMTPChanged, false, true, false));
			settingMenu.AddItem (new ItemWithCharacterInput ("Password", "SMTP Password", settings.UserSettings.Password, OnPasswordChanged, true, true, false));
			settingMenu.AddItem (new ItemWithNumericInput("Port Number", settings.UserSettings.Port, OnPortNmberChanged, 1, 49151));
			settingMenu.AddItem (new ItemWithCheckBox("Enable SSL", settings.UserSettings.EnableSsl, OnEnableSslChanged));

			Menu menu = new Menu ("E-Mail Client");
			menu.AddItem (settingMenu);
			menu.AddItem (new ItemWithCharacterInput ("To", "To", emailTo, (emailAddress) => emailTo = emailAddress, false, true, false ));
			menu.AddItem (new ItemWithCharacterInput ("Subject", "Subject", subject, (newSubject) => subject = newSubject, false, true, false ));
			menu.AddItem (new ItemWithCharacterInput ("Message", "Message", "", (newMessage) => message = newMessage));
			menu.AddItem (new ItemWithDialog<StepDialog>(new StepDialog( "Sending mail ", new List<IStep>{SendStep}), "Send mail")); 
			MenuContainer container = new MenuContainer (menu);
			container.Show (true);
		}
		
		private static bool SendEmail()
		{
			SmtpClient smptpClient = new SmtpClient(settings.UserSettings.Smtp, settings.UserSettings.Port);
			smptpClient.EnableSsl = settings.UserSettings.EnableSsl;
			smptpClient.UseDefaultCredentials = false;
			smptpClient.Credentials = new NetworkCredential(settings.UserSettings.User, settings.UserSettings.Password);
			smptpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
			ServicePointManager.ServerCertificateValidationCallback = 
				delegate(object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) 
			{ return true; };
			MailMessage mailMessage = new MailMessage();
			mailMessage.To.Add(emailTo);
			mailMessage.From = new MailAddress(settings.UserSettings.User);
			mailMessage.Subject = subject;
			mailMessage.Body = message;
			try
			{
				smptpClient.Send(mailMessage);
			}
			catch(Exception e)
			{
				Console.WriteLine (e.Message);
				return false;
			}
			return true;
		}
		
		private static void OnPasswordChanged(string password)
		{
			settings.UserSettings.Smtp = password;
			settings.Save ();
		}

		private static void OnUserChanged(string user)
		{
			settings.UserSettings.User = user;
			settings.Save ();
		}

		private static void OnSMTPChanged(string smtpServer)
		{
			settings.UserSettings.Smtp = smtpServer;
			settings.Save ();						
		}

		private static void OnPortNmberChanged(int portNumner)
		{
			settings.UserSettings.Port = portNumner;
			settings.Save ();
		}

		private static void OnEnableSslChanged(bool enable)
		{
			settings.UserSettings.EnableSsl = enable;
			settings.Save ();
		}
		
	}
}
