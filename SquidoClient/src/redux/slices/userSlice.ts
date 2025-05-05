import { createSlice, createAsyncThunk, type PayloadAction } from "@reduxjs/toolkit"
import type { User, UserResponse, UserParams } from "../../types/user"

interface UserState {
  users: User[]
  currentUser: User | null
  loading: boolean
  error: string | null
  pagination: {
    currentPage: number
    pageCount: number
    totalCount: number
    hasMore: boolean
  }
}

// Fetch users from API with pagination
export const fetchUsers = createAsyncThunk("users/fetchUsers", async (params: UserParams = {}, { rejectWithValue }) => {
  const { page = 1, pageSize = 10, keyword } = params

  try {
    console.log("Fetching users with params:", params)
    const API_URL = import.meta.env.REACT_APP_API_URL || "http://localhost:5083/api"

    // Build the URL with query parameters
    let url = `${API_URL}/User?page=${page}&pageSize=${pageSize}`
    if (keyword) {
      url += `&keyword=${encodeURIComponent(keyword)}`
    }

    console.log("Fetching from URL:", url)

    // Add a timeout to the fetch request
    const controller = new AbortController()
    const timeoutId = setTimeout(() => controller.abort(), 30000) // 30 second timeout

    const response = await fetch(url, {
      signal: controller.signal,
      headers: {
        Accept: "application/json",
        "Content-Type": "application/json",
      },
    })

    clearTimeout(timeoutId)

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
    console.log("Fetch users response:", data)

    return data as UserResponse
  } catch (error) {
    // Check if it's an abort error (timeout)
    if (error.name === "AbortError") {
      console.error("Request timed out")
      return rejectWithValue("Request timed out after 30 seconds")
    }

    console.error("Error fetching users:", error)
    return rejectWithValue(error instanceof Error ? error.message : "Failed to fetch users")
  }
})

// Fetch more users (for pagination)
export const fetchMoreUsers = createAsyncThunk(
  "users/fetchMoreUsers",
  async (params: UserParams = {}, { rejectWithValue }) => {
    const { page = 1, pageSize = 10, keyword } = params

    try {
      console.log("Fetching more users with params:", params)
      const API_URL = import.meta.env.REACT_APP_API_URL || "http://localhost:5083/api"

      // Build the URL with query parameters
      let url = `${API_URL}/User?page=${page}&pageSize=${pageSize}`
      if (keyword) {
        url += `&keyword=${encodeURIComponent(keyword)}`
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
      console.log("Fetch more users response:", data)

      return data as UserResponse
    } catch (error) {
      console.error("Error fetching more users:", error)
      return rejectWithValue(error instanceof Error ? error.message : "Failed to fetch more users")
    }
  },
)

// Fetch a single user by ID
export const fetchUserById = createAsyncThunk("users/fetchUserById", async (id: string, { rejectWithValue }) => {
  try {
    console.log(`Fetching user with ID: ${id}`)
    const API_URL = import.meta.env.REACT_APP_API_URL || "http://localhost:5083/api"

    const response = await fetch(`${API_URL}/User/${id}`)

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
    console.log("User API response:", data)

    return data as User
  } catch (error) {
    console.error("Error fetching user:", error)
    return rejectWithValue(error instanceof Error ? error.message : "Failed to fetch user")
  }
})

// Update user
export const updateUser = createAsyncThunk(
  "users/updateUser",
  async ({ id, userData }: { id: string; userData: Partial<User> }, { rejectWithValue }) => {
    try {
      console.log("Updating user with ID:", id, "Data:", userData)
      const API_URL = import.meta.env.REACT_APP_API_URL || "http://localhost:5083/api"

      const response = await fetch(`${API_URL}/User/${id}`, {
        method: "PUT",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify(userData),
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
      console.log("Update user response:", data)

      return data as User
    } catch (error) {
      console.error("Error updating user:", error)
      return rejectWithValue(error instanceof Error ? error.message : "Failed to update user")
    }
  },
)

// Delete user
export const deleteUser = createAsyncThunk("users/deleteUser", async (id: string, { rejectWithValue }) => {
  try {
    console.log(`Deleting user with ID: ${id}`)
    const API_URL = import.meta.env.REACT_APP_API_URL || "http://localhost:5083/api"

    const response = await fetch(`${API_URL}/User/${id}`, {
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

    // Some APIs return no content for DELETE operations
    let responseData
    try {
      const text = await response.text()
      responseData = text ? JSON.parse(text) : {}
      console.log("Delete user response:", responseData)
    } catch (e) {
      console.log("No response body or invalid JSON")
    }

    return id
  } catch (error) {
    console.error("Error deleting user:", error)
    return rejectWithValue(error instanceof Error ? error.message : "Failed to delete user")
  }
})

// Create user
export const createUser = createAsyncThunk("users/createUser", async (userData: User, { rejectWithValue }) => {
  try {
    console.log("Creating user with data:", userData)
    const API_URL = import.meta.env.REACT_APP_API_URL || "http://localhost:5083/api"

    const response = await fetch(`${API_URL}/User`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify(userData),
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
    console.log("Create user response:", data)

    return data as User
  } catch (error) {
    console.error("Error creating user:", error)
    return rejectWithValue(error instanceof Error ? error.message : "Failed to create user")
  }
})

const initialState: UserState = {
  users: [],
  currentUser: null,
  loading: false,
  error: null,
  pagination: {
    currentPage: 1,
    pageCount: 1,
    totalCount: 0,
    hasMore: false,
  },
}

const userSlice = createSlice({
  name: "users",
  initialState,
  reducers: {
    clearUserError: (state) => {
      state.error = null
    },
  },
  extraReducers: (builder) => {
    builder
      // Fetch users
      .addCase(fetchUsers.pending, (state) => {
        state.loading = true
        state.error = null
      })
      .addCase(fetchUsers.fulfilled, (state, action: PayloadAction<UserResponse>) => {
        state.loading = false
        state.users = action.payload.records
        state.pagination = {
          currentPage: action.payload.currentPage,
          pageCount: Math.ceil(action.payload.totalRecords / action.payload.pageSize),
          totalCount: action.payload.totalRecords,
          hasMore: action.payload.currentPage * action.payload.pageSize < action.payload.totalRecords,
        }
      })
      .addCase(fetchUsers.rejected, (state, action) => {
        state.loading = false
        state.error = action.error.message || "Failed to fetch users"
      })

      // Fetch more users (pagination)
      .addCase(fetchMoreUsers.pending, (state) => {
        state.loading = true
        state.error = null
      })
      .addCase(fetchMoreUsers.fulfilled, (state, action: PayloadAction<UserResponse>) => {
        state.loading = false
        // Append new users to existing users
        state.users = [...state.users, ...action.payload.records]
        state.pagination = {
          currentPage: action.payload.currentPage,
          pageCount: Math.ceil(action.payload.totalRecords / action.payload.pageSize),
          totalCount: action.payload.totalRecords,
          hasMore: action.payload.currentPage * action.payload.pageSize < action.payload.totalRecords,
        }
      })
      .addCase(fetchMoreUsers.rejected, (state, action) => {
        state.loading = false
        state.error = action.error.message || "Failed to fetch more users"
      })

      // Fetch user by ID
      .addCase(fetchUserById.pending, (state) => {
        state.loading = true
        state.error = null
      })
      .addCase(fetchUserById.fulfilled, (state, action: PayloadAction<User>) => {
        state.loading = false
        state.currentUser = action.payload
      })
      .addCase(fetchUserById.rejected, (state, action) => {
        state.loading = false
        state.error = action.error.message || "Failed to fetch user"
      })

      // Update user
      .addCase(updateUser.fulfilled, (state, action: PayloadAction<User>) => {
        const index = state.users.findIndex((user) => user.id === action.payload.id)
        if (index !== -1) {
          state.users[index] = action.payload
        }
        if (state.currentUser?.id === action.payload.id) {
          state.currentUser = action.payload
        }
      })

      // Delete user
      .addCase(deleteUser.fulfilled, (state, action: PayloadAction<string>) => {
        state.users = state.users.filter((user) => user.id !== action.payload)
        if (state.currentUser?.id === action.payload) {
          state.currentUser = null
        }
      })
      // Create user
      .addCase(createUser.fulfilled, (state, action: PayloadAction<User>) => {
        state.users.push(action.payload)
      })
  },
})

export const { clearUserError } = userSlice.actions
export default userSlice.reducer
