import { useEffect, useState } from 'react';
import axios from 'axios';

const API_BASE = 'https://localhost:7139/api/timelog';

const TimeLogsSection = () => {
  const [status, setStatus] = useState(null);
  const [myLogs, setMyLogs] = useState([]);
  const [notes, setNotes] = useState('');
  const [bossGrouped, setBossGrouped] = useState([]);
  const [selectedEmployeeId, setSelectedEmployeeId] = useState('');
  const [employeeLogs, setEmployeeLogs] = useState([]);
  const [error, setError] = useState('');

  const loadData = async () => {
    try {
      const [st, logs] = await Promise.all([
        axios.get(`${API_BASE}/status`),
        axios.get(`${API_BASE}/my-logs`)
      ]);
      setStatus(st.data);
      setMyLogs(logs.data?.data ?? []);

      // Try boss view, ignore errors silently (for non-boss users)
      try {
        const bossRes = await axios.get(`${API_BASE}/employee-logs`);
        setBossGrouped(bossRes.data?.data ?? []);
      } catch {}
    } catch (e) {
      setError(e.response?.data?.message || 'Failed to load time logs');
    }
  };

  const loadEmployeeLogs = async (employeeId) => {
    setEmployeeLogs([]);
    if (!employeeId) return;
    try {
      const res = await axios.get(`${API_BASE}/employee/${employeeId}/logs`);
      setEmployeeLogs(res.data?.data ?? []);
    } catch (e) {
      // surface boss error
      setError(e.response?.data?.message || 'Failed to load employee logs');
    }
  };

  useEffect(() => { loadData(); }, []);
  useEffect(() => { loadEmployeeLogs(selectedEmployeeId); }, [selectedEmployeeId]);

  const checkIn = async () => {
    setError('');
    try {
      await axios.post(`${API_BASE}/checkin`, { notes });
      setNotes('');
      loadData();
    } catch (e) {
      setError(e.response?.data?.message || 'Failed to check in');
    }
  };

  const checkOut = async () => {
    setError('');
    try {
      await axios.post(`${API_BASE}/checkout`, { notes });
      setNotes('');
      loadData();
    } catch (e) {
      setError(e.response?.data?.message || 'Failed to check out');
    }
  };

  return (
    <div>
      <h2>Time Logs</h2>
      {error && <div className="error-message" style={{ marginBottom: 16 }}>{error}</div>}

      <div className="add-employee-section" style={{ marginTop: 10 }}>
        <h3>My Status</h3>
        <div className="add-form">
          <input className="form-input" placeholder="Notes (optional)" value={notes} onChange={e => setNotes(e.target.value)} />
          <button className="add-button" onClick={checkIn} disabled={status?.isCheckedIn}>Check In</button>
          <button className="delete-button" onClick={checkOut} disabled={!status?.isCheckedIn}>Check Out</button>
        </div>
        {status && (
          <div style={{ marginTop: 10 }}>
            <strong>Checked In:</strong> {String(status.isCheckedIn)} | <strong>Current Time:</strong> {status.currentTime}
          </div>
        )}
      </div>

      <div className="employees-section" style={{ marginTop: 20 }}>
        <h3>My Logs (Daily Summary)</h3>
        {myLogs.length === 0 ? <div className="no-employees">No logs</div> : (
          <div className="employees-grid">
            {myLogs.map((g) => (
              <div key={g.date} className="employee-card">
                <div className="employee-info">
                  <h3>{g.date}</h3>
                  <p>Total: {g.totalWorkTime}</p>
                </div>
              </div>
            ))}
          </div>
        )}
      </div>

      {bossGrouped.length > 0 && (
        <div className="employees-section" style={{ marginTop: 20 }}>
          <h3>Team Logs (Boss)</h3>
          <div className="add-form" style={{ marginBottom: 12 }}>
            <select className="form-input" value={selectedEmployeeId} onChange={e => setSelectedEmployeeId(e.target.value)}>
              <option value="">Select employee to view detailed logs</option>
              {bossGrouped.map((r) => (
                <option key={r.employeeId} value={r.employeeId}>{r.employeeName} ({r.roleName})</option>
              ))}
            </select>
          </div>
          <div className="employees-grid">
            {bossGrouped.map((r) => (
              <div key={r.employeeId} className="employee-card">
                <div className="employee-info">
                  <h3>{r.employeeName}</h3>
                  <p>Role: {r.roleName}</p>
                  <p>Sessions: {r.totalSessions}, Total: {r.totalWorkTime}</p>
                </div>
              </div>
            ))}
          </div>

          {selectedEmployeeId && (
            <div style={{ marginTop: 16 }}>
              <h3>Selected Employee Logs</h3>
              {employeeLogs.length === 0 ? (
                <div className="no-employees">No logs</div>
              ) : (
                <div className="employees-grid">
                  {employeeLogs.map((log) => (
                    <div key={log.id} className="employee-card">
                      <div className="employee-info">
                        <h3>{log.employeeName}</h3>
                        <p>Check-in: {log.checkInTime}</p>
                        <p>Check-out: {log.checkOutTime || '-'}</p>
                        <p>Work: {log.workDuration}</p>
                        {log.notes && <p>Notes: {log.notes}</p>}
                      </div>
                    </div>
                  ))}
                </div>
              )}
            </div>
          )}
        </div>
      )}
    </div>
  );
};

export default TimeLogsSection;
