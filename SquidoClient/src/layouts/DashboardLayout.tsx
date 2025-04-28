 

import { useState } from "react"
import { Outlet } from "react-router-dom"
import Sidebar from "../components/dashboard/Sidebar"
import TopNav from "../components/dashboard/TopNav"

function DashboardLayout() {
  const [sidebarOpen, setSidebarOpen] = useState(true)

  const toggleSidebar = () => {
    setSidebarOpen(!sidebarOpen)
  }

  return (
    <div className="flex min-h-screen">
      <Sidebar open={sidebarOpen} />
      <div className={`flex-1 ${sidebarOpen ? "md:ml-64" : ""} transition-all duration-300`}>
        <TopNav toggleSidebar={toggleSidebar} />
        <main className="p-6">
          <Outlet />
        </main>
      </div>
    </div>
  )
}

export default DashboardLayout
