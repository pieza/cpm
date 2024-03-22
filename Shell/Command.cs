using System.Diagnostics;

namespace cpm.Shell
{
    class Command
    {
        public required string Script { get; set; }
        public string WorkingDir { get; set; } = Directory.GetCurrentDirectory();

        public void Run()
        {
            var (fileName, arguments) = ParseScript();

            ProcessStartInfo startInfo = new()
            {
                FileName = fileName,
                Arguments = arguments,
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            };

            Process process = new()
            {
                StartInfo = startInfo,
            };

            HandleStandardOutput(process);

            process.Start();

            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            process.WaitForExit();
        }

        private static void HandleStandardOutput(Process process)
        {
            process.OutputDataReceived += (sender, e) =>
            {
                if (!string.IsNullOrEmpty(e.Data))
                {
                    Console.WriteLine(e.Data);
                }
            };

            process.ErrorDataReceived += (sender, e) =>
            {
                if (!string.IsNullOrEmpty(e.Data))
                {
                    Console.Error.WriteLine(e.Data);
                }
            };
        }

        private (string, string) ParseScript()
        {
            string[] commandParts = Script.Split([' '], 2);
            string fileName = commandParts[0];
            string arguments = commandParts.Length > 1 ? commandParts[1] : "";
            return (fileName, arguments);
        }
    }
}
