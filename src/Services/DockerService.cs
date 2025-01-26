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

        public async Task RunLocalCommand(string command, string workingDirectory)
        {
            // Создаем процесс
            Process process = new Process();

            process.StartInfo.FileName = "cmd";
            process.StartInfo.Arguments = $"/c {command}";
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.WorkingDirectory = workingDirectory;

            // Запуск процесса
            process.Start();

            // Чтение стандартного вывода в реальном времени
            var outputTask = Task.Run(() =>
            {
                while (!process.StandardOutput.EndOfStream)
                {
                    string line = process.StandardOutput.ReadLine();
                    Console.WriteLine(line);
                }
            });

            // Чтение стандартного потока ошибок в реальном времени
            var errorTask = Task.Run(() =>
            {
                while (!process.StandardError.EndOfStream)
                {
                    string line = process.StandardError.ReadLine();

                    // Проверяем, является ли строка реальной ошибкой
                    if (line.Contains("error", StringComparison.OrdinalIgnoreCase) ||
                        line.Contains("failed", StringComparison.OrdinalIgnoreCase))
                    {
                        Console.WriteLine($"Error: {line}");
                    }
                    else
                    {
                        Console.WriteLine($"[INFO] {line}");
                    }
                }
            });

            // Ожидание завершения процесса
            await Task.WhenAll(outputTask, errorTask);
            process.WaitForExit();

            // Проверка кода завершения
            if (process.ExitCode != 0)
            {
                Console.WriteLine($"Command failed with exit code: {process.ExitCode}");
            }
        }

        public async Task RunRemoteCommand(string command)
        {
            try
            {
                // Подключение к серверу
                await _sshClient.ConnectAsync(CancellationToken.None);

                // Создание команды
                using (var sshCommand = _sshClient.CreateCommand(command))
                {
                    // Асинхронное чтение вывода
                    var outputTask = Task.Run(async () =>
                    {
                        using (var reader = new System.IO.StreamReader(sshCommand.OutputStream))
                        {
                            while (!reader.EndOfStream)
                            {
                                string line = await reader.ReadLineAsync();
                                Console.WriteLine(line);
                            }
                        }
                    });

                    // Асинхронное чтение ошибок
                    var errorTask = Task.Run(async () =>
                    {
                        using (var reader = new System.IO.StreamReader(sshCommand.ExtendedOutputStream))
                        {
                            while (!reader.EndOfStream)
                            {
                                string line = await reader.ReadLineAsync();
                                Console.WriteLine($"Error: {line}");
                            }
                        }
                    });

                    // Запуск команды
                    sshCommand.Execute();

                    // Ожидание завершения чтения вывода и ошибок
                    await Task.WhenAll(outputTask, errorTask);

                    // Проверка кода завершения
                    if (sshCommand.ExitStatus != 0)
                    {
                        Console.WriteLine($"Command failed with exit code: {sshCommand.ExitStatus}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
            finally
            {
                // Отключение от сервера
                _sshClient.Disconnect();
            }
        }
    }
}
