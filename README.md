# Zariya - Donor Management Portal

## About the Project

**Zariya** is a web-based Blood Donor Management Portal designed to connect blood donors with recipients efficiently and securely. The platform serves as a bridge between individuals willing to donate blood and those in urgent need, enabling faster communication and better management of blood donation requests.

The system allows donors to register, manage their profiles, and respond to blood requests, while recipients can search for suitable donors, view donor information, and submit donation requests. Administrators oversee the platform, verify records, manage users, and monitor donation activities.

Zariya aims to simplify the blood donation process, improve accessibility to blood donors, and contribute to saving lives through technology.



## Key Objectives

* Connect blood donors with recipients quickly.
* Maintain a centralized donor database.
* Simplify blood request management.
* Improve donor availability tracking.
* Enable secure communication between donors and recipients.
* Provide administrators with complete control over the platform.
* Promote voluntary blood donation within communities.



## Core Features

### Donor Management

* Donor Registration
* Donor Profile Management
* Blood Group Information
* Availability Status
* Donation History
* Contact Information Management
* Profile Verification

### Recipient Management

* Recipient Registration
* Blood Request Submission
* Search Donors by Blood Group
* View Donor Profiles
* Send Donation Requests
* Track Request Status

### Request Management

* Create Blood Requests
* Accept or Reject Requests
* Request Notifications
* Request History
* Emergency Blood Requests

### Admin Dashboard

* Manage Donors
* Manage Recipients
* Manage Blood Requests
* User Verification
* Analytics and Reporting
* System Monitoring
* Role and Permission Management

### Search & Filtering

* Search by Blood Group
* Search by Location
* Search by Availability
* Search by Donor Status

### Security Features

* Authentication & Authorization
* Role-Based Access Control (RBAC)
* Secure Password Storage
* Data Validation
* Audit Logging
* HTTPS Support


## User Roles

### Donor

A registered user willing to donate blood.

**Capabilities:**

* Manage personal profile
* Update availability status
* View incoming requests
* Accept or decline requests
* Track donation history

### Recipient

A user seeking blood donation assistance.

**Capabilities:**

* Search donors
* View donor profiles
* Submit blood requests
* Track request progress
* Contact donors

### Administrator

System manager responsible for platform operations.

**Capabilities:**

* Manage all users
* Verify donor records
* Manage blood requests
* Monitor platform activity
* Generate reports
* Configure system settings


## Technology Stack

### Frontend

* ASP.NET Core Razor Pages
* HTML5
* CSS3
* Bootstrap 5
* JavaScript

### Backend

* ASP.NET Core 9
* C#
* Entity Framework Core

### Database

* Microsoft SQL Server

### Authentication

* ASP.NET Core Identity

### Development Tools

* Visual Studio 2022
* Git & GitHub


## System Architecture

```text
Presentation Layer (Razor Pages)
            │
            ▼
Application Layer (Services)
            │
            ▼
Repository Layer
            │
            ▼
Entity Framework Core
            │
            ▼
SQL Server Database
```



## Project Structure

```text
Zariya
│
├── Data
├── Models
├── DTOs
├── ViewModels
├── Interfaces
├── Repositories
├── Services
├── Validations
├── Middleware
├── Helpers
├── Extensions
│
├── Pages
│   ├── Account
│   ├── Dashboard
│   ├── Donors
│   ├── Recipients
│   ├── Requests
│   ├── Reports
│   ├── Settings
│   └── Shared
│
├── wwwroot
│
├── Program.cs
├── appsettings.json
└── README.md
```


## Future Roadmap

* SMS Notifications
* Email Notifications
* Blood Donation Campaign Management
* Mobile Application
* Geo-location Based Donor Search
* Real-Time Chat Between Donor and Recipient
* AI-Powered Donor Matching
* Donation Certificates
* Community Awareness Dashboard
* Hospital Integration

---

###Mission

**"Connecting donors with those in need, making every donation count and every request reachable."**

Zariya leverages modern technology to create a reliable, secure, and efficient blood donor management ecosystem that helps save lives by reducing the gap between blood donors and recipients. ❤️🩸



### Developed By

**7Scribes**
Innovative Software Solutions for Social Impact

[7Scribes Official Website](https://www.7scribes.com?utm_source=chatgpt.com)
