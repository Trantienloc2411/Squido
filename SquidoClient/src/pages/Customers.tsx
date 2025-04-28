import { Card, CardContent, CardHeader, CardTitle } from "../components/ui/card"
import CustomerList from "../components/customers/CustomerList"

function Customers() {
  return (
    <div className="space-y-6">
      <h1 className="text-3xl font-bold">Customers</h1>

      <Card>
        <CardHeader>
          <CardTitle>All Customers</CardTitle>
        </CardHeader>
        <CardContent>
          <CustomerList />
        </CardContent>
      </Card>
    </div>
  )
}

export default Customers
