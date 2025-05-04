import { createSlice, createAsyncThunk, type PayloadAction } from "@reduxjs/toolkit"
import { fetchFromAPI } from "../../utils/api"

interface TopBook {
  bookId: string
  title: string
  categoryName: string
  authorName: string | null
  quantity: number
  price: number
  buyCount: number
  imageUrls: string[]
  createdDate: string
  updatedDate: string | null
}

interface TopCategory {
  categoryId: number
  name: string
  description: string | null
  bookCount: number
}

interface DashboardStats {
  totalBooks: number
  totalCategories: number
  totalCustomers: number
  totalRevenues: number
  topBooks: TopBook[]
  topCategories: TopCategory[]
}

interface DashboardState {
  stats: DashboardStats | null
  loading: boolean
  error: string | null
}

// Fetch dashboard stats from API
export const fetchDashboardStats = createAsyncThunk("dashboard/fetchStats", async () => {
  try {
    const stats = await fetchFromAPI("Stats")
    return stats
  } catch (error) {
    throw new Error("Failed to fetch dashboard statistics")
  }
})

const initialState: DashboardState = {
  stats: null,
  loading: false,
  error: null,
}

const dashboardSlice = createSlice({
  name: "dashboard",
  initialState,
  reducers: {},
  extraReducers: (builder) => {
    builder
      .addCase(fetchDashboardStats.pending, (state) => {
        state.loading = true
        state.error = null
      })
      .addCase(fetchDashboardStats.fulfilled, (state, action: PayloadAction<DashboardStats>) => {
        state.loading = false
        state.stats = action.payload
      })
      .addCase(fetchDashboardStats.rejected, (state, action) => {
        state.loading = false
        state.error = action.error.message || "Failed to fetch dashboard stats"
      })
  },
})

export default dashboardSlice.reducer
