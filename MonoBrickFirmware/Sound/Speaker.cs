using System;
using System.IO;
using MonoBrickFirmware.Native;

namespace MonoBrickFirmware.Sound
{
	
	
	public enum AudioMode{ Break = 0, Tone = 1, Play = 2, Repeat = 3, Service = 4}
		
	
	public class Speaker
	{
		
		private UnixDevice soundDevice = new UnixDevice("/dev/lms_sound");
		//private MemoryArea soundMemory;
		private	int currentVolume;
		private const UInt16 beepFrequency = 600;
		private const UInt16 buzzFrequency = 100;
		private const UInt16 clickFrequency = 100;
		
		private const UInt16 buzzDurationMs = 300;
		private const UInt16 beepDurationMs = 300;
		private const UInt16 clickDurationMs = 100;
		
		private const int RiffHeaderSize = 44;
		//private const int ChunkId = 0x52494646;//"RIFF"
    	private const int ChunkId = 0x46464952;//"RIFF"
    	//private const int WaveFormat = 0x57415645;//"WAVE"
    	private const int WaveFormat = 0x45564157;//"WAVE"
    	//private const int SubChunkId = 0x666d7420;//"fmt "
    	private const int SubChunkId = 0x20746d66;//"fmt "
    	//private const UInt16 AudioFormat = 0x0100;//PCM
    	private const UInt16 AudioFormat = 0x0001;//PCM
    	//private const UInt16 NumChannels = 0x0100;//mono
    	private const UInt16 NumChannels = 0x0001;//mono
    	//private const UInt16 BitsPerSample  = 0x0800;
    	private const UInt16 BitsPerSample  = 0x0008;
    	//private const int SubChunk2Id = 0x64617461;//"data"
    	private const int SubChunk2Id = 0x61746164;//"data"
    
    	private const int BufferSize = 250;
		
		
		public Speaker (int volume)
		{
			currentVolume = volume;
		}
		
		public int Volume {
			get{return currentVolume;}
			set{currentVolume = value; }
		}
		
		/// <summary>
		/// Play a tone.
		/// </summary>
		/// <param name="volume">Volume.</param>
		/// <param name="frequency">Frequency of the tone</param>
		/// <param name="durationMs">Duration in ms.</param>
		public void PlayTone(UInt16 frequency, UInt16 durationMs){
			PlayTone(frequency,durationMs, Volume);
		}

		/// <summary>
		/// Play a tone.
		/// </summary>
		/// <param name="volume">Volume.</param>
		/// <param name="frequency">Frequency of the tone</param>
		/// <param name="durationMs">Duration in ms.</param>
		/// <param name="durationMs">Volume .</param>
		public void PlayTone(UInt16 frequency, UInt16 durationMs, int volume){
			if (volume < 0)
		    	volume = -volume;
			var command = new MonoBrickFirmware.Tools.ByteArrayCreator();
			command.Append(AudioMode.Tone);
			command.Append((byte)volume);
			command.Append(frequency);
			command.Append(durationMs);
			command.Print();
			soundDevice.Write(command.Data);
			System.Threading.Thread.Sleep(durationMs);
		}
		
		/// <summary>
		/// Make the brick beep
		/// </summary>
		public void Beep(){
			Beep(beepDurationMs, Volume);
		}
		
		
		/// <summary>
		/// Make the brick beep
		/// </summary>
		/// <param name="durationMs">Duration in ms.</param>
		public void Beep(UInt16 durationMs){
			Beep(durationMs, Volume);
		}
		
		/// <summary>
		/// Make the brick beep
		/// </summary>
		/// <param name="durationMs">Duration in ms.</param>
		/// <param name="volume">Volume of the beep</param>
		public void Beep(UInt16 durationMs, int volume){
			PlayTone(beepFrequency, durationMs,volume);
		}
		
		/// <summary>
		/// Make the brick buzz
		/// </summary>
		public void Buzz ()
		{
			Buzz(buzzDurationMs, Volume);
		}
		
		/// <summary>
		/// Make the brick buzz
		/// </summary>
		/// <param name="durationMs">Duration in ms.</param>
		public void Buzz (UInt16 durationMs)
		{
			Buzz(durationMs, Volume);
		}

		/// <summary>
		/// Make the brick buzz
		/// </summary>
		/// <param name="durationMs">Duration in ms.</param>
		/// <param name="volume">Volume of the beep</param>
		public void Buzz (UInt16 durationMs, int volume)
		{
			PlayTone(buzzFrequency, durationMs, volume);
		}
		
		/// <summary>
		/// Make the brick click
		/// </summary>
		public void Click ()
		{
			Click(Volume);
		}
		
		/// <summary>
		/// Make the brick click
		/// </summary>
		public void Click (int volume)
		{
			//Click(clickDurationMs, Volume);
		}
		
		/// <summary>
		/// Play a sound file.
		/// </summary>
		/// <param name="name">Name the name of the file to play</param>
		/// <param name="volume">Volume.</param>
		/// <param name="repeat">If set to <c>true</c> the file will play in a loop</param>
		public void PlaySoundFile(string name){
			PlaySoundFile(name, Volume);
		}

		/// <summary>
		/// Play a sound file.
		/// </summary>
		/// <param name="name">File to play</param>
		/// <param name="volume">Volume.</param>
		/// <param name="repeat">If set to <c>true</c> the file will play in a loop</param>
		public void PlaySoundFile (string name, int volume)
		{
			if (new FileInfo (name).Length < RiffHeaderSize)
				throw new IOException ("Not a valid sound file");
			var audioFileStream = File.Open (name, FileMode.Open);
			byte[] headerData = new byte[RiffHeaderSize];
			audioFileStream.Read (headerData, 0, RiffHeaderSize);
			
			//check file
			bool headerOK = true;
			if (BitConverter.ToInt32 (headerData, 0) != ChunkId)
				headerOK = false;
			if (BitConverter.ToInt32 (headerData, 8) != WaveFormat)
				headerOK = false;
			if (BitConverter.ToInt32 (headerData, 12) != SubChunkId)
				headerOK = false;
			if (BitConverter.ToUInt16 (headerData, 20) != AudioFormat)
				headerOK = false;
			if (BitConverter.ToUInt16 (headerData, 22) != NumChannels)
				headerOK = false;
			//if (BitConverter.ToUInt16 (headerData, 32) != BitsPerSample)
			//	headerOK = false;
			if (!headerOK) {
				audioFileStream.Close ();
				throw new IOException ("Not a valid sound file");
			}
			try {
				
			byte []buffer = new byte[2];
            // get ready to play, set the volume
           	buffer [0] = (byte)AudioMode.Play;
			buffer [1] = (byte)volume;
			soundDevice.Write(buffer);
            
            
            int size = (int)audioFileStream.Length;
			int totalWritten  =  RiffHeaderSize;
            buffer = new byte[BufferSize*4 + 1];
            audioFileStream.Read (buffer, 1, BufferSize*4);
			int dataLen = BufferSize*4; 
			while(dataLen > 0)
            {
                // now make sure we write all of the data
                int offset = 0;
                while (offset < dataLen)
                {
                    buffer[offset] = (byte)AudioMode.Play;
                    int len = dataLen - offset;
                    if (len >BufferSize) 
                    	len = BufferSize;
					byte[] data = new byte[len+1];
					Array.Copy(buffer, offset,data,0,len+1);
					soundDevice.Write(data);
					int written = len+1;
                    /*if (written == 0)
                    {
                        Delay.msDelay(1);
                    }
                    else*/
                    offset += written;
                    totalWritten += written-1;
                }
				if(size - totalWritten >= BufferSize*4){
					audioFileStream.Read (buffer, 1, BufferSize*4);
					dataLen = BufferSize*4;
				}
				else{
					audioFileStream.Read (buffer, 1, size- totalWritten);
						dataLen = size-totalWritten;
				}
            }
				
				
				/*byte[] buffer = new byte[2];
				buffer [0] = (byte)AudioMode.Play;
				if (volume < 0)
					volume = -volume;
				buffer [1] = (byte)volume;
				soundDevice.Write (buffer);
				buffer = new byte[BufferSize + 1];
				
				int size = (int)audioFileStream.Length;
				int offset = RiffHeaderSize;
				while (offset < size) {
					if (size - offset >= BufferSize) {
						buffer [0] = (byte)AudioMode.Play;
						audioFileStream.Read (buffer, 1, BufferSize);
						MonoBrickFirmware.Display.LcdConsole.WriteLine(audioFileStream.Position.ToString());
						soundDevice.Write (buffer);
						offset += (BufferSize);
					} else {
						buffer = new byte[size - offset + 1];
						buffer [0] = (byte)AudioMode.Play;
						audioFileStream.Read (buffer, 1, size - offset);
						soundDevice.Write (buffer);
						offset += (size - offset);
						 
					}
				}*/
			} 
			catch(Exception e)
			{
				audioFileStream.Close();
				throw e;
			}
			audioFileStream.Close();
		}
		
		/// <summary>
		/// Stops all sound playback.
		/// </summary>
		public void StopSoundPlayback(){
			/*var command = new Command(0,0,123,reply);
			command.Append(ByteCodes.Sound);
			command.Append(SoundSubCodes.Break);
			connection.Send(command);
			if(reply){
				var brickReply = connection.Receive();
				Error.CheckForError(brickReply,123);
			}*/
		}
		
		
		
	}
}

