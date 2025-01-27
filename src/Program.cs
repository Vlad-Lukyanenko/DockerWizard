using DockerWizard.Commands;
using DockerWizard.Services;
using DockerWizard.Settings;
using Microsoft.Extensions.Configuration;

namespace DockerWizard
{
    internal class Program
    {
        public static ServerCredentials? _serverCredentials { get; private set; }
        private static FilePocketUICommands? _filePocketUICommands;
        private static FilePocketAPICommands? _filePocketAPICommands;
        private static DockerService? _dockerService;
        private static string? _filePocketRootFolder;

        private static async Task Main()
        {
            InitApplication();

            Console.WriteLine("Welcome to Docker Wizard!\n");
            Console.WriteLine("To deploy [ FilePocket FE ] press - [ 1 ]");
            Console.WriteLine("To deploy [ FilePocket BE ] press - [ 2 ]");

            Console.WriteLine();

            var key = Console.ReadKey();

            if (key.KeyChar == '1')
            {
                Console.WriteLine("\nBuild and Deploy FilePocket UI");

                Console.WriteLine("\n[ Local commands ]\n");
                await RunCreateAndPushCommands(_filePocketUICommands!.CreateAndPushCommands!);

                Console.WriteLine("[ Server commands ]\n");
                await PullAndDeploy(_filePocketUICommands!.PullAndDeployCommands!);
            }

            if (key.KeyChar == '2')
            {
                Console.WriteLine("\n\nBuild and Deploy FilePocket Host");

                Console.WriteLine("\n[ Local commands ]\n");
                await RunCreateAndPushCommands(_filePocketAPICommands!.CreateAndPushCommands!);

                Console.WriteLine("[ Server commands ]\n");
                await PullAndDeploy(_filePocketAPICommands!.PullAndDeployCommands!);
            }
        }

        private static void InitApplication()
        {
            Console.WriteLine("Application setup\n");
            var builder = new ConfigurationBuilder().AddJsonFile("appsettings.json", optional: false);
            IConfiguration config = builder.Build();

            _serverCredentials = config.GetSection("ServerCredentials").Get<ServerCredentials>()!;

            Console.WriteLine("Init Docker commands\n");
            _filePocketUICommands = config.GetSection("FilePocketUICommands").Get<FilePocketUICommands?>()!;
            _filePocketAPICommands = config.GetSection("FilePocketAPICommands").Get<FilePocketAPICommands?>()!;
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
