using System;
using System.Collections.Generic;
using System.Timers;
using System.Threading;

namespace MonoBrickFirmware.Movement
{
	/// <summary>
	/// Base class for EV3 motors 
	/// </summary>
	public class MotorBase{
		
		private System.Timers.Timer timer;
		private const double pollRate = 50;
		private ManualResetEvent stop = new ManualResetEvent(false);
		private ManualResetEvent start = new ManualResetEvent(false);

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

		public MotorBase ()
		{
			timer = new System.Timers.Timer(pollRate);
			timer.Elapsed += PollFunction;
		}

		/// <summary>
		/// Determines whether the motor(s) are running.
		/// </summary>
		/// <returns>
		/// <c>true</c> if this motor is moving; otherwise, <c>false</c>.
		/// </returns>
	    public bool IsRunning()
		{
			bool status = true;
			foreach (var port in PortList) {
				if (output.GetSpeed (port) == 0) 
				{
					status = false;
					break;
				} 
			}
			return status;
		}

		/// <summary>
		/// Brake the motor (is still on but does not move)
		/// </summary>
		public virtual void Brake(){
			CancelPolling();
			output.Stop(true);
		}
		
		/// <summary>
		/// Turn the motor off
		/// </summary>
		public virtual void Off(){
			CancelPolling();
			output.Stop (false);
			output.SetPower(0);
		}
		
		/// <summary>
		/// Sets the power of the motor.
		/// </summary>
		/// <param name="power">Power to use.</param>
		public virtual void SetPower(sbyte power)
		{
			CancelPolling();
			output.SetPower(power);
			output.Start();
		}

		private void PollFunction (Object source, ElapsedEventArgs e)
		{
			if (!Monitor.TryEnter (this)) {
				//cancel has the lock
				return;
			}
			//Do the polling
			if (IsRunning ()) {
				stop.Reset ();
				start.Set ();
			} 
			else 
			{
				stop.Set ();
				start.Reset ();
			}
			Monitor.Exit(this);

		}

		protected void CancelPolling()
		{
			if (timer.Enabled) 
			{
				timer.Stop();
				Monitor.Enter(this);
				stop.Set();
				start.Set();
				Monitor.Exit(this);
			}	
		}

		protected void WaitForMotorsToStart()
		{
			timer.Start();
			if(!IsRunning())
				start.WaitOne();
			timer.Stop();
			start.Reset();
			stop.Reset();
		}

		protected void WaitForMotorsToStop()
		{
			timer.Start();
			if(IsRunning())
				stop.WaitOne();
			timer.Stop();
			start.Reset();
			stop.Reset();
		}

		protected void WaitForMotorsToStartAndStop()
		{
			timer.Start();
			if(!IsRunning())
				start.WaitOne();
			stop.WaitOne();
			timer.Stop();
			start.Reset();
			stop.Reset();
		}
	}
}

