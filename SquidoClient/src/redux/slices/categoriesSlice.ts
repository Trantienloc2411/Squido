import { createSlice, createAsyncThunk } from "@reduxjs/toolkit"
import { baseApiRequest } from "./baseApi"
// Mock data
// Types
interface Category {
  id: string
  name: string
  description: string
}

interface CategoriesState {
  categories: Category[]
  loading: boolean
  error: string | null
}

// Initial state
const initialState: CategoriesState = {
  categories: [],
  loading: false,
  error: null,
}

// Async thunks
export const fetchCategories = createAsyncThunk("categories/fetchCategories", async () => {
  // Simulate API call
  await new Promise((resolve) => setTimeout(resolve, 500))
  return mockCategories
})

export const addCategory = createAsyncThunk("categories/addCategory", async (category: Omit<Category, "id">) => {
  // Simulate API call
  await new Promise((resolve) => setTimeout(resolve, 500))
  return {
    id: Date.now().toString(),
    ...category,
  }
})

export const updateCategory = createAsyncThunk("categories/updateCategory", async (category: Category) => {
  // Simulate API call
  await new Promise((resolve) => setTimeout(resolve, 500))
  return category
})

export const deleteCategory = createAsyncThunk("categories/deleteCategory", async (id: string) => {
  // Simulate API call
  await new Promise((resolve) => setTimeout(resolve, 500))
  return id
})

// Slice
const categoriesSlice = createSlice({
  name: "categories",
  initialState,
  reducers: {},
  extraReducers: (builder) => {
    builder
      .addCase(fetchCategories.pending, (state) => {
        state.loading = true
        state.error = null
      })
      .addCase(fetchCategories.fulfilled, (state, action) => {
        state.loading = false
        state.categories = action.payload
      })
      .addCase(fetchCategories.rejected, (state, action) => {
        state.loading = false
        state.error = action.error.message || "Failed to fetch categories"
      })
      .addCase(addCategory.fulfilled, (state, action) => {
        state.categories.push(action.payload)
      })
      .addCase(updateCategory.fulfilled, (state, action) => {
        const index = state.categories.findIndex((category) => category.id === action.payload.id)
        if (index !== -1) {
          state.categories[index] = action.payload
        }
      })
      .addCase(deleteCategory.fulfilled, (state, action) => {
        state.categories = state.categories.filter((category) => category.id !== action.payload)
      })
  },
})

export default categoriesSlice.reducer
