using System;
using System.Text;
using System.Collections.Generic;
using MonoBrickFirmware.Movement;
using MonoBrickFirmware.Sound;
using MonoBrickFirmware.Native;

namespace MonoBrickFirmware.Tools
{
	/// <summary>
	/// Class for creating a byte command
	/// </summary>
	internal class ByteArrayCreator
	{
		
		/// <summary>
		/// A list that holds the data bytes of the command
		/// </summary>
		protected List<byte> dataArr = new List<byte>();
		
		/// <summary>
		/// Append a bool value
		/// </summary>
		/// <param name='b'>
		/// The bool value to append
		/// </param>
		public void Append(bool b){
			if(b)
				dataArr.Add(0x01);
			else
				dataArr.Add(0x00);
		}

		/// <summary>
		/// Append a string
		/// </summary>
		/// <param name='s'>
		/// The string to append
		/// </param>
		public void Append(String s){
			for(int i = 0; i < s.Length; i++){
				dataArr.Add((byte)(s[i]));
			}
			dataArr.Add((byte)0x00);
		}

		/// <summary>
		/// Append a string
		/// </summary>
		/// <param name='s'>
		/// The string to append
		/// </param>
		/// <param name='maxSize'>
		/// The maximum size to append
		/// </param>
		/// <param name='padWithZero'>
		/// If set to <c>true</c> and string length is less that maxsize the remaining bytes will be padded with zeros
		/// If set to <c>false</c> and string length is less that maxsize no padding will be added
		/// </param>
		public void Append(String s , int maxSize, bool padWithZero){
			if(s.Length >maxSize)
			   s.Remove(maxSize);
			if(padWithZero && !(s.Length == maxSize)){
				s = s + new string((char)0, maxSize- s.Length);
			}
			Append(s);
		}

		/// <summary>
		/// Append a byte
		/// </summary>
		/// <param name='b'>
		/// The byte value to append
		/// </param>
		public void Append(byte b){
			dataArr.Add(b);
		}

		/// <summary>
		/// Append a signed byte
		/// </summary>
		/// <param name='b'>
		/// The signed byte to append
		/// </param>
		public void Append(sbyte b){
			Append((byte)b);
		}
		
		/// <summary>
		/// Append a UInt16
		/// </summary>
		/// <param name='data'>
		/// The UInt16 to append
		/// </param>
		public void Append(UInt16 data){
			Append(BitConverter.GetBytes(data));
		}

		/// <summary>
		/// Append a Int16
		/// </summary>
		/// <param name='data'>
		/// The Int16 to append
		/// </param>
		public void Append(Int16 data){
			Append(BitConverter.GetBytes(data));
		}

		/// <summary>
		/// Append a UInt32
		/// </summary>
		/// <param name='data'>
		/// The UInt32 to append
		/// </param>
		public void Append(UInt32 data){
			byte[] bytes = BitConverter.GetBytes(data);
			Append(bytes);
		}
		
		/// <summary>
		/// Append a Int32
		/// </summary>
		/// <param name='data'>
		/// The Int32 to append
		/// </param>
		public void Append(Int32 data){
			Append(BitConverter.GetBytes(data));
		}
		
		/// <summary>
		/// Append a float
		/// </summary>
		/// <param name='data'>
		/// The float to append
		/// </param>
		public void Append(float data){
			Append(BitConverter.GetBytes(data));
		}

		/// <summary>
		/// Append a byte array
		/// </summary>
		/// <param name='data'>
		/// The array to append
		/// </param>
		public void Append(byte [] data){
			Append(data,0,data.Length);
		}
		
		/// <summary>
		/// Append a byte array
		/// </summary>
		/// <param name='data'>
		/// The array to append
		/// </param>
		/// <param name='offset'>
		/// The byte array offset
		/// </param>
		public void Append(byte [] data, int offset){
			Append(data,offset,data.Length);
		}

		/// <summary>
		/// Append a byte array
		/// </summary>
		/// <param name='data'>
		/// The array to append
		/// </param>
		/// <param name='offset'>
		/// The byte array offset
		/// </param>
		/// <param name='length'>
		/// The length to append
		/// </param>
		public void Append(byte [] data, int offset , int length){
			for(int i = 0; i < length; i++){
				dataArr.Add(data[i+offset]);
			}
		}
		
		/// <summary>
		/// Append a brick byte code
		/// </summary>
		/// <param name="code">Code to append</param>
		public void Append (KernelByteCodes code)
		{
			Append((byte) code);
		}
		
		/// <summary>
		/// Append the a motor port
		/// </summary>
		/// <param name="port">Port to append</param>
		public void Append (MotorPort port)
		{
			Append((byte) port);
		}
		
		/// <summary>
		/// Append a output bit field.
		/// </summary>
		/// <param name="bitField">Output bit field to append</param>
		public void Append (OutputBitfield bitField)
		{
			Append((byte) bitField);
		}
		
		public void Append (AudioMode mode)
		{
			Append((byte) mode);
		}
		
		/// <summary>
		/// Appends zeros 
		/// </summary>
		/// <param name='zeros'>
		/// Number of zeros to append
		/// </param>
		public void AppendZeros(int zeros){
			for(int i = 0; i < zeros; i++){
				dataArr.Add(0);
			}
		}

		/// <summary>
		/// Gets byte array of the command
		/// </summary>
		/// <value>
		/// The command data
		/// </value>
		public byte[] Data{
			get{return  dataArr.ToArray();}
		}

		/// <summary>
		/// Gets the length of the command
		/// </summary>
		/// <value>
		/// The length of the command
		/// </value>
		public int Length{
			get{return dataArr.Count;}
		}
		
		private static string AddSpacesToString(string text)
		{
	        if (string.IsNullOrEmpty(text))
	           return "";
	        StringBuilder newText = new StringBuilder(text.Length * 2);
	        newText.Append(text[0]);
	        for (int i = 1; i < text.Length; i++)
	        {
	            if (char.IsUpper(text[i]) && text[i - 1] != ' ')
	                newText.Append(' ');
	            newText.Append(text[i]);
	        }
	        return newText.ToString();
		}
		
		internal void Print ()
		{
			for (int i =0; i < Data.Length; i ++) {
				Console.WriteLine ("Data[{0}]: {1:X}",i,Data[i]);
			}	
		}
		
	}
}

