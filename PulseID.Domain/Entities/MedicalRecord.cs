using PulseID.Domain.Common;
using PulseID.Domain.Enums;
using PulseID.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PulseID.Domain.Entities
{
    public class MedicalRecord : BaseEntity
    {
        
        public Guid PatientId { get; private set; }
        public Patient Patient { get; private set; } = default!;
        
        public Guid DoctorId { get; private set; }
        public Doctor Doctor { get; private set; } = default!;

        public Guid HospitalId { get; private set; }
        public Hospital Hospital { get; private set; } = default!;

        public DateTime VisitDate { get; private set; }
        public string Diagnosis { get; private set; } = default!;
        public string? Symptoms { get; private set; }
        public string? Notes { get; private set; }

        public MedicalRecordStatus Status { get; private set; } = MedicalRecordStatus.Active;

        // Versioning Feature
        // Each Version iterates on the previous version (Can't Edit any version)

        public Guid? PreviousVersionId { get; private set; }
        public MedicalRecord? PreviousVersion { get; private set; }




        readonly List<MedicalFile> _files = new List<MedicalFile>();
        public IReadOnlyCollection<MedicalFile> MedicalFiles => _files.AsReadOnly();

        readonly List<Prescription> _prescriptions = new List<Prescription>();
        public IReadOnlyCollection<Prescription> Prescriptions => _prescriptions.AsReadOnly();

        MedicalRecord() { }

        public MedicalRecord(Guid patientId, Guid doctorId, Guid hospitalId, DateTime visitDate,
                             string diagnosis, string? symptoms = null, string? notes = null, Guid? previousVersionId = null)
        {
            if (string.IsNullOrWhiteSpace(diagnosis))
                throw new ArgumentException("Diagnosis is required", nameof(diagnosis));

            PatientId = patientId;
            DoctorId = doctorId;
            HospitalId = hospitalId;
            VisitDate = visitDate;
            Diagnosis = diagnosis;
            Symptoms = symptoms;
            Notes = notes;
            PreviousVersionId = previousVersionId;
        }


        public Prescription AddPrescription(string? notes)
        {
            EnsureIsEditable();
            if (_prescriptions.Count >= 1)
                throw new BusinessRuleViolationException("This version of medical record already has a prescription, make a new version of medical record to add a new prescription");
            var prescription = new Prescription(Id, notes);
            _prescriptions.Add(prescription);
            MarkAsUpdated();
            return prescription;
        }

        public MedicalRecord CreateNewVersion(string diagnosis,
            string? symptoms = null, string? notes = null)
        {
            if (Status == MedicalRecordStatus.Superseded)
                throw new BusinessRuleViolationException("This Version of Medical Record has already Superseded, Can't make a new version of it.");

            var newVersion = new MedicalRecord(PatientId, DoctorId, HospitalId, VisitDate,
                diagnosis, symptoms, notes, previousVersionId: Id);

            MarkAsSuperseded();

            return newVersion;
        }

        private void EnsureIsEditable()
        {
            if (Status == MedicalRecordStatus.Superseded)
                throw new BusinessRuleViolationException("This Version of Medical Record has already Superseded, Can't make a new version of it.");
        }

        private void MarkAsSuperseded()
        {
            Status = MedicalRecordStatus.Superseded;
            MarkAsUpdated();
        }

    }
}
