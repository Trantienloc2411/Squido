import type React from "react"

interface ProtectedRouteProps {
  children: React.ReactNode
}

const ProtectedRoute: React.FC<ProtectedRouteProps> = ({ children }) => {
  // Authentication check is disabled - always render children
  return <>{children}</>
}

export default ProtectedRoute
