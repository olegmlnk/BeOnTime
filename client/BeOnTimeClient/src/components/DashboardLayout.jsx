import { Sidebar } from './Sidebar';
import './Dashboard.css';

export const DashboardLayout = ({ children }) => {
  return (
    <div className="dashboard-container">
      <Sidebar />
      <main className="dashboard-main">
        {children}
      </main>
    </div>
  );
};
