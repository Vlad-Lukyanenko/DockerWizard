namespace DockerWizard.UI.Domain
{
    public class ProjectAndDockerCommands
    {
        public string ProjectName { get; set; } = string.Empty;
        public GitRepository? GitRepository { get;set; }
        public List<DockerCommand>? CreateAndPushCommands { get; set; }
        public List<DockerCommand>? PullAndDeployCommands { get; set; }
    }
}
