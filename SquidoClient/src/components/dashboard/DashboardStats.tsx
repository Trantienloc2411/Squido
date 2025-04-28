 
import { useEffect } from "react"
import { useAppDispatch, useAppSelector } from "../../redux/hooks"
import { fetchStats } from "../../redux/slices/statsSlice"
import { Card, CardContent } from "../ui/card"
import { ShoppingBag, Tags, Users, DollarSign } from "lucide-react"

function DashboardStats() {
  const dispatch = useAppDispatch();
  // Access the stats directly from the state (no nested "stats" property)
  const { totalProducts, totalCategories, totalCustomers, totalRevenue, loading } = useAppSelector((state) => state.stats)

  useEffect(() => {
    dispatch(fetchStats())
  }, [dispatch])

  const statItems = [
    {
      title: "Total Products",
      value: totalProducts,
      icon: ShoppingBag,
      color: "text-blue-500",
      bgColor: "bg-blue-100",
    },
    {
      title: "Total Categories",
      value: totalCategories,
      icon: Tags,
      color: "text-green-500",
      bgColor: "bg-green-100",
    },
    {
      title: "Total Customers",
      value: totalCustomers,
      icon: Users,
      color: "text-purple-500",
      bgColor: "bg-purple-100",
    },
    {
      title: "Total Revenue",
      value: `$${totalRevenue.toFixed(2)}`,
      icon: DollarSign,
      color: "text-orange-500",
      bgColor: "bg-orange-100",
    },
  ]

  return (
    <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
      {statItems.map((item, index) => (
        <Card key={index}>
          <CardContent className="p-6">
            <div className="flex items-center justify-between">
              <div>
                <p className="text-sm font-medium text-muted-foreground">{item.title}</p>
                <h3 className="text-2xl font-bold mt-1">{loading ? "..." : item.value}</h3>
              </div>
              <div className={`p-2 rounded-full ${item.bgColor}`}>
                <item.icon className={`h-5 w-5 ${item.color}`} />
              </div>
            </div>
          </CardContent>
        </Card>
      ))}
    </div>
  )
}

export default DashboardStats