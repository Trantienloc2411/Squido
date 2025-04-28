import { NavLink } from "react-router-dom"
import { LayoutDashboard, ShoppingBag, Tags, Users, Settings, LogOut } from "lucide-react"

interface SidebarProps {
  open: boolean
}

function Sidebar({ open }: SidebarProps) {
  const sidebarClass = `sidebar ${open ? "" : "sidebar-collapsed"} md:sidebar-open`

  return (
    <aside className={sidebarClass}>
      <div className="sidebar-header">
        <div className="flex items-center">
          <span className="logo-text">
            <span>e</span>Dox Admin
          </span>
        </div>
      </div>
      <div className="sidebar-content">
        <ul className="sidebar-menu">
          <li className="sidebar-menu-item">
            <NavLink to="/dashboard" className={({ isActive }) => `sidebar-menu-button ${isActive ? "active" : ""}`}>
              <LayoutDashboard size={20} />
              <span>Dashboard</span>
            </NavLink>
          </li>
          <li className="sidebar-menu-item">
            <NavLink to="/products" className={({ isActive }) => `sidebar-menu-button ${isActive ? "active" : ""}`}>
              <ShoppingBag size={20} />
              <span>Products</span>
            </NavLink>
          </li>
          <li className="sidebar-menu-item">
            <NavLink to="/categories" className={({ isActive }) => `sidebar-menu-button ${isActive ? "active" : ""}`}>
              <Tags size={20} />
              <span>Categories</span>
            </NavLink>
          </li>
          <li className="sidebar-menu-item">
            <NavLink to="/customers" className={({ isActive }) => `sidebar-menu-button ${isActive ? "active" : ""}`}>
              <Users size={20} />
              <span>Customers</span>
            </NavLink>
          </li>
        </ul>
      </div>
      <div className="sidebar-footer">
        <ul className="sidebar-menu">
          <li className="sidebar-menu-item">
            <button className="sidebar-menu-button">
              <Settings size={20} />
              <span>Settings</span>
            </button>
          </li>
          <li className="sidebar-menu-item">
            <button className="sidebar-menu-button">
              <LogOut size={20} />
              <span>Logout</span>
            </button>
          </li>
        </ul>
      </div>
    </aside>
  )
}

export default Sidebar
