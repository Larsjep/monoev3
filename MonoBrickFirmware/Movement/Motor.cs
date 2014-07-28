using System.Runtime.InteropServices;
using System;
using System.Collections.Generic;
using MonoBrickFirmware.Native;


namespace MonoBrickFirmware.Movement
{	
	/// <summary>
	/// Class for EV3 motor
	/// </summary>
	public class Motor :  MotorBase
	{
		private PositionPID controller = null;
		private const float standardPValue = 0.1f;
		private const float standardIValue = 80.1f;
		private const float standardDValue = 1.05f;
		private const float controllerSampleTime = 40; 


		/// <summary>
		/// Initializes a new instance of the <see cref="MonoBrickFirmware.IO.Motor"/> class.
		/// </summary>
		/// <param name="port">Port.</param>
		public Motor(MotorPort port) : base()
		{
			this.BitField = MotorPortToBitfield(port);
			Reverse = false;
			controller = new PositionPID (port, 0, false, 100, standardPValue, standardIValue, standardIValue, controllerSampleTime);
		}

		/// <summary>
		/// The reverse value
		/// </summary>
		protected bool reverse = false;
		
		/// <summary>
		/// Gets or sets a value indicating whether motor runs in reverse direction
		/// </summary>
		/// <value>
		/// <c>true</c> if reverse; otherwise, <c>false</c>.
		/// </value>
		public bool Reverse {
			get {
				return reverse;
			} 
			set {
				reverse = value;
				if(reverse){
					output.SetPolarity(Polarity.Backward);
				}
				else{
					output.SetPolarity(Polarity.Forward);
				}
			}
		}
		
		/// <summary>
		/// Move the motor
		/// </summary>
		/// <param name='speed'>
		/// Speed of the motor -100 to 100
		/// </param>
		public void SetSpeed(sbyte speed){
			controller.Cancel ();
			CancelPolling();
			output.Start(speed);
		}
		
		/// <summary>
		/// Moves the motor to an absolute position
		/// </summary>
		/// <param name='maxPower'>
		/// maxPower of the motor 0 to 100
		/// </param>
		/// <param name='position'>
		/// Absolute position
		/// </param>
		/// <param name='brake'>
		/// Set to <c>true</c> if the motor should brake when done
		/// </param>
		/// <param name='waitForCompletion'>
		/// Set to <c>true</c> to wait for movement to be completed before returning
		/// </param>
		public void MoveTo (sbyte maxPower, Int32 position, bool brake, bool waitForCompletion = true)
		{
			MoveTo (maxPower, position, brake, standardPValue, standardIValue, standardDValue, waitForCompletion);
		}

		public void  MoveTo (sbyte maxPower, Int32 position, bool brake, float P, float I, float D, bool waitForCompletion = true)
		{
			CancelPolling();
			controller.Cancel ();
			controller = new PositionPID (PortList [0], position, brake, maxPower, P, I, D, controllerSampleTime);
			controller.Run (waitForCompletion);
		}

		/// <summary>
		/// Create a speed profile where ramp up and down is specified in steps
		/// </summary>
		/// <param name="speed">Maximum speed of the motor.</param>
		/// <param name="rampUpSteps">Ramp up steps.</param>
		/// <param name="constantSpeedSteps">Constant speed steps.</param>
		/// <param name="rampDownSteps">Ramp down steps.</param>
		/// <param name="brake">If set to <c>true</c> the motor will brake when movement is done.</param>
		/// <param name='waitForCompletion'>
		/// Set to <c>true</c> to wait for movement to be completed before returning
		/// </param>
		public void SpeedProfileStep (sbyte speed, UInt32 rampUpSteps, UInt32 constantSpeedSteps, UInt32 rampDownSteps, bool brake, bool waitForCompletion = true)
		{
			controller.Cancel ();
			if(!waitForCompletion)
				CancelPolling();
			output.SetPower (0);
			output.SetStepSpeed (speed, rampUpSteps, constantSpeedSteps, rampDownSteps, brake);
			if (waitForCompletion) 
				WaitForMotorsToStartAndStop();
			
		}
		
		/// <summary>
		/// Create a speed profile where ramp up and down is specified in time
		/// </summary>
		/// <param name="speed">Maximum speed of the motor.</param>
		/// <param name="rampUpTimeMs">Ramp up time ms.</param>
		/// <param name="constantSpeedTimeMs">Constant speed time ms.</param>
		/// <param name="rampDownTimeMs">Ramp down time ms.</param>
		/// <param name="brake">If set to <c>true</c> the motor will brake when movement is done.</param>
		/// <param name='waitForCompletion'>
		/// Set to <c>true</c> to wait for movement to be completed before returning
		/// </param>
		public void SpeedProfileTime(sbyte speed, UInt32 rampUpTimeMs, UInt32 constantSpeedTimeMs, UInt32 rampDownTimeMs, bool brake, bool waitForCompletion = true)
		{
			controller.Cancel ();
			if(!waitForCompletion)
				CancelPolling();
			output.SetPower (0);
			output.SetTimeSpeed(speed, rampUpTimeMs, constantSpeedTimeMs, rampUpTimeMs, brake);
			if(waitForCompletion)
				WaitForMotorsToStartAndStop();
				
			
		}
		
		/// <summary>
		/// Create a power profile where ramp up and down is specified in steps
		/// </summary>
		/// <param name="power">Maximum power of the motor.</param>
		/// <param name="rampUpSteps">Ramp up steps.</param>
		/// <param name="constantSpeedSteps">Constant speed steps.</param>
		/// <param name="rampDownSteps">Ramp down steps.</param>
		/// <param name="brake">If set to <c>true</c> the motor will brake when movement is done.</param>
		/// <param name='waitForCompletion'>
		/// Set to <c>true</c> to wait for movement to be completed before returning
		/// </param>
		public void PowerProfileStep(sbyte power, UInt32 rampUpSteps, UInt32 constantSpeedSteps, UInt32 rampDownSteps, bool brake, bool waitForCompletion = true)
		{
			controller.Cancel ();
			if(!waitForCompletion)
				CancelPolling();
			output.SetPower (0);
			output.SetStepPower(power,rampUpSteps, constantSpeedSteps, rampDownSteps, brake);
			if(waitForCompletion)
				WaitForMotorsToStartAndStop();
		}
		
		/// <summary>
		/// Create a power profile where ramp up and down is specified in time
		/// </summary>
		/// <param name="power">Maximum power of the motor.</param>
		/// <param name="rampUpTimeMs">Ramp up time ms.</param>
		/// <param name="constantSpeedTimeMs">Constant speed time ms.</param>
		/// <param name="rampDownTimeMs">Ramp down time ms.</param>
		/// <param name="brake">If set to <c>true</c> the motor will brake when movement is done.</param>
		public void PowerProfileTime (byte power, UInt32 rampUpTimeMs, UInt32 constantSpeedTimeMs, UInt32 rampDownTimeMs, bool brake, bool waitForCompletion = true)
		{
			controller.Cancel();
			if(!waitForCompletion)
				CancelPolling();
			output.SetPower (0);
			output.SetTimePower(power, rampUpTimeMs,constantSpeedTimeMs,rampDownTimeMs, brake);
			if(waitForCompletion)
				WaitForMotorsToStartAndStop();
			
		}

		public override void Brake ()
		{
			controller.Cancel();
			base.Brake ();
		}

		public override void Off ()
		{
			controller.Cancel();
			base.Off ();
		}

		public override void SetPower (sbyte power)
		{
			controller.Cancel();
			base.SetPower (power);
		}

		/// <summary>
		/// Resets the tacho
		/// </summary>
		public void ResetTacho(){
			output.ClearCount();
		}
	
		/// <summary>
		/// Gets the tacho count.
		/// </summary>
		/// <returns>
		/// The tacho count
		/// </returns>
	    public Int32 GetTachoCount(){
			return output.GetCount(this.PortList[0]);
		}
		
		/// <summary>
		/// Gets the speed of the motor
		/// </summary>
		/// <returns>The speed.</returns>
		public sbyte GetSpeed ()
		{
			return output.GetSpeed(this.PortList[0]);
		}
	}
}