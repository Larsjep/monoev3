using System;

namespace MonoBrickFirmware.Native
{
	/// <summary>
	/// kernel byte codes - this is taken from the MonoBrick communication library
	/// </summary>
	public enum KernelByteCodes{
		#pragma warning disable 
		//VM
		ProgramStop	= 0x02,
		ProgramStart = 0x03,
		
		//Move
		InitBytes = 0x2F,
		
		//VM
		Info = 0x7C,
  		String = 0x7D,
  		MemoryWrite = 0x7E,
  		MemoryRead = 0x7F, 
		
		//Sound
		Sound = 0x94,
		SoundTest = 095,
		SoundReady = 0x96,
		
		//Input
		InputSample = 0x97,
		InputDeviceList = 0x98,
		InputDevice = 0x99,
		InputRead = 0x9a,
		InputTest = 0x9b,
		InputReady = 0x9c,
		InputReadSI = 0x9d,
		InputReadExt = 0x9e,
		InputWrite = 0x9f,
		
		//output
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

		//Memory
		File = 0xC0,
  		Array = 0xc1,
  		ArrayWrite = 0xc2,
  		ArrayRead = 0xc3,
  		ArrayAppend = 0xc4,
  		MemoryUsage = 0xc5,
  		FileName = 0xc6,
		
		//Mailbox
		MailboxOpen = 0xD8,
		MailboxWrite = 0xD9,
		MailboxRead = 0xDA,
		MailboxTest = 0xDB,
		MailboxReady = 0xDC,
		MailboxClose = 0xDD,

		#pragma warning restore
	}
}

