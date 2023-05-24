using System;

namespace Randstad.RSM.PollingService.PwP.Models
{
    public class Client
    {
        public Guid ClientId { get; set; }
        public string ClientRef { get; set; }
        public string Description { get; set; }
        public string Name { get; set; }
        public string RandstadOpCo { get; set; }
        public string NotificationEmailAddresses { get; set; }
        public bool TimesheetsMustBeSignedOff { get; set; }
        public bool IdentifyAssignmentByJob { get; set; }
        public bool IdentifyAssignmentByCostCentre { get; set; }
        public bool WantsConsolidatedTimesheets { get; set; }
        public bool ValidateArchivedWorkers { get; set; }
        public string TimesheetNumberPrefix { get; set; }
        public bool ConsolidateByPurchaseOrderNumber { get; set; }
        public string TnASystem { get; set; }
        public string EmployerReference { get; set; }
        public string Department { get; set; }
        public string Division { get; set; }
        public bool CreateManagedAgencyWorkers { get; set; }
        public bool CreateAssignments { get; set; }
        public string DirectWorkersSupplierRef { get; set; }
    }
}
