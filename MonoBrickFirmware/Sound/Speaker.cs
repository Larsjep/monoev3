using System;
using MonoBrickFirmware.Native;
using MonoBrickFirmware.Settings;

namespace MonoBrickFirmware.Sound
{
	
	
	public enum AudioMode{ Break = 0, Tone = 1, Play = 2, Repeat = 3, Service = 4}
		
	
	public class Speaker
	{
		
		private UnixDevice soundDevice = new UnixDevice("/dev/lms_sound");
		private MemoryArea soundMemory;
		private	int currentVolume;
		private bool enable;
		private const UInt16 beepFrequency = 10000;
		private const UInt16 buzzFrequency = 100;
		private const UInt16 buzzDurationMs = 200;
		private const UInt16 beepDurationMs = 200;
		
		
		public Speaker ()
		{
			currentVolume = FirmwareSettings.Instance.SoundSettings.Volume;
			enable = FirmwareSettings.Instance.SoundSettings.EnableSound;
			
		}
		
		public int Volume {
			get{return currentVolume;}
			set{currentVolume = value; }
		}
		
		public bool Enable {
			get{return enable;}
			set{enable = value; }
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
		/// Play a sound file.
		/// </summary>
		/// <param name="name">Name the name of the file to play</param>
		/// <param name="volume">Volume.</param>
		/// <param name="repeat">If set to <c>true</c> the file will play in a loop</param>
		public void PlaySoundFile(string name, byte volume){
			PlaySoundFile(name, volume, false);
		}

		/// <summary>
		/// Play a sound file.
		/// </summary>
		/// <param name="name">File to play</param>
		/// <param name="volume">Volume.</param>
		/// <param name="repeat">If set to <c>true</c> the file will play in a loop</param>
		public void PlaySoundFile(string name, byte volume, bool repeat){
			// First check that we have a wave file. File must be at least 44 bytes
        	// in size to contain a RIFF header.
        	/*if (file.length() < RIFF_HDR_SIZE)
            	return -9;
        	// Now check for a RIFF header
        	FileInputStream f = null; 
        	DataInputStream d = null;
        	try
        	{
	        	f = new FileInputStream(file);
	        	d = new DataInputStream(f);
	
	            if (d.readInt() != RIFF_RIFF_SIG)
	                return -1;
	            // Skip chunk size
	            d.readInt();
	            // Check we have a wave file
	            if (d.readInt() != RIFF_WAVE_SIG)
	                return -2;
	            if (d.readInt() != RIFF_FMT_SIG)
	                return -3;
	            int offset = 16;
	            // Now check that the format is PCM, Mono 8 bits. Note that these
	            // values are stored little endian.
	            int sz = readLSBInt(d);
	            if (d.readShort() != RIFF_FMT_PCM)
	                return -4;
	            if (d.readShort() != RIFF_FMT_1CHAN)
	                return -5;
	            int sampleRate = readLSBInt(d);
	            d.readInt();
	            d.readShort();
	            if (d.readShort() != RIFF_FMT_8BITS)
	                return -6;
	            // Skip any data in this chunk after the 16 bytes above
	            sz -= 16;
	            offset += 20 + sz;
	            while (sz-- > 0)
	                d.readByte();
	            int dataLen;
	            // Skip optional chunks until we find a data sig (or we hit eof!)
	            for(;;)
	            {
	                int chunk = d.readInt();
	                dataLen = readLSBInt(d); 
	                offset += 8;
	                if (chunk == RIFF_DATA_SIG) break;
	                // Skip to the start of the next chunk
	                offset += dataLen;
	                while(dataLen-- > 0)
	                    d.readByte();
	            }
	            if (vol >= 0)
	                vol = (vol*masterVolume)/100;
	            else
	                vol = -vol;
	            byte []buf = new byte[PCM_BUFFER_SIZE*4+1];
	            // get ready to play, set the volume
	            buf[0] = OP_PLAY;
	            buf[1] = (byte)vol;
	            dev.write(buf, 2);
	            // now play the file
	            buf[1] = 0;
	            while((dataLen = d.read(buf, 1, buf.length - 1)) > 0)
	            {
	                // now make sure we write all of the data
	                offset = 0;
	                while (offset < dataLen)
	                {
	                    buf[offset] = OP_SERVICE;
	                    int len = dataLen - offset;
	                    if (len > PCM_BUFFER_SIZE) len = PCM_BUFFER_SIZE;
	                    int written = dev.write(buf, offset, len + 1);
	                    if (written == 0)
	                    {
	                        Delay.msDelay(1);
	                    }
	                    else
	                        offset += written;
	                }
	            }
        	}
	        catch (IOException e)
	        {
	            return -1;
	        }
	        finally
	        {
	            try {
	                if (d != null)
	                    d.close();
	                if (f != null)
	                    f.close();
	            }
	            catch (IOException e)
	            {
	                return -1;
	            }                
	        }
	        return 0;
			
			
			
			
			
			
			
			
			Command command = null;
			if(repeat){
				command = new Command(0,0,200,reply);
				command.Append(ByteCodes.Sound);
				command.Append(SoundSubCodes.Repeat);
				command.Append(volume, ConstantParameterType.Value);
				command.Append(name, ConstantParameterType.Value);
				command.Append(ByteCodes.SoundReady);//should this be here?
			}
			else{
				command = new Command(0,0,200,reply);
				command.Append(ByteCodes.Sound);
				command.Append(SoundSubCodes.Play);
				command.Append(volume, ConstantParameterType.Value);
				command.Append(name, ConstantParameterType.Value);
				command.Append(ByteCodes.SoundReady);//should this be here?
			}
			connection.Send(command);
			if(reply){
				var brickReply = connection.Receive();
				Error.CheckForError(brickReply,200);
			}*/
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

