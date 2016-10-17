using System;
using System.Runtime.InteropServices;

namespace MonoBrickFirmware.Native
{
	static public class Libc
	{
		public enum OpenFlags
		{ 
			O_RDONLY	= 0x0000,		/* open for reading only */
			O_WRONLY	= 0x0001,		/* open for writing only */
			O_RDWR		= 0x0002,		/* open for reading and writing */
			O_ACCMODE	= 0x0003		/* mask for above modes */
		}
		public enum ProtectionFlags
		{
			PROT_NONE       = 0,
			PROT_READ       = 1,
			PROT_WRITE      = 2,
			PROT_EXEC       = 4
		}
		public enum MMapFlags
		{
			MAP_FILE        = 0,
			MAP_SHARED      = 1,
			MAP_PRIVATE     = 2,
			MAP_TYPE        = 0xf,
			MAP_FIXED       = 0x10,
			MAP_ANONYMOUS   = 0x20,
			MAP_ANON        = 0x20			
		}
		
		[DllImport("libc.so.6")]
		public static extern int open(byte[] name, OpenFlags flags);
		
		[DllImport("libc.so.6")]
		public static extern IntPtr mmap(IntPtr addr, uint len, ProtectionFlags prot, MMapFlags flags, int fd, int offset);
		
		[DllImport("libc.so.6")]
		public static extern int write(int file, IntPtr buffer, uint count);
		
		[DllImport("libc.so.6")]
		public static extern int read(int file, IntPtr buffer,uint length);
		
		[DllImport("libc.so.6")]
		public static extern int ioctl(int fd, int cmd, IntPtr buffer);
		
		[DllImport("libc.so.6")]
		public static extern int close(int fd);
	}
	
	public class UnixDevice : IDisposable
	{
		static System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();
		int fd = -1;
		public UnixDevice(string name)
		{			
			
			fd = Libc.open(encoding.GetBytes(name + Char.MinValue), Libc.OpenFlags.O_RDWR);
			if (fd < 0)
				throw new InvalidOperationException("Couldn't open device: " + name);
		}
		
		public MemoryArea MMap(uint size, int offset)
		{
			IntPtr ptr = Libc.mmap(IntPtr.Zero, size, Libc.ProtectionFlags.PROT_READ | Libc.ProtectionFlags.PROT_WRITE, Libc.MMapFlags.MAP_SHARED, fd, offset);
			if ((int)ptr == -1)
				throw new InvalidOperationException("MMap operation failed");
			return new MemoryArea(ptr, size);
		}

        /// <summary>
        /// Writes the supplied array to the correspondent Unix Device
        /// Modified by Oreste Riccardo Natale in answer to the int PlaySoundFile(string name, int volume)
        /// which needs the number of bytes written to the brick speaker
        /// Original hint from user Rasep Retrep on the Monobrick firmware library forum
        /// </summary>
        /// <param name="data">Array to be written on the Unix Device</param>
        /// <returns>Number of bytes succesfully written, -1 on error</returns>
        public int Write (byte[] data)
		{
			IntPtr pnt = IntPtr.Zero;
			bool hasError = false;
			Exception inner = null;
            int bytesWritten = 0;
			try {
				int size = Marshal.SizeOf (data [0]) * data.Length;
				pnt = Marshal.AllocHGlobal (size);
				Marshal.Copy (data, 0, pnt, data.Length);
				bytesWritten = Libc.write (fd, pnt,(uint) size);
				if (bytesWritten == -1)
					hasError = true;
			} 
			catch (Exception e) {
				hasError = true;
				inner = e;
			} 
			finally {
				if (pnt != IntPtr.Zero)    
					Marshal.FreeHGlobal (pnt);
			}
			if (hasError) {
				if (inner != null) {
					throw inner;
				} 
				else 
				{
					throw new InvalidOperationException("Failed to write to Unix device");
				}
			}
            return bytesWritten;
		}
		
		public byte[] Read (int length)
		{
			
			byte[] reply = new byte[length];
			Exception inner = null;
			IntPtr pnt = IntPtr.Zero;
			int bytesRead = 0;
			bool hasError = false;
			try {
				pnt = Marshal.AllocHGlobal (Marshal.SizeOf (reply [0]) * length);
				bytesRead = Libc.read (fd, pnt, (uint)length);
				if (bytesRead == -1) {
					hasError = true;
					Marshal.FreeHGlobal (pnt);
					pnt = IntPtr.Zero;
				} else {
					if (bytesRead != length)
						reply = new byte[bytesRead];
					Marshal.Copy (pnt, reply, 0, bytesRead);
					Marshal.FreeHGlobal (pnt);
					pnt = IntPtr.Zero;	
				}
            
			} catch (Exception e) {
				hasError = true;
				inner = e;    
			} finally {
				if (pnt != IntPtr.Zero) {
					Marshal.FreeHGlobal(pnt);
				}    
            }
            if(hasError){
            	if (inner != null) {
					throw inner;
				} 
				else 
				{
					throw new InvalidOperationException("Failed to read from Unix device");
				}    
            }
			return reply;		
		}
		
		/// <summary>
		/// IO control command that copies to the IO output to a buffer
		/// </summary>
		/// <returns>Zero if successful</returns>
		/// <param name="cmd">IoCtl request code</param>
		/// <param name="input">Input arguments</param>
		/// <param name="output">Output buffer</param>
		/// <param name="ioOutputIndex">IO start index to copy to output buffer</param>
		public int IoCtl (int cmd, byte[] input, byte[] output, int indexToOutput)
		{
			IntPtr pnt = IntPtr.Zero;
			bool hasError = false;
			Exception inner = null;
			int result = -1;
			try {				
				int size = Marshal.SizeOf (typeof(byte))* input.Length;
				pnt = Marshal.AllocHGlobal (size);
				Marshal.Copy (input, 0, pnt, input.Length);
				result = Libc.ioctl (fd, cmd, pnt);
				if (result == -1) {
					hasError = true;
				} else {
					output = new byte[input.Length - indexToOutput];
					Marshal.Copy (pnt, output, indexToOutput, input.Length - indexToOutput);
				}
			} catch (Exception e) {
				hasError = true;
				inner = e;
			} finally {
				if (pnt != IntPtr.Zero) {    
					Marshal.FreeHGlobal (pnt);
				}
			}
			if (hasError) {
				if (inner != null) {
					throw inner;
				} 
				else 
				{
					throw new InvalidOperationException("Failed to excute IO control command");
				}
			}			
			return result;
		}
		
		
		/// <summary>
		/// IO control command. Output is copied back to buffer
		/// </summary>
		/// <returns>Zero if successful</returns>
		/// <param name="requestCode">IoCtl request code</param>
		/// <param name="arguments">IO arguments</param>
		public int IoCtl (int requestCode, byte[] arguments)
		{
			IntPtr pnt = IntPtr.Zero;
			bool hasError = false;
			Exception inner = null;
			int result = -1;
			try {				
				int size = Marshal.SizeOf (typeof(byte))* arguments.Length;
				pnt = Marshal.AllocHGlobal (size);
				Marshal.Copy (arguments, 0, pnt, arguments.Length);
				result = Libc.ioctl (fd, requestCode, pnt);
				if (result == -1) {
					hasError = true;
				}
				else{
					Marshal.Copy (pnt, arguments, 0, arguments.Length);
				} 
			} catch (Exception e) {
				hasError = true;
				inner = e;
			} finally {
				if (pnt != IntPtr.Zero) {    
					Marshal.FreeHGlobal (pnt);
				}
			}
			if (hasError) {
				if (inner != null) {
					throw inner;
				} 
				else 
				{
					throw new InvalidOperationException("Failed to excute IO control command");
				}
			}			
			return result;
		}
		
		
		public void Dispose()
		{			
			Libc.close(fd);
			fd = -1;
		}
		
		~UnixDevice()
		{
			if (fd >= 0)
			{
				Libc.close(fd);
			}
		}
	}
	
	public class MemoryArea
	{
		IntPtr ptr;
		uint size;
		public MemoryArea(IntPtr ptr, uint size)
		{
			this.ptr = ptr;
			this.size = size;
		}
		
		/// <summary>
		/// Write a byte array to the memory map
		/// </summary>
		/// <param name="data">Data.</param>
		public void Write (byte[] data)
		{
			Write(0, data);
		}
		
		/// <summary>
		/// Write a byte array to the memory map
		/// </summary>
		/// <param name="offset">Memory map offset</param>
		/// <param name="data">Data to write</param>
		public void Write (int offset, byte[] data)
		{
			if (offset + data.Length > size)
				throw new IndexOutOfRangeException (string.Format ("Out of range accessing index {0}, max {1}", offset + data.Length, size));
			if (offset != 0) {
				Marshal.Copy (data, 0, ptr.Add (offset), data.Length);
			} 
			else 
			{
				Marshal.Copy (data, 0, ptr , data.Length);
			}
		}
		
		/// <summary>
		/// Copy the whole memory map into an array and return it
		/// </summary>
		public byte[] Read ()
		{
			return Read(0,(int)size);
		}
		
		/// <summary>
		/// Copy part of the memory map into an array and return it
		/// </summary>
		/// <param name="offset">Memory map offset</param>
		/// <param name="length">Number of bytes to read</param>
		public byte[] Read (int offset, int length)
		{
			if (offset + length > size)
				throw new IndexOutOfRangeException (string.Format ("Out of range accessing index {0}, max {1}", offset + length, size));
			byte[] reply = new byte[length];
			if (offset != 0) {
				Marshal.Copy (ptr.Add(offset), reply, 0, length);
			} 
			else 
			{
				Marshal.Copy (ptr, reply, 0, length);
			}
			return reply;	
		}
	}
	
    public static class IntPtrExtensions
    {
        /// <summary>
        /// Add a value to a int pointer
        /// </summary>
        /// <param name="ptr">Pointer to add value to</param>
        /// <param name="val">Value to add</param>
        public static IntPtr Add(this IntPtr ptr, int val)
        {
            return new IntPtr(ptr.ToInt64() + val);
        }
    }   
}

