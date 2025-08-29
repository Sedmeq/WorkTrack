import { useEffect, useMemo, useState } from 'react';
import axios from 'axios';

const API_BASE = 'https://localhost:7139/api';

const initialForm = {
  username: '',
  email: '',
  password: '',
  phone: '',
  salary: '',
  roleId: '',
  workScheduleId: ''
};

const EmployeesSection = () => {
  const [employees, setEmployees] = useState([]);
  const [roles, setRoles] = useState([]);
  const [workSchedules, setWorkSchedules] = useState([]);
  const [form, setForm] = useState(initialForm);
  const [editingId, setEditingId] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');

  const canSubmit = useMemo(() => {
    if (editingId) {
      // For editing, only require username and email
      return form.username && form.email;
    }
    // For creating, require username, email, password, and salary
    return form.username && form.email && form.password && String(form.salary) !== '';
  }, [form, editingId]);

  const loadAll = async () => {
    try {
      setLoading(true);
      const [empRes, roleRes, scheduleRes] = await Promise.all([
        axios.get(`${API_BASE}/employee`),
        axios.get(`${API_BASE}/employee/available-roles`),
        axios.get(`${API_BASE}/workschedule`)
      ]);
      setEmployees(empRes.data?.data ?? []);
      setRoles(roleRes.data?.data ?? []);
      setWorkSchedules(scheduleRes.data?.data ?? []);
    } catch (e) {
      console.error('Load error:', e);
      let errorMessage = 'Failed to load employees';
      
      if (e.response?.data) {
        if (typeof e.response.data === 'string') {
          errorMessage = e.response.data;
        } else if (e.response.data.message) {
          errorMessage = e.response.data.message;
        } else if (e.response.data.title) {
          errorMessage = e.response.data.title;
        } else if (e.response.data.errors) {
          // Handle validation errors
          const errors = Object.values(e.response.data.errors).flat();
          errorMessage = errors.join(', ');
        }
      }
      
      setError(errorMessage);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    loadAll();
  }, []);

  const startEdit = (emp) => {
    setEditingId(emp.id);
    setForm({
      username: emp.username || '',
      email: emp.email || '',
      password: '', // Always empty for editing
      phone: emp.phone || '',
      salary: emp.salary ?? '',
      roleId: emp.roleId || '',
      workScheduleId: emp.workScheduleId || ''
    });
    setError(''); // Clear any previous errors
  };

  const resetForm = () => {
    setForm(initialForm);
    setEditingId(null);
    setError('');
  };

  const handleChange = (e) => {
    const { name, value } = e.target;
    setForm((f) => ({ ...f, [name]: value }));
  };

  const createEmployee = async () => {
    setError('');
    try {
      const payload = {
        username: form.username,
        email: form.email,
        password: form.password,
        phone: form.phone || null,
        salary: Number(form.salary) || 0,
        roleId: form.roleId || null,
        workScheduleId: form.workScheduleId || null
      };
      
      console.log('Creating employee with payload:', payload);
      await axios.post(`${API_BASE}/employee`, payload);
      resetForm();
      loadAll();
    } catch (e) {
      console.error('Create employee error:', e);
      let errorMessage = 'Failed to create employee';
      
      if (e.response?.data) {
        if (typeof e.response.data === 'string') {
          errorMessage = e.response.data;
        } else if (e.response.data.message) {
          errorMessage = e.response.data.message;
        } else if (e.response.data.title) {
          errorMessage = e.response.data.title;
        } else if (e.response.data.errors) {
          // Handle validation errors
          const errors = Object.values(e.response.data.errors).flat();
          errorMessage = errors.join(', ');
        }
      }
      
      setError(errorMessage);
    }
  };

  const updateEmployee = async () => {
    setError('');
    try {
      const payload = {
        username: form.username,
        email: form.email,
        phone: form.phone || null,
        salary: Number(form.salary) || 0,
        roleId: form.roleId || null,
        workScheduleId: form.workScheduleId || null
      };

      // Only include password if it's not empty
      if (form.password && form.password.trim() !== '') {
        payload.password = form.password;
      }

      console.log('Updating employee with payload:', payload);
      await axios.put(`${API_BASE}/employee/${editingId}`, payload);
      resetForm();
      loadAll();
    } catch (e) {
      console.error('Update employee error:', e);
      let errorMessage = 'Failed to update employee';
      
      if (e.response?.data) {
        if (typeof e.response.data === 'string') {
          errorMessage = e.response.data;
        } else if (e.response.data.message) {
          errorMessage = e.response.data.message;
        } else if (e.response.data.title) {
          errorMessage = e.response.data.title;
        } else if (e.response.data.errors) {
          // Handle validation errors
          const errors = Object.values(e.response.data.errors).flat();
          errorMessage = errors.join(', ');
        }
      }
      
      setError(errorMessage);
    }
  };

  const deleteEmployee = async (id) => {
    if (!window.confirm('Are you sure you want to delete this employee?')) {
      return;
    }
    
    setError('');
    try {
      await axios.delete(`${API_BASE}/employee/${id}`);
      loadAll();
    } catch (e) {
      console.error('Delete employee error:', e);
      let errorMessage = 'Failed to delete employee';
      
      if (e.response?.data) {
        if (typeof e.response.data === 'string') {
          errorMessage = e.response.data;
        } else if (e.response.data.message) {
          errorMessage = e.response.data.message;
        } else if (e.response.data.title) {
          errorMessage = e.response.data.title;
        } else if (e.response.data.errors) {
          // Handle validation errors
          const errors = Object.values(e.response.data.errors).flat();
          errorMessage = errors.join(', ');
        }
      }
      
      setError(errorMessage);
    }
  };

  if (loading) return <div>Loading employees...</div>;

  return (
    <div>
      <h2>Employees</h2>
      {error && <div className="error-message" style={{ marginBottom: 16 }}>{error}</div>}

      <div className="add-employee-section" style={{ marginTop: 10 }}>
        <h3>{editingId ? 'Edit Employee' : 'Create Employee'}</h3>
        <div className="add-form">
          <input 
            name="username" 
            className="form-input" 
            placeholder="Username" 
            value={form.username} 
            onChange={handleChange}
            required
          />
          <input 
            name="email" 
            type="email" 
            className="form-input" 
            placeholder="Email" 
            value={form.email} 
            onChange={handleChange}
            required
          />
          <input 
            name="password" 
            type="password" 
            className="form-input" 
            placeholder={editingId ? "New Password (leave empty to keep current)" : "Password"} 
            value={form.password} 
            onChange={handleChange}
            required={!editingId}
          />
          <input 
            name="phone" 
            className="form-input" 
            placeholder="Phone" 
            value={form.phone} 
            onChange={handleChange} 
          />
          <input 
            name="salary" 
            type="number" 
            step="0.01"
            min="0"
            className="form-input" 
            placeholder="Salary" 
            value={form.salary} 
            onChange={handleChange} 
          />
          <select 
            name="roleId" 
            className="form-input" 
            value={form.roleId} 
            onChange={handleChange}
          >
            <option value="">Select Role</option>
            {roles.map((r) => (
              <option key={r.id} value={r.id}>{r.name}</option>
            ))}
          </select>
          <select 
            name="workScheduleId" 
            className="form-input" 
            value={form.workScheduleId} 
            onChange={handleChange}
          >
            <option value="">Select Work Schedule</option>
            {workSchedules.map((ws) => (
              <option key={ws.id} value={ws.id}>
                {ws.name} ({ws.startTime}-{ws.endTime})
              </option>
            ))}
          </select>
          <button 
            className="add-button" 
            disabled={!canSubmit} 
            onClick={editingId ? updateEmployee : createEmployee}
          >
            {editingId ? 'Update' : 'Create'}
          </button>
          {editingId && (
            <button className="delete-button" onClick={resetForm}>Cancel</button>
          )}
        </div>
      </div>

      <div className="employees-section" style={{ marginTop: 20 }}>
        <h3>Employee List ({employees.length})</h3>
        {employees.length === 0 ? (
          <div className="no-employees">No employees found</div>
        ) : (
          <div className="employees-grid">
            {employees.map((emp) => (
              <div key={emp.id} className="employee-card">
                <div className="employee-info">
                  <h3>{emp.username || emp.name}</h3>
                  <p><strong>Email:</strong> {emp.email}</p>
                  <p><strong>Phone:</strong> {emp.phone || 'N/A'}</p>
                  <p><strong>Salary:</strong> ${emp.salary}</p>
                  {emp.roleName && (
                    <p><strong>Role:</strong> {emp.roleName}</p>
                  )}
                  {emp.workScheduleName && (
                    <p><strong>Schedule:</strong> {emp.workScheduleName} ({emp.workStartTime}-{emp.workEndTime})</p>
                  )}
                  {emp.bossName && (
                    <p><strong>Boss:</strong> {emp.bossName}</p>
                  )}
                </div>
                <div style={{ display: 'flex', gap: 8, flexDirection: 'column' }}>
                  <button className="add-button" onClick={() => startEdit(emp)}>Edit</button>
                  <button className="delete-button" onClick={() => deleteEmployee(emp.id)}>Delete</button>
                </div>
              </div>
            ))}
          </div>
        )}
      </div>
    </div>
  );
};

export default EmployeesSection;