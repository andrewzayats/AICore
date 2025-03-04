using System.Diagnostics;

public class Audio
{
    public static string ConvertWebmBase64ToWavBase64(string base64Webm)
    {
        // Decode Base64 input to binary
        byte[] webmBytes = Convert.FromBase64String(base64Webm);

        // Create temporary file paths
        string tempInputPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".webm");
        string tempOutputPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".wav");

        try
        {
            // Write input file to disk
            File.WriteAllBytes(tempInputPath, webmBytes);

            // Convert WebM to WAV using FFmpeg
            ConvertWebmToWav(tempInputPath, tempOutputPath);

            // Read WAV file and encode as Base64
            byte[] wavBytes = File.ReadAllBytes(tempOutputPath);
            return Convert.ToBase64String(wavBytes);
        }
        finally
        {
            // Cleanup temp files
            if (File.Exists(tempInputPath)) File.Delete(tempInputPath);
            if (File.Exists(tempOutputPath)) File.Delete(tempOutputPath);
        }
    }

    private static void ConvertWebmToWav(string inputFile, string outputFile)
    {
        string ffmpegArgs = $"-i \"{inputFile}\" -ac 1 -ar 24000 -c:a pcm_s16le \"{outputFile}\"";

        ProcessStartInfo startInfo = new ProcessStartInfo
        {
            FileName = "ffmpeg",
            Arguments = ffmpegArgs,
            UseShellExecute = false,
            RedirectStandardError = true,
            RedirectStandardOutput = true,
            CreateNoWindow = true
        };

        using (Process ffmpegProcess = new Process())
        {
            ffmpegProcess.StartInfo = startInfo;
            ffmpegProcess.Start();
            ffmpegProcess.WaitForExit();

            if (ffmpegProcess.ExitCode != 0)
            {
                throw new Exception("FFmpeg conversion failed.");
            }
        }
    }
}
