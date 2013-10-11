using System.Runtime.InteropServices;
using System;
using System.Collections.Generic;
using MonoBrickFirmware.Native;


namespace MonoBrickFirmware.IO
{	
	/// <summary>
	/// Output bitfields
	/// </summary>
	[Flags]
	public enum OutputBitfield  {
		#pragma warning disable
		OutA = 0x01, OutB = 0x02, OutC = 0x04, OutD = 0x08
		#pragma warning restore
	};
	
	/// <summary>
	/// Motor ports
	/// </summary>
	public enum MotorPort{
		#pragma warning disable
		OutA = 0x00, OutB = 0x01, OutC = 0x02, OutD = 0x03
		#pragma warning restore
	};
	
	
	/// <summary>
	/// Polarity of the motor
	/// </summary>
	public enum Polarity{
		#pragma warning disable
		Backward = -1, Forward = 1, OppositeDirection = 0
		#pragma warning restore
	};


	/// <summary>
	/// Class for controlling the output port
	/// </summary>
	internal class Output
	{
		private UnixDevice pwmDevice;
		private UnixDevice tachoDevice;
		private MemoryArea tachoMemory;
		public Output ()
		{
			
			pwmDevice = new UnixDevice("/dev/lms_pwm");
			tachoDevice = new UnixDevice("/dev/lms_motor");
			tachoMemory = tachoDevice.MMap(96,0);
			this.BitField = OutputBitfield.OutA;
		}
		
		
		/// <summary>
		/// Gets or sets the output bit field.
		/// </summary>
		/// <value>The bit field.</value>
		public OutputBitfield BitField{get;set;}
		
		/// <summary>
		/// Reset the output
		/// </summary>
		public void Reset(){
			var command = new DeviceCommand();
			command.Append(ByteCodes.OutputReset);
			command.Append(BitField);
			pwmDevice.Write(command.Data);
		}
		
		/// <summary>
		/// Stop the output
		/// </summary>
		/// <param name="brake">If set to <c>true</c> the motor will brake and not coast</param>
		public void Stop(bool brake){
			var command = new DeviceCommand();
			command.Append(ByteCodes.OutputStop);
			command.Append(BitField);
			byte b  = 0;
			if(brake){
				b = 1;
			}
			command.Append(b);
			pwmDevice.Write(command.Data);
		}
		
		/// <summary>
		/// Sets the speed.
		/// </summary>
		/// <param name="speed">Speed.</param>
		public void SetSpeed(sbyte speed){
			var command = new DeviceCommand();
			command.Append(ByteCodes.OutputSpeed);
			command.Append(BitField);
			command.Append(speed);
			pwmDevice.Write(command.Data);
		}
		
		/// <summary>
		/// Sets the power.
		/// </summary>
		/// <param name="power">Power.</param>
		public void SetPower(byte power){
			var command = new DeviceCommand();
			command.Append(ByteCodes.OutputPower);
			command.Append(BitField);
			command.Append(power);
			pwmDevice.Write(command.Data);
		}
		
		/// <summary>
		/// Sets the absolute position from last reset
		/// </summary>
		/// <param name="position">Position to use</param>
		public void SetPosition(Int32 position){
			var command = new DeviceCommand();
			command.Append(ByteCodes.OutputPosition);
			command.Append(BitField);
			command.Append(position);
			pwmDevice.Write(command.Data);
		}
		
		/// <summary>
		/// Start
		/// </summary>
		public void Start(){
			var command = new DeviceCommand();
			command.Append(ByteCodes.OutputStart);
			command.Append(BitField);
			pwmDevice.Write(command.Data);
		}
		
		/// <summary>
		/// Start with the specified speed
		/// </summary>
		/// <param name="speed">Speed.</param>
		public void Start(sbyte speed){
			SetSpeed(speed);
			Start();
		}
		
		/// <summary>
		/// Sets the polarity.
		/// </summary>
		/// <param name="polarity">Polarity of the output</param>
		public void SetPolarity(Polarity polarity){
			var command = new DeviceCommand();
			command.Append(ByteCodes.OutputPolarity);
			command.Append(BitField);
			command.Append((sbyte) polarity);
			pwmDevice.Write(command.Data);
		}
		
		/// <summary>
		/// Set Ramp up, constant and rampdown steps and power of the outputs
		/// </summary>
		/// <param name="power">Power to use</param>
		/// <param name="rampUpSteps">Steps used to ramp up</param>
		/// <param name="constantSpeedSteps">Steps used for constant speed</param>
		/// <param name="rampDownSteps">Steps used to ramp down</param>
		/// <param name="brake">If set to <c>true</c> brake when done.</param>
		public void SetStepPower(sbyte power, UInt32 rampUpSteps, UInt32 constantSpeedSteps, UInt32 rampDownSteps, bool brake){
			WriteProfile(ByteCodes.OutputStepPower, power, rampUpSteps, constantSpeedSteps, rampDownSteps, brake);
		}
		
		/// <summary>
		/// Set Ramp up, constant and rampdown time and power of the outputs
		/// </summary>
		/// <param name="power">Power to use</param>
		/// <param name="rampUpTime">Time in ms to ramp up</param>
		/// <param name="constantSpeedTime">Time in ms for constant speed</param>
		/// <param name="rampDownTime">Time in ms to ramp down</param>
		/// <param name="brake">If set to <c>true</c> brake when done.</param>
		public void SetTimePower(byte power, UInt32 rampUpTime, UInt32 constantSpeedTime, UInt32 rampDownTime, bool brake){
			WriteProfile(ByteCodes.OutputTimePower,(sbyte) power, rampUpTime, constantSpeedTime, rampDownTime, brake);
		}
		
		/// <summary>
		/// Set Ramp up, constant and rampdown steps and speed of the outputs
		/// </summary>
		/// <param name="speed">Speed to use</param>
		/// <param name="rampUpSteps">Steps used to ramp up</param>
		/// <param name="constantSpeedSteps">Steps used for constant speed</param>
		/// <param name="rampDownSteps">Steps used to ramp down</param>
		/// <param name="brake">If set to <c>true</c> brake when done.</param>
		public void SetStepSpeed(sbyte speed, UInt32 rampUpSteps, UInt32 constantSpeedSteps, UInt32 rampDownSteps, bool brake){
			WriteProfile(ByteCodes.OutputStepSpeed, speed, rampUpSteps, constantSpeedSteps, rampDownSteps, brake);
		}
		
		/// <summary>
		/// Set Ramp up, constant and rampdown time and power of the outputs
		/// </summary>
		/// <param name="speed">Speed to use</param>
		/// <param name="rampUpTime">Time in ms to ramp up</param>
		/// <param name="constantSpeedTime">Time in ms for constant speed</param>
		/// <param name="rampDownTime">Time in ms to ramp down</param>
		/// <param name="brake">If set to <c>true</c> brake when done</param>
		public void SetTimeSpeed(sbyte speed, UInt32 rampUpTime, UInt32 constantSpeedTime, UInt32 rampDownTime, bool brake){
			WriteProfile(ByteCodes.OutputTimeSpeed, speed, rampUpTime, constantSpeedTime, rampDownTime, brake);
		}
		
		/// <summary>
		/// Sync steps between two motors
		/// </summary>
		/// <param name="speed">Speed.</param>
		/// <param name="turnRatio">Turn ratio between two syncronized motors</param>
		/// <param name="steps">Steps in degrees</param>
		/// <param name="brake">If set to <c>true</c> brake.</param>
		public void SetStepSync(sbyte speed, Int32 turnRatio, UInt32 steps, bool brake){
			var command = new DeviceCommand();
			command.Append(ByteCodes.OutputStepSync);
			command.Append(BitField);
			command.Append(speed);
			command.Append((byte)0x00);//Align
			command.Append(turnRatio);
			command.Append(steps);
			byte b = 0;//coast
			if(brake)
				b = 1;
			command.Append(b);
			pwmDevice.Write(command.Data);
		}
		
		/// <summary>
		/// Time sync between two motors
		/// </summary>
		/// <param name="speed">Speed.</param>
		/// <param name="turnRatio">Turn ratio between two syncronized motors</param>
		/// <param name="timeInMs">Time in ms</param>
		/// <param name="brake">If set to <c>true</c> brake.</param>
		public void SetTimeSync(sbyte speed, Int32 turnRatio, UInt32 timeInMs, bool brake){
			var command = new DeviceCommand();
			command.Append(ByteCodes.OutputTimeSync);
			command.Append(BitField);
			command.Append(speed);
			command.Append((byte)0x00);//Align
			command.Append(turnRatio);
			command.Append(timeInMs);
			byte b = 0;//coast
			if(brake)
				b = 1;
			command.Append(b);
			pwmDevice.Write(command.Data);
		}
		
		/// <summary>
		/// Wait for output ready (wait for completion)
		/// </summary>
		public void WaitForReady(){
			throw new NotSupportedException ();
		}
		
		/// <summary>
		/// Testing if output is used 
		/// </summary>
		/// <returns><c>true</c> if this instance is ready; otherwise, <c>false</c>.</returns>
		public bool IsReady(){
			throw new NotSupportedException ();
		}
		
		/// <summary>
		/// Clearing tacho count
		/// </summary>
		public void ClearCount(){
			var command =  new DeviceCommand();
			command.Append(ByteCodes.OutputClrCount);
			command.Append(BitField);
			pwmDevice.Write(command.Data);
		}
		
		/// <summary>
		/// Gets the tacho count.
		/// </summary>
		/// <returns>The tacho count.</returns>
		/// <param name="port">Motor port to read from</param>
		public Int32 GetCount (MotorPort port)
		{
			byte[] data = tachoMemory.Read (0, 48);
			var reply = new DeviceReply (data);
			int index = (int)port * 12 + 8;
			return reply.GetInt32(index);
		}
		
		/// <summary>
		/// Gets the speed of the motor
		/// </summary>
		/// <returns>The speed.</returns>
		/// <param name="port">Motor port to read</param>
		public sbyte GetSpeed(MotorPort port){
			byte[] data = tachoMemory.Read (0, 48);
			var reply = new DeviceReply (data);
			int index = (int)port * 12 + 4;
			return reply.GetSbyte(index);
		}
		
		private void WriteProfile (ByteCodes code, sbyte speedOrPower, UInt32 rampUp, UInt32 constant, UInt32 rampDown, bool brake)
		{
			var command = new DeviceCommand();
			command.Append(code);
			command.Append(BitField);
			command.Append(speedOrPower);
			command.Append((byte)0x00);//Align
			command.Append(rampUp);
			command.Append(constant);
			command.Append(rampDown);
			byte b = 0;//coast
			if(brake)
				b = 1;
			command.Append(b);
			pwmDevice.Write(command.Data);	
		}
		
		
	}
	
	/// <summary>
	/// Base class for EV3 motors 
	/// </summary>
	public class MotorBase{
		
		/// <summary>
		/// The output.
		/// </summary>
		internal Output output = new Output();
		
		/// <summary>
		/// Gets or sets the motor port this is set by the bitfield. 
		/// Do not set this use the bitfield property instead
		/// </summary>
		/// <value>The port.</value>
		protected List<MotorPort> PortList {get; private set;}
		
		/// <summary>
		/// Gets or sets the bit field.
		/// </summary>
		/// <value>The bit field.</value>
		protected OutputBitfield BitField {	
			get{return output.BitField;} 
			set{
				// Check if only one motor is set
				// Only if outA or outB or outC or outD is set
				output.BitField = value;
				PortList = new List<MotorPort>();
				if( ( (value & OutputBitfield.OutA) == OutputBitfield.OutA) || 
					( (value & OutputBitfield.OutB) == OutputBitfield.OutB) || 
					( (value & OutputBitfield.OutC) == OutputBitfield.OutC) || 
					( (value & OutputBitfield.OutD) == OutputBitfield.OutD)){
						if((value & OutputBitfield.OutA) == OutputBitfield.OutA){
							PortList.Add(MotorPort.OutA);
						}
						if((value & OutputBitfield.OutB) == OutputBitfield.OutB){
							PortList.Add(MotorPort.OutB);
						}
						if((value & OutputBitfield.OutC) == OutputBitfield.OutC){
							PortList.Add(MotorPort.OutC);
						}
						if((value & OutputBitfield.OutD) == OutputBitfield.OutD){
							PortList.Add(MotorPort.OutD);
						}
					}
				else{
					//more than one motor is set to run take one of them and use as motor port
					if(Convert.ToBoolean(value & OutputBitfield.OutA)){
						PortList.Add(MotorPort.OutA);
					}
					if(Convert.ToBoolean(value & OutputBitfield.OutB)){
						PortList.Add(MotorPort.OutB);
					}
					if(Convert.ToBoolean(value & OutputBitfield.OutC)){
						PortList.Add(MotorPort.OutC);
					}
					if(Convert.ToBoolean(value & OutputBitfield.OutA)){
						PortList.Add(MotorPort.OutD);
					}		
				}	
			}
		}
		
		/// <summary>
		/// Convert a motor port to bitfield.
		/// </summary>
		/// <returns>The port to bitfield.</returns>
		internal OutputBitfield MotorPortToBitfield(MotorPort port){
			if(port == MotorPort.OutA)
				return OutputBitfield.OutA;
			if(port == MotorPort.OutB)
				return OutputBitfield.OutB;
			if(port == MotorPort.OutC)
				return OutputBitfield.OutC;
			return OutputBitfield.OutD;
			
		}
		
		/// <summary>
		/// Brake the motor (is still on but does not move)
		/// </summary>
		public void Brake(){
			output.Stop(true);
		}
		
		/// <summary>
		/// Turn the motor off
		/// </summary>
		public void Off(){
			output.Stop (false);
		}
		
		/// <summary>
		/// Sets the power of the motor.
		/// </summary>
		/// <param name="power">Power to use.</param>
		public void SetPower(byte power){
			output.SetPower(power);
		}
		
		
		protected virtual void WaitForMotorToStop ()
		{
			System.Threading.Thread.Sleep(300);
			do{
				System.Threading.Thread.Sleep(50);
			}while(output.GetSpeed(this.PortList[0])!= 0);
		}
	
	}
	
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
			if(rampUpDownSteps > 300){//To make sure ramp up is not too long
				rampUpDownSteps = 300;
				constantsSteps = degrees - 2*rampUpDownSteps;
			}
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
	
	/// <summary>
	/// Class for synchronizing two motors
	/// </summary>
	public class MotorSync : MotorBase{
		
		/// <summary>
		/// Initializes a new instance of the <see cref="MonoBrickFirmware.IO.MotorSync"/> class.
		/// </summary>
		/// <param name="bitfield">Bitfield.</param>
		public MotorSync (OutputBitfield bitfield)
		{
			this.BitField = bitfield;
		}
		
		/// <summary>
		/// Initializes a new instance of the <see cref="MonoBrickFirmware.IO.MotorSync"/> class.
		/// </summary>
		/// <param name="port1">Port1.</param>
		/// <param name="port2">Port2.</param>
		public MotorSync (MotorPort port1, MotorPort port2)
		{
			this.BitField = base.MotorPortToBitfield(port1) | base.MotorPortToBitfield(port2);
		}
		
		/// <summary>
		/// Gets or sets the motor bit field.
		/// </summary>
		/// <value>The bit field.</value>
		public new OutputBitfield BitField {
			get{return base.BitField;}
			set{base.BitField = value;}
		}
		
		/// <summary>
		/// Syncronise steps between two motors
		/// </summary>
		/// <param name="speed">Speed of the motors.</param>
		/// <param name="turnRatio">Turn ratio (-200 to 200).</param>
		/// <param name="steps">Steps to move.</param>
		/// <param name="brake">If set to <c>true</c> motors will brake when done otherwise off.</param>
		/// <param name='waitForCompletion'>
		/// Set to <c>true</c> to wait for movement to be completed before returning
		/// </param>
		public void StepSync(sbyte speed, Int16 turnRatio, UInt32 steps, bool brake, bool waitForCompletion = true){
			output.SetStepSync(speed, turnRatio, steps, brake);
			if(waitForCompletion)
				WaitForMotorToStop();
		}
		
		/// <summary>
		/// Syncronise time between two motors
		/// </summary>
		/// <param name="speed">Speed of the motors.</param>
		/// <param name="turnRatio">Turn ratio (-200 to 200).</param>
		/// <param name="timeInMs">Time in ms to move.</param>
		/// <param name="brake">If set to <c>true</c> motors will brake when done otherwise off.</param>
		/// <param name='waitForCompletion'>
		/// Set to <c>true</c> to wait for movement to be completed before returning
		/// </param>
		public void TimeSync(sbyte speed, Int16 turnRatio, UInt32 timeInMs, bool brake, bool waitForCompletion = true){
			output.SetTimeSync(speed, turnRatio, timeInMs, brake);
			if(waitForCompletion)
				WaitForMotorToStop();
		}
		
		/// <summary>
		/// Move both motors with the same speed
		/// </summary>
		/// <param name="speed">Speed of the motors.</param>
		/// <param name="turnRatio">Turn ratio (-200 to 200).</param>
		public void On(sbyte speed, Int16 turnRatio){
			On(speed, turnRatio, 0, false);
		}
		
		/// <summary>
		/// Move both motors with the same speed a given number of steps
		/// </summary>
		/// <param name="speed">Speed of the motors.</param>
		/// <param name="turnRatio">Turn ratio (-200 to 200).</param>
		/// <param name="degrees">Degrees to move.</param>
		/// <param name="brake">If set to <c>true</c> motors will brake when done otherwise off.</param>
		/// <param name='waitForCompletion'>
		/// Set to <c>true</c> to wait for movement to be completed before returning
		/// </param>
		public void On (sbyte speed, Int16 turnRatio, uint degrees, bool brake, bool waitForCompletion = true){
			StepSync(speed, turnRatio , degrees, brake, waitForCompletion);
			if(waitForCompletion)
				WaitForMotorToStop();
		}
		
		protected override void WaitForMotorToStop ()
		{
			System.Threading.Thread.Sleep (300);
			foreach (var port in PortList) {
				do{
					System.Threading.Thread.Sleep(50);
				}while(output.GetSpeed(port)!= 0);	
			}
		}
		
	}
	
	
	/// <summary>
	/// Class for controlling a vehicle
	/// </summary>
	public class Vehicle{
		private MotorSync motorSync = new MotorSync(MotorPort.OutA, MotorPort.OutD);
		private MotorPort leftPort;
		private MotorPort rightPort;
		
		/// <summary>
		/// Initializes a new instance of the Vehicle class.
		/// </summary>
		/// <param name='left'>
		/// The left motor of the vehicle
		/// </param>
		/// <param name='right'>
		/// The right motor of the vehicle
		/// </param>
		public Vehicle(MotorPort left, MotorPort right){
			LeftPort = left;
			RightPort = right;
		}
	
		/// <summary>
		/// Gets or sets the left motor
		/// </summary>
		/// <value>
		/// The left motor
		/// </value>
		public MotorPort LeftPort{
			get{
				return leftPort;
			}
			set{
				leftPort = value;
				motorSync.BitField = motorSync.MotorPortToBitfield(leftPort) | motorSync.MotorPortToBitfield(rightPort);
			}
		}
	
		/// <summary>
		/// Gets or sets the right motor
		/// </summary>
		/// <value>
		/// The right motor
		/// </value>
		public MotorPort RightPort{
			get{
				return rightPort;
			}
			set{
				rightPort = value;
				motorSync.BitField = motorSync.MotorPortToBitfield(leftPort) | motorSync.MotorPortToBitfield(rightPort);
			}
		}
	
		/// <summary>
		/// Gets or sets a value indicating whether the left motor is running in reverse direction
		/// </summary>
		/// <value>
		/// <c>true</c> if left motor is reverse; otherwise, <c>false</c>.
		/// </value>
		public bool ReverseLeft{get; set;}
	
		/// <summary>
		/// Gets or sets a value indicating whether the right motor is running in reverse direction
		/// </summary>
		/// <value>
		/// <c>true</c> if right motor is reverse; otherwise, <c>false</c>.
		/// </value>
		public bool ReverseRight{get;set;}
	
		/// <summary>
		/// Run backwards
		/// </summary>
		/// <param name='speed'>
		/// Speed of the vehicle -100 to 100
		/// </param>
		public void Backward(sbyte speed){
			Backward((sbyte)-speed, 0, false);
		}
	
		/// <summary>
		/// Run backwards
		/// </summary>
		/// <param name="speed">Speed.</param>
		/// <param name="degrees">Degrees.</param>
		/// <param name="brake">If set to <c>true</c> motors will brake when done otherwise off.</param>
		/// <param name='waitForCompletion'>
		/// Set to <c>true</c> to wait for movement to be completed before returning
		/// </param>
		public void Backward(sbyte speed, UInt32 degrees, bool brake, bool waitForCompletion = true){
			Move((sbyte)-speed,degrees, brake, waitForCompletion);
		}
	
		/// <summary>
		/// Run forward
		/// </summary>
		/// <param name='speed'>
		/// Speed of the vehicle -100 to 100
		/// </param>
		public void Forward(sbyte speed){
			Forward(speed,0, false, false);
		}
		
		/// <summary>
		/// Run forward
		/// </summary>
		/// <param name="speed">Speed.</param>
		/// <param name="degrees">Degrees.</param>
		/// <param name="brake">If set to <c>true</c> motors will brake when done otherwise off.</param>
		/// <param name='waitForCompletion'>
		/// Set to <c>true</c> to wait for movement to be completed before returning
		/// </param>
		public void Forward(sbyte speed, UInt32 degrees, bool brake, bool waitForCompletion = true){
			Move(speed,degrees, brake, waitForCompletion);
		}
	
		/// <summary>
		/// Spins the vehicle left.
		/// </summary>
		/// <param name="speed">Speed of the vehicle -100 to 100</param>
		public void SpinLeft(sbyte speed){
			SpinLeft(speed,0, false, false);
		}
		
		/// <summary>
		/// Spins the left.
		/// </summary>
		/// <param name="speed">Speed.</param>
		/// <param name="degrees">Degrees.</param>
		/// <param name="brake">If set to <c>true</c> motors will brake when done otherwise off.</param>
		/// <param name='waitForCompletion'>
		/// Set to <c>true</c> to wait for movement to be completed before returning
		/// </param>
		public void SpinLeft(sbyte speed, UInt32 degrees, bool brake, bool waitForCompletion = true){
			if(leftPort < rightPort){
				HandleSpinLeft(speed, degrees,  brake, waitForCompletion);	
			}
			else{
				HandleSpinRight(speed, degrees, brake, waitForCompletion);	
			}
		}
		
		/// <summary>
		/// Spins the vehicle right
		/// </summary>
		/// <param name='speed'>
		/// Speed -100 to 100
		/// </param>
		public void SpinRight(sbyte speed){
			SpinRight(speed,0, false, false );
		}
		
		/// <summary>
		/// Spins the vehicle right
		/// </summary>
		/// <param name="speed">Speed.</param>
		/// <param name="degrees">Degrees.</param>
		/// <param name="brake">If set to <c>true</c> motors will brake when done otherwise off.</param>
		/// <param name='waitForCompletion'>
		/// Set to <c>true</c> to wait for movement to be completed before returning
		/// </param>
		public void SpinRight(sbyte speed, UInt32 degrees, bool brake, bool waitForCompletion = true){
			if(leftPort < rightPort){
				HandleSpinRight(speed, degrees, brake, waitForCompletion);	
			}
			else{
				HandleSpinLeft(speed, degrees, brake, waitForCompletion);	
			}
		}
	
		/// <summary>
		/// Stop moving the vehicle
		/// </summary>
		public void Off(){
			motorSync.Off();
		}
	
		/// <summary>
		/// Brake the vehicle (the motor is still on but it does not move)
		/// </summary>
		public void Brake(){
			motorSync.Brake();
		}
	
		/// <summary>
		/// Turns the vehicle right
		/// </summary>
		/// <param name='speed'>
		/// Speed of the vehicle -100 to 100
		/// </param>
		/// <param name='turnPercent'>
		/// Turn percent 
		/// </param>
		public void TurnRightForward(sbyte speed, sbyte turnPercent){
			TurnRightForward(speed, turnPercent,0,false,false);
		}
		
		
		/// <summary>
		/// Turns the vehicle right
		/// </summary>
		/// <param name="speed">Speed.</param>
		/// <param name="turnPercent">Turn percent.</param>
		/// <param name="degrees">Degrees.</param>
		/// <param name="brake">If set to <c>true</c> motors will brake when done otherwise off.</param>
		/// <param name='waitForCompletion'>
		/// Set to <c>true</c> to wait for movement to be completed before returning
		/// </param>
		public void TurnRightForward(sbyte speed, sbyte turnPercent, UInt32 degrees, bool brake, bool waitForCompletion = true){
			if(leftPort < rightPort){
				HandleRightForward(speed, turnPercent, degrees, brake, waitForCompletion);	
			}
			else{
				HandleLeftForward(speed,turnPercent, degrees, brake, waitForCompletion);	
			}
		}
	
		/// <summary>
		/// Turns the vehicle right while moving backwards
		/// </summary>
		/// <param name='speed'>
		/// Speed of the vehicle -100 to 100
		/// </param>
		/// <param name='turnPercent'>
		/// Turn percent.
		/// </param>
		public void TurnRightReverse(sbyte speed, sbyte turnPercent){
			TurnRightReverse(speed,turnPercent,0, false, false);
		}
		
		/// <summary>
		/// Turns the vehicle right while moving backwards
		/// </summary>
		/// <param name="speed">Speed.</param>
		/// <param name="turnPercent">Turn percent.</param>
		/// <param name="degrees">Degrees.</param>
		/// <param name="brake">If set to <c>true</c> motors will brake when done otherwise off.</param>
		/// <param name='waitForCompletion'>
		/// Set to <c>true</c> to wait for movement to be completed before returning
		/// </param>
		public void TurnRightReverse(sbyte speed, sbyte turnPercent, UInt32 degrees, bool brake, bool waitForCompletion = true){
			if(leftPort < rightPort){
				HandleRightReverse(speed, turnPercent, degrees, brake, waitForCompletion);	
			}
			else{
				HandleLeftReverse(speed,turnPercent, degrees, brake, waitForCompletion);	
			}
		}
	
		/// <summary>
		/// Turns the vehicle left
		/// </summary>
		/// <param name='speed'>
		/// Speed of the vehicle -100 to 100
		/// </param>
		/// <param name='turnPercent'>
		/// Turn percent.
		/// </param>
		public void TurnLeftForward(sbyte speed, sbyte turnPercent){
			TurnLeftForward(speed,turnPercent, 0, false, false);
		}
		
		
		/// <summary>
		/// Turns the vehicle left
		/// </summary>
		/// <param name="speed">Speed.</param>
		/// <param name="turnPercent">Turn percent.</param>
		/// <param name="degrees">Degrees.</param>
		/// <param name="brake">If set to <c>true</c> motors will brake when done otherwise off.</param>
		/// <param name='waitForCompletion'>
		/// Set to <c>true</c> to wait for movement to be completed before returning
		/// </param>
		public void TurnLeftForward(sbyte speed, sbyte turnPercent, UInt32 degrees, bool brake, bool waitForCompletion = true){
			if(leftPort < rightPort){
				HandleLeftForward(speed, turnPercent, degrees, brake, waitForCompletion);	
			}
			else{
				HandleRightForward(speed,turnPercent, degrees, brake, waitForCompletion);
			}
		}
	
		/// <summary>
		/// Turns the vehicle left while moving backwards
		/// </summary>
		/// <param name='speed'>
		/// Speed of the vehicle -100 to 100
		/// </param>
		/// <param name='turnPercent'>
		/// Turn percent.
		/// </param>
		public void TurnLeftReverse(sbyte speed, sbyte turnPercent){
			TurnLeftReverse(speed,turnPercent, 0, false, false);
		}
		
		
		/// <summary>
		/// Turns the vehicle left while moving backwards
		/// </summary>
		/// <param name="speed">Speed.</param>
		/// <param name="turnPercent">Turn percent.</param>
		/// <param name="degrees">Degrees.</param>
		/// <param name="brake">If set to <c>true</c> motors will brake when done otherwise off.</param>
		/// <param name='waitForCompletion'>
		/// Set to <c>true</c> to wait for movement to be completed before returning
		/// </param>
		public void TurnLeftReverse(sbyte speed, sbyte turnPercent, UInt32 degrees, bool brake, bool waitForCompletion = true){
			if(leftPort < rightPort){
				HandleLeftReverse(speed, turnPercent, degrees, brake, waitForCompletion);	
			}
			else{
				HandleRightReverse(speed,turnPercent, degrees, brake, waitForCompletion);
			}
		}
		
		private void HandleLeftForward(sbyte speed, sbyte turnPercent, UInt32 degrees, bool brake, bool waitForCompletion){
			if(!ReverseLeft && !ReverseRight){
				motorSync.On(speed, (short) -turnPercent, degrees, brake, waitForCompletion);
			}
			if(!ReverseLeft && ReverseRight){
				motorSync.On((sbyte)-speed, (short) ((short)-200+ (short)turnPercent), degrees, brake, waitForCompletion);
			}
			if(ReverseLeft && !ReverseRight){
				motorSync.On(speed, (short) ((short)-200+(short)turnPercent), degrees, brake, waitForCompletion);
			}
			if(ReverseLeft && ReverseRight){
				motorSync.On((sbyte)-speed, (short) -turnPercent, degrees, brake, waitForCompletion);				
			}
		}
		
		private void HandleRightForward(sbyte speed, sbyte turnPercent, UInt32 degrees, bool brake, bool waitForCompletion){
			if(!ReverseLeft && !ReverseRight){
				motorSync.On(speed, (short) turnPercent, degrees, brake, waitForCompletion);
			}
			if(!ReverseLeft && ReverseRight){
				motorSync.On(speed, (short) ((short)200- (short)turnPercent), degrees, brake, waitForCompletion);
			}
			if(ReverseLeft && !ReverseRight){
				motorSync.On((sbyte)-speed, (short) ((short)200-(short)turnPercent), degrees, brake, waitForCompletion);
			}
			if(ReverseLeft && ReverseRight){
				motorSync.On((sbyte)-speed, (short) turnPercent, degrees, brake, waitForCompletion);				
			}
		}
		
		private void HandleLeftReverse (sbyte speed, sbyte turnPercent, UInt32 degrees, bool brake, bool waitForCompletion)
		{
			if(!ReverseLeft && !ReverseRight){
				motorSync.On((sbyte)-speed, (short) -turnPercent, degrees, brake, waitForCompletion);
			}
			if(!ReverseLeft && ReverseRight){
				motorSync.On((sbyte)speed,(short) ( (short)-200+(short)turnPercent), degrees, brake, waitForCompletion);
			}
			if(ReverseLeft && !ReverseRight){
				motorSync.On((sbyte)-speed, (short) ( (short)-200+(short)turnPercent), degrees, brake, waitForCompletion);
			}
			if(ReverseLeft && ReverseRight){
				motorSync.On(speed, (short) -turnPercent, degrees, brake, waitForCompletion);				
			}
		}
		
		private void HandleRightReverse(sbyte speed, sbyte turnPercent, UInt32 degrees, bool brake, bool waitForCompletion){
			if(!ReverseLeft && !ReverseRight){
				motorSync.On((sbyte)-speed, (short) turnPercent, degrees, brake, waitForCompletion);
			}
			if(!ReverseLeft && ReverseRight){
				motorSync.On((sbyte)-speed,(short) ( (short)200-(short)turnPercent), degrees, brake, waitForCompletion);
			}
			if(ReverseLeft && !ReverseRight){
				motorSync.On((sbyte)speed, (short) ( (short)200-(short)turnPercent), degrees, brake, waitForCompletion);
			}
			if(ReverseLeft && ReverseRight){
				motorSync.On(speed, (short) turnPercent, degrees, brake, waitForCompletion);				
			}
	
		}
		
		private void HandleSpinRight(sbyte speed, UInt32 degrees, bool brake, bool waitForCompletion){
			if(!ReverseLeft && !ReverseRight){
				motorSync.On(speed, 200, degrees, brake, waitForCompletion);
			}
			if(!ReverseLeft && ReverseRight){
				motorSync.On(speed, (short) 0, degrees, brake, waitForCompletion);
			}
			if(ReverseLeft && !ReverseRight){
				motorSync.On((sbyte)-speed, 0, degrees, brake, waitForCompletion);
			}
			if(ReverseLeft && ReverseRight){
				motorSync.On((sbyte)-speed, 200, degrees, brake, waitForCompletion);				
			}
		}
		
		private void HandleSpinLeft(sbyte speed, UInt32 degrees, bool brake, bool waitForCompletion){
			if(!ReverseLeft && !ReverseRight){
				motorSync.On(speed, -200, degrees, brake, waitForCompletion);
			}
			if(!ReverseLeft && ReverseRight){
				motorSync.On((sbyte)-speed, (short) 0, degrees, brake, waitForCompletion);
			}
			if(ReverseLeft && !ReverseRight){
				motorSync.On(speed, 0, degrees, brake, waitForCompletion);
			}
			if(ReverseLeft && ReverseRight){
				motorSync.On((sbyte)-speed, -200, degrees, brake, waitForCompletion);				
			}
		}
		
		private void Move(sbyte speed, UInt32 degrees, bool brake, bool waitForCompletion){
			if(!ReverseLeft && !ReverseRight){
				motorSync.On(speed, 0, degrees, brake, waitForCompletion); 
			}
			if(!ReverseLeft && ReverseRight){
				motorSync.On(speed, (short) 200, degrees, brake, waitForCompletion);
			}
			if(ReverseLeft && !ReverseRight){
				motorSync.On(speed, -200, degrees, brake, waitForCompletion);
			}
			if(ReverseLeft && ReverseRight){
				motorSync.On((sbyte)-speed, 0, degrees, brake, waitForCompletion);				
			}
		}
	}

}