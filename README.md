# Employee Admin Portal

A comprehensive employee management system built with .NET 8 that provides time tracking, vacation management, permission requests, and hierarchical role-based access control.

## 🚀 Features

### Core Functionality
- **Employee Management**: Create, read, update, delete employee records
- **Time Tracking**: Check-in/check-out system with work duration calculation
- **Vacation Management**: Submit, approve, and track vacation requests
- **Permission Requests**: Request and manage temporary leave permissions
- **Role-Based Access Control**: Hierarchical permissions system
- **Work Schedule Management**: Flexible work schedules with lateness tracking

### User Roles
- **Boss**: Full system access and management capabilities
- **Department Bosses**: Manage employees within their department (IT, Marketing, Finance, HR, Sales, Operations)
- **Employees**: Access to personal data and request submissions

## 🏗️ System Architecture

The application follows a clean architecture pattern with clear separation of concerns:

```
├── EmployeeSystem/          # Web API Layer (Controllers)
├── BusinessLogicLayer/      # Business Logic & Services
├── DataAccessLayer/         # Data Access & Entity Framework
├── Models/                  # DTOs and Entity Models
└── frontend/                # React Frontend Application
```

## 🔧 Technology Stack

### Backend
- **.NET 8**: Core framework
- **Entity Framework Core 9.0.8**: ORM and database management
- **SQL Server**: Database
- **JWT Authentication**: Secure token-based authentication
- **AutoMapper**: Object-to-object mapping
- **Swagger/OpenAPI**: API documentation

### Frontend
- **React 18**: Frontend framework
- **Axios**: HTTP client for API communication
- **Context API**: State management for authentication
- **CSS3**: Styling with modern design patterns
- **Create React App**: Development and build tooling

## 📋 Prerequisites

### Backend
- .NET 8 SDK
- SQL Server (LocalDB or Full)
- Visual Studio 2022 or VS Code

### Frontend
- Node.js (v16 or higher)
- npm or yarn package manager

## 🚀 Getting Started

### Backend Setup

#### 1. Clone the Repository
```bash
git clone [https://github.com/Sedmeq/WorkTrack.git]
cd WorkTrack
```

#### 2. Database Setup

Update the connection string in `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=EmployeeAdminPortal;Trusted_Connection=true;MultipleActiveResultSets=true"
  }
}
```


#### 4. Run Database Migrations
```bash
dotnet ef database update --project DataAccessLayer
```

#### 5. Run the Backend API
```bash
dotnet run --project EmployeeSystem
```

The API will be available at `https://localhost:7139` with Swagger documentation at `https://localhost:7139/swagger`.

### Frontend Setup

#### 1. Navigate to Frontend Directory
```bash
cd frontend
```

#### 2. Install Dependencies
```bash
npm install
```

#### 3. Start the Development Server
```bash
npm start
```

The React application will be available at `http://localhost:3000`.

### Full System Setup
1. Start the backend API (`dotnet run --project EmployeeSystem`)
2. Start the frontend development server (`npm start` in the frontend directory)
3. Access the application at `http://localhost:3000`

## 🔐 Authentication

### API Authentication
All endpoints (except registration and login) require JWT authentication. Include the token in the Authorization header:
```
Authorization: Bearer <your-jwt-token>
```

## 🌐 Frontend Features

### User Interface Components

#### Dashboard
- **Tabbed Navigation**: Switch between Employees, Permissions, Vacations, and Time Logs
- **Role-Based UI**: Different views and capabilities based on user role
- **Responsive Design**: Mobile-friendly interface

#### Authentication
- **Login/Register Forms**: Clean, modern authentication interface
- **JWT Token Management**: Automatic token storage and header injection
- **User Session Persistence**: Maintains login state across browser sessions

#### Employee Management
- **Employee CRUD**: Create, view, edit, and delete employees
- **Role Assignment**: Assign roles and work schedules
- **Hierarchical Display**: Shows boss-subordinate relationships
- **Vacation Balance Tracking**: Real-time vacation day calculations

#### Time Tracking
- **Check-In/Check-Out Interface**: Simple time tracking buttons
- **Status Display**: Shows current check-in status
- **Daily Summaries**: Grouped time logs by day
- **Boss Overview**: Managers can view team time logs

#### Permission & Vacation Management
- **Request Forms**: Submit permission and vacation requests
- **Approval Interface**: Boss-only approval/denial interface
- **Status Tracking**: View request status and history
- **Date/Time Pickers**: Intuitive date and time selection
## 🏢 Business Logic

### Vacation Balance Calculation
- Employees accrue 30 vacation days per year (365 days)
- Balance = (Days Worked / 365) × 30 - Days Already Taken
- Only approved vacations count towards days taken

### Time Tracking Features
- Real-time check-in/check-out
- Work duration calculation
- Schedule compliance monitoring
- Lateness detection and tracking
- Work efficiency calculations

## 📁 Project Structure

### Models Layer
- **Entities**: Database models (Employee, Role, TimeLog, etc.)
- **DTOs**: Data transfer objects for API communication

### Data Access Layer
- **ApplicationDbContext**: Entity Framework database context
- **Migrations**: Database schema versioning

### Business Logic Layer
- **Services**: Core business logic implementation
- **Interfaces**: Service contracts
- **Mappings**: AutoMapper configuration

### API Layer
- **Controllers**: HTTP endpoint handlers
- **Middleware**: Global exception handling
- **Authentication**: JWT configuration

## 🔧 Configuration

### Work Schedules (Pre-seeded)
- **8-17**: Standard 8:00-17:00 (8 hours)
- **9-18**: Standard 9:00-18:00 (8 hours)
- **9-14**: Morning shift 9:00-14:00 (5 hours)
- **14-18**: Afternoon shift 14:00-18:00 (4 hours)

### Roles (Pre-seeded)
- **Boss**: Company-wide administrator
- **Boss-IT**: IT Department manager
- **Boss-Marketing**: Marketing Department manager
- **Boss-Finance**: Finance Department manager
- **Boss-HR**: HR Department manager
- **Boss-Sales**: Sales Department manager
- **Boss-Operations**: Operations Department manager
- **Employee**: Standard employee role

## 🛡️ Security Features

- **JWT Authentication**: Secure token-based authentication
- **Role-Based Authorization**: Hierarchical access control
- **Password Hashing**: Secure password storage using ASP.NET Core Identity
- **Input Validation**: Comprehensive DTO validation
- **CORS Configuration**: Configurable cross-origin resource sharing


## 🎨 Frontend Architecture

### Component Structure

#### Authentication Flow
- **AuthContext**: Manages authentication state, token storage, and API configuration
- **Authentication**: Wrapper component that switches between login and register
- **Login/Register**: Form components with validation and error handling

#### Dashboard Layout
- **Dashboard**: Main container with tab navigation
- **Section Components**: Feature-specific components for each major functionality



## 📞 Support

For questions or issues, please contact the development team or create an issue in the repository.

---

**Note**: This system includes multi-language support with some Azerbaijani text in user messages. The API primarily uses English for technical elements while supporting localized user-facing messages.
