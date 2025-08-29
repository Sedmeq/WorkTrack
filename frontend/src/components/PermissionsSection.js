import { useEffect, useState } from 'react';
import axios from 'axios';
import { useAuth } from '../contexts/AuthContext';

const API_BASE = 'https://localhost:7139/api/permission';

// Helper function to safely parse dates from backend
const parseDate = (dateString) => {
  if (!dateString) return new Date();
  
  // Try to parse as custom format first (dd.MM.yyyy HH:mm:ss)
  const customFormat = /^(\d{2})\.(\d{2})\.(\d{4}) (\d{2}):(\d{2}):(\d{2})$/.exec(dateString);
  if (customFormat) {
    const [, day, month, year, hour, minute, second] = customFormat;
    return new Date(year, month - 1, day, hour, minute, second);
  }
  
  // Fallback to standard date parsing
  return new Date(dateString);
};

const PermissionsSection = () => {
  const { user } = useAuth();
  const [myRequests, setMyRequests] = useState([]);
  const [pending, setPending] = useState([]);
  const [employees, setEmployees] = useState([]);
  const [startDate, setStartDate] = useState('');
  const [startTime, setStartTime] = useState('09:00');
  const [endDate, setEndDate] = useState('');
  const [endTime, setEndTime] = useState('10:00');
  const [reason, setReason] = useState('');
  const [selectedEmployee, setSelectedEmployee] = useState('');
  const [error, setError] = useState('');
  const [isBoss, setIsBoss] = useState(false);

  const loadData = async () => {
    try {
      const [mine, pend, emp] = await Promise.all([
        axios.get(`${API_BASE}/my-requests`),
        axios.get(`${API_BASE}/pending-for-approval`),
        axios.get('https://localhost:7139/api/employee')
      ]);
      setMyRequests(mine.data?.data ?? []);
      setPending(pend.data?.data ?? []);
      const allEmployees = emp.data?.data ?? [];
      setEmployees(allEmployees);
      
      // Check if user is boss (has employees under them)
      const subordinateEmployees = allEmployees.filter(e => e.bossId === user?.id && e.id !== user?.id);
      setIsBoss(subordinateEmployees.length > 0);
    } catch (e) {
      setError(e.response?.data?.message || 'Failed to load permissions');
    }
  };

  useEffect(() => { loadData(); }, []);

  const formatDateTime = (date, time) => {
    if (!date || !time) return '';
    const dateTime = new Date(`${date}T${time}`);
    return dateTime.toISOString();
  };

  const submit = async () => {
    setError('');
    if (!startDate || !endDate) { setError('Start and End dates required'); return; }
    if (!startTime || !endTime) { setError('Start and End times required'); return; }
    
    try {
      const startDateTime = formatDateTime(startDate, startTime);
      const endDateTime = formatDateTime(endDate, endTime);
      
      await axios.post(`${API_BASE}/submit`, { 
        startDate: startDateTime, 
        endDate: endDateTime, 
        reason 
      });
      setStartDate(''); setEndDate(''); setStartTime('09:00'); setEndTime('10:00'); setReason('');
      loadData();
    } catch (e) {
      setError(e.response?.data?.message || 'Failed to submit');
    }
  };

  const grantPermission = async () => {
    setError('');
    if (!selectedEmployee || !startDate || !endDate) { 
      setError('Employee, Start and End dates required'); 
      return; 
    }
    if (!startTime || !endTime) { setError('Start and End times required'); return; }
    
    try {
      const startDateTime = formatDateTime(startDate, startTime);
      const endDateTime = formatDateTime(endDate, endTime);
      
      await axios.post(`${API_BASE}/grant`, { 
        employeeId: selectedEmployee,
        startDate: startDateTime, 
        endDate: endDateTime, 
        reason 
      });
      setSelectedEmployee(''); setStartDate(''); setEndDate(''); 
      setStartTime('09:00'); setEndTime('10:00'); setReason('');
      loadData();
    } catch (e) {
      setError(e.response?.data?.message || 'Failed to grant permission');
    }
  };

  const approve = async (id) => {
    try { await axios.post(`${API_BASE}/approve/${id}`); loadData(); } 
    catch (e) { setError(e.response?.data?.message || 'Failed to approve'); }
  };
  const deny = async (id) => {
    try { await axios.post(`${API_BASE}/deny/${id}`); loadData(); } 
    catch (e) { setError(e.response?.data?.message || 'Failed to deny'); }
  };

  return (
    <div>
      <h2>Permissions</h2>
      {error && <div className="error-message" style={{ marginBottom: 16 }}>{error}</div>}

      <div className="add-employee-section" style={{ marginTop: 10 }}>
        <h3>Request Permission</h3>
        <div className="add-form">
          <div style={{ display: 'flex', gap: 10, marginBottom: 10 }}>
            <input type="date" className="form-input" value={startDate} onChange={e => setStartDate(e.target.value)} placeholder="Start Date" />
            <input type="time" className="form-input" value={startTime} onChange={e => setStartTime(e.target.value)} />
          </div>
          <div style={{ display: 'flex', gap: 10, marginBottom: 10 }}>
            <input type="date" className="form-input" value={endDate} onChange={e => setEndDate(e.target.value)} placeholder="End Date" />
            <input type="time" className="form-input" value={endTime} onChange={e => setEndTime(e.target.value)} />
          </div>
          <input className="form-input" placeholder="Reason (optional)" value={reason} onChange={e => setReason(e.target.value)} />
          <button className="add-button" onClick={submit}>Submit Request</button>
        </div>
      </div>

      {isBoss && (
        <div className="add-employee-section" style={{ marginTop: 20 }}>
          <h3>Grant Permission to Employee (Boss)</h3>
          <div className="add-form">
                         <select 
               className="form-input" 
               value={selectedEmployee} 
               onChange={e => setSelectedEmployee(e.target.value)}
             >
               <option value="">Select Employee</option>
               {employees.filter(emp => emp.bossId === user?.id && emp.id !== user?.id).map(emp => (
                 <option key={emp.id} value={emp.id}>{emp.username} ({emp.email})</option>
               ))}
             </select>
            <div style={{ display: 'flex', gap: 10, marginBottom: 10 }}>
              <input type="date" className="form-input" value={startDate} onChange={e => setStartDate(e.target.value)} placeholder="Start Date" />
              <input type="time" className="form-input" value={startTime} onChange={e => setStartTime(e.target.value)} />
            </div>
            <div style={{ display: 'flex', gap: 10, marginBottom: 10 }}>
              <input type="date" className="form-input" value={endDate} onChange={e => setEndDate(e.target.value)} placeholder="End Date" />
              <input type="time" className="form-input" value={endTime} onChange={e => setEndTime(e.target.value)} />
            </div>
            <input className="form-input" placeholder="Reason (optional)" value={reason} onChange={e => setReason(e.target.value)} />
            <button className="add-button" onClick={grantPermission}>Grant Permission</button>
          </div>
        </div>
      )}

      <div className="employees-section" style={{ marginTop: 20 }}>
        <h3>My Requests</h3>
        {myRequests.length === 0 ? <div className="no-employees">No requests</div> : (
          <div className="employees-grid">
            {myRequests.map((r) => (
              <div key={r.id} className="employee-card">
                <div className="employee-info">
                  <h3>{r.reason || 'No reason'}</h3>
                  <p>{parseDate(r.startDate).toLocaleDateString()} {parseDate(r.startDate).toLocaleTimeString()} - {parseDate(r.endDate).toLocaleDateString()} {parseDate(r.endDate).toLocaleTimeString()}</p>
                  <p>Status: {r.status}</p>
                </div>
              </div>
            ))}
          </div>
        )}
      </div>

      <div className="employees-section" style={{ marginTop: 20 }}>
        <h3>Pending For Approval (Boss)</h3>
        {pending.length === 0 ? <div className="no-employees">No pending requests</div> : (
          <div className="employees-grid">
            {pending.map((r) => (
              <div key={r.id} className="employee-card">
                <div className="employee-info">
                  <h3>{r.employeeName}</h3>
                  <p>{parseDate(r.startDate).toLocaleDateString()} {parseDate(r.startDate).toLocaleTimeString()} - {parseDate(r.endDate).toLocaleDateString()} {parseDate(r.endDate).toLocaleTimeString()}</p>
                  <p>{r.reason || 'No reason'}</p>
                </div>
                <div style={{ display: 'flex', gap: 8 }}>
                  <button className="add-button" onClick={() => approve(r.id)}>Approve</button>
                  <button className="delete-button" onClick={() => deny(r.id)}>Deny</button>
                </div>
              </div>
            ))}
          </div>
        )}
      </div>
    </div>
  );
};

export default PermissionsSection;
