import { createSlice, createAsyncThunk, type PayloadAction } from "@reduxjs/toolkit"
import type { User } from "../../types/user"

interface UserState {
  users: User[]
  loading: boolean
  error: string | null
}

interface FetchUsersParams {
  search?: string
}

// Mock data
const mockUsers: User[] = [
  {
    id: "1",
    name: "John Doe",
    email: "john.doe@example.com",
    role: "admin",
    status: "active",
    lastLogin: "2023-05-01",
    avatar: "/abstract-geometric-shapes.png",
  },
  {
    id: "2",
    name: "Jane Smith",
    email: "jane.smith@example.com",
    role: "manager",
    status: "active",
    lastLogin: "2023-05-02",
    avatar: "/abstract-geometric-shapes.png",
  },
  {
    id: "3",
    name: "Robert Johnson",
    email: "robert.johnson@example.com",
    role: "editor",
    status: "inactive",
    lastLogin: "2023-04-15",
    avatar: "/abstract-geometric-shapes.png",
  },
  {
    id: "4",
    name: "Emily Davis",
    email: "emily.davis@example.com",
    role: "user",
    status: "active",
    lastLogin: "2023-05-03",
    avatar: "/abstract-geometric-shapes.png",
  },
  {
    id: "5",
    name: "Michael Wilson",
    email: "michael.wilson@example.com",
    role: "user",
    status: "suspended",
    lastLogin: "2023-03-20",
    avatar: "/abstract-geometric-shapes.png",
  },
]

// Mock API calls
export const fetchUsers = createAsyncThunk("users/fetchUsers", async (params: FetchUsersParams = {}) => {
  // Simulate API delay
  await new Promise((resolve) => setTimeout(resolve, 1000))

  let filteredUsers = [...mockUsers]

  if (params.search) {
    const searchLower = params.search.toLowerCase()
    filteredUsers = filteredUsers.filter(
      (user) => user.name.toLowerCase().includes(searchLower) || user.email.toLowerCase().includes(searchLower),
    )
  }

  return filteredUsers
})

export const createUser = createAsyncThunk("users/createUser", async (userData: User) => {
  // Simulate API delay
  await new Promise((resolve) => setTimeout(resolve, 1000))

  // In a real app, this would make an API call
  // Create a plain object instead of using a constructor
  const newUser: User = {
    ...userData,
    id: Date.now().toString(),
    lastLogin: "Never",
    avatar: "/abstract-geometric-shapes.png",
  }

  return newUser
})

export const updateUser = createAsyncThunk(
  "users/updateUser",
  async ({ id, userData }: { id: string; userData: User }) => {
    // Simulate API delay
    await new Promise((resolve) => setTimeout(resolve, 1000))

    // In a real app, this would make an API call
    // Create a plain object instead of using a constructor
    return {
      ...userData,
      id,
    }
  },
)

export const deleteUser = createAsyncThunk("users/deleteUser", async (id: string) => {
  // Simulate API delay
  await new Promise((resolve) => setTimeout(resolve, 1000))

  // In a real app, this would make an API call
  return id
})

const initialState: UserState = {
  users: [],
  loading: false,
  error: null,
}

const userSlice = createSlice({
  name: "users",
  initialState,
  reducers: {},
  extraReducers: (builder) => {
    builder
      .addCase(fetchUsers.pending, (state) => {
        state.loading = true
        state.error = null
      })
      .addCase(fetchUsers.fulfilled, (state, action: PayloadAction<User[]>) => {
        state.loading = false
        state.users = action.payload
      })
      .addCase(fetchUsers.rejected, (state, action) => {
        state.loading = false
        state.error = action.error.message || "Failed to fetch users"
      })
      .addCase(createUser.fulfilled, (state, action: PayloadAction<User>) => {
        state.users.push(action.payload)
      })
      .addCase(updateUser.fulfilled, (state, action: PayloadAction<User>) => {
        const index = state.users.findIndex((user) => user.id === action.payload.id)
        if (index !== -1) {
          state.users[index] = action.payload
        }
      })
      .addCase(deleteUser.fulfilled, (state, action: PayloadAction<string>) => {
        state.users = state.users.filter((user) => user.id !== action.payload)
      })
  },
})

export default userSlice.reducer
