using System;
using System.Threading;

namespace MonoBrickFirmware.Movement
{
	/// <summary>
	/// Class for synchronizing two motors
	/// </summary>
	public class MotorSync : MotorBase{
		
		/// <summary>
		/// Initializes a new instance of the <see cref="MonoBrickFirmware.Movement.MotorSync"/> class.
		/// </summary>
		/// <param name="bitfield">Bitfield.</param>
		public MotorSync (OutputBitfield bitfield)
		{
			this.BitField = bitfield;
		}
		
		/// <summary>
		/// Initializes a new instance of the <see cref="MonoBrickFirmware.Movement.MotorSync"/> class.
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
		public WaitHandle StepSync(sbyte speed, Int16 turnRatio, UInt32 steps, bool brake){
			output.SetStepSync(speed, turnRatio, steps, brake);
			return  WaitForMotorsToStop();
		}
		
		/// <summary>
		/// Syncronise time between two motors
		/// </summary>
		/// <param name="speed">Speed of the motors.</param>
		/// <param name="turnRatio">Turn ratio (-200 to 200).</param>
		/// <param name="timeInMs">Time in ms to move.</param>
		/// <param name="brake">If set to <c>true</c> motors will brake when done otherwise off.</param>
		public WaitHandle TimeSync(sbyte speed, Int16 turnRatio, UInt32 timeInMs, bool brake){
			output.SetTimeSync(speed, turnRatio, timeInMs, brake);
			return  WaitForMotorsToStop();
		}
		
		/// <summary>
		/// Move both motors with the same speed
		/// </summary>
		/// <param name="speed">Speed of the motors.</param>
		/// <param name="turnRatio">Turn ratio (-200 to 200).</param>
		public void SetSpeed(sbyte speed, Int16 turnRatio){
			CancelPolling();
			output.SetStepSync(speed, turnRatio, 0, false);
		}

		internal new void CancelPolling ()
		{
			base.CancelPolling();
		}
	}
}

