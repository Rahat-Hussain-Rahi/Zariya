-- Clean existing default accounts if they exist to avoid duplicate key errors
DELETE FROM Feedback WHERE request_id IN (SELECT request_id FROM Requests WHERE patient_id IN (SELECT patient_id FROM Patients WHERE user_id IN (SELECT user_id FROM Users WHERE email IN ('admin@zariya.org', 'donor@zariya.org', 'patient@zariya.org'))));
DELETE FROM Donations WHERE donor_id IN (SELECT donor_id FROM Donors WHERE user_id IN (SELECT user_id FROM Users WHERE email IN ('admin@zariya.org', 'donor@zariya.org', 'patient@zariya.org')));
DELETE FROM Requests WHERE patient_id IN (SELECT patient_id FROM Patients WHERE user_id IN (SELECT user_id FROM Users WHERE email IN ('admin@zariya.org', 'donor@zariya.org', 'patient@zariya.org')));
DELETE FROM Patients WHERE user_id IN (SELECT user_id FROM Users WHERE email IN ('admin@zariya.org', 'donor@zariya.org', 'patient@zariya.org'));
DELETE FROM Donors WHERE user_id IN (SELECT user_id FROM Users WHERE email IN ('admin@zariya.org', 'donor@zariya.org', 'patient@zariya.org'));
DELETE FROM Users WHERE email IN ('admin@zariya.org', 'donor@zariya.org', 'patient@zariya.org');

-- Get city IDs
DECLARE @CityLhr INT, @CityKhi INT;
SELECT @CityLhr = city_id FROM Cities WHERE city_name = 'Lahore';
SELECT @CityKhi = city_id FROM Cities WHERE city_name = 'Karachi';

-- Fallback to first available city if Lahore/Karachi doesn't exist
IF @CityLhr IS NULL SELECT TOP 1 @CityLhr = city_id FROM Cities;
IF @CityKhi IS NULL SELECT TOP 1 @CityKhi = city_id FROM Cities;

-- Insert Admin User
INSERT INTO Users (full_name, email, password_hash, role, is_email_verified, is_active, failed_attempts, created_at, updated_at)
VALUES ('System Admin', 'admin@zariya.org', 'HASHED_admin123', 'admin', 1, 1, 0, GETDATE(), GETDATE());

-- Insert Donor User
INSERT INTO Users (full_name, email, password_hash, role, is_email_verified, is_active, failed_attempts, created_at, updated_at)
VALUES ('Default Donor', 'donor@zariya.org', 'HASHED_donor123', 'donor', 1, 1, 0, GETDATE(), GETDATE());

-- Insert Patient User
INSERT INTO Users (full_name, email, password_hash, role, is_email_verified, is_active, failed_attempts, created_at, updated_at)
VALUES ('Default Recipient', 'patient@zariya.org', 'HASHED_patient123', 'patient', 1, 1, 0, GETDATE(), GETDATE());

-- Get generated User IDs
DECLARE @IdAdmin INT, @IdDonor INT, @IdPatient INT;
SELECT @IdAdmin = user_id FROM Users WHERE email = 'admin@zariya.org';
SELECT @IdDonor = user_id FROM Users WHERE email = 'donor@zariya.org';
SELECT @IdPatient = user_id FROM Users WHERE email = 'patient@zariya.org';

-- Insert Donor profile
INSERT INTO Donors (user_id, full_name, cnic, date_of_birth, gender, phone, city_id, blood_group, donation_type_id, availability, is_admin_verified, last_donated, total_donations, bio, profile_photo, created_at, updated_at)
VALUES (@IdDonor, 'Default Donor', '11111-1111111-1', '1990-01-01', 'Male', '+92 300 0000000', @CityLhr, 'O-', 1, 'Available', 1, NULL, 0, 'Seeded default donor.', '/images/avatar_default.jpg', GETDATE(), GETDATE());

-- Insert Patient profile
INSERT INTO Patients (user_id, full_name, cnic, date_of_birth, gender, phone, city_id, blood_group, medical_condition, urgency_level, profile_photo, created_at, updated_at)
VALUES (@IdPatient, 'Default Recipient', '22222-2222222-2', '1995-05-05', 'Female', '+92 312 0000000', @CityKhi, 'A+', 'Anemia', 'normal', '/images/avatar_default.jpg', GETDATE(), GETDATE());
