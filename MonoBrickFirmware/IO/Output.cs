using System;
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
		
		private const int TachoBufferSize = 12;
		private const int NumberOfSensorPorts = 4;
		private const int TachoMemorySize = TachoBufferSize * NumberOfSensorPorts;
		
		private const int UnknownDataOffset = 0; // I don't know what the first four bytes are used for
		private const int SpeedDataOffset = 4; 
		private const int TachoDataOffset = 8;
		
		public Output ()
		{
			
			pwmDevice = new UnixDevice("/dev/lms_pwm");
			tachoDevice = new UnixDevice("/dev/lms_motor");
			tachoMemory = tachoDevice.MMap(TachoMemorySize,0);
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
			byte[] data = tachoMemory.Read ((int)port * TachoBufferSize+ TachoDataOffset, 4);
			var reply = new DeviceReply (data);
			return reply.GetInt32(0);
		}
		
		/// <summary>
		/// Gets the speed of the motor
		/// </summary>
		/// <returns>The speed.</returns>
		/// <param name="port">Motor port to read</param>
		public sbyte GetSpeed(MotorPort port){
			byte[] data = tachoMemory.Read ((int)port * TachoBufferSize + SpeedDataOffset, 1);
			var reply = new DeviceReply (data);
			return reply.GetSbyte(0);
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
}

