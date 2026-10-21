namespace MediCore.Domain.Enums
{
    public enum LabTestRequestStatus
    {
        Pending = 1,
        SentToLab = 2,
        SampleCollected = 3,
        InProgress = 4,
        Completed = 5,
        Rejected = 6
    }
}