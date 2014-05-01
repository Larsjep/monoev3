namespace MonoBrickWebServer
{
	using System;
	using System.Web;
	using System.Web.UI;
	using System.Web.UI.WebControls; 
	using MonoBrickFirmware.Sensors;
	using MonoBrickFirmware.Movement;

	public partial class Default : System.Web.UI.Page
	{
		private static ISensor[] sensorArray = {null, null, null, null};
		private static SensorListner listner = new SensorListner(1000);
		private static object sensorArrayLock = new object();
		private static sbyte speed = 0;
		private Label[] typeLabel = new Label[4];
		private Label[] modeLabel = new Label[4];
		private Label[] valueLabel = new Label[4];
		private Label[] tachoLabel = new Label[4];
		private const sbyte increaseSpeed = 10;
		private Motor[] motorArray = {new Motor(MotorPort.OutA), new Motor(MotorPort.OutB), new Motor(MotorPort.OutC), new Motor(MotorPort.OutD)};
		
		protected override void OnLoad (EventArgs e)
		{
			base.OnLoad (e);
			typeLabel[0] = Sensor1TypeText;
			typeLabel[1] = Sensor2TypeText; 
			typeLabel[2] = Sensor3TypeText;
			typeLabel[3] = Sensor4TypeText;
			
			modeLabel[0] = Sensor1ModeText;
			modeLabel[1] = Sensor2ModeText; 
			modeLabel[2] = Sensor3ModeText;
			modeLabel[3] = Sensor4ModeText;
			
			valueLabel[0] = Sensor1ValueText;
			valueLabel[1] = Sensor2ValueText; 
			valueLabel[2] = Sensor3ValueText;
			valueLabel[3] = Sensor4ValueText;
			
			tachoLabel[0] = Motor1TachoText;
			tachoLabel[1] = Motor2TachoText;
			tachoLabel[2] = Motor3TachoText;
			tachoLabel[3] = Motor4TachoText;
			
			
			if (!listner.IsListning) {
				lock (sensorArrayLock) 
				{
					listner.SensorAttached += OnSensorAttached;
					listner.SensorDetached += OnSensorDetached;
					listner.Start();
				}
			}
		}
		
		private static void OnSensorAttached (ISensor sensor)
		{
			lock (sensorArrayLock) {
				if (sensor != null) {
					sensorArray [(int)sensor.Port] = sensor;
				}
			}
		}
		
		private static void OnSensorDetached (SensorPort port)
		{
			lock (sensorArrayLock) 
			{
				sensorArray [(int)port] = null;
			}
		}
		
		private int SensorStringToIdx (string sensorNameIdx)
		{
			int idx = -1;
			switch (sensorNameIdx) {
				case "Sensor1":
	                idx = 0;
					break;
				case "Sensor2":
					idx = 1;
					break;
				case "Sensor3":
	                idx = 2;
					break;
				case "Sensor4":
					idx = 3;
					break;
			}
			return idx;	
		}
		
		private int MotorStringToIdx (string motorNameIdx)
		{
			int idx = -1;
			switch (motorNameIdx) {
				case "Motor1":
	                idx = 0;
					break;
				case "Motor2":
					idx = 1;
					break;
				case "Motor3":
	                idx = 2;
					break;
				case "Motor4":
					idx = 3;
					break;
			}
			return idx;	
		}
		
		public void FwdClicked (object sender, EventArgs args)
		{
			int idx = MotorStringToIdx (((Button)sender).CommandName);
			if (motorArray [idx] != null) {
				if (speed < 100) {
					speed = (sbyte)(speed + increaseSpeed);
				}
				if (speed == 0) 
				{
					motorArray [idx].Off ();
				}
				else 
				{
					motorArray[idx].On(speed);	
				}
			}
		}
		
		public void MoveToClicked (object sender, EventArgs args)
		{
			int idx = MotorStringToIdx (((Button)sender).CommandName);
			if (motorArray [idx] != null) {	
				int position;
				if (int.TryParse (Motor1PositionText.Text, out position)) 
				{
					motorArray[idx].MoveTo(50, position,true, false);
				}
				else
				{
					Console.WriteLine("Not a valid position");
				}
			}
			
		
		}
		
		public void ResetTachoClicked (object sender, EventArgs args)
		{
			int idx = MotorStringToIdx (((Button)sender).CommandName);
			if (motorArray [idx] != null) {	
				motorArray[idx].ResetTacho();
			}
		}
		
		
		
		public void RevClicked (object sender, EventArgs args)
		{
			int idx = MotorStringToIdx (((Button)sender).CommandName);
			if (motorArray [idx] != null) {
				if (speed > -100) {
					speed = (sbyte)(speed - increaseSpeed);
				}
				if (speed == 0) 
				{
					motorArray [idx].Off ();
				}
				else 
				{
					motorArray[idx].On(speed);	
				}
			}	
		}
		
		public void OffClicked (object sender, EventArgs args)
		{	
			int idx = MotorStringToIdx (((Button)sender).CommandName);
			if (motorArray [idx] != null) 
			{
				motorArray [idx].On(0);
				motorArray [idx].Off();
				speed = 0;
			}	
		}
		
		public void BrakeClicked (object sender, EventArgs args)
		{
			int idx = MotorStringToIdx (((Button)sender).CommandName);
			if (motorArray [idx] != null) 
			{
				motorArray [idx].On(0);
				motorArray [idx].Brake();
				speed = 0;
			}			
		}
		
		
		
		public void NextClicked (object sender, EventArgs args)
		{
			lock (sensorArrayLock) {
				int idx = SensorStringToIdx (((Button)sender).CommandName);
				if (sensorArray [idx] != null) {
					sensorArray [idx].SelectNextMode ();
				}
			}
			UpdateSensorTable(null,null);
		}
		
		public void PrevClicked (object sender, EventArgs args)
		{
			lock (sensorArrayLock) {
				int idx = SensorStringToIdx(((Button)sender).CommandName);
				if(sensorArray[idx] != null)
					sensorArray[idx].SelectPreviousMode();
			}
			UpdateSensorTable(null,null);
		}
		
		
		
		protected void UpdateSensorTable (object sender, EventArgs e)
		{
			
			lock (sensorArrayLock) {
				
				for (int i = 0; i < sensorArray.Length; i++) {
					if (sensorArray [i] != null) {
						typeLabel [i].Text = sensorArray [i].GetSensorName ();
						modeLabel [i].Text = sensorArray [i].SelectedMode ();
						valueLabel [i].Text = sensorArray [i].ReadAsString ();
					} else {
						typeLabel [i].Text = "Not connected";
						modeLabel [i].Text = "-";
						valueLabel [i].Text = "-";
					}
				}
				
			}
			for (int i = 0; i < motorArray.Length; i++) {
				
				if (motorArray [i] != null) 
				{
					tachoLabel[i].Text = motorArray[i].GetTachoCount().ToString();
				}
				else 
				{
					tachoLabel[i].Text  = "-";
				}
			}
			
	    }
		
	}
}

