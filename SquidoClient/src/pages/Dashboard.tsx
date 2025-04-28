import { Card, CardContent, CardHeader, CardTitle } from "../components/ui/card"
import DashboardStats from "../components/dashboard/DashboardStats"
import RecentProducts from "../components/dashboard/RecentProducts"
import TopCategories from "../components/dashboard/TopCategories"

function Dashboard() {
  return (
    <div className="space-y-6">
      <h1 className="text-3xl font-bold">Dashboard</h1>

      <DashboardStats />

      <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
        <Card>
          <CardHeader>
            <CardTitle>Recent Products</CardTitle>
          </CardHeader>
          <CardContent>
            <RecentProducts />
          </CardContent>
        </Card>

        <Card>
          <CardHeader>
            <CardTitle>Top Categories</CardTitle>
          </CardHeader>
          <CardContent>
            <TopCategories />
          </CardContent>
        </Card>
      </div>
    </div>
  )
}

export default Dashboard
