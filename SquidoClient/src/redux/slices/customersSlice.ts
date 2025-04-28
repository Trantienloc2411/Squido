import { createSlice, createAsyncThunk } from "@reduxjs/toolkit"

// Mock data
const mockCustomers = [
  {
    id: "1",
    name: "John Doe",
    email: "john.doe@example.com",
    orders: 5,
    joinedDate: "2023-01-10T12:00:00Z",
    avatar: null,
  },
  {
    id: "2",
    name: "Jane Smith",
    email: "jane.smith@example.com",
    orders: 8,
    joinedDate: "2023-02-15T12:00:00Z",
    avatar: null,
  },
  {
    id: "3",
    name: "Robert Johnson",
    email: "robert.johnson@example.com",
    orders: 3,
    joinedDate: "2023-03-20T12:00:00Z",
    avatar: null,
  },
  {
    id: "4",
    name: "Emily Davis",
    email: "emily.davis@example.com",
    orders: 12,
    joinedDate: "2023-01-05T12:00:00Z",
    avatar: null,
  },
  {
    id: "5",
    name: "Michael Wilson",
    email: "michael.wilson@example.com",
    orders: 7,
    joinedDate: "2023-04-18T12:00:00Z",
    avatar: null,
  },
]

// Types
interface Customer {
  id: string
  name: string
  email: string
  orders: number
  joinedDate: string
  avatar: string | null
}

interface CustomersState {
  customers: Customer[]
  loading: boolean
  error: string | null
}

// Initial state
const initialState: CustomersState = {
  customers: [],
  loading: false,
  error: null,
}

// Async thunks
export const fetchCustomers = createAsyncThunk("customers/fetchCustomers", async () => {
  // Simulate API call
  await new Promise((resolve) => setTimeout(resolve, 500))
  return mockCustomers
})

// Slice
const customersSlice = createSlice({
  name: "customers",
  initialState,
  reducers: {},
  extraReducers: (builder) => {
    builder
      .addCase(fetchCustomers.pending, (state) => {
        state.loading = true
        state.error = null
      })
      .addCase(fetchCustomers.fulfilled, (state, action) => {
        state.loading = false
        state.customers = action.payload
      })
      .addCase(fetchCustomers.rejected, (state, action) => {
        state.loading = false
        state.error = action.error.message || "Failed to fetch customers"
      })
  },
})

export default customersSlice.reducer
