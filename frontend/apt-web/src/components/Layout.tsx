
// frontend/apt-web/src/components/Layout.tsx
import { Outlet } from 'react-router-dom';

export const Layout = () => (
    <>
        <header className="app-header">
            <h1>APT Alternate Parts Monitoring</h1>
        </header>
        <main className="container">
            <Outlet />
        </main>
    </>
);
