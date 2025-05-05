import { createSlice, createAsyncThunk, type PayloadAction } from "@reduxjs/toolkit"

interface User {
  id: string
  name: string
  email: string
  role: string
}

interface AuthState {
  user: User | null
  isAuthenticated: boolean
  loading: boolean
  error: string | null
}

interface LoginCredentials {
  email: string
  password: string
}

// Mock API call
const loginApi = async (credentials: LoginCredentials): Promise<User> => {
  // Simulate API delay
  await new Promise((resolve) => setTimeout(resolve, 1000))

  // Mock successful login for demo
  if (credentials.email && credentials.password) {
    return {
      id: "1",
      name: "Admin User",
      email: credentials.email,
      role: "admin",
    }
  }

  throw new Error("Invalid credentials")
}

export const login = createAsyncThunk("auth/login", async (credentials: LoginCredentials) => {
  const response = await loginApi(credentials)
  return response
})

const initialState: AuthState = {
  user: {
    id: "1",
    name: "Admin User",
    email: "admin@squido.com",
    role: "admin",
  },
  isAuthenticated: true,
  loading: false,
  error: null,
}

const authSlice = createSlice({
  name: "auth",
  initialState,
  reducers: {
    logout: (state) => {
      state.user = null
      state.isAuthenticated = false
    },
  },
  extraReducers: (builder) => {
    builder
      .addCase(login.pending, (state) => {
        state.loading = true
        state.error = null
      })
      .addCase(login.fulfilled, (state, action: PayloadAction<User>) => {
        state.loading = false
        state.user = action.payload
        state.isAuthenticated = true
      })
      .addCase(login.rejected, (state, action) => {
        state.loading = false
        state.error = action.error.message || "Login failed"
      })
  },
})

export const { logout } = authSlice.actions
export default authSlice.reducer
