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

        public Guid? PrescriptionId { get; private set; } = default;
        public Prescription? Prescription { get; private set; } = default;

        public MedicalRecordStatus Status { get; private set; } = MedicalRecordStatus.Active;

        // Versioning Feature
        // Each Version iterates on the previous version (Can't Edit any version)

        public Guid? PreviousVersionId { get; private set; }
        public MedicalRecord? PreviousVersion { get; private set; }
        public Guid RootRecordId { get; private set; }




        readonly List<MedicalFile> _files = new List<MedicalFile>();
        public IReadOnlyCollection<MedicalFile> MedicalFiles => _files.AsReadOnly();

        //readonly List<Prescription> _prescriptions = new List<Prescription>();
        //public IReadOnlyCollection<Prescription> Prescriptions => _prescriptions.AsReadOnly();

        MedicalRecord() { }

        
        // This Constructor Initialize the Root Medical Record For The First Time 
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
            RootRecordId = Id;
        }

        // This Constructor For Creating New Version of The Medical Record
        private MedicalRecord(Guid patientId, Guid doctorId, Guid hospitalId, DateTime visitDate,
                              string diagnosis, string? symptoms, string? notes, Guid previousVersionId, Guid rootRecordId)
        {
            PatientId = patientId;
            DoctorId = doctorId;
            HospitalId = hospitalId;
            VisitDate = visitDate;
            Diagnosis = diagnosis;
            Symptoms = symptoms;
            Notes = notes;

            PreviousVersionId = previousVersionId;
            RootRecordId = rootRecordId;

            Status = MedicalRecordStatus.Active;
        }


        public Prescription AddPrescription(string? notes)
        {
            EnsureIsEditable();
            if (Prescription is not null)
                throw new BusinessRuleViolationException("This version of medical record already has a prescription, make a new version of medical record to add a new prescription");
            Prescription = new Prescription(Id, notes);
            MarkAsUpdated();
            return Prescription;
        }

        public MedicalRecord CreateNewVersion(Guid newDoctorId, Guid newHospitalId, string diagnosis, string? symptoms, string? notes)
        {
            if (Status == MedicalRecordStatus.Superseded)
                throw new BusinessRuleViolationException("This Version of Medical Record has already Superseded, Can't make a new version of it.");

            var newVersion = new MedicalRecord(PatientId, newDoctorId, newHospitalId, VisitDate,
                diagnosis, symptoms, notes, previousVersionId: Id, rootRecordId: RootRecordId);

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
