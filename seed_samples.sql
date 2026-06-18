-- Seed sample donors and recipients with varied blood groups and cities
-- Safe to run multiple times (checks email existence before inserting)

-- ============================================================
-- Extra Donors
-- ============================================================
IF NOT EXISTS (SELECT 1 FROM Users WHERE email = 'ahmed.r@email.com')
BEGIN
    INSERT INTO Users (full_name, email, password_hash, role, is_email_verified, is_active, failed_attempts, created_at, updated_at)
    VALUES ('Ahmed Raza', 'ahmed.r@email.com', 'HASHED_donor123', 'donor', 1, 1, 0, GETDATE(), GETDATE());
    DECLARE @U1 INT = SCOPE_IDENTITY();
    INSERT INTO Donors (user_id, full_name, cnic, date_of_birth, gender, phone, city_id, blood_group, donation_type_id, availability, is_admin_verified, total_donations, created_at, updated_at)
    VALUES (@U1, 'Ahmed Raza', '35201-1111111-1', '1992-05-15', 'Male', '+92 300 1111111', 1, 'A+', 1, 'Available', 1, 0, GETDATE(), GETDATE());
END

IF NOT EXISTS (SELECT 1 FROM Users WHERE email = 'sana.k@email.com')
BEGIN
    INSERT INTO Users (full_name, email, password_hash, role, is_email_verified, is_active, failed_attempts, created_at, updated_at)
    VALUES ('Sana Khan', 'sana.k@email.com', 'HASHED_donor123', 'donor', 1, 1, 0, GETDATE(), GETDATE());
    DECLARE @U2 INT = SCOPE_IDENTITY();
    INSERT INTO Donors (user_id, full_name, cnic, date_of_birth, gender, phone, city_id, blood_group, donation_type_id, availability, is_admin_verified, total_donations, created_at, updated_at)
    VALUES (@U2, 'Sana Khan', '42201-2222222-2', '1995-08-22', 'Female', '+92 321 2222222', 2, 'B+', 1, 'Available', 1, 0, GETDATE(), GETDATE());
END

IF NOT EXISTS (SELECT 1 FROM Users WHERE email = 'usman.a@email.com')
BEGIN
    INSERT INTO Users (full_name, email, password_hash, role, is_email_verified, is_active, failed_attempts, created_at, updated_at)
    VALUES ('Usman Ali', 'usman.a@email.com', 'HASHED_donor123', 'donor', 1, 1, 0, GETDATE(), GETDATE());
    DECLARE @U3 INT = SCOPE_IDENTITY();
    INSERT INTO Donors (user_id, full_name, cnic, date_of_birth, gender, phone, city_id, blood_group, donation_type_id, availability, is_admin_verified, total_donations, created_at, updated_at)
    VALUES (@U3, 'Usman Ali', '61101-3333333-3', '1988-11-02', 'Male', '+92 333 3333333', 3, 'O-', 1, 'Available', 1, 0, GETDATE(), GETDATE());
END

IF NOT EXISTS (SELECT 1 FROM Users WHERE email = 'fatima.z@email.com')
BEGIN
    INSERT INTO Users (full_name, email, password_hash, role, is_email_verified, is_active, failed_attempts, created_at, updated_at)
    VALUES ('Fatima Zafar', 'fatima.z@email.com', 'HASHED_donor123', 'donor', 1, 1, 0, GETDATE(), GETDATE());
    DECLARE @U4 INT = SCOPE_IDENTITY();
    INSERT INTO Donors (user_id, full_name, cnic, date_of_birth, gender, phone, city_id, blood_group, donation_type_id, availability, is_admin_verified, total_donations, created_at, updated_at)
    VALUES (@U4, 'Fatima Zafar', '35202-4444444-4', '1993-03-14', 'Female', '+92 345 4444444', 1, 'AB+', 1, 'Available', 1, 0, GETDATE(), GETDATE());
END

IF NOT EXISTS (SELECT 1 FROM Users WHERE email = 'bilal.h@email.com')
BEGIN
    INSERT INTO Users (full_name, email, password_hash, role, is_email_verified, is_active, failed_attempts, created_at, updated_at)
    VALUES ('Bilal Hussain', 'bilal.h@email.com', 'HASHED_donor123', 'donor', 1, 1, 0, GETDATE(), GETDATE());
    DECLARE @U5 INT = SCOPE_IDENTITY();
    INSERT INTO Donors (user_id, full_name, cnic, date_of_birth, gender, phone, city_id, blood_group, donation_type_id, availability, is_admin_verified, total_donations, created_at, updated_at)
    VALUES (@U5, 'Bilal Hussain', '37301-5555555-5', '1990-07-19', 'Male', '+92 312 5555555', 4, 'O+', 1, 'Available', 1, 0, GETDATE(), GETDATE());
END

-- ============================================================
-- Extra Recipients (Patients)
-- ============================================================
IF NOT EXISTS (SELECT 1 FROM Users WHERE email = 'zainab.i@email.com')
BEGIN
    INSERT INTO Users (full_name, email, password_hash, role, is_email_verified, is_active, failed_attempts, created_at, updated_at)
    VALUES ('Zainab Iqbal', 'zainab.i@email.com', 'HASHED_patient123', 'patient', 1, 1, 0, GETDATE(), GETDATE());
    DECLARE @P1 INT = SCOPE_IDENTITY();
    INSERT INTO Patients (user_id, full_name, cnic, date_of_birth, gender, phone, city_id, blood_group, medical_condition, urgency_level, created_at, updated_at)
    VALUES (@P1, 'Zainab Iqbal', '35201-6666666-6', '1985-12-10', 'Female', '+92 300 6666666', 1, 'A+', 'Thalassemia', 'normal', GETDATE(), GETDATE());
END

IF NOT EXISTS (SELECT 1 FROM Users WHERE email = 'tariq.m@email.com')
BEGIN
    INSERT INTO Users (full_name, email, password_hash, role, is_email_verified, is_active, failed_attempts, created_at, updated_at)
    VALUES ('Tariq Mehmood', 'tariq.m@email.com', 'HASHED_patient123', 'patient', 1, 1, 0, GETDATE(), GETDATE());
    DECLARE @P2 INT = SCOPE_IDENTITY();
    INSERT INTO Patients (user_id, full_name, cnic, date_of_birth, gender, phone, city_id, blood_group, medical_condition, urgency_level, created_at, updated_at)
    VALUES (@P2, 'Tariq Mehmood', '42203-7777777-7', '1978-06-25', 'Male', '+92 321 7777777', 2, 'B-', 'Kidney disease', 'urgent', GETDATE(), GETDATE());
END

IF NOT EXISTS (SELECT 1 FROM Users WHERE email = 'aisha.r@email.com')
BEGIN
    INSERT INTO Users (full_name, email, password_hash, role, is_email_verified, is_active, failed_attempts, created_at, updated_at)
    VALUES ('Aisha Riaz', 'aisha.r@email.com', 'HASHED_patient123', 'patient', 1, 1, 0, GETDATE(), GETDATE());
    DECLARE @P3 INT = SCOPE_IDENTITY();
    INSERT INTO Patients (user_id, full_name, cnic, date_of_birth, gender, phone, city_id, blood_group, medical_condition, urgency_level, created_at, updated_at)
    VALUES (@P3, 'Aisha Riaz', '61102-8888888-8', '1998-09-30', 'Female', '+92 333 8888888', 3, 'O+', 'Anemia', 'critical', GETDATE(), GETDATE());
END

IF NOT EXISTS (SELECT 1 FROM Users WHERE email = 'nadeem.a@email.com')
BEGIN
    INSERT INTO Users (full_name, email, password_hash, role, is_email_verified, is_active, failed_attempts, created_at, updated_at)
    VALUES ('Nadeem Ahmed', 'nadeem.a@email.com', 'HASHED_patient123', 'patient', 1, 1, 0, GETDATE(), GETDATE());
    DECLARE @P4 INT = SCOPE_IDENTITY();
    INSERT INTO Patients (user_id, full_name, cnic, date_of_birth, gender, phone, city_id, blood_group, medical_condition, urgency_level, created_at, updated_at)
    VALUES (@P4, 'Nadeem Ahmed', '35203-9999999-9', '1965-04-18', 'Male', '+92 345 9999999', 5, 'AB-', 'Surgery scheduled', 'urgent', GETDATE(), GETDATE());
END

PRINT 'Sample donors and recipients seeded successfully.';
