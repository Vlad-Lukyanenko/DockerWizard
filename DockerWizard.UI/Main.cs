using DockerWizard.UI.Domain;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Renci.SshNet;
using System.Diagnostics;

namespace DockerWizard.UI
{
    public partial class Main : Form
    {
        private SshClient? _sshClient;

        private string _selectedServer = string.Empty;
        private string _selectedProject = string.Empty;

        private List<Server>? _servers;
        private GitSettings? _gitSettings;
        private List<ProjectAndDockerCommands> _projectsAndDockerCommands = new List<ProjectAndDockerCommands>();

        public Main()
        {
            InitializeComponent();

            AddLog("Application setup");

            InitSettings();

            InitServers(_servers);
            InitProjects(_projectsAndDockerCommands);
        }

        private void InitServers(List<Server> servers)
        {
            foreach (var s in servers)
            {
                var item = new ComboboxItem()
                {
                    Text = s.Name,
                    Value = s.Name
                };

                ServersComboBox.Items.Add(item);
            }
        }

        private void InitProjects(List<ProjectAndDockerCommands> projects)
        {
            foreach (var s in projects)
            {
                var item = new ComboboxItem()
                {
                    Text = s.ProjectName,
                    Value = s.ProjectName
                };

                ProjectsComboBox.Items.Add(item);
            }
        }

        private void ServersComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            var selectedItem = ServersComboBox.SelectedItem;

            _selectedServer = (string)(selectedItem as ComboboxItem)!.Value;
        }

        private void ProjectsComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            var selectedItem = ProjectsComboBox.SelectedItem;

            _selectedProject = (string)(selectedItem as ComboboxItem)!.Value;
        }

        private async void DeployButton_Click(object sender, EventArgs e)
        {
            var server = _servers!.FirstOrDefault(c => c.Name == _selectedServer);
            var project = _projectsAndDockerCommands!.FirstOrDefault(c => c.ProjectName == _selectedProject);

            if (server?.Credentials is null)
            {
                return;
            }

            if (project?.CreateAndPushCommands is null)
            {
                return;
            }

            if (project?.PullAndDeployCommands is null)
            {
                return;
            }

            _sshClient = new SshClient(
                server.Credentials.Host,
                server.Credentials.UserName,
                server.Credentials.Password);

            AddLog($"Build and Deploy - {project.ProjectName}");
            AddLog("[ Local commands ]");

            project.CreateAndPushCommands.Single(c => c.Name == "Build").Command
                += $" https://{_gitSettings!.AccessToken}@github.com/{_gitSettings.UserName}/{project!.GitRepository!.RepositoryName}.git#{project.GitRepository.BranchName}";

            await RunCreateAndPushCommands(project.CreateAndPushCommands);

            AddLog("[ Server commands ]");
            await PullAndDeploy(project.PullAndDeployCommands);

            AddLog(" - [ deployment finished successfully ] - ".ToUpper());
        }

        private void AddLog(string message)
        {
            if (LogsListBox.InvokeRequired)
            {
                LogsListBox.BeginInvoke(new Action(() => AddLog(message)));
            }
            else
            {
                LogsListBox.Items.Add(message);
            }
        }

        private async Task RunCreateAndPushCommands(List<DockerCommand> commands)
        {
            foreach (var c in commands)
            {
                AddLog(c.Name);
                await RunLocalCommand(c.Command);
            }
        }

        private async Task PullAndDeploy(List<DockerCommand> commands)
        {
            foreach (var c in commands)
            {
                AddLog(c.Name);
                await RunRemoteCommand(c.Command);
            }
        }

        public async Task RunLocalCommand(string command)
        {
            var process = new Process();

            process.StartInfo.FileName = "cmd";
            process.StartInfo.Arguments = $"/c {command}";
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;

            process.Start();

            var outputTask = Task.Run(() =>
            {
                while (!process.StandardOutput.EndOfStream)
                {
                    string line = process.StandardOutput.ReadLine();
                    AddLog(line);
                }
            });

            var errorTask = Task.Run(() =>
            {
                while (!process.StandardError.EndOfStream)
                {
                    string line = process.StandardError.ReadLine();

                    if (line.Contains("error", StringComparison.OrdinalIgnoreCase) ||
                        line.Contains("failed", StringComparison.OrdinalIgnoreCase))
                    {
                        AddLog($"Error: {line}");
                    }
                    else
                    {
                        AddLog($"[INFO] {line}");
                    }
                }
            });

            await Task.WhenAll(outputTask, errorTask);
            process.WaitForExit();

            if (process.ExitCode != 0)
            {
                AddLog($"Command failed with exit code: {process.ExitCode}");
            }
        }

        public async Task RunRemoteCommand(string command)
        {
            try
            {
                await _sshClient.ConnectAsync(CancellationToken.None);

                using (var sshCommand = _sshClient.CreateCommand(command))
                {
                    var outputTask = Task.Run(async () =>
                    {
                        using (var reader = new System.IO.StreamReader(sshCommand.OutputStream))
                        {
                            while (!reader.EndOfStream)
                            {
                                string line = await reader.ReadLineAsync();
                                AddLog(line);
                            }
                        }
                    });

                    var errorTask = Task.Run(async () =>
                    {
                        using (var reader = new System.IO.StreamReader(sshCommand.ExtendedOutputStream))
                        {
                            while (!reader.EndOfStream)
                            {
                                string line = await reader.ReadLineAsync();
                                AddLog($"Error: {line}");
                            }
                        }
                    });

                    sshCommand.Execute();

                    await Task.WhenAll(outputTask, errorTask);

                    if (sshCommand.ExitStatus != 0)
                    {
                        AddLog($"Command failed with exit code: {sshCommand.ExitStatus}");
                    }
                }
            }
            catch (Exception ex)
            {
                AddLog($"An error occurred: {ex.Message}");
            }
            finally
            {
                _sshClient.Disconnect();
            }
        }

        private void InitSettings()
        {
            var configFolder = Path.Combine(Directory.GetCurrentDirectory(), "ConfigFiles");

            var builder = new ConfigurationBuilder();

            // Get all JSON files from the directory
            if (Directory.Exists(configFolder))
            {
                foreach (var file in Directory.GetFiles(configFolder, "*.json"))
                {
                    builder.AddJsonFile(file, optional: true, reloadOnChange: true);

                }
            }

            IConfiguration config = builder.Build();

            _servers = config.GetSection("Servers").Get<List<Server>>()!;

            _gitSettings = config.GetSection("GitSettings").Get<GitSettings>()!;

            int fileCount = builder.Sources
            .OfType<JsonConfigurationSource>()
            .Count();

            for (int i = 0; i < fileCount - 1; i++)
            {
                var temp = config.GetSection($"ProjectAndDockerCommands.{i+1}").Get<List<ProjectAndDockerCommands>>()!;
                _projectsAndDockerCommands.AddRange(temp);
            }
        }
    }

    public class ComboboxItem
    {
        public string Text { get; set; } = string.Empty;
        public object Value { get; set; } = string.Empty;

        public override string ToString()
        {
            return Text;
        }
    }
}
