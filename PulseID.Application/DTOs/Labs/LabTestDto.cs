namespace PulseID.Application.DTOs.Labs
{
    public class LabTestDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = default!;
        public string Type { get; set; } = default!;
        public string Description { get; set; } = default!;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class CreateLabTestDto
    {
        public string Name { get; set; } = default!;
        public string Type { get; set; } = default!;
        public string Description { get; set; } = default!;
    }

    public class UpdateLabTestDto
    {
        public string Name { get; set; } = default!;
        public string Type { get; set; } = default!;
        public string Description { get; set; } = default!;
    }
}