
import { useEffect } from "react"
import { useAppDispatch, useAppSelector } from "../../redux/hooks"
import { fetchStats } from "../../redux/slices/statsSlice"
import { Skeleton } from "../ui/skeleton"

function TopCategories() {
  const dispatch = useAppDispatch()
  const { topCategories, loading } = useAppSelector((state) => state.stats)

  useEffect(() => {
    dispatch(fetchStats())
  }, [dispatch])


  const categoriesWithCount = topCategories?.map((category) => ({
    id: category.id,
    name: category.name,
    count: category.bookCount,
  })) || []

  if (loading) {
    return (
      <div className="space-y-4">
        {[...Array(5)].map((_, i) => (
          <div key={i} className="flex items-center justify-between">
            <Skeleton className="h-4 w-32" />
            <Skeleton className="h-4 w-16" />
          </div>
        ))}
      </div>
    )
  }

  return (
    <div className="space-y-4">
      {categoriesWithCount.map((category) => (
        <div key={category.id} className="flex items-center justify-between">
          <p className="font-medium">{category.name}</p>
          <p className="text-sm text-muted-foreground">{category.count} book</p>
        </div>
      ))}
    </div>
  )
}

export default TopCategories
