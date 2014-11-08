using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using VideoEncoder;

namespace FFMpegLib
{
    public enum OutputMode
    {
        Mp3
    }

    public class FFMpegInstance : IDisposable
    {
        private String tempPath;
        private String executablePath;
        private String outputPath;

        public FFMpegInstance()
        {
            var stream = GetType().Assembly.GetManifestResourceStream("FFMpegLib.Resources.ffmpeg.exe");
            if (stream == null)
            {
                throw new FileLoadException("Could not find the ffmpeg resource");
            }

            var tempPath = Path.Combine(Path.GetTempPath(), "B3");

            executablePath = Path.Combine(tempPath, "ffmpeg.exe");

            Directory.CreateDirectory(tempPath);
            
            if (!File.Exists(executablePath))
            {
                byte[] bytes = new byte[stream.Length];
                stream.Read(bytes, 0, bytes.Length);
                File.WriteAllBytes(executablePath, bytes);
            }
        }

        public String ExecutablePath { get { return executablePath; } }

        public String OutputPath { get { return outputPath; } }

        public String TempPath { get { return tempPath; } }

        public Encoder CreateEncoder()
        {
            //TODO: Implement outputMode
            var encoder = new Encoder();
            encoder.FFmpegPath = executablePath;
            return encoder;
        }

        public void Dispose()
        {
            try
            {
                File.Delete(executablePath);
            }
            catch (Exception e)
            {
                // Could not delete file, maybe it is in use
            }
        }
    }
}
