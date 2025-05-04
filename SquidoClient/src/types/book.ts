export interface Book {
  bookId: string
  title: string
  categoryName: string
  authorName: string | null
  authorId?: string | null
  quantity: number
  price: number
  buyCount: number
  imageUrls: string[] | null
  createdDate: string
  updatedDate: string | null
}

export interface BookResponse {
  currentPage: number
  pageCount: number
  data: Book[]
  totalCount: number
  categories: any[] | null
}

export interface BookParams {
  page?: number
  pageSize?: number
  keyword?: string
  categoryId?: number
}

export interface Category {
  categoryId: number
  name: string
  description: string | null
  bookCount: number | null
}

export interface BookDetail {
  book: Book
  category: Category
  bookRelated: Book[]
  bookDescription: string
  bio: string
  imageUrl: string
  ratingValueAverage: number
}
