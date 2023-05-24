using System;
using System.Collections.Generic;

namespace Randstad.RSM.PollingService.PwP.Models
{
    public class Timesheet
    {
        public Timesheet()
        {
            Id = Guid.NewGuid();
            CorrelationId = Id;
            Errors = new List<TimesheetError>();
        }

        public Guid Id { get; set; }
        public Guid CorrelationId { get; set; }
        public DateTimeOffset Date { get; set; }
        public string OpCo { get; set; }
        public string ClientRef { get; set; }
        public string ClientName { get; set; }
        public string SystemName { get; set; }
        public string TimesheetNumberPrefix { get; set; }
        public string EmployerRef { get; set; }
        public decimal? InvoiceNetAmount { get; set; }
        public decimal? InvoiceVatAmount { get; set; }
        public decimal? InvoiceGrossAmount { get; set; }
        public string PurchaseOrder { get; set; }
        public string InvoiceNumber { get; set; }
        public DateTimeOffset? InvoiceDate { get; set; }
        public string ExternalTimesheetReference { get; set; }
        public string TimesheetPrefix { get; set; }
        public string TimesheetReference { get; set; }
        public string Department { get; set; }
        public string Division { get; set; }
        public string RateFrequency { get; set; }
        public string Job { get; set; }
        public bool? IsAdjustment { get; set; }

        public List<TimesheetLine> Lines { get; set; }
        public List<TimesheetError> Errors { get; set; }

        #region assignment
        public bool? CreateAssignments { get; set; }
        public string AssignmentReference { get; set; }
        public DateTimeOffset? AssignmentStartDate { get; set; }
        public DateTimeOffset? AssignmentEndDate { get; set; }
        #endregion assignment

        #region worker
        public string WorkerReference { get; set; }
        public bool? CreateManagedAgencyWorkers { get; set; }
        public bool? WorkerIsManagedAgencyWorker { get; set; }
        public DateTimeOffset? WorkerDateOfBirth { get; set; }
        public string WorkerFirstName { get; set; }
        public string WorkerLastName { get; set; }
        public string VMSWorkerReference { get; set; }
        public string WorkerGender { get; set; }
        public string SupplierReference { get; set; }
        public string WorkerAddressLine1 { get; set; }
        public string WorkerAddressLine2 { get; set; }
        public string WorkerPostCode { get; set; }
        #endregion worker

    }

    public class TimesheetLine
    {
        public TimesheetLine()
        {
            Id = Guid.NewGuid();
        }

        public Guid Id { get; set; }
        public Guid TimesheetId { get; set; }
        public string RateCode { get; set; }
        public decimal Hours { get; set; }
        public decimal PayRate { get; set; }
        public decimal BillRate { get; set; }
        public string RateType { get; set; }
        public string PurchaseOrderNumber { get; set; }
        public decimal? SupplierRate { get; set; }
        public string RateDescription { get; set; }
        public string RateTreatmentIdentifier { get; set; }
    }

    public class TimesheetError
    {
        public TimesheetError()
        {
            Id = Guid.NewGuid();
        }
        public Guid Id { get; set; }
        public Guid TimesheetId { get; set; }
        public string Message { get; set; }
    }

}
