using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FFMpegLib
{
    public class FFMpegInstance : IDisposable
    {
        private String tempPath;
        private String executablePath;
        private String outputPath;

        public FFMpegInstance(Stream stream)
        {
            tempPath = Path.Combine(Path.GetTempPath(), "B3");

            executablePath = Path.Combine(tempPath, "ffmpeg.exe");
            outputPath = Path.Combine(tempPath, "output");

            Directory.CreateDirectory(outputPath);

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
