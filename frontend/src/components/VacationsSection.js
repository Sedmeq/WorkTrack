import { useEffect, useState } from 'react';
import axios from 'axios';
import { useAuth } from '../contexts/AuthContext';

const API_BASE = 'https://localhost:7139/api/vacation';

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

const VacationsSection = () => {
  const { user, loading } = useAuth();
  const [myRequests, setMyRequests] = useState([]);
  const [pending, setPending] = useState([]);
  const [balance, setBalance] = useState(null);
  const [startDate, setStartDate] = useState('');
  const [endDate, setEndDate] = useState('');
  const [reason, setReason] = useState('');
  const [error, setError] = useState('');

  const loadData = async () => {
    try {
      const [mine, pend] = await Promise.all([
        axios.get(`${API_BASE}/my-requests`),
        axios.get(`${API_BASE}/pending-for-approval`)
      ]);
      setMyRequests(mine.data?.data ?? []);
      setPending(pend.data?.data ?? []);
      // Load my vacation balance (requires user id)
      if (user?.id) {
        try {
          const bal = await axios.get(`https://localhost:7139/api/employee/${user.id}/vacation-balance`);
          setBalance(bal.data?.data ?? null);
        } catch (e) {
          // If balance cannot be retrieved, do not block other data
          setBalance(null);
        }
      }
    } catch (e) {
      setError(e.response?.data?.message || 'Failed to load vacations');
    }
  };

  useEffect(() => {
    if (!loading) {
      loadData();
    }
  }, [loading, user?.id]);

  const submit = async () => {
    setError('');
    if (!startDate || !endDate) { setError('Start and End dates required'); return; }
    try {
      await axios.post(`${API_BASE}/submit`, { startDate, endDate, reason });
      setStartDate(''); setEndDate(''); setReason('');
      loadData();
    } catch (e) {
      setError(e.response?.data?.message || 'Failed to submit');
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
      <h2>Vacations</h2>
      {error && <div className="error-message" style={{ marginBottom: 16 }}>{error}</div>}

      {balance && (
        <div className="add-employee-section" style={{ marginTop: 10 }}>
          <h3>My Vacation Balance</h3>
          <div style={{ marginTop: 8 }}>
            <div><strong>Total Accrued:</strong> {balance.totalAccruedDays}</div>
            <div><strong>Taken:</strong> {balance.daysTaken}</div>
            <div><strong>Remaining:</strong> {balance.remainingDays}</div>
          </div>
        </div>
      )}

      <div className="add-employee-section" style={{ marginTop: 10 }}>
        <h3>Request Vacation</h3>
        <div className="add-form">
          <input type="date" className="form-input" value={startDate} onChange={e => setStartDate(e.target.value)} />
          <input type="date" className="form-input" value={endDate} onChange={e => setEndDate(e.target.value)} />
          <input className="form-input" placeholder="Reason (optional)" value={reason} onChange={e => setReason(e.target.value)} />
          <button className="add-button" onClick={submit}>Submit</button>
        </div>
      </div>

      <div className="employees-section" style={{ marginTop: 20 }}>
        <h3>My Requests</h3>
        {myRequests.length === 0 ? <div className="no-employees">No requests</div> : (
          <div className="employees-grid">
            {myRequests.map((r) => (
              <div key={r.id} className="employee-card">
                <div className="employee-info">
                  <h3>{r.reason || 'No reason'}</h3>
                  <p>{parseDate(r.startDate).toLocaleDateString()} - {parseDate(r.endDate).toLocaleDateString()}</p>
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
                  <p>{parseDate(r.startDate).toLocaleDateString()} - {parseDate(r.endDate).toLocaleDateString()}</p>
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

export default VacationsSection;
