using System;
using System.Threading;
namespace MonoBrickFirmware.Movement
{
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
			new Thread(delegate() {
				Backward((sbyte)-speed, 0, false);
			}).Start();
			motorSync.CancelPolling();
		}
	
		/// <summary>
		/// Run backwards
		/// </summary>
		/// <param name="speed">Speed.</param>
		/// <param name="degrees">Degrees.</param>
		/// <param name="brake">If set to <c>true</c> motors will brake when done otherwise off.</param>
		/// </param>
		public WaitHandle Backward(sbyte speed, UInt32 degrees, bool brake){
			return Move((sbyte)-speed, degrees, brake);
		}
	
		/// <summary>
		/// Run forward
		/// </summary>
		/// <param name='speed'>
		/// Speed of the vehicle -100 to 100
		/// </param>
		public void Forward(sbyte speed){
			new Thread(delegate() {
				Forward(speed, 0, false);
			}).Start();
			motorSync.CancelPolling();
		}
		
		/// <summary>
		/// Run forward
		/// </summary>
		/// <param name="speed">Speed.</param>
		/// <param name="degrees">Degrees.</param>
		/// <param name="brake">If set to <c>true</c> motors will brake when done otherwise off.</param>
		/// </param>
		public WaitHandle Forward(sbyte speed, UInt32 degrees, bool brake){
			return Move(speed,degrees, brake);
		}
	
		/// <summary>
		/// Spins the vehicle left.
		/// </summary>
		/// <param name="speed">Speed of the vehicle -100 to 100</param>
		public void SpinLeft(sbyte speed){
			new Thread(delegate() {
				SpinLeft(speed,0, false);
			}).Start();
			motorSync.CancelPolling();
		}
		
		/// <summary>
		/// Spins the left.
		/// </summary>
		/// <param name="speed">Speed.</param>
		/// <param name="degrees">Degrees.</param>
		/// <param name="brake">If set to <c>true</c> motors will brake when done otherwise off.</param>
		/// </param>
		public WaitHandle SpinLeft(sbyte speed, UInt32 degrees, bool brake){
			WaitHandle handle;
			if(leftPort < rightPort){
				handle = HandleSpinLeft(speed, degrees,  brake);	
			}
			else{
				handle = HandleSpinRight(speed, degrees, brake);	
			}
			return handle;
		}
		
		/// <summary>
		/// Spins the vehicle right
		/// </summary>
		/// <param name='speed'>
		/// Speed -100 to 100
		/// </param>
		public void SpinRight(sbyte speed){
			new Thread(delegate() {
				SpinRight(speed,0, false);
			}).Start();
			motorSync.CancelPolling();
		}
		
		/// <summary>
		/// Spins the vehicle right
		/// </summary>
		/// <param name="speed">Speed.</param>
		/// <param name="degrees">Degrees.</param>
		/// <param name="brake">If set to <c>true</c> motors will brake when done otherwise off.</param>
		/// </param>
		public WaitHandle SpinRight(sbyte speed, UInt32 degrees, bool brake){
			WaitHandle handle;
			if(leftPort < rightPort){
				handle = HandleSpinRight(speed, degrees, brake);	
			}
			else{
				handle = HandleSpinLeft(speed, degrees, brake);	
			}
			return handle;
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
			new Thread(delegate() {
				TurnRightForward(speed, turnPercent,0,false);
			}).Start();
			motorSync.CancelPolling();
		}
		

		/// <summary>
		/// Turns the vehicle right
		/// </summary>
		/// <param name="speed">Speed.</param>
		/// <param name="turnPercent">Turn percent.</param>
		/// <param name="degrees">Degrees.</param>
		/// <param name="brake">If set to <c>true</c> motors will brake when done otherwise off.</param>
		/// </param>
		public WaitHandle TurnRightForward(sbyte speed, sbyte turnPercent, UInt32 degrees, bool brake){
			WaitHandle handle;
			if(leftPort < rightPort){
				handle = HandleRightForward(speed, turnPercent, degrees, brake);	
			}
			else{
				handle = HandleLeftForward(speed,turnPercent, degrees, brake);	
			}
			return handle;
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
			new Thread(delegate() {
				TurnRightReverse(speed,turnPercent,0, false);
			}).Start();
			motorSync.CancelPolling();
		}
		
		/// <summary>
		/// Turns the vehicle right while moving backwards
		/// </summary>
		/// <param name="speed">Speed.</param>
		/// <param name="turnPercent">Turn percent.</param>
		/// <param name="degrees">Degrees.</param>
		/// <param name="brake">If set to <c>true</c> motors will brake when done otherwise off.</param>
		/// </param>
		public WaitHandle TurnRightReverse(sbyte speed, sbyte turnPercent, UInt32 degrees, bool brake){
			WaitHandle handle;
			if(leftPort < rightPort){
				handle = HandleRightReverse(speed, turnPercent, degrees, brake);	
			}
			else{
				handle = HandleLeftReverse(speed,turnPercent, degrees, brake);	
			}
			return handle;
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
			new Thread(delegate() {
				TurnLeftForward(speed,turnPercent, 0, false);
			}).Start();
			motorSync.CancelPolling();
		}
		
		
		/// <summary>
		/// Turns the vehicle left
		/// </summary>
		/// <param name="speed">Speed.</param>
		/// <param name="turnPercent">Turn percent.</param>
		/// <param name="degrees">Degrees.</param>
		/// <param name="brake">If set to <c>true</c> motors will brake when done otherwise off.</param>
		/// </param>
		public WaitHandle TurnLeftForward(sbyte speed, sbyte turnPercent, UInt32 degrees, bool brake){
			WaitHandle handle;
			if(leftPort < rightPort){
				handle = HandleLeftForward(speed, turnPercent, degrees, brake);	
			}
			else{
				handle = HandleRightForward(speed,turnPercent, degrees, brake);
			}
			return handle;
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
			new Thread(delegate() {
				TurnLeftReverse(speed,turnPercent, 0, false);
			}).Start();
			motorSync.CancelPolling();
		}
		
		
		/// <summary>
		/// Turns the vehicle left while moving backwards
		/// </summary>
		/// <param name="speed">Speed.</param>
		/// <param name="turnPercent">Turn percent.</param>
		/// <param name="degrees">Degrees.</param>
		/// <param name="brake">If set to <c>true</c> motors will brake when done otherwise off.</param>
		/// </param>
		public WaitHandle TurnLeftReverse(sbyte speed, sbyte turnPercent, UInt32 degrees, bool brake){
			WaitHandle handle;
			if(leftPort < rightPort){
				handle = HandleLeftReverse(speed, turnPercent, degrees, brake);	
			}
			else{
				handle = HandleRightReverse(speed,turnPercent, degrees, brake);
			}
			return handle;
		}
		
		private WaitHandle HandleLeftForward(sbyte speed, sbyte turnPercent, UInt32 degrees, bool brake){
			WaitHandle handle = null;
			if(!ReverseLeft && !ReverseRight){
				handle = motorSync.StepSync(speed, (short) -turnPercent, degrees, brake);
			}
			if(!ReverseLeft && ReverseRight){
				handle = motorSync.StepSync((sbyte)-speed, (short) ((short)-200+ (short)turnPercent), degrees, brake);
			}
			if(ReverseLeft && !ReverseRight){
				handle = motorSync.StepSync(speed, (short) ((short)-200+(short)turnPercent), degrees, brake);
			}
			if(ReverseLeft && ReverseRight){
				handle = motorSync.StepSync((sbyte)-speed, (short) -turnPercent, degrees, brake);				
			}
		    return handle;
		}
		
		private WaitHandle HandleRightForward(sbyte speed, sbyte turnPercent, UInt32 degrees, bool brake){
			WaitHandle handle = null;
			if(!ReverseLeft && !ReverseRight){
				handle = motorSync.StepSync(speed, (short) turnPercent, degrees, brake);
			}
			if(!ReverseLeft && ReverseRight){
				handle = motorSync.StepSync(speed, (short) ((short)200- (short)turnPercent), degrees, brake);
			}
			if(ReverseLeft && !ReverseRight){
				handle = motorSync.StepSync((sbyte)-speed, (short) ((short)200-(short)turnPercent), degrees, brake);
			}
			if(ReverseLeft && ReverseRight){
				handle = motorSync.StepSync((sbyte)-speed, (short) turnPercent, degrees, brake);				
			}
			return handle;
		}
		
		private WaitHandle HandleLeftReverse (sbyte speed, sbyte turnPercent, UInt32 degrees, bool brake)
		{
			WaitHandle handle = null;
			if(!ReverseLeft && !ReverseRight){
				handle = motorSync.StepSync((sbyte)-speed, (short) -turnPercent, degrees, brake);
			}
			if(!ReverseLeft && ReverseRight){
				handle = motorSync.StepSync((sbyte)speed,(short) ( (short)-200+(short)turnPercent), degrees, brake);
			}
			if(ReverseLeft && !ReverseRight){
				handle = motorSync.StepSync((sbyte)-speed, (short) ( (short)-200+(short)turnPercent), degrees, brake);
			}
			if(ReverseLeft && ReverseRight){
				handle = motorSync.StepSync(speed, (short) -turnPercent, degrees, brake);				
			}
			return handle;
		}
		
		private WaitHandle HandleRightReverse(sbyte speed, sbyte turnPercent, UInt32 degrees, bool brake){
			WaitHandle handle = null;
			if(!ReverseLeft && !ReverseRight){
				handle = motorSync.StepSync((sbyte)-speed, (short) turnPercent, degrees, brake);
			}
			if(!ReverseLeft && ReverseRight){
				handle = motorSync.StepSync((sbyte)-speed,(short) ( (short)200-(short)turnPercent), degrees, brake);
			}
			if(ReverseLeft && !ReverseRight){
				handle = motorSync.StepSync((sbyte)speed, (short) ( (short)200-(short)turnPercent), degrees, brake);
			}
			if(ReverseLeft && ReverseRight){
				handle = motorSync.StepSync(speed, (short) turnPercent, degrees, brake);				
			}
			return handle;
	
		}
		
		private WaitHandle HandleSpinRight(sbyte speed, UInt32 degrees, bool brake){
			WaitHandle handle = null;
			if(!ReverseLeft && !ReverseRight){
				handle = motorSync.StepSync(speed, 200, degrees, brake);
			}
			if(!ReverseLeft && ReverseRight){
				handle = motorSync.StepSync(speed, (short) 0, degrees, brake);
			}
			if(ReverseLeft && !ReverseRight){
				handle = motorSync.StepSync((sbyte)-speed, 0, degrees, brake);
			}
			if(ReverseLeft && ReverseRight){
				handle = motorSync.StepSync((sbyte)-speed, 200, degrees, brake);				
			}
			return handle;
		}
		
		private WaitHandle HandleSpinLeft(sbyte speed, UInt32 degrees, bool brake){
			WaitHandle handle = null;
			if(!ReverseLeft && !ReverseRight){
				handle = motorSync.StepSync(speed, -200, degrees, brake);
			}
			if(!ReverseLeft && ReverseRight){
				handle = motorSync.StepSync((sbyte)-speed, (short) 0, degrees, brake);
			}
			if(ReverseLeft && !ReverseRight){
				handle = motorSync.StepSync(speed, 0, degrees, brake);
			}
			if(ReverseLeft && ReverseRight){
				handle = motorSync.StepSync((sbyte)-speed, -200, degrees, brake);				
			}
			return handle; 
		}
		
		private WaitHandle Move(sbyte speed, UInt32 degrees, bool brake){
			WaitHandle handle = null;
			if(!ReverseLeft && !ReverseRight){
				handle = motorSync.StepSync(speed, 0, degrees, brake); 
			}
			if(!ReverseLeft && ReverseRight){
				handle = motorSync.StepSync(speed, (short) 200, degrees, brake);
			}
			if(ReverseLeft && !ReverseRight){
				handle = motorSync.StepSync(speed, -200, degrees, brake);
			}
			if(ReverseLeft && ReverseRight){
				handle = motorSync.StepSync((sbyte)-speed, 0, degrees, brake);				
			}
			return handle;
		}
	}
}

