import { useState } from "react";
import { useAuth } from '../contexts/AuthContext';
import EmployeesSection from './EmployeesSection';
import PermissionsSection from './PermissionsSection';
import VacationsSection from './VacationsSection';
import TimeLogsSection from './TimeLogsSection';
import './Dashboard.css';

const TABS = [
  { key: 'employees', label: 'Employees' },
  { key: 'permissions', label: 'Permissions' },
  { key: 'vacations', label: 'Vacations' },
  { key: 'timelogs', label: 'Time Logs' }
];

const Dashboard = () => {
  const { user, logout } = useAuth();
  const [activeTab, setActiveTab] = useState('employees');

  const handleLogout = () => { logout(); };

  return (
    <div className="dashboard-container">
      <header className="dashboard-header">
        <div className="header-content">
          <h1>Employee Management System</h1>
          <div className="user-info">
            <span>Welcome, {user?.username || user?.email}!</span>
            <button onClick={handleLogout} className="logout-button">Logout</button>
          </div>
        </div>
      </header>

      <main className="dashboard-main">
        <div className="tab-bar">
          {TABS.map((t) => (
            <button
              key={t.key}
              className={`tab-button ${activeTab === t.key ? 'active' : ''}`}
              onClick={() => setActiveTab(t.key)}
            >
              {t.label}
            </button>
          ))}
        </div>

        <div className="tab-content">
          {activeTab === 'employees' && <EmployeesSection />}
          {activeTab === 'permissions' && <PermissionsSection />}
          {activeTab === 'vacations' && <VacationsSection />}
          {activeTab === 'timelogs' && <TimeLogsSection />}
        </div>
      </main>
    </div>
  );
};

export default Dashboard;
