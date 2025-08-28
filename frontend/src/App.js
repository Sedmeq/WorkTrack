import { useAuth } from './contexts/AuthContext';
import Authentication from './components/Authentication';
import Dashboard from './components/Dashboard';
import './App.css';

function App() {
  const { user, loading } = useAuth();

  if (loading) {
    return (
      <div className="app-loading">
        <div className="loading-spinner">Loading...</div>
      </div>
    );
  }

  return (
    <div className="App">
      {user ? <Dashboard /> : <Authentication />}
    </div>
  );
}

export default App;
