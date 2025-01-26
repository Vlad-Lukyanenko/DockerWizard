using DockerWizard.Settings;
using Renci.SshNet;
using System.Diagnostics;

namespace DockerWizard.Services
{
    internal class DockerService
    {
        private readonly SshClient _sshClient;

        public DockerService(ServerCredentials credentials)
        {
            _sshClient = new SshClient(credentials.Host, credentials.UserName, credentials.Password);
        }

        public string RunLocalCommand(string command, string workingDirectory)
        {
            var psi = new ProcessStartInfo
            {
                FileName = "docker",
                Arguments = command,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                CreateNoWindow = true,
                WorkingDirectory = workingDirectory
            };

            using (var process = Process.Start(psi))
            {
                string result = process!.StandardOutput.ReadToEnd();
                process.WaitForExit();

                return result;
            }
        }

        public async Task<string> RunRemoteCommand(string command)
        {
            await _sshClient.ConnectAsync(CancellationToken.None);

            var response = _sshClient.RunCommand(command);

            _sshClient.Disconnect();

            return response.Result;
        }
    }
}
