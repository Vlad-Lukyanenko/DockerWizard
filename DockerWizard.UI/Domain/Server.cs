namespace DockerWizard.UI.Domain
{
    public class Server
    {
        public string Name { get; set; } = string.Empty;
        public ServerCredentials? Credentials { get; set; }
    }
}
