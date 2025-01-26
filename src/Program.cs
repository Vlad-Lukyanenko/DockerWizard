using DockerWizard.Commands;
using DockerWizard.Services;
using DockerWizard.Settings;
using Microsoft.Extensions.Configuration;

namespace DockerWizard
{
    internal class Program
    {
        public static ServerCredentials? _serverCredentials { get; private set; }
        private static FilePocketCommands? _filePocketCommands;
        private static DockerService? _dockerService;
        private static string? _filePocketRootFolder;

        private static async Task Main()
        {
            InitApplication();

            Console.WriteLine("Welcome to Docker Wizard!");
            Console.WriteLine("To deploy FilePocket IU press - [ 1 ]");

            var key = Console.ReadKey();

            if (key.KeyChar == '1')
            {
                Console.WriteLine("\n\n[ Local commands ]\n");
                await RunCreateAndPushCommands(_filePocketCommands!.CreateAndPushCommands!);

                Console.WriteLine("[ Server commands ]\n");
                await PullAndDeploy(_filePocketCommands!.PullAndDeployCommands!);
            }
        }

        private static void InitApplication()
        {
            var builder = new ConfigurationBuilder().AddJsonFile("appsettings.json", optional: false);
            IConfiguration config = builder.Build();

            _serverCredentials = config.GetSection("ServerCredentials").Get<ServerCredentials>()!;

            _filePocketCommands = config.GetSection("FilePocketCommands").Get<FilePocketCommands?>()!;
            _filePocketRootFolder = config.GetSection("FilePocketRootFolder").Get<string?>()!;

            _dockerService = new DockerService(_serverCredentials);
        }

        private static async Task RunCreateAndPushCommands(List<DockerCommand> commands)
        {
            foreach (var c in commands)
            {
                Console.WriteLine(c.Name);
                await _dockerService!.RunLocalCommand(c.Command, _filePocketRootFolder!);
                Console.WriteLine();
            }
        }

        private static async Task PullAndDeploy(List<DockerCommand> commands)
        {
            foreach (var c in commands)
            {
                Console.WriteLine(c.Name);
                await _dockerService!.RunRemoteCommand(c.Command);
            }
        }
    }
}
