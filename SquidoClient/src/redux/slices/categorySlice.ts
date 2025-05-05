import { createSlice, createAsyncThunk, type PayloadAction } from "@reduxjs/toolkit"
import type { Category, CategoryParams } from "../../types/category"

interface CategoryState {
  categories: Category[]
  currentCategory: Category | null
  loading: boolean
  error: string | null
}

// Fetch all categories
export const fetchCategories = createAsyncThunk(
  "categories/fetchCategories",
  async (params: CategoryParams = {}, { rejectWithValue }) => {
    try {
      console.log("Fetching categories with params:", params)
      const API_URL = import.meta.env.REACT_APP_API_URL || "http://localhost:5083/api"

      // Build the URL with query parameters
      let url = `${API_URL}/Category`
      if (params.keyword) {
        url += `?keyword=${encodeURIComponent(params.keyword)}`
      }

      console.log("Fetching from URL:", url)

      const response = await fetch(url, {
        headers: {
          Accept: "application/json",
          "Content-Type": "application/json",
        },
      })

      if (!response.ok) {
        let errorText
        try {
          errorText = await response.text()
          console.error(`API request failed with status ${response.status}:`, errorText)
        } catch (e) {
          errorText = `Could not read error response: ${e}`
        }
        return rejectWithValue(`API request failed with status ${response.status}: ${errorText}`)
      }

      const data = await response.json()
      console.log("Fetch categories response:", data)

      return data as Category[]
    } catch (error) {
      console.error("Error fetching categories:", error)
      return rejectWithValue(error instanceof Error ? error.message : "Failed to fetch categories")
    }
  },
)

// Fetch a single category by ID
export const fetchCategoryById = createAsyncThunk(
  "categories/fetchCategoryById",
  async (id: number, { rejectWithValue }) => {
    try {
      console.log(`Fetching category with ID: ${id}`)
      const API_URL = import.meta.env.REACT_APP_API_URL || "http://localhost:5083/api"

      const response = await fetch(`${API_URL}/Category/${id}`)

      if (!response.ok) {
        let errorText
        try {
          errorText = await response.text()
          console.error(`API request failed with status ${response.status}:`, errorText)
        } catch (e) {
          errorText = `Could not read error response: ${e}`
        }
        return rejectWithValue(`API request failed with status ${response.status}: ${errorText}`)
      }

      const data = await response.json()
      console.log("Category API response:", data)

      return data as Category
    } catch (error) {
      console.error("Error fetching category:", error)
      return rejectWithValue(error instanceof Error ? error.message : "Failed to fetch category")
    }
  },
)

// Create a new category
export const createCategory = createAsyncThunk(
  "categories/createCategory",
  async (categoryData: Partial<Category>, { rejectWithValue }) => {
    try {
      console.log("Creating category with data:", categoryData)
      const API_URL = import.meta.env.REACT_APP_API_URL || "http://localhost:5083/api"

      const response = await fetch(`${API_URL}/Category`, {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify(categoryData),
      })

      if (!response.ok) {
        let errorText
        try {
          errorText = await response.text()
          console.error(`API request failed with status ${response.status}:`, errorText)
        } catch (e) {
          errorText = `Could not read error response: ${e}`
        }
        return rejectWithValue(`API request failed with status ${response.status}: ${errorText}`)
      }

      const data = await response.json()
      console.log("Create category response:", data)

      return data as Category
    } catch (error) {
      console.error("Error creating category:", error)
      return rejectWithValue(error instanceof Error ? error.message : "Failed to create category")
    }
  },
)

// Update an existing category
export const updateCategory = createAsyncThunk(
  "categories/updateCategory",
  async ({ id, categoryData }: { id: number; categoryData: Partial<Category> }, { rejectWithValue }) => {
    try {
      console.log("Updating category with ID:", id, "Data:", categoryData)
      const API_URL = import.meta.env.REACT_APP_API_URL || "http://localhost:5083/api"

      const response = await fetch(`${API_URL}/Category/${id}`, {
        method: "PUT",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify(categoryData),
      })

      if (!response.ok) {
        let errorText
        try {
          errorText = await response.text()
          console.error(`API request failed with status ${response.status}:`, errorText)
        } catch (e) {
          errorText = `Could not read error response: ${e}`
        }
        return rejectWithValue(`API request failed with status ${response.status}: ${errorText}`)
      }

      const data = await response.json()
      console.log("Update category response:", data)

      // Return the updated category with the id
      return { ...data, id } as Category
    } catch (error) {
      console.error("Error updating category:", error)
      return rejectWithValue(error instanceof Error ? error.message : "Failed to update category")
    }
  },
)

// Delete a category
export const deleteCategory = createAsyncThunk("categories/deleteCategory", async (id: number, { rejectWithValue }) => {
  try {
    console.log(`Deleting category with ID: ${id}`)
    const API_URL = import.meta.env.REACT_APP_API_URL || "http://localhost:5083/api"

    const response = await fetch(`${API_URL}/Category/${id}`, {
      method: "DELETE",
      headers: {
        Accept: "application/json",
        "Content-Type": "application/json",
      },
    })

    if (!response.ok) {
      let errorText
      try {
        errorText = await response.text()
        console.error(`API request failed with status ${response.status}:`, errorText)
      } catch (e) {
        errorText = `Could not read error response: ${e}`
      }
      return rejectWithValue(`API request failed with status ${response.status}: ${errorText}`)
    }

    return id
  } catch (error) {
    console.error("Error deleting category:", error)
    return rejectWithValue(error instanceof Error ? error.message : "Failed to delete category")
  }
})

const initialState: CategoryState = {
  categories: [],
  currentCategory: null,
  loading: false,
  error: null,
}

const categorySlice = createSlice({
  name: "categories",
  initialState,
  reducers: {
    clearCategoryError: (state) => {
      state.error = null
    },
  },
  extraReducers: (builder) => {
    builder
      // Fetch categories
      .addCase(fetchCategories.pending, (state) => {
        state.loading = true
        state.error = null
      })
      .addCase(fetchCategories.fulfilled, (state, action: PayloadAction<Category[]>) => {
        state.loading = false
        state.categories = action.payload
      })
      .addCase(fetchCategories.rejected, (state, action) => {
        state.loading = false
        state.error = action.error.message || "Failed to fetch categories"
      })

      // Fetch category by ID
      .addCase(fetchCategoryById.pending, (state) => {
        state.loading = true
        state.error = null
      })
      .addCase(fetchCategoryById.fulfilled, (state, action: PayloadAction<Category>) => {
        state.loading = false
        state.currentCategory = action.payload
      })
      .addCase(fetchCategoryById.rejected, (state, action) => {
        state.loading = false
        state.error = action.error.message || "Failed to fetch category"
      })

      // Create category
      .addCase(createCategory.fulfilled, (state, action: PayloadAction<Category>) => {
        state.categories.push(action.payload)
      })

      // Update category
      .addCase(updateCategory.fulfilled, (state, action: PayloadAction<Category>) => {
        const index = state.categories.findIndex((category) => category.id === action.payload.id)
        if (index !== -1) {
          state.categories[index] = action.payload
        }
        if (state.currentCategory?.id === action.payload.id) {
          state.currentCategory = action.payload
        }
      })

      // Delete category
      .addCase(deleteCategory.fulfilled, (state, action: PayloadAction<number>) => {
        state.categories = state.categories.filter((category) => category.id !== action.payload)
        if (state.currentCategory?.id === action.payload) {
          state.currentCategory = null
        }
      })
  },
})

export const { clearCategoryError } = categorySlice.actions
export default categorySlice.reducer
