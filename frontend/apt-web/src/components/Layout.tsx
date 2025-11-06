
// frontend/apt-web/src/components/Layout.tsx
import { Outlet, NavLink } from 'react-router-dom';

export const Layout = () => (
    <>
        <header className="app-header">
            <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
                <h1>APT Alternate Parts Monitoring</h1>
                <nav style={{ display: 'flex', gap: '1rem' }}>
                    <NavLink to="/" end>Dashboard</NavLink>
                    <NavLink to="/prs">Problem Reports</NavLink>
                </nav>
            </div>
        </header>
        <main className="container">
            <Outlet />
        </main>
    </>
);
