import { createSlice, createAsyncThunk } from "@reduxjs/toolkit"

// Mock data
const mockProducts = [
  {
    id: "1",
    name: "Abstract Design",
    category: "Fiction",
    description: "A captivating novel about abstract art and design.",
    price: 15.99,
    images: ["/placeholder.svg?height=200&width=200"],
    createdDate: "2023-01-15T12:00:00Z",
    updatedDate: "2023-01-15T12:00:00Z",
  },
  {
    id: "2",
    name: "The Winter",
    category: "Fiction",
    description: "A chilling tale set in the depths of winter.",
    price: 12.5,
    images: ["/placeholder.svg?height=200&width=200"],
    createdDate: "2023-02-20T12:00:00Z",
    updatedDate: "2023-02-20T12:00:00Z",
  },
  {
    id: "3",
    name: "The Birds",
    category: "Thriller",
    description: "A suspenseful story about mysterious bird behavior.",
    price: 18.75,
    images: ["/placeholder.svg?height=200&width=200"],
    createdDate: "2023-03-10T12:00:00Z",
    updatedDate: "2023-03-10T12:00:00Z",
  },
  {
    id: "4",
    name: "Science",
    category: "Non-Fiction",
    description: "An educational book about scientific discoveries.",
    price: 22.99,
    images: ["/placeholder.svg?height=200&width=200"],
    createdDate: "2023-04-05T12:00:00Z",
    updatedDate: "2023-04-05T12:00:00Z",
  },
  {
    id: "5",
    name: "Black Night",
    category: "Thriller",
    description: "A thrilling mystery set during a moonless night.",
    price: 14.5,
    images: ["/placeholder.svg?height=200&width=200"],
    createdDate: "2023-05-12T12:00:00Z",
    updatedDate: "2023-05-12T12:00:00Z",
  },
]

// Types
interface Product {
  id: string
  name: string
  category: string
  description: string
  price: number
  images: string[]
  createdDate: string
  updatedDate: string
}

interface ProductsState {
  products: Product[]
  currentProduct: Product | null
  loading: boolean
  error: string | null
}

// Initial state
const initialState: ProductsState = {
  products: [],
  currentProduct: null,
  loading: false,
  error: null,
}

// Async thunks
export const fetchProducts = createAsyncThunk("products/fetchProducts", async ({ limit }: { limit?: number } = {}) => {
  // Simulate API call
  await new Promise((resolve) => setTimeout(resolve, 500))
  return limit ? mockProducts.slice(0, limit) : mockProducts
})

export const fetchProductById = createAsyncThunk("products/fetchProductById", async (id: string) => {
  // Simulate API call
  await new Promise((resolve) => setTimeout(resolve, 500))
  const product = mockProducts.find((p) => p.id === id)
  if (!product) {
    throw new Error("Product not found")
  }
  return product
})

export const addProduct = createAsyncThunk("products/addProduct", async (product: Omit<Product, "id">) => {
  // Simulate API call
  await new Promise((resolve) => setTimeout(resolve, 500))
  return {
    id: Date.now().toString(),
    ...product,
  }
})

export const updateProduct = createAsyncThunk("products/updateProduct", async (product: Product) => {
  // Simulate API call
  await new Promise((resolve) => setTimeout(resolve, 500))
  return product
})

export const deleteProduct = createAsyncThunk("products/deleteProduct", async (id: string) => {
  // Simulate API call
  await new Promise((resolve) => setTimeout(resolve, 500))
  return id
})

// Slice
const productsSlice = createSlice({
  name: "products",
  initialState,
  reducers: {},
  extraReducers: (builder) => {
    builder
      .addCase(fetchProducts.pending, (state) => {
        state.loading = true
        state.error = null
      })
      .addCase(fetchProducts.fulfilled, (state, action) => {
        state.loading = false
        state.products = action.payload
      })
      .addCase(fetchProducts.rejected, (state, action) => {
        state.loading = false
        state.error = action.error.message || "Failed to fetch products"
      })
      .addCase(fetchProductById.pending, (state) => {
        state.loading = true
        state.error = null
      })
      .addCase(fetchProductById.fulfilled, (state, action) => {
        state.loading = false
        state.currentProduct = action.payload
      })
      .addCase(fetchProductById.rejected, (state, action) => {
        state.loading = false
        state.error = action.error.message || "Failed to fetch product"
      })
      .addCase(addProduct.fulfilled, (state, action) => {
        state.products.push(action.payload)
      })
      .addCase(updateProduct.fulfilled, (state, action) => {
        const index = state.products.findIndex((product) => product.id === action.payload.id)
        if (index !== -1) {
          state.products[index] = action.payload
        }
        state.currentProduct = action.payload
      })
      .addCase(deleteProduct.fulfilled, (state, action) => {
        state.products = state.products.filter((product) => product.id !== action.payload)
      })
  },
})

export default productsSlice.reducer
