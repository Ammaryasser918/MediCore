using PulseID.Domain.Common;
using PulseID.Domain.Enums;
using PulseID.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PulseID.Domain.Entities
{
    public class Patient : BaseEntity
    {

        // ===================================== < Attributes Section > ===================================
        public Guid UserId { get; private set; } = default!;

        public string NationalId { get; private set; } = default!;
        public string FullName { get; private set; } = default!;
        public DateTime DateOfBirth { get; private set; }
        public Gender Gender { get; set; }
        public BloodType? BloodType { get; set; }
        public string PhoneNumber { get; private set; } = default!;
        public string? Address { get; private set; }


        // ================================= < Collections Section > ======================================

        readonly List<PatientAllergy> _patientAllergies = new List<PatientAllergy>();
        public IReadOnlyCollection<PatientAllergy> PatientAllergies => _patientAllergies.AsReadOnly();


        readonly List<PatientChronicDisease> _patientChronicDiseases = new List<PatientChronicDisease>();
        public IReadOnlyCollection<PatientChronicDisease> PatientChronicDiseases => _patientChronicDiseases.AsReadOnly();

        readonly List<EmergencyContact> _emergencyContacts = new List<EmergencyContact>();
        public IReadOnlyCollection<EmergencyContact> EmergencyContacts => _emergencyContacts.AsReadOnly();

        readonly List<Prescription> _prescriptions = new List<Prescription>();
        public IReadOnlyCollection<Prescription> Prescriptions => _prescriptions.AsReadOnly();

        readonly List<MedicalRecord> _medicalRecords = new List<MedicalRecord>();
        public IReadOnlyCollection<MedicalRecord> MedicalRecords => _medicalRecords.AsReadOnly();

        readonly List<MedicalFile> _files = new List<MedicalFile>();
        public IReadOnlyCollection<MedicalFile> MedicalFiles => _files.AsReadOnly();



        // ================================== < Constructors Section > =================================

        Patient() { }

        public Patient(Guid userId, string nationalId, string fullName, DateTime dateOfBirth,
            Gender gender, string phoneNumber, BloodType? bloodType = null)
        {
            if (userId == Guid.Empty)
                throw new ArgumentException("User Id is required", nameof(userId));
            if (string.IsNullOrWhiteSpace(nationalId))
                throw new ArgumentException("National Id is Required", nameof(nationalId));
            if (string.IsNullOrWhiteSpace(fullName))
                throw new ArgumentException("Full Name is Required", nameof(fullName));
            if (string.IsNullOrWhiteSpace(phoneNumber))
            {
                throw new ArgumentException("Phone Number is Required", nameof(phoneNumber));
            }

            UserId = userId;
            NationalId = nationalId;
            FullName = fullName;
            DateOfBirth = dateOfBirth;
            Gender = gender;
            PhoneNumber = phoneNumber;
            BloodType = bloodType;

        }
        




        // ================================ < Methods Section > ======================================


        public void UpdateContactInfo(string phoneNumber, string? address)
        {
            if (string.IsNullOrWhiteSpace(phoneNumber))
                throw new ArgumentException("Phone Number is Required", nameof(phoneNumber));

            PhoneNumber = phoneNumber;
            Address = address;
            MarkAsUpdated();

        }

        public void SetBloodType(BloodType bloodType)
        {
            BloodType = bloodType;
            MarkAsUpdated();
        }

        public int GetAge()
        {
            return DateTime.UtcNow.Year - DateOfBirth.Year - (DateTime.UtcNow.DayOfYear < DateOfBirth.DayOfYear ? 1 : 0);
        }


        // ---- Emergency Contacts ----
        public EmergencyContact AddEmergencyContact(string fullName, string relationship, string phoneNumber, string? address = null)
        {
            var contact = new EmergencyContact(Id, fullName, relationship, phoneNumber, address);
            _emergencyContacts.Add(contact);
            MarkAsUpdated();
            return contact;
        }

        public void RemoveEmergencyContact(Guid emergencyContactId)
        {
            if (emergencyContactId == Guid.Empty)
                throw new ArgumentException("Emergency Contact Id is required", nameof(emergencyContactId));

            var contact = _emergencyContacts.FirstOrDefault(c => c.Id == emergencyContactId);
            if (contact is not null)
            {
                _emergencyContacts.Remove(contact);
                MarkAsUpdated();
            }
        }



        // ---- Allergies ----

        public PatientAllergy AddAllergy(Guid allergyId, DateTime? diagnosedDate = null)
        {
            if (_patientAllergies.Any(pa => pa.AllergyId == allergyId))
                throw new BusinessRuleViolationException("This Allergy is already assigned to this Patient");

            var patientAllergy = new PatientAllergy(Id, allergyId, diagnosedDate);
            _patientAllergies.Add(patientAllergy);
            MarkAsUpdated();
            return patientAllergy;
        }

        public void RemoveAllergy(Guid allergyId)
        {
            if (allergyId == Guid.Empty)
                throw new ArgumentException("Allergy Id is required", nameof(allergyId));

            var patientAllergy = _patientAllergies.FirstOrDefault(pa => pa.AllergyId == allergyId);
            if (patientAllergy is not null)
            {
                _patientAllergies.Remove(patientAllergy);
                MarkAsUpdated();
            }
        }


        // ------ < Chronic Diseases > ------

        public PatientChronicDisease AddChronicDisease(Guid chronicDiseaseId, DateTime? diagnosedDate = null)
        {
            if (_patientChronicDiseases.Any(pcd => pcd.ChronicDiseaseId == chronicDiseaseId))
                throw new BusinessRuleViolationException("This Chronic Disease is already assigned to this Patient");

            var patientChronicDisease = new PatientChronicDisease(Id, chronicDiseaseId, diagnosedDate);
            _patientChronicDiseases.Add(patientChronicDisease);
            MarkAsUpdated();
            return patientChronicDisease;
        }


        public void RemoveChronicDisease(Guid chronicDiseaseId)
        {
            if (chronicDiseaseId == Guid.Empty)
                throw new ArgumentException("Chronic Disease Id is required.", nameof(chronicDiseaseId));

            var patientDisease = _patientChronicDiseases.FirstOrDefault(pcd => pcd.ChronicDiseaseId == chronicDiseaseId);
            if (patientDisease is not null)
            {
                _patientChronicDiseases.Remove(patientDisease);
                MarkAsUpdated();
            }
        }


        // ------ < Medical Files > --------

        public MedicalFile AddMedicalFile(Guid medicalRecordId, string fileName, string filePath, string contentType, MedicalFileType fileType, string? description = null)
        {
            var file = new MedicalFile(Id, medicalRecordId, fileName, filePath, contentType, fileType, description);

            _files.Add(file);
            MarkAsUpdated();
            return file;
        }







    }
}
