using System.Runtime.InteropServices;
using System;
using MonoBrickFirmware.Native;

namespace MonoBrickFirmware.IO
{	
	/// <summary>
	/// Motor ports
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
	/// Output byte codes.
	/// </summary>
	public enum OutputCodes{
		#pragma warning disable
		OutputGetType = 0xa0,
		OutputSetType = 0xa1,         
  		OutputReset = 0xa2,           
  		OutputStop = 0xA3,
		OutputPower = 0xA4,
		OutputSpeed = 0xA5,
		OutputStart	= 0xA6,
		OutputPolarity = 0xA7,
		OutputRead = 0xA8,
		OutputTest = 0xA9,
		OutputReady = 0xAA,
		OutputPosition = 0xAB,
		OutputStepPower = 0xAC,
		OutputTimePower = 0xAD,
		OutputStepSpeed = 0xAE,
		OutputTimeSpeed = 0xAF,
		OutputStepSync = 0xB0,
		OutputTimeSync = 0xB1,
		OutputClrCount = 0xB2,
		OutputGetCount = 0xB3,
		#pragma warning restore
	}
	
	/// <summary>
	/// Class for reading and writing a tacho value
	/// </summary>
	internal class Tacho : Ev3Device
	{
		private MotorPort port;
		public Tacho (MotorPort port): base("/dev/lms_motor", 1000, 0)
		{	
			this.port = port;
		}
			
	
	}
	
	
	/// <summary>
	/// Class for controlling the output port
	/// </summary>
	internal class Output : Ev3Device
	{
		private MotorPort port;
		Tacho tacho = null;
		public Output (MotorPort port): base("/dev/lms_pwm", 1000, 0)
		{
			this.port = port;
			this.tacho = new Tacho(port);
		}
		
		
		/// <summary>
		/// Gets or sets the output bit field.
		/// </summary>
		/// <value>The bit field.</value>
		public OutputBitfield BitField{get;set;}
		
		/// <summary>
		/// Sets the type of the output. I don't know what this is used for.
		/// </summary>
		/// <param name="port">Motor port to use.</param>
		/// <param name="type">Type to use.</param>
		/// <param name="reply">If set to <c>true</c> reply from brick will be send.</param>
		public void SetType(MotorPort port, byte type, bool reply = false){
			/*var command = new Command(0,0,200,reply);
			command.Append(ByteCodes.OutputSetType);
			command.Append(this.DaisyChainLayer);
			command.Append(port);
			command.Append(type, ParameterFormat.Short);
			connection.Send(command);
			if(reply){
				var brickReply = connection.Receive();
				Error.CheckForError(brickReply,200);
			}*/	
		}
		
		/// <summary>
		/// Reset the output
		/// </summary>
		/// <param name="reply">If set to <c>true</c> reply from brick will be send.</param>
		public void Reset(bool reply = false){
			/*var command = new Command(0,0,201,reply);
			command.Append(ByteCodes.OutputReset);
			command.Append(this.DaisyChainLayer);
			command.Append(this.BitField);
			connection.Send(command);
			if(reply){
				var brickReply = connection.Receive();
				Error.CheckForError(brickReply,201);
			}*/	
		}
		
		/// <summary>
		/// Stop the specified brake and reply.
		/// </summary>
		/// <param name="brake">If set to <c>true</c> the motor will brake and not coast</param>
		/// <param name="reply">If set to <c>true</c> reply from brick will be send.</param>
		public void Stop(bool brake, bool reply = false){
			/*var command = new Command(0,0,202,reply);
			command.Append(ByteCodes.OutputStop);
			command.Append(this.DaisyChainLayer);
			command.Append(this.BitField);
			byte b  = 0;
			if(brake){
				b = 1;
			}
			command.Append(b, ParameterFormat.Short);
			connection.Send(command);
			if(reply){
				var brickReply = connection.Receive();
				Error.CheckForError(brickReply,202);
			}*/
		}
		
		/// <summary>
		/// Sets the speed.
		/// </summary>
		/// <param name="speed">Speed.</param>
		/// <param name="reply">If set to <c>true</c> reply from brick will be send.</param>
		public void SetSpeed(byte speed, bool reply = false){
			/*var command = new Command(0,0,203,reply);
			command.Append(ByteCodes.OutputSpeed);
			command.Append(this.DaisyChainLayer);
			command.Append(this.BitField);
			command.Append(speed, ParameterFormat.Long);
			connection.Send(command);
			if(reply){
				var brickReply = connection.Receive();
				Error.CheckForError(brickReply,203);
			}*/		
		}
		
		/// <summary>
		/// Sets the power.
		/// </summary>
		/// <param name="power">Power.</param>
		/// <param name="reply">If set to <c>true</c> reply from brick will be send.</param>
		public void SetPower(byte power, bool reply = false){
			/*var command = new Command(0,0,204,reply);
			command.Append(ByteCodes.OutputPower);
			command.Append(this.DaisyChainLayer);
			command.Append(this.BitField);
			command.Append(power, ParameterFormat.Long);
			connection.Send(command);
			if(reply){
				var brickReply = connection.Receive();
				Error.CheckForError(brickReply,204);
			}*/
		}
		
		/// <summary>
		/// Sets the absolute position from last reset
		/// </summary>
		/// <param name="position">Position to use</param>
		/// <param name="reply">If set to <c>true</c> reply from the brick will be send</param>
		public void SetPosition(Int32 position, bool reply = false){
			/*var command = new Command(0,0,214,reply);
			command.Append(ByteCodes.OutputPosition);
			command.Append(this.DaisyChainLayer);
			command.Append(this.BitField);
			command.Append(position, ConstantParameterType.Value);
			connection.Send(command);
			if(reply){
				var brickReply = connection.Receive();
				Error.CheckForError(brickReply,214);
			}*/
		}
		
		/// <summary>
		/// Start
		/// </summary>
		/// <param name="reply">If set to <c>true</c> reply from brick will be send.</param>
		public void Start(bool reply = false){
			/*var command = new Command(0,0,205,reply);
			command.Append(ByteCodes.OutputStart);
			command.Append(this.DaisyChainLayer);
			command.Append(this.BitField);
			connection.Send(command);
			if(reply){
				var brickReply = connection.Receive();
				Error.CheckForError(brickReply,205);
			}*/
		}
		
		/// <summary>
		/// Start with the specified speed
		/// </summary>
		/// <param name="speed">Speed.</param>
		/// <param name="reply">If set to <c>true</c> reply from brick will be send.</param>
		public void Start(sbyte speed, bool reply = false){
			/*var command = new Command(0,0,215,reply);
			command.Append(ByteCodes.OutputSpeed);
			command.Append(this.DaisyChainLayer);
			command.Append(this.BitField);
			command.Append(speed, ParameterFormat.Long);
			command.Append(ByteCodes.OutputStart);
			command.Append(this.DaisyChainLayer);
			command.Append(this.BitField);
			connection.Send (command);
			if(reply){
				var brickReply = connection.Receive();
				Error.CheckForError(brickReply,215);
			}*/
		}
		
		/// <summary>
		/// Sets the polarity.
		/// </summary>
		/// <param name="polarity">Polarity of the output</param>
		/// <param name="reply">If set to <c>true</c> reply from brick will be send.</param>
		public void SetPolarity(Polarity polarity, bool reply = false){
			/*var command = new Command(0,0,206,reply);
			command.Append(ByteCodes.OutputPolarity);
			command.Append(this.DaisyChainLayer);
			command.Append(this.BitField);
			command.Append((sbyte) polarity, ParameterFormat.Short);
			connection.Send(command);
			if(reply){
				var brickReply = connection.Receive();
				Error.CheckForError(brickReply,206);
			}*/
		}
		
		/// <summary>
		/// Set Ramp up, constant and rampdown steps and power of the outputs
		/// </summary>
		/// <param name="power">Power to use</param>
		/// <param name="rampUpSteps">Steps used to ramp up</param>
		/// <param name="constantSpeedSteps">Steps used for constant speed</param>
		/// <param name="rampDownSteps">Steps used to ramp down</param>
		/// <param name="brake">If set to <c>true</c> brake when done.</param>
		/// <param name="reply">If set to <c>true</c> reply from brick will be send.</param>
		public void SetStepPower(sbyte power, UInt32 rampUpSteps, UInt32 constantSpeedSteps, UInt32 rampDownSteps, bool brake, bool reply = false){
			/*var command = new Command(0,0,207,reply);
			command.Append(ByteCodes.OutputStepPower);
			command.Append(this.DaisyChainLayer);
			command.Append(this.BitField);
			command.Append(power, ConstantParameterType.Value);
			command.Append(rampUpSteps, ConstantParameterType.Value);
			command.Append(constantSpeedSteps, ConstantParameterType.Value);
			command.Append(rampDownSteps, ConstantParameterType.Value);
			byte b = 0;//coast
			if(brake)
				b = 1;
			command.Append(b, ParameterFormat.Short);
			connection.Send(command);
			if(reply){
				var brickReply = connection.Receive();
				Error.CheckForError(brickReply,207);
			}*/
		}
		
		/// <summary>
		/// Set Ramp up, constant and rampdown time and power of the outputs
		/// </summary>
		/// <param name="power">Power to use</param>
		/// <param name="rampUpTime">Time in ms to ramp up</param>
		/// <param name="constantSpeedTime">Time in ms for constant speed</param>
		/// <param name="rampDownTime">Time in ms to ramp down</param>
		/// <param name="brake">If set to <c>true</c> brake when done.</param>
		/// <param name="reply">If set to <c>true</c> reply from brick will be send.</param>
		public void SetTimePower(byte power, UInt32 rampUpTime, UInt32 constantSpeedTime, UInt32 rampDownTime, bool brake, bool reply = false){
			/*var command = new Command(0,0,208,reply);
			command.Append(ByteCodes.OutputTimePower);
			command.Append(this.DaisyChainLayer);
			command.Append(this.BitField);
			command.Append(power, ParameterFormat.Short);
			command.Append(rampUpTime, ConstantParameterType.Value);
			command.Append(constantSpeedTime, ConstantParameterType.Value);
			command.Append(rampDownTime, ConstantParameterType.Value);
			byte b = 0;//coast
			if(brake)
				b = 1;
			command.Append(b, ParameterFormat.Short);
			connection.Send(command);
			if(reply){
				var brickReply = connection.Receive();
				Error.CheckForError(brickReply,208);
			}*/
		}
		
		/// <summary>
		/// Set Ramp up, constant and rampdown steps and speed of the outputs
		/// </summary>
		/// <param name="speed">Speed to use</param>
		/// <param name="rampUpSteps">Steps used to ramp up</param>
		/// <param name="constantSpeedSteps">Steps used for constant speed</param>
		/// <param name="rampDownSteps">Steps used to ramp down</param>
		/// <param name="brake">If set to <c>true</c> brake when done.</param>
		/// <param name="reply">If set to <c>true</c> reply from brick will be send.</param>
		public void SetStepSpeed(sbyte speed, UInt32 rampUpSteps, UInt32 constantSpeedSteps, UInt32 rampDownSteps, bool brake, bool reply = false){
			/*var command = new Command(0,0,209,reply);
			command.Append(ByteCodes.OutputStepSpeed);
			command.Append(this.DaisyChainLayer);
			command.Append(this.BitField);
			command.Append(speed, ConstantParameterType.Value);
			command.Append(rampUpSteps, ConstantParameterType.Value);
			command.Append(constantSpeedSteps, ConstantParameterType.Value);
			command.Append(rampDownSteps, ConstantParameterType.Value);
			byte b = 0;//coast
			if(brake)
				b = 1;
			command.Append(b, ParameterFormat.Short);
			connection.Send(command);
			if(reply){
				var brickReply = connection.Receive();
				Error.CheckForError(brickReply,209);
			}*/		
		}
		
		/// <summary>
		/// Set Ramp up, constant and rampdown time and power of the outputs
		/// </summary>
		/// <param name="speed">Speed to use</param>
		/// <param name="rampUpTime">Time in ms to ramp up</param>
		/// <param name="constantSpeedTime">Time in ms for constant speed</param>
		/// <param name="rampDownTime">Time in ms to ramp down</param>
		/// <param name="brake">If set to <c>true</c> brake when done</param>
		/// <param name="reply">If set to <c>true</c> reply from brick will be send.</param>
		public void SetTimeSpeed(sbyte speed, UInt32 rampUpTime, UInt32 constantSpeedTime, UInt32 rampDownTime, bool brake, bool reply = false){
			/*var command = new Command(0,0,210,reply);
			command.Append(ByteCodes.OutputTimeSpeed);
			command.Append(this.DaisyChainLayer);
			command.Append(this.BitField);
			command.Append(speed, ConstantParameterType.Value);
			command.Append(rampUpTime, ConstantParameterType.Value);
			command.Append(constantSpeedTime, ConstantParameterType.Value);
			command.Append(rampDownTime, ConstantParameterType.Value);
			byte b = 0;//coast
			if(brake)
				b = 1;
			command.Append(b, ParameterFormat.Short);
			connection.Send(command);
			if(reply){
				var brickReply = connection.Receive();
				Error.CheckForError(brickReply,210);
			}*/
		}
		
		/// <summary>
		/// Sync steps between two motors
		/// </summary>
		/// <param name="speed">Speed.</param>
		/// <param name="turnRatio">Turn ratio between two syncronized motors</param>
		/// <param name="steps">Steps in degrees</param>
		/// <param name="brake">If set to <c>true</c> brake.</param>
		/// <param name="reply">If set to <c>true</c> reply.</param>
		public void SetStepSync(sbyte speed, Int16 turnRatio, UInt32 steps, bool brake, bool reply = false){
			/*var command = new Command(0,0,209,reply);
			command.Append(ByteCodes.OutputStepSync);
			command.Append(this.DaisyChainLayer);
			command.Append(this.BitField);
			command.Append(speed, ConstantParameterType.Value);
			command.Append(turnRatio, ConstantParameterType.Value);
			command.Append(steps, ConstantParameterType.Value);
			byte b = 0;//coast
			if(brake)
				b = 1;
			command.Append(b, ParameterFormat.Short);
			connection.Send(command);
			if(reply){
				var brickReply = connection.Receive();
				Error.CheckForError(brickReply,209);
			}*/
		}
		
		/// <summary>
		/// Time sync between two motors
		/// </summary>
		/// <param name="speed">Speed.</param>
		/// <param name="turnRatio">Turn ratio between two syncronized motors</param>
		/// <param name="timeInMs">Time in ms</param>
		/// <param name="brake">If set to <c>true</c> brake.</param>
		/// <param name="reply">If set to <c>true</c> reply.</param>
		public void SetTimeSync(sbyte speed, Int16 turnRatio, UInt32 timeInMs, bool brake, bool reply = false){
			/*var command = new Command(0,0,210,reply);
			command.Append(ByteCodes.OutputTimeSync);
			command.Append(this.DaisyChainLayer);
			command.Append(this.BitField);
			command.Append(speed, ConstantParameterType.Value);
			command.Append(turnRatio, ConstantParameterType.Value);
			command.Append(timeInMs, ConstantParameterType.Value);
			byte b = 0;//coast
			if(brake)
				b = 1;
			command.Append(b, ParameterFormat.Short);
			connection.Send(command);
			if(reply){
				var brickReply = connection.Receive();
				Error.CheckForError(brickReply,210);
			}*/
		}
		
		/// <summary>
		/// Wait for output ready (wait for completion)
		/// </summary>
		/// <param name="reply">If set to <c>true</c> reply from brick will be send.</param>
		public void WaitForReady(bool reply = false){
			/*var command = new Command(0,0,211,reply);
			command.Append(ByteCodes.OutputReady);
			command.Append(this.DaisyChainLayer);
			command.Append(this.BitField);
			connection.Send(command);
			if(reply){
				var brickReply = connection.Receive();
				Error.CheckForError(brickReply,211);
			}*/	
		}
		
		/// <summary>
		/// Testing if output is used 
		/// </summary>
		/// <returns><c>true</c> if this instance is ready; otherwise, <c>false</c>.</returns>
		public bool IsReady(){
			/*var command = new Command(1,0,212,true);
			command.Append(ByteCodes.OutputTest);
			command.Append(this.DaisyChainLayer);
			command.Append(this.BitField);
			var brickReply = connection.SendAndReceive(command);
			Error.CheckForError(brickReply,212);
			return ! Convert.ToBoolean(brickReply.GetByte(3));*/
			return false;
		}
		
		/// <summary>
		/// Clearing tacho count when used as sensor 
		/// </summary>
		/// <param name="reply">If set to <c>true</c> reply from brick will be send.</param>
		public void ClearCount(bool reply = false){
			/*var command = new Command(0,0,210,reply);
			command.Append(ByteCodes.OutputClrCount);
			command.Append(this.DaisyChainLayer);
			command.Append(this.BitField);
			connection.Send(command);
			if(reply){
				var brickReply = connection.Receive();
				Error.CheckForError(brickReply,210);
			}*/
		}
		
		/// <summary>
		/// Gets the tacho count.
		/// </summary>
		/// <returns>The tacho count.</returns>
		/// <param name="port">Motor port to use</param>
		public Int32 GetCount(MotorPort port){
			/*var command = new Command(4,0,212,true);
			command.Append(ByteCodes.OutputGetCount);
			command.Append(this.DaisyChainLayer);
			command.Append(port);
			command.Append((byte)0, VariableScope.Global);
			var brickReply = connection.SendAndReceive(command);
			Error.CheckForError(brickReply,212);
			return brickReply.GetInt32(3);*/
			return 0;
		}
		
		/// <summary>
		/// Gets the speed of the motor
		/// </summary>
		/// <returns>The speed.</returns>
		/// <param name="port">Motor port to read</param>
		public sbyte GetSpeed(MotorPort port){
			/*var command = new Command(8,0,220,true);
			command.Append(ByteCodes.OutputRead);
			command.Append(this.DaisyChainLayer);
			command.Append(port);
			command.Append((byte)0, VariableScope.Global);
			command.Append((byte)4, VariableScope.Global);
			var brickReply = connection.SendAndReceive(command);
			Error.CheckForError(brickReply,220);
			return brickReply.GetSbyte(3);
			//The tacho speed from outputRead does not work
			// I have also tried to place the tacho reply in offset 1 (and with 5 global bytes in the reply) but get a error each time*/
			return 0;
			
		}
		
	}
	
	/// <summary>
	/// Base class for EV3 motors 
	/// </summary>
	public class MotorBase{
		
		/// <summary>
		/// Initializes a new instance of the <see cref="MonoBrickFirmware.IO.MotorBase"/> class.
		/// </summary>
		/// <param name="port">Port.</param>
		public MotorBase(MotorPort port){
			Port = port;
			output = new Output(port);
		}
		
		
		/// <summary>
		/// The output.
		/// </summary>
		internal Output output = null;
		
		/// <summary>
		/// Gets or sets the motor port this is set by the bitfield. 
		/// Do not set this use the bitfield property instead
		/// </summary>
		/// <value>The port.</value>
		protected MotorPort Port {get; private set;}
		
		/// <summary>
		/// Gets or sets the bit field.
		/// </summary>
		/// <value>The bit field.</value>
		internal OutputBitfield BitField {	
			get{return output.BitField;} 
			set{
				// Check if only one motor is set
				// Only if outA or outB or outC or outD is set
				output.BitField = value;
				if( ( (value & OutputBitfield.OutA) == OutputBitfield.OutA) || 
					( (value & OutputBitfield.OutB) == OutputBitfield.OutB) || 
					( (value & OutputBitfield.OutC) == OutputBitfield.OutC) || 
					( (value & OutputBitfield.OutD) == OutputBitfield.OutD)){
						if((value & OutputBitfield.OutA) == OutputBitfield.OutA){
							Port = MotorPort.OutA;
						}
						if((value & OutputBitfield.OutB) == OutputBitfield.OutB){
							Port = MotorPort.OutB;
						}
						if((value & OutputBitfield.OutC) == OutputBitfield.OutC){
							Port = MotorPort.OutC;
						}
						if((value & OutputBitfield.OutD) == OutputBitfield.OutD){
							Port = MotorPort.OutD;
						}
					}
				else{
					//more than one motor is set to run take one of them and use as motor port
					if(Convert.ToBoolean(value & OutputBitfield.OutA)){
						Port = MotorPort.OutA;
						return;
					}
					if(Convert.ToBoolean(value & OutputBitfield.OutB)){
						Port = MotorPort.OutB;
						return;
					}
					if(Convert.ToBoolean(value & OutputBitfield.OutC)){
						Port = MotorPort.OutC;
						return;
					}
					if(Convert.ToBoolean(value & OutputBitfield.OutA)){
						Port = MotorPort.OutD;
						return;
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
		/// Initializes a new instance of the <see cref="MonoBrick.EV3.Motor"/> class.
		/// </summary>
		public MotorBase ()
		{
			this.BitField = OutputBitfield.OutA;
		}
		
		/// <summary>
		/// Brake the motor (is still on but does not move)
		/// </summary>
		public void Brake(){
			Brake (false);
		}
		
		/// <summary>
		/// Brake the motor (is still on but does not move)
		/// </summary>
		/// <param name='reply'>
		/// If set to <c>true</c> the brick will send a reply
		/// </param>
		public void Brake(bool reply){
			output.Stop(true, reply);
		}
		
		/// <summary>
		/// Turn the motor off
		/// </summary>
		public void Off(){
			Off(false);
		}
	    
		/// <summary>
		/// Turn the motor off
		/// </summary>
		/// <param name='reply'>
		/// If set to <c>true</c> the brick will send a reply
		/// </param>
	    public void Off(bool reply){
			output.Stop (false,reply);
		}
		
		/// <summary>
		/// Sets the power of the motor.
		/// </summary>
		/// <param name="power">Power to use.</param>
		public void SetPower(byte power){
			SetPower(power, false);
		}
		
		/// <summary>
		/// Sets the power of the motor.
		/// </summary>
		/// <param name="power">Power to use.</param>
		/// <param name="reply">If set to <c>true</c> reply from brick will be send.</param>
		public void SetPower(byte power, bool reply){
			output.SetPower(power, reply);
		}
	}
	
	
	
	/// <summary>
	/// Class for EV3 motor
	/// </summary>
	public class Motor :  MotorBase
	{
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
				//This does strange things and should not be used
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
			On(speed,false);
		}
		
		/// <summary>
		/// Move the motor
		/// </summary>
		/// <param name='speed'>
		/// Speed of the motor -100 to 100
		/// </param>
		/// <param name='reply'>
		/// If set to <c>true</c> brick will send a reply
		/// </param>
		public void On(sbyte speed, bool reply){
			output.Start(speed, reply);
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
		public void On(sbyte speed, UInt32 degrees, bool brake){
			On(speed,degrees, brake,false);
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
		/// <param name='reply'>
		/// If set to <c>true</c> the brick will send a reply
		/// </param>
		public void On(sbyte speed, UInt32 degrees, bool brake, bool reply){
			UInt64 longDegrees = (UInt64)degrees;
			UInt32 rampUpDownSteps =(UInt32) (15 *  longDegrees * 100)/10000;
			UInt32 constantsSteps = (UInt32) (70 *  longDegrees * 100)/10000;
			if(rampUpDownSteps > 300){//To make sure ramp up is not too long
				rampUpDownSteps = 300;
				constantsSteps = degrees - 2*rampUpDownSteps;
			}
			output.SetStepSpeed(speed,rampUpDownSteps,constantsSteps, rampUpDownSteps, brake,reply);
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
		public void MoveTo(byte speed, Int32 position, bool brake){
			MoveTo(speed,position, brake, false);	
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
		/// <param name='reply'>
		/// If set to <c>true</c> the brick will send a reply
		/// </param>
		public void MoveTo(byte speed, Int32 position, bool brake, bool reply){
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
			this.On(motorSpeed, diff, reply);
		}
		
		/// <summary>
		/// Create a speed profile where ramp up and down is specified in steps
		/// </summary>
		/// <param name="speed">Maximum speed of the motor.</param>
		/// <param name="rampUpSteps">Ramp up steps.</param>
		/// <param name="constantSpeedSteps">Constant speed steps.</param>
		/// <param name="rampDownSteps">Ramp down steps.</param>
		/// <param name="brake">If set to <c>true</c> the motor will brake when movement is done.</param>
		/// <param name="reply">If set to <c>true</c> reply from brick will be send.</param>
		public void SpeedProfileStep(sbyte speed, UInt32 rampUpSteps, UInt32 constantSpeedSteps, UInt32 rampDownSteps, bool brake, bool reply = false)
		{
			output.SetStepSpeed(speed, rampUpSteps, constantSpeedSteps,rampDownSteps, brake, reply);
		}
		
		/// <summary>
		/// Create a speed profile where ramp up and down is specified in time
		/// </summary>
		/// <param name="speed">Maximum speed of the motor.</param>
		/// <param name="rampUpTimeMs">Ramp up time ms.</param>
		/// <param name="constantSpeedTimeMs">Constant speed time ms.</param>
		/// <param name="rampDownTimeMs">Ramp down time ms.</param>
		/// <param name="brake">If set to <c>true</c> the motor will brake when movement is done.</param>
		/// <param name="reply">If set to <c>true</c> reply from brick will be send.</param>
		public void SpeedProfileTime(sbyte speed, UInt32 rampUpTimeMs, UInt32 constantSpeedTimeMs, UInt32 rampDownTimeMs, bool brake, bool reply = false)
		{
			output.SetTimeSpeed(speed, rampUpTimeMs, constantSpeedTimeMs, rampUpTimeMs, brake, reply);
		}
		
		/// <summary>
		/// Create a power profile where ramp up and down is specified in steps
		/// </summary>
		/// <param name="power">Maximum power of the motor.</param>
		/// <param name="rampUpSteps">Ramp up steps.</param>
		/// <param name="constantSpeedSteps">Constant speed steps.</param>
		/// <param name="rampDownSteps">Ramp down steps.</param>
		/// <param name="brake">If set to <c>true</c> the motor will brake when movement is done.</param>
		/// <param name="reply">If set to <c>true</c> reply from brick will be send.</param>
		public void PowerProfileStep(sbyte power, UInt32 rampUpSteps, UInt32 constantSpeedSteps, UInt32 rampDownSteps, bool brake, bool reply = false)
		{
			output.SetStepPower(power,rampUpSteps, constantSpeedSteps, rampDownSteps, brake, reply);
		}
		
		/// <summary>
		/// Create a power profile where ramp up and down is specified in time
		/// </summary>
		/// <param name="power">Maximum power of the motor.</param>
		/// <param name="rampUpTimeMs">Ramp up time ms.</param>
		/// <param name="constantSpeedTimeMs">Constant speed time ms.</param>
		/// <param name="rampDownTimeMs">Ramp down time ms.</param>
		/// <param name="brake">If set to <c>true</c> the motor will brake when movement is done.</param>
		/// <param name="reply">If set to <c>true</c> reply from brick will be send.</param>
		public void PowerProfileTime (byte power, UInt32 rampUpTimeMs, UInt32 constantSpeedTimeMs, UInt32 rampDownTimeMs, bool brake, bool reply = false)
		{
			output.SetTimePower(power, rampUpTimeMs,constantSpeedTimeMs,rampDownTimeMs, brake, reply);
		}
		
		/// <summary>
		/// Resets the tacho
		/// </summary>
		public void ResetTacho(){
			ResetTacho(false);
		}
		
		/// <summary>
		/// Resets the tacho
		/// </summary>
		/// <param name='reply'>
		/// If set to <c>true</c> the brick will send a reply
		/// </param>
	    public void ResetTacho(bool reply = false){
			output.ClearCount(reply);
		}
	
		/// <summary>
		/// Gets the tacho count.
		/// </summary>
		/// <returns>
		/// The tacho count
		/// </returns>
	    public Int32 GetTachoCount(){
			return output.GetCount(this.Port);
		}
		
		/// <summary>
		/// Determines whether this motor is running.
		/// </summary>
		/// <returns>
		/// <c>true</c> if this motor is running; otherwise, <c>false</c>.
		/// </returns>
	    public bool IsRunning(){
			return output.GetSpeed(this.Port)!= 0;				
		}
		
		/// <summary>
		/// Gets the speed of the motor
		/// </summary>
		/// <returns>The speed.</returns>
		public sbyte GetSpeed ()
		{
			return output.GetSpeed(this.Port);
		}
	}
	
	/// <summary>
	/// Class for synchronizing two motors
	/// </summary>
	public class MotorSync : MotorBase{
		
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
		/// <param name="reply">If set to <c>true</c> reply from brick will be send.</param>
		public void StepSync(sbyte speed, Int16 turnRatio, UInt32 steps, bool brake, bool reply = false){
			output.SetStepSync(speed, turnRatio, steps, brake, reply);
		}
		
		/// <summary>
		/// Syncronise time between two motors
		/// </summary>
		/// <param name="speed">Speed of the motors.</param>
		/// <param name="turnRatio">Turn ratio (-200 to 200).</param>
		/// <param name="timeInMs">Time in ms to move.</param>
		/// <param name="brake">If set to <c>true</c> motors will brake when done otherwise off.</param>
		/// <param name="reply">If set to <c>true</c> reply from brick will be send.</param>
		public void TimeSync(sbyte speed, Int16 turnRatio, UInt32 timeInMs, bool brake, bool reply = false){
			output.SetTimeSync(speed, turnRatio, timeInMs, brake, reply);
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
		/// Move both motors with the same speed
		/// </summary>
		/// <param name="speed">Speed of the motors.</param>
		/// <param name="turnRatio">Turn ratio (-200 to 200).</param>
		/// <param name="reply">If set to <c>true</c> reply from brick will be send.</param>
		public void On(sbyte speed, Int16 turnRatio, bool reply){
			StepSync(speed, turnRatio,0, reply);
		}
		
		/// <summary>
		/// Move both motors with the same speed a given number of steps
		/// </summary>
		/// <param name="speed">Speed of the motors.</param>
		/// <param name="turnRatio">Turn ratio (-200 to 200).</param>
		/// <param name="degrees">Degrees to move.</param>
		public void On (sbyte speed, Int16 turnRatio, uint degrees){
			On(speed,turnRatio,degrees, false);
		}
		
		/// <summary>
		/// Move both motors with the same speed a given number of steps
		/// </summary>
		/// <param name="speed">Speed of the motors.</param>
		/// <param name="turnRatio">Turn ratio (-200 to 200).</param>
		/// <param name="degrees">Degrees to move.</param>
		/// <param name="reply">If set to <c>true</c> reply from brick will be send.</param>
		public void On (sbyte speed, Int16 turnRatio, uint degrees, bool reply)
		{
			StepSync(speed,turnRatio,degrees, false, reply);
		}
	}
	
	
	/// <summary>
	/// Class for controlling a vehicle
	/// </summary>
	public class Vehicle{
		private MotorSync motorSync = new MotorSync();
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
			Move((sbyte)-speed,0,false);
		}
	
		/// <summary>
		/// Run backwards
		/// </summary>
		/// <param name='speed'>
		/// Speed of the vehicle -100 to 100
		/// </param>
		/// <param name='reply'>
		/// If set to <c>true</c> the brick will send a reply
		/// </param>
		public void Backward(sbyte speed, bool reply){
			Backward((sbyte)-speed,0,reply);
		}
		
		/// <summary>
		/// Run backwards
		/// </summary>
		/// <param name="speed">Speed.</param>
		/// <param name="degrees">Degrees.</param>
		/// <param name="reply">If set to <c>true</c> the brick will send a reply</param>
		public void Backward(sbyte speed, UInt32 degrees, bool reply = false){
			Move((sbyte)-speed,degrees,reply);
		}
	
		/// <summary>
		/// Run forward
		/// </summary>
		/// <param name='speed'>
		/// Speed of the vehicle -100 to 100
		/// </param>
		public void Forward(sbyte speed){
			Move(speed,0,false);
		}
	
		/// <summary>
		/// Run forward
		/// </summary>
		/// <param name='speed'>
		/// Speed of the vehicle -100 to 100
		/// </param>
		/// <param name='reply'>
		/// If set to <c>true</c> the brick will send a reply
		/// </param>
		public void Forward(sbyte speed, bool reply){
			Forward(speed,0,reply);
		}
		
		/// <summary>
		/// Run forward
		/// </summary>
		/// <param name="speed">Speed.</param>
		/// <param name="degrees">Degrees.</param>
		/// <param name="reply">If set to <c>true</c> reply will be send</param>
		public void Forward(sbyte speed, UInt32 degrees, bool reply = false){
			Move(speed,degrees,reply);
		}
	
		/// <summary>
		/// Spins the vehicle left.
		/// </summary>
		/// <param name='speed'>
		/// Speed of the vehicle -100 to 100
		/// </param>
		public void SpinLeft(sbyte speed){
			SpinLeft(speed,false);
		}
		
		/// <summary>
		/// Spins the vehicle left.
		/// </summary>
		/// <param name="speed">Speed of the vehicle -100 to 100</param>
		/// <param name="reply">If set to <c>true</c> reply will be send.</param>
		public void SpinLeft(sbyte speed, bool reply){
			SpinLeft(speed,0,reply);
		}
		
		/// <summary>
		/// Spins the left.
		/// </summary>
		/// <param name="speed">Speed.</param>
		/// <param name="degrees">Degrees.</param>
		/// <param name="reply">If set to <c>true</c> reply will be send.</param>
		public void SpinLeft(sbyte speed, UInt32 degrees, bool reply = false){
			if(leftPort < rightPort){
				HandleSpinLeft(speed, degrees, reply);	
			}
			else{
				HandleSpinRight(speed, degrees, reply);	
			}
		}
		
		
	
		/// <summary>
		/// Spins the vehicle right
		/// </summary>
		/// <param name='speed'>
		/// Speed of the vehicle -100 to 100
		/// </param>
		public void SpinRight(sbyte speed){
			SpinRight(speed,false);
	
		}
	
		/// <summary>
		/// Spins the vehicle right
		/// </summary>
		/// <param name='speed'>
		/// Speed -100 to 100
		/// </param>
		/// <param name='reply'>
		/// If set to <c>true</c> the brick will send a reply
		/// </param>
		public void SpinRight(sbyte speed, bool reply){
			SpinRight(speed,0,reply);
		}
		
		/// <summary>
		/// Spins the vehicle right
		/// </summary>
		/// <param name="speed">Speed.</param>
		/// <param name="degrees">Degrees.</param>
		/// <param name="reply">If set to <c>true</c> reply will be send.</param>
		public void SpinRight(sbyte speed, UInt32 degrees, bool reply = false){
			if(leftPort < rightPort){
				HandleSpinRight(speed, degrees, reply);	
			}
			else{
				HandleSpinLeft(speed, degrees, reply);	
			}
		}
	
		/// <summary>
		/// Stop moving the vehicle
		/// </summary>
		/// <param name='reply'>
		/// If set to <c>true</c> the brick will send a reply
		/// </param>
		public void Off(bool reply){
			motorSync.Off(reply);
		}
	
		/// <summary>
		/// Stop moving the vehicle
		/// </summary>
		public void Off(){
			Off(false);
		}
	
		/// <summary>
		/// Brake the vehicle (the motor is still on but it does not move)
		/// </summary>
		public void Brake(){
			Brake(false);
		}
	
		/// <summary>
		/// Brake the vehicle (the motor is still on but it does not move)
		/// </summary>
		/// <param name='reply'>
		/// If set to <c>true</c> the brick will send a reply
		/// </param>
		public void Brake(bool reply){
			motorSync.Brake(reply);	
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
			TurnRightForward(speed, turnPercent, false);
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
		/// <param name='reply'>
		/// If set to <c>true</c> the brick will send a reply
		/// </param>
		public void TurnRightForward(sbyte speed, sbyte turnPercent, bool reply = false){
			if(leftPort < rightPort){
				HandleRightForward(speed, turnPercent, 0, reply);	
			}
			else{
				HandleLeftForward(speed,turnPercent, 0, reply);	
			}
		}
		
		
		/// <summary>
		/// Turns the vehicle right
		/// </summary>
		/// <param name="speed">Speed.</param>
		/// <param name="turnPercent">Turn percent.</param>
		/// <param name="degrees">Degrees.</param>
		/// <param name="reply">If set to <c>true</c> reply will be send.</param>
		public void TurnRightForward(sbyte speed, sbyte turnPercent, UInt32 degrees, bool reply= false){
			if(leftPort < rightPort){
				HandleRightForward(speed, turnPercent, degrees, reply);	
			}
			else{
				HandleLeftForward(speed,turnPercent, degrees, reply);	
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
			TurnRightReverse(speed, turnPercent, false);
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
		/// <param name='reply'>
		/// If set to <c>true</c> the brick will send a reply
		/// </param>
		public void TurnRightReverse(sbyte speed, sbyte turnPercent, bool reply){
			TurnRightReverse(speed,turnPercent,0,reply);
	
		}
		
		/// <summary>
		/// Turns the vehicle right while moving backwards
		/// </summary>
		/// <param name="speed">Speed.</param>
		/// <param name="turnPercent">Turn percent.</param>
		/// <param name="degrees">Degrees.</param>
		/// <param name="reply">If set to <c>true</c> reply will be send.</param>
		public void TurnRightReverse(sbyte speed, sbyte turnPercent, UInt32 degrees, bool reply = false){
			if(leftPort < rightPort){
				HandleRightReverse(speed, turnPercent, degrees, reply);	
			}
			else{
				HandleLeftReverse(speed,turnPercent, degrees, reply);	
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
			TurnLeftForward(speed, turnPercent, false);
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
		/// <param name='reply'>
		/// If set to <c>true</c> the brick will send a reply
		/// </param>
		public void TurnLeftForward(sbyte speed, sbyte turnPercent, bool reply){
			TurnLeftForward(speed,turnPercent, 0, reply);
		}
		
		
		/// <summary>
		/// Turns the vehicle left
		/// </summary>
		/// <param name="speed">Speed.</param>
		/// <param name="turnPercent">Turn percent.</param>
		/// <param name="degrees">Degrees.</param>
		/// <param name="reply">If set to <c>true</c> reply will be send.</param>
		public void TurnLeftForward(sbyte speed, sbyte turnPercent, UInt32 degrees, bool reply = false){
			if(leftPort < rightPort){
				HandleLeftForward(speed, turnPercent, degrees, reply);	
			}
			else{
				HandleRightForward(speed,turnPercent, degrees, reply);
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
			TurnLeftReverse(speed, turnPercent, false);
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
		/// <param name='reply'>
		/// If set to <c>true</c> the brick will send a reply
		/// </param>
		public void TurnLeftReverse(sbyte speed, sbyte turnPercent, bool reply){
			TurnLeftReverse(speed,turnPercent, 0, reply);
		}
		
		
		/// <summary>
		/// Turns the vehicle left while moving backwards
		/// </summary>
		/// <param name="speed">Speed.</param>
		/// <param name="turnPercent">Turn percent.</param>
		/// <param name="degrees">Degrees.</param>
		/// <param name="reply">If set to <c>true</c> reply.</param>
		public void TurnLeftReverse(sbyte speed, sbyte turnPercent, UInt32 degrees, bool reply = false){
			if(leftPort < rightPort){
				HandleLeftReverse(speed, turnPercent, degrees, reply);	
			}
			else{
				HandleRightReverse(speed,turnPercent, degrees, reply);
			}
		}
		
		private void HandleLeftForward(sbyte speed, sbyte turnPercent, UInt32 degrees, bool reply){
			if(!ReverseLeft && !ReverseRight){
				motorSync.On(speed, (short) -turnPercent, degrees, reply);
			}
			if(!ReverseLeft && ReverseRight){
				motorSync.On((sbyte)-speed, (short) ((short)-200+ (short)turnPercent), degrees, reply);
			}
			if(ReverseLeft && !ReverseRight){
				motorSync.On(speed, (short) ((short)-200+(short)turnPercent), degrees, reply);
			}
			if(ReverseLeft && ReverseRight){
				motorSync.On((sbyte)-speed, (short) -turnPercent, degrees, reply);				
			}
		}
		
		private void HandleRightForward(sbyte speed, sbyte turnPercent, UInt32 degrees, bool reply){
			if(!ReverseLeft && !ReverseRight){
				motorSync.On(speed, (short) turnPercent, reply);
			}
			if(!ReverseLeft && ReverseRight){
				motorSync.On(speed, (short) ((short)200- (short)turnPercent), degrees, reply);
			}
			if(ReverseLeft && !ReverseRight){
				motorSync.On((sbyte)-speed, (short) ((short)200-(short)turnPercent), degrees, reply);
			}
			if(ReverseLeft && ReverseRight){
				motorSync.On((sbyte)-speed, (short) turnPercent, degrees, reply);				
			}
		}
		
		private void HandleLeftReverse (sbyte speed, sbyte turnPercent, UInt32 degrees, bool reply)
		{
			if(!ReverseLeft && !ReverseRight){
				motorSync.On((sbyte)-speed, (short) -turnPercent, degrees, reply);
			}
			if(!ReverseLeft && ReverseRight){
				motorSync.On((sbyte)speed,(short) ( (short)-200+(short)turnPercent), degrees, reply);
			}
			if(ReverseLeft && !ReverseRight){
				motorSync.On((sbyte)-speed, (short) ( (short)-200+(short)turnPercent), degrees, reply);
			}
			if(ReverseLeft && ReverseRight){
				motorSync.On(speed, (short) -turnPercent, degrees, reply);				
			}
		}
		
		private void HandleRightReverse(sbyte speed, sbyte turnPercent, UInt32 degrees, bool reply){
			if(!ReverseLeft && !ReverseRight){
				motorSync.On((sbyte)-speed, (short) turnPercent, degrees, reply);
			}
			if(!ReverseLeft && ReverseRight){
				motorSync.On((sbyte)-speed,(short) ( (short)200-(short)turnPercent), degrees, reply);
			}
			if(ReverseLeft && !ReverseRight){
				motorSync.On((sbyte)speed, (short) ( (short)200-(short)turnPercent), degrees, reply);
			}
			if(ReverseLeft && ReverseRight){
				motorSync.On(speed, (short) turnPercent, degrees, reply);				
			}
	
		}
		
		private void HandleSpinRight(sbyte speed, UInt32 degrees, bool reply){
			if(!ReverseLeft && !ReverseRight){
				motorSync.On(speed, 200, degrees, reply);
			}
			if(!ReverseLeft && ReverseRight){
				motorSync.On(speed, (short) 0, degrees, reply);
			}
			if(ReverseLeft && !ReverseRight){
				motorSync.On((sbyte)-speed, 0, degrees, reply);
			}
			if(ReverseLeft && ReverseRight){
				motorSync.On((sbyte)-speed, 200, degrees, reply);				
			}
		}
		
		private void HandleSpinLeft(sbyte speed, UInt32 degrees, bool reply){
			if(!ReverseLeft && !ReverseRight){
				motorSync.On(speed, -200, degrees, reply);
			}
			if(!ReverseLeft && ReverseRight){
				motorSync.On((sbyte)-speed, (short) 0, degrees, reply);
			}
			if(ReverseLeft && !ReverseRight){
				motorSync.On(speed, 0, degrees, reply);
			}
			if(ReverseLeft && ReverseRight){
				motorSync.On((sbyte)-speed, -200, degrees, reply);				
			}
		}
		
		private void Move(sbyte speed, UInt32 degrees, bool reply){
			if(!ReverseLeft && !ReverseRight){
				motorSync.On(speed, 0, degrees,reply); 
			}
			if(!ReverseLeft && ReverseRight){
				motorSync.On(speed, (short) 200, degrees, reply);
			}
			if(ReverseLeft && !ReverseRight){
				motorSync.On(speed, -200, degrees, reply);
			}
			if(ReverseLeft && ReverseRight){
				motorSync.On((sbyte)-speed, 0, degrees, reply);				
			}
		}
	}

}