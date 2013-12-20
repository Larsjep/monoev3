using System;

namespace MonoBrickFirmware.Native
{
	public class WiFiDevice
	{
		
		public WiFiDevice ()
		{
				
		
		}
		
		public static bool IsLinkUp()
		{
			bool up = false;
			try {
				string output = ProcessHelper.RunAndWaitForProcessWithOutput ("ip", "link show wlan0");
				string[] outputs = output.Split (new string[] { "\r\n", "\n" }, StringSplitOptions.None);
				foreach (var s in outputs){
					if(s.ToLower().Contains("up")){
						up = true;
					}
				}
			} 
			catch{
				
			}
			return up;
		}
		
		public static string Gateway()
		{
			if(IsLinkUp())
				return ProcessHelper.RunAndWaitForProcessWithOutput ("route");
			return "";
		}
		
		public static void TurnOff()
		{
			ProcessHelper.RunAndWaitForProcess("killall","wpa_supplicant");
		}
		
		public static bool TurnOn (int timeout = 0)
		{
			if (!IsLinkUp ()) {
				if (ProcessHelper.RunAndWaitForProcess ("/lejos/bin/startwlan", "", timeout) == 0) 
				{
					return true;
				}
				TurnOff();
			}
			return false; 
		}
	}
}

