using System.Runtime.InteropServices;
using System;
using System.Collections.Generic;
using MonoBrickFirmware.Native;


namespace MonoBrickFirmware.IO
{	
	/// <summary>
	/// Class for EV3 motor
	/// </summary>
	public class Motor :  MotorBase
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="MonoBrickFirmware.IO.Motor"/> class.
		/// </summary>
		/// <param name="port">Port.</param>
		public Motor(MotorPort port){
			this.BitField = MotorPortToBitfield(port);
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
		public void On(sbyte speed){
			output.Start(speed);
		}
		
		/// <summary>
		/// Move the motor to a relative position
		/// </summary>
		/// <param name='speed'>
		/// Speed of the motor -100 to 100
		/// </param>
		/// <param name='degrees'>
		/// The relative position of the motor
		/// </param>
		/// <param name='brake'>
		/// Set to <c>true</c> if the motor should brake when done
		/// </param>
		/// <param name='waitForCompletion'>
		/// Set to <c>true</c> to wait for movement to be completed before returning
		/// </param>
		public void On(sbyte speed, UInt32 degrees, bool brake, bool waitForCompletion = true){
			UInt64 longDegrees = (UInt64)degrees;
			UInt32 rampUpDownSteps =(UInt32) (15 *  longDegrees * 100)/10000;
			UInt32 constantsSteps = (UInt32) (70 *  longDegrees * 100)/10000;
			if(rampUpDownSteps > 200){//To make sure ramp up is not too long
				rampUpDownSteps = 200;
				constantsSteps = degrees - 2*rampUpDownSteps;
			}
			On(GetSpeed());
			System.Threading.Thread.Sleep (50);
			output.SetStepSpeed(speed,rampUpDownSteps,constantsSteps, rampUpDownSteps, brake);
			if(waitForCompletion)
				this.WaitForMotorToStop();
		}
		
		/// <summary>
		/// Moves the motor to an absolute position
		/// </summary>
		/// <param name='speed'>
		/// Speed of the motor 0 to 100
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
		public void MoveTo(byte speed, Int32 position, bool brake, bool waitForCompletion = true){
			Int32 currentPos = GetTachoCount();
			UInt32 diff = 0;
			sbyte motorSpeed =0;
			bool moveForward = false; 
			if(currentPos < position){
				diff =(UInt32) (position - currentPos);
				moveForward = true;
			}
			else{
				diff = (UInt32)(currentPos - position);
				moveForward = false;
			}
			if(moveForward){
				motorSpeed = (sbyte)speed;
			}
			else{
				motorSpeed = (sbyte)-speed;
			}
			this.On(motorSpeed, diff, brake, waitForCompletion);	
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
		public void SpeedProfileStep(sbyte speed, UInt32 rampUpSteps, UInt32 constantSpeedSteps, UInt32 rampDownSteps, bool brake, bool waitForCompletion = true)
		{
			On(GetSpeed());
			System.Threading.Thread.Sleep (50);
			output.SetStepSpeed(speed, rampUpSteps, constantSpeedSteps,rampDownSteps, brake);
			if(waitForCompletion)
				WaitForMotorToStop();
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
			On(GetSpeed());
			System.Threading.Thread.Sleep (50);
			output.SetTimeSpeed(speed, rampUpTimeMs, constantSpeedTimeMs, rampUpTimeMs, brake);
			if(waitForCompletion)
				WaitForMotorToStop();
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
			On(GetSpeed());
			System.Threading.Thread.Sleep (50);
			output.SetStepPower(power,rampUpSteps, constantSpeedSteps, rampDownSteps, brake);
			if(waitForCompletion)
				WaitForMotorToStop();
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
			On(GetSpeed());
			System.Threading.Thread.Sleep (50);
			output.SetTimePower(power, rampUpTimeMs,constantSpeedTimeMs,rampDownTimeMs, brake);
			if(waitForCompletion)
				WaitForMotorToStop();
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
		/// Determines whether this motor is running.
		/// </summary>
		/// <returns>
		/// <c>true</c> if this motor is running; otherwise, <c>false</c>.
		/// </returns>
	    public bool IsRunning(){
			return output.GetSpeed(this.PortList[0])!= 0;				
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