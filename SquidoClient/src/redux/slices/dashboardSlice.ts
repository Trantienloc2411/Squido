import { createSlice, createAsyncThunk, type PayloadAction } from "@reduxjs/toolkit"

interface DashboardStats {
  totalBooks: number
  totalUsers: number
  totalOrders: number
  revenue: number
  recentOrders: any[]
  topSellingBooks: any[]
}

interface DashboardState {
  stats: DashboardStats | null
  loading: boolean
  error: string | null
}

// Mock API call
export const fetchDashboardStats = createAsyncThunk("dashboard/fetchStats", async () => {
  // Simulate API delay
  await new Promise((resolve) => setTimeout(resolve, 1000))

  // Mock data
  const stats: DashboardStats = {
    totalBooks: 1250,
    totalUsers: 845,
    totalOrders: 328,
    revenue: 15680.75,
    recentOrders: [],
    topSellingBooks: [],
  }

  return stats
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
