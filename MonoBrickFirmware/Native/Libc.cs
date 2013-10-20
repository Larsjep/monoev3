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
		
		public void Write (byte[] data)
		{
			IntPtr pnt = IntPtr.Zero;
			bool hasError = false;
			Exception inner = null;
			try {
				int size = Marshal.SizeOf (data [0]) * data.Length;
				pnt = Marshal.AllocHGlobal (size);
				Marshal.Copy (data, 0, pnt, data.Length);
				int bytesWritten = Libc.write (fd, pnt,(uint) size);
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
		}
		
		public byte[] Read (int length)
		{
			
			byte[] reply = new byte[length];
            Exception inner = null;
            IntPtr pnt = IntPtr.Zero;
            int bytesRead = 0;
            bool hasError = false;
            try{
                pnt = Marshal.AllocHGlobal(Marshal.SizeOf(reply[0]) * length);
                bytesRead =  Libc.read(fd, pnt, (uint) length);
                if(bytesRead == -1)
                {
                	hasError = true;
                	Marshal.FreeHGlobal(pnt);
                    pnt = IntPtr.Zero;
                }
                else
                {
                	if(bytesRead != length)
                		reply = new byte[bytesRead];
                	Marshal.Copy(pnt, reply, 0, bytesRead);
                	Marshal.FreeHGlobal(pnt);
                    pnt = IntPtr.Zero;	
                }
            
            }
            catch(Exception e){
            	hasError = true;
				inner = e;    
            }
            finally{
                if(pnt != IntPtr.Zero)
                    Marshal.FreeHGlobal(pnt);    
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
		
		public int IoCtl (int cmd, byte[] data)
		{
			IntPtr pnt = IntPtr.Zero;
			bool hasError = false;
			Exception inner = null;
			int result = -1;
			try {
				
				if (data.Length > 0)
				{
					int size = Marshal.SizeOf(typeof(byte));
					pnt = Marshal.AllocHGlobal (size);
					Marshal.Copy (data, 0, pnt, data.Length);
				}
				result = Libc.ioctl(fd, cmd, pnt);
				if (result == -1){
					hasError = true;
				}
				else{
					Marshal.Copy(pnt, data, 0, data.Length);
				}
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
		
		public void Write (byte[] data)
		{
			Marshal.Copy(data,0, ptr, data.Length);
		}
		
		/// <summary>
		/// Write a byte array to a memory area
		/// </summary>
		/// <param name="offset">Offset where to write the data to</param>
		/// <param name="data">Data to write</param>
		public void Write(int offset, byte[] data)
		{
			if (offset+data.Length > size)
				throw new IndexOutOfRangeException(string.Format("Out of range accessing index {0}, max {1}", offset+data.Length, size));
			byte[] currentData = Read();
			Array.Copy(data,0, currentData, offset, data.Length);
			Write(currentData);
		}
		
		/// <summary>
		/// Copy the memory area into a array and return it
		/// </summary>
		public byte[] Read ()
		{
			byte[] currentData = new byte[size];
            Marshal.Copy(ptr, currentData, 0, (int)size);
            return currentData;
		}
		
		/// <summary>
		/// Copy the memory area into a array and return it
		/// </summary>
		/// <param name="offset">Read offset</param>
		/// <param name="length">Length of the bytes to read</param>
		public byte[] Read (int offset, int length)
		{
			if (offset+length > size)
				throw new IndexOutOfRangeException(string.Format("Out of range accessing index {0}, max {1}", offset+length, size));
			byte[] currentData = Read();
            byte[] reply = new byte[length];
            Array.Copy(currentData,offset, reply, 0, length);
            return reply;	
		}
		
		
	}
	
}

