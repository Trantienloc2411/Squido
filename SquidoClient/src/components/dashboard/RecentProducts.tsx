import { useEffect } from "react"
import { useAppDispatch, useAppSelector } from "../../redux/hooks"
import { fetchStats } from "../../redux/slices/statsSlice"
import { formatDate } from "../../lib/utils"
import { Skeleton } from "../ui/skeleton"
import { ShoppingBag } from "lucide-react"

function RecentProducts() {
  const dispatch = useAppDispatch();
  const { recentBooks, loading } = useAppSelector((state) => state.stats);

  useEffect(() => {
    dispatch(fetchStats());
  }, [dispatch]);

  const formattedProducts = recentBooks?.map((product) => ({
    id: product.id,
    title: product.title,
    createdDate: product.createdDate,
    imagesUrl: product.imageUrl[0], // Assuming imagesUrl is an array and we want the first image
  }));

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
    );
  }

  return (
    <div className="space-y-4">
      {(formattedProducts ?? []).slice(0, 5).map((product) => (
        <div key={product.id} className="flex items-center gap-4">
          <div className="h-12 w-12 rounded-md bg-muted flex items-center justify-center">
            {product.imagesUrl ? (
              <img src={product.imagesUrl} alt={product.title} className="h-full w-full object-cover rounded-md" />
            ) : (
              <ShoppingBag className="h-6 w-6 text-muted-foreground" />
            )}
            
          </div>
          <div>
            <p className="font-medium">{product.title}</p>
            <p className="text-sm text-muted-foreground">Added {formatDate(product.createdDate)}</p>
            {/* <p className="text-sm text-muted-foreground">Category: {product.categoryName}</p> */}
          </div>
        </div>
      ))}
    </div>
  );
}

export default RecentProducts;
