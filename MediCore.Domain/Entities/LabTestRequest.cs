using MediCore.Domain.Common;
using MediCore.Domain.Enums;
using MediCore.Domain.Exceptions;

namespace MediCore.Domain.Entities;

public class LabTestRequest : BaseEntity
{
    public Guid PatientId { get; private set; }
    public Patient Patient { get; private set; } = default!;

    public Guid DoctorId { get; private set; }
    public Doctor Doctor { get; private set; } = default!;

    public Guid LabTestId { get; private set; }
    public LabTest LabTest { get; private set; } = default!;

    public Guid? LabId { get; private set; }
    public Lab? Lab { get; private set; }

    public LabTestRequestStatus Status { get; private set; } = LabTestRequestStatus.Pending;

    public string? Notes { get; private set; }

    public string? ResultFileUrl { get; private set; }

    public DateTime RequestedAt { get; private set; }
    public DateTime? CompletedAt { get; private set; }
    public DateTime? SampleCollectedAt { get; private set; }

    LabTestRequest() { }

    public LabTestRequest(Guid patientId, Guid doctorId, Guid labTestId, Guid? labId = null, string? notes = null)
    {
        if (patientId == Guid.Empty)
            throw new ArgumentException("Patient Id is Required", nameof(patientId));

        if (doctorId == Guid.Empty)
            throw new ArgumentException("Doctor Id is Required", nameof(doctorId));

        if (labTestId == Guid.Empty)
            throw new ArgumentException("Lab Test Id is Required", nameof(labTestId));

        PatientId = patientId;
        DoctorId = doctorId;
        LabTestId = labTestId;
        LabId = labId;
        Notes = notes;

        RequestedAt = DateTime.UtcNow;
        Status = LabTestRequestStatus.Pending;
    }

    public void Update(Guid patientId, Guid doctorId, Guid labTestId, Guid? labId, string? notes)
    {
        if (patientId == Guid.Empty)
            throw new ArgumentException("Patient Id is Required", nameof(patientId));

        if (doctorId == Guid.Empty)
            throw new ArgumentException("Doctor Id is Required", nameof(doctorId));

        if (labTestId == Guid.Empty)
            throw new ArgumentException("Lab Test Id is Required", nameof(labTestId));

        PatientId = patientId;
        DoctorId = doctorId;
        LabTestId = labTestId;
        LabId = labId;
        Notes = notes;

        MarkAsUpdated();
    }

    public void AssignLab(Guid labId)
    {
        if (labId == Guid.Empty)
            throw new ArgumentException("Lab Id is Required", nameof(labId));

        LabId = labId;
        MarkAsUpdated();
    }

    public void SendToLab()
    {
        if (!LabId.HasValue)
            throw new BusinessRuleViolationException("Can't send a test request without assigning a lab.");

        if (Status != LabTestRequestStatus.Pending)
            throw new BusinessRuleViolationException("Only pending test requests can be sent to a lab.");

        Status = LabTestRequestStatus.SentToLab;
        MarkAsUpdated();
    }

    public void CollectSample()
    {
        if (Status != LabTestRequestStatus.SentToLab)
            throw new BusinessRuleViolationException("Sample can only be collected after the request is sent to a lab.");

        Status = LabTestRequestStatus.SampleCollected;
        SampleCollectedAt = DateTime.UtcNow;
        MarkAsUpdated();
    }

    public void StartProcessing()
    {
        if (Status != LabTestRequestStatus.SampleCollected)
            throw new BusinessRuleViolationException("Test processing can only start after the sample is collected.");

        Status = LabTestRequestStatus.InProgress;
        MarkAsUpdated();
    }

    public void UploadResult(string resultFileUrl)
    {
        if (string.IsNullOrWhiteSpace(resultFileUrl))
            throw new ArgumentException("Result file URL is required.", nameof(resultFileUrl));

        if (Status == LabTestRequestStatus.Rejected)
            throw new BusinessRuleViolationException("Can't upload results for a rejected test request.");

        if (Status == LabTestRequestStatus.Completed)
            throw new BusinessRuleViolationException("Results have already been uploaded.");

        if (Status != LabTestRequestStatus.InProgress)
            throw new BusinessRuleViolationException("Results can only be uploaded while the test is in progress.");

        ResultFileUrl = resultFileUrl;

        CompletedAt = DateTime.UtcNow;
        Status = LabTestRequestStatus.Completed;

        MarkAsUpdated();
    }

    public void UploadPatientResult(string resultFileUrl)
    {
        if (LabId.HasValue)
            throw new BusinessRuleViolationException("Patient can only upload results for external labs.");

        if (string.IsNullOrWhiteSpace(resultFileUrl))
            throw new ArgumentException("Result file URL is required.", nameof(resultFileUrl));

        Status = LabTestRequestStatus.InProgress;

        ResultFileUrl = resultFileUrl;
        CompletedAt = DateTime.UtcNow;
        Status = LabTestRequestStatus.Completed;

        MarkAsUpdated();
    }

    public void Reject()
    {
        if (Status == LabTestRequestStatus.Completed)
            throw new BusinessRuleViolationException("Can't reject a completed test request.");

        Status = LabTestRequestStatus.Rejected;
        MarkAsUpdated();
    }
}