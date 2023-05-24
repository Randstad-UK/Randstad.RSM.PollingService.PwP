
using Randstad.Environments;

namespace Randstad.RSM.PollingService.PwP.Settings
{
    public class ApplicationSettings
    {
        public string RoutingKey { get; set; }
        public DeploymentEnvironment Environment { get; set; }
        public string ServiceName { get; set; }
        public int MaxPollingIntervalInSeconds { get; set; }
        public int PollingIntervalInSeconds { get; set; }
        public int PollingIntervalIncrementInSeconds { get; set; }
        public int MaxKnownErrorsCount { get; set; }
        public string ArchiveDirectory { get; set; }
        public string SftpDropZone { get; set; }
        public string ValidFilePattern { get; set; }
    }
}
