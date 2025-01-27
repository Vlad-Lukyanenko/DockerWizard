namespace DockerWizard.UI.Domain
{
    public class ProjectAndDockerCommands
    {
        public string ProjectName { get; set; } = string.Empty;
        public string Path { get; set; } = string.Empty;
        public List<DockerCommand>? CreateAndPushCommands { get; set; }
        public List<DockerCommand>? PullAndDeployCommands { get; set; }
    }
}
