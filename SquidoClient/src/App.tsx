import { Routes, Route, Navigate } from "react-router-dom"
import { Toaster } from "./components/ui/toaster"
import DashboardLayout from "./layouts/DashboardLayout"
import Dashboard from "./pages/Dashboard"
import Products from "./pages/Products"
import NewProduct from "./pages/NewProduct"
import EditProduct from "./pages/EditProduct"
import Categories from "./pages/Categories"
import Customers from "./pages/Customers"

function App() {
  return (
    <>
      <Routes>
        <Route path="/" element={<Navigate to="/dashboard" replace />} />
        <Route path="/" element={<DashboardLayout />}>
          <Route path="dashboard" element={<Dashboard />} />
          <Route path="products" element={<Products />} />
          <Route path="products/new" element={<NewProduct />} />
          <Route path="products/:id" element={<EditProduct />} />
          <Route path="categories" element={<Categories />} />
          <Route path="customers" element={<Customers />} />
        </Route>
      </Routes>
      <Toaster />
    </>
  )
}

export default App
