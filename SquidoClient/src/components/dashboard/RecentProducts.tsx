"use client"

import { useEffect } from "react"
import { useAppDispatch, useAppSelector } from "../../redux/hooks"
import { fetchProducts } from "../../redux/slices/productsSlice"
import { formatDate } from "../../lib/utils"
import { Skeleton } from "../ui/skeleton"
import { ShoppingBag } from "lucide-react"

function RecentProducts() {
  const dispatch = useAppDispatch()
  const { products, loading } = useAppSelector((state) => state.products)

  useEffect(() => {
    dispatch(fetchProducts({ limit: 5 }))
  }, [dispatch])

  if (loading) {
    return (
      <div className="space-y-4">
        {[...Array(5)].map((_, i) => (
          <div key={i} className="flex items-center gap-4">
            <Skeleton className="h-12 w-12 rounded-md" />
            <div className="space-y-2">
              <Skeleton className="h-4 w-32" />
              <Skeleton className="h-4 w-24" />
            </div>
          </div>
        ))}
      </div>
    )
  }

  return (
    <div className="space-y-4">
      {products.slice(0, 5).map((product) => (
        <div key={product.id} className="flex items-center gap-4">
          <div className="h-12 w-12 rounded-md bg-muted flex items-center justify-center">
            {product.images && product.images.length > 0 ? (
              <img
                src={product.images[0] || "/placeholder.svg"}
                alt={product.name}
                className="h-full w-full object-cover rounded-md"
              />
            ) : (
              <ShoppingBag className="h-6 w-6 text-muted-foreground" />
            )}
          </div>
          <div>
            <p className="font-medium">{product.name}</p>
            <p className="text-sm text-muted-foreground">Added {formatDate(product.createdDate)}</p>
          </div>
        </div>
      ))}
    </div>
  )
}

export default RecentProducts
