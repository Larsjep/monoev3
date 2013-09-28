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
		public static extern int close(int fd);
	}
	
	public class UnixDevice : IDisposable
	{
		static System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();
		int fd;
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
		
		public void Write(int offset, byte[] data)
		{
			if (offset+data.Length > size)
				throw new IndexOutOfRangeException(string.Format("Out of range accessing index {0}, max {1}", offset+data.Length, size));
			Marshal.Copy(data, offset, ptr, data.Length);
		}
	}
	
}

