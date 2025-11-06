import { Outlet, NavLink } from 'react-router-dom'

export default function Layout() {
  return (
    <div>
      <header className="header">
        <div className="container" style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
          <h1>APT Alternate Parts</h1>
          <nav style={{ display: 'flex', gap: 12 }}>
            <NavLink to="/" end>PRs</NavLink>
          </nav>
        </div>
      </header>
      <main className="container" style={{ paddingTop: 16 }}>
        <Outlet />
      </main>
    </div>
  )
}