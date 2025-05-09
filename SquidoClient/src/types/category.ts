export interface Category {
  id: number
  name: string
  description: string | null
  bookCount: number | null
}

export interface CategoryParams {
  keyword?: string
}
