import { useEffect, useState } from 'react';
import axios from 'axios';

const API_BASE = 'https://localhost:7139/api/permission';

const PermissionsSection = () => {
  const [myRequests, setMyRequests] = useState([]);
  const [pending, setPending] = useState([]);
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
    } catch (e) {
      setError(e.response?.data?.message || 'Failed to load permissions');
    }
  };

  useEffect(() => { loadData(); }, []);

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
      <h2>Permissions</h2>
      {error && <div className="error-message" style={{ marginBottom: 16 }}>{error}</div>}

      <div className="add-employee-section" style={{ marginTop: 10 }}>
        <h3>Request Permission</h3>
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
                  <p>{new Date(r.startDate).toLocaleDateString()} - {new Date(r.endDate).toLocaleDateString()}</p>
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
                  <p>{new Date(r.startDate).toLocaleDateString()} - {new Date(r.endDate).toLocaleDateString()}</p>
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
