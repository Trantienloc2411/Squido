import type React from "react"
import { Routes, Route, Navigate } from "react-router-dom"
import { Box, useColorModeValue } from "@chakra-ui/react"
import Layout from "./components/layout/Layout"
import Dashboard from "./pages/Dashboard"
import BookManagement from "./pages/BookManagement"
import UserManagement from "./pages/UserManagement"
import OrderManagement from "./pages/OrderManagement"
import NotFound from "./pages/NotFound"
import ProtectedRoute from "./components/auth/ProtectedRoute"

const App: React.FC = () => {
  const bgColor = useColorModeValue("gray.50", "gray.900")

  return (
    <Box bg={bgColor} minH="100vh">
      <Routes>
        <Route path="/login" element={<Navigate to="/dashboard" replace />} />
        <Route
          path="/"
          element={
            <ProtectedRoute>
              <Layout />
            </ProtectedRoute>
          }
        >
          <Route index element={<Navigate to="/dashboard" replace />} />
          <Route path="dashboard" element={<Dashboard />} />
          <Route path="books/*" element={<BookManagement />} />
          <Route path="users/*" element={<UserManagement />} />
          <Route path="orders/*" element={<OrderManagement />} />
        </Route>
        <Route path="*" element={<NotFound />} />
      </Routes>
    </Box>
  )
}

export default App
