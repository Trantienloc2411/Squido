"use client"

import { useEffect } from "react"
import { useAppDispatch, useAppSelector } from "../../redux/hooks"
import { fetchCategories } from "../../redux/slices/categoriesSlice"
import { Skeleton } from "../ui/skeleton"

function TopCategories() {
  const dispatch = useAppDispatch()
  const { categories, loading } = useAppSelector((state) => state.categories)

  useEffect(() => {
    dispatch(fetchCategories())
  }, [dispatch])

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

  // Mock data for product counts
  const categoriesWithCount = categories.slice(0, 5).map((category, index) => ({
    ...category,
    count: 100 - index * 15,
  }))

  return (
    <div className="space-y-4">
      {categoriesWithCount.map((category) => (
        <div key={category.id} className="flex items-center justify-between">
          <p className="font-medium">{category.name}</p>
          <p className="text-sm text-muted-foreground">{category.count} products</p>
        </div>
      ))}
    </div>
  )
}

export default TopCategories
