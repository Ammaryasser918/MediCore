using PulseID.Domain.Common;

namespace PulseID.Domain.Entities
{
    public class LabTest : BaseEntity
    {
        public string Name { get; private set; } = default!;
        public string Type { get; private set; } = default!;
        public string Description { get; private set; } = default!;


        readonly List<LabTestRequest> _testRequests = new List<LabTestRequest>();
        public IReadOnlyCollection<LabTestRequest> TestRequests => _testRequests.AsReadOnly();


        LabTest() { }

        public LabTest(string name, string type, string description)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Test Name is Required", nameof(name));

            Name = name;
            Type = type;
            Description = description;
        }

        public void UpdateDetails(string name, string type, string description)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Test Name is Required", nameof(name));

            Name = name;
            Type = type;
            Description = description;

            MarkAsUpdated();
        }
    }
}