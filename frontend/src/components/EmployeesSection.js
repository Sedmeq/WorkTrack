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
  const [form, setForm] = useState(initialForm);
  const [editingId, setEditingId] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');

  const canSubmit = useMemo(() => {
    if (editingId) return true; // allow partial update
    return form.username && form.email && form.password && String(form.salary) !== '';
  }, [form, editingId]);

  const loadAll = async () => {
    try {
      setLoading(true);
      const [empRes, roleRes] = await Promise.all([
        axios.get(`${API_BASE}/employee`),
        axios.get(`${API_BASE}/employee/available-roles`)
      ]);
      setEmployees(empRes.data?.data ?? []);
      setRoles(roleRes.data?.data ?? []);
    } catch (e) {
      setError(e.response?.data?.message || 'Failed to load employees');
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
      password: '',
      phone: emp.phone || '',
      salary: emp.salary ?? '',
      roleId: emp.roleId || '',
      workScheduleId: emp.workScheduleId || ''
    });
  };

  const resetForm = () => {
    setForm(initialForm);
    setEditingId(null);
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
      await axios.post(`${API_BASE}/employee`, payload);
      resetForm();
      loadAll();
    } catch (e) {
      setError(e.response?.data?.message || 'Failed to create employee');
    }
  };

  const updateEmployee = async () => {
    setError('');
    try {
      const payload = {
        username: form.username,
        email: form.email,
        password: form.password || undefined,
        phone: form.phone || null,
        salary: Number(form.salary) || 0,
        roleId: form.roleId || null,
        workScheduleId: form.workScheduleId || null
      };
      await axios.put(`${API_BASE}/employee/${editingId}`, payload);
      resetForm();
      loadAll();
    } catch (e) {
      setError(e.response?.data?.message || 'Failed to update employee');
    }
  };

  const deleteEmployee = async (id) => {
    setError('');
    try {
      await axios.delete(`${API_BASE}/employee/${id}`);
      loadAll();
    } catch (e) {
      setError(e.response?.data?.message || 'Failed to delete employee');
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
          <input name="username" className="form-input" placeholder="Username" value={form.username} onChange={handleChange} />
          <input name="email" type="email" className="form-input" placeholder="Email" value={form.email} onChange={handleChange} />
          {!editingId && (
            <input name="password" type="password" className="form-input" placeholder="Password" value={form.password} onChange={handleChange} />
          )}
          <input name="phone" className="form-input" placeholder="Phone" value={form.phone} onChange={handleChange} />
          <input name="salary" type="number" className="form-input" placeholder="Salary" value={form.salary} onChange={handleChange} />
          <select name="roleId" className="form-input" value={form.roleId} onChange={handleChange}>
            <option value="">Select Role</option>
            {roles.map((r) => (
              <option key={r.id} value={r.id}>{r.name}</option>
            ))}
          </select>
          <button className="add-button" disabled={!canSubmit} onClick={editingId ? updateEmployee : createEmployee}>
            {editingId ? 'Update' : 'Create'}
          </button>
          {editingId && (
            <button className="delete-button" onClick={resetForm}>Cancel</button>
          )}
        </div>
      </div>

      <div className="employees-section" style={{ marginTop: 20 }}>
        <h3>Employee List</h3>
        {employees.length === 0 ? (
          <div className="no-employees">No employees</div>
        ) : (
          <div className="employees-grid">
            {employees.map((emp) => (
              <div key={emp.id} className="employee-card">
                <div className="employee-info">
                  <h3>{emp.username || emp.name}</h3>
                  <p>{emp.email}</p>
                </div>
                <div style={{ display: 'flex', gap: 8 }}>
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
