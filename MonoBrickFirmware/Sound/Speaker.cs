using System;
using System.IO;
using MonoBrickFirmware.Native;
namespace MonoBrickFirmware.Sound
{
    public enum AudioMode { Break = 0, Tone = 1, Play = 2, Repeat = 3, Service = 4 }
    public class Speaker
    {
        private UnixDevice soundDevice = new UnixDevice("/dev/lms_sound");
        //private MemoryArea soundMemory;
        private int currentVolume;
        private const UInt16 beepFrequency = 600;
        private const UInt16 buzzFrequency = 100;
        private const UInt16 clickFrequency = 100;
        private const UInt16 buzzDurationMs = 300;
        private const UInt16 beepDurationMs = 300;
        private const UInt16 clickDurationMs = 100;
        private const int RIFF_HDR_SIZE = 44;
        private const int RIFF_RIFF_SIG = 0x52494646;
        private const int RIFF_WAVE_SIG = 0x57415645;
        private const int RIFF_FMT_SIG = 0x666d7420;
        private const short RIFF_FMT_PCM = 0x0100;
        private const short RIFF_FMT_1CHAN = 0x0100;
        private const short RIFF_FMT_8BITS = 0x0800;
        private const int RIFF_DATA_SIG = 0x64617461;
        private const int PCM_BUFFER_SIZE = 250;
        public Speaker(int volume)
        {
            currentVolume = volume;
        }
        public int Volume
        {
            get { return currentVolume; }
            set { currentVolume = value; }
        }
        /// <summary>
        /// Play a tone.
        /// </summary>
        /// <param name=”volume”>Volume.</param>
        /// <param name=”frequency”>Frequency of the tone</param>
        /// <param name=”durationMs”>Duration in ms.</param>
        public void PlayTone(UInt16 frequency, UInt16 durationMs)
        {
            PlayTone(frequency, durationMs, Volume);
        }
        /// <summary>
        /// Play a tone.
        /// </summary>
        /// <param name=”volume”>Volume.</param>
        /// <param name=”frequency”>Frequency of the tone</param>
        /// <param name=”durationMs”>Duration in ms.</param>
        /// <param name=”durationMs”>Volume .</param>
        public void PlayTone(UInt16 frequency, UInt16 durationMs, int volume)
        {
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
        public void Beep()
        {
            Beep(beepDurationMs, Volume);
        }
        /// <summary>
        /// Make the brick beep
        /// </summary>
        /// <param name=”durationMs”>Duration in ms.</param>
        public void Beep(UInt16 durationMs)
        {
            Beep(durationMs, Volume);
        }
        /// <summary>
        /// Make the brick beep
        /// </summary>
        /// <param name=”durationMs”>Duration in ms.</param>
        /// <param name=”volume”>Volume of the beep</param>
        public void Beep(UInt16 durationMs, int volume)
        {
            PlayTone(beepFrequency, durationMs, volume);
        }
        /// <summary>
        /// Make the brick buzz
        /// </summary>
        public void Buzz()
        {
            Buzz(buzzDurationMs, Volume);
        }
        /// <summary>
        /// Make the brick buzz
        /// </summary>
        /// <param name=”durationMs”>Duration in ms.</param>
        public void Buzz(UInt16 durationMs)
        {
            Buzz(durationMs, Volume);
        }
        /// <summary>
        /// Make the brick buzz
        /// </summary>
        /// <param name=”durationMs”>Duration in ms.</param>
        /// <param name=”volume”>Volume of the beep</param>
        public void Buzz(UInt16 durationMs, int volume)
        {
            PlayTone(buzzFrequency, durationMs, volume);
        }
        /// <summary>
        /// Make the brick click
        /// </summary>
        public void Click()
        {
            Click(Volume);
        }
        /// <summary>
        /// Make the brick click
        /// </summary>
        public void Click(int volume)
        {
            //Click(clickDurationMs, Volume);
        }
        /// <summary>
        /// Play a sound file.
        /// </summary>
        /// <param name=”name”>Name the name of the file to play</param>
        /// <param name=”volume”>Volume.</param>
        /// <param name=”repeat”>If set to <c>true</c> the file will play in a loop</param>
        public int PlaySoundFile(string name)
        {
            return PlaySoundFile(name, Volume);
        }
        private int readLSBInt(Stream d)
        {
            int val = d.ReadByte() & 0xff;
            val |= (d.ReadByte() & 0xff) << 8;
            val |= (d.ReadByte() & 0xff) << 16;
            val |= (d.ReadByte() & 0xff) << 24;
            return val;
        }
        private int readInt(Stream d)
        {
            int val = (d.ReadByte() & 0xff) << 24;
            val |= (d.ReadByte() & 0xff) << 16;
            val |= (d.ReadByte() & 0xff) << 8;
            val |= (d.ReadByte() & 0xff);
            return val;
        }
        private Int16 readShort(Stream d)
        {
            Int16 val = (Int16)((d.ReadByte() & 0xff) << 8);
            val |= (Int16)(d.ReadByte() & 0xff);
            return val;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name=”name”></param>
        /// <param name=”volume”></param>
        /// <returns>0 == ok else error </returns>
        public int PlaySoundFile(string name, int volume)
        {
            if (new FileInfo(name).Length < RIFF_HDR_SIZE)
                throw new IOException("Not a valid sound file");
            using (var afs = File.Open(name, FileMode.Open))
            {
                int offset = 0;
                int sampleRate = 0;
                int dataLen = 0;
                //check file
                if (readInt(afs) != RIFF_RIFF_SIG)
                    return -1;
                readInt(afs);
                if (readInt(afs) != RIFF_WAVE_SIG)
                    return -2;
                if (readInt(afs) != RIFF_FMT_SIG)
                    return -3;
                offset += 16;
                int sz = readLSBInt(afs);
                if (readShort(afs) != RIFF_FMT_PCM)
                    return -4;
                if (readShort(afs) != RIFF_FMT_1CHAN)
                    return -5;
                sampleRate = readLSBInt(afs);
                readInt(afs);
                readShort(afs);
                if (readShort(afs) != RIFF_FMT_8BITS)
                    return -6;

                // Skip any data in this chunk after the 16 bytes above
                sz -= 16;
                offset += 20 + sz;
                while (sz-- > 0) // Original was (sz- > 0)
                    afs.ReadByte();
                // Skip optional chunks until we find a data sig (or we hit eof!)
                for (;;)
                {
                    int chunk = readInt(afs);
                    dataLen = readLSBInt(afs);
                    offset += 8;
                    if (chunk == RIFF_DATA_SIG) break;
                    // Skip to the start of the next chunk
                    offset += dataLen;
                    while (dataLen-- > 0)
                        afs.ReadByte();
                }
            if (volume < 0)
                volume = -volume;
            byte[] buf = new byte[2];
            // get ready to play, set the volume
            buf[0] = (byte)AudioMode.Play;
            buf[1] = (byte)volume;
            soundDevice.Write(buf);
            buf = new byte[PCM_BUFFER_SIZE * 4 + 1];
            while ((dataLen = afs.Read(buf, 1, buf.Length - 1)) > 0)
            {
                // now make sure we write all of the data
                offset = 0;
                while (offset < dataLen)
                {
                    buf[offset] = (byte)AudioMode.Service;
                    int len = dataLen - offset;
                    if (len > PCM_BUFFER_SIZE) len = PCM_BUFFER_SIZE;
                    byte[] buf2 = new byte[len + 1];
                    Array.Copy(buf, offset, buf2, 0, len + 1);
                    int bytesWritten = soundDevice.Write(buf2);
                    if (bytesWritten <= 0)
                    {
                        System.Threading.Thread.Sleep(1);
                    }
                    else
                    {
                        offset += bytesWritten;
                    }
                }
            }
        }
return 0;
}
    /// <summary>
    /// Stops all sound playback.
    /// </summary>
    public void StopSoundPlayback()
    {
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