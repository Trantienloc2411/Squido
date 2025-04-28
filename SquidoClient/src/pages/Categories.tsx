import { Card, CardContent, CardHeader, CardTitle } from "../components/ui/card"
import CategoryList from "../components/categories/CategoryList"
import CategoryForm from "../components/categories/CategoryForm"

function Categories() {
  return (
    <div className="space-y-6">
      <h1 className="text-3xl font-bold">Product Categories</h1>

      <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
        <div className="md:col-span-2">
          <Card>
            <CardHeader>
              <CardTitle>All Categories</CardTitle>
            </CardHeader>
            <CardContent>
              <CategoryList />
            </CardContent>
          </Card>
        </div>

        <div>
          <Card>
            <CardHeader>
              <CardTitle>Add Category</CardTitle>
            </CardHeader>
            <CardContent>
              <CategoryForm />
            </CardContent>
          </Card>
        </div>
      </div>
    </div>
  )
}

export default Categories
