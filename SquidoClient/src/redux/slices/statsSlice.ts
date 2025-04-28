import { createSlice, createAsyncThunk } from "@reduxjs/toolkit"

// Types
interface Stats {
  totalProducts: number
  totalCategories: number
  totalCustomers: number
  totalRevenue: number
}

interface StatsState {
  stats: Stats | null
  loading: boolean
  error: string | null
}

// Initial state
const initialState: StatsState = {
  stats: null,
  loading: false,
  error: null,
}

// Async thunks
export const fetchStats = createAsyncThunk("stats/fetchStats", async () => {
  // Simulate API call
  await new Promise((resolve) => setTimeout(resolve, 500))

  // Mock stats data
  return {
    totalProducts: 125,
    totalCategories: 8,
    totalCustomers: 450,
    totalRevenue: 28750.5,
  }
})

// Slice
const statsSlice = createSlice({
  name: "stats",
  initialState,
  reducers: {},
  extraReducers: (builder) => {
    builder
      .addCase(fetchStats.pending, (state) => {
        state.loading = true
        state.error = null
      })
      .addCase(fetchStats.fulfilled, (state, action) => {
        state.loading = false
        state.stats = action.payload
      })
      .addCase(fetchStats.rejected, (state, action) => {
        state.loading = false
        state.error = action.error.message || "Failed to fetch stats"
      })
  },
})

export default statsSlice.reducer
