using Randstad.Environments;

// ReSharper disable once CheckNamespace
namespace Randstad.RSM.PollingService.PwP.Template.Settings
{
    partial class ApplicationSettings
    {
        public DeploymentEnvironment Environment { get; set; }
        public string ServiceName { get; set; }
        public string PaidStatuses { get; set; }
        public string OpCo { get; set; }
        public int MaxPollingIntervalInSeconds { get; set; }
        public int PollingIntervalInSeconds { get; set; }
        public int PollingIntervalIncrementInSeconds { get; set; }
        public int MaxKnownErrorsCount { get; set; }
        public string ReportAdminRecipient { get; set; }
        public string ReportFrom { get; set; }
        public string SMTPClientHost { get; set; }
    }
}
