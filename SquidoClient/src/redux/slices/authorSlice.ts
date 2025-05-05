import { createSlice, createAsyncThunk, type PayloadAction } from "@reduxjs/toolkit"
import { fetchFromAPI } from "../../utils/api"
import type { Author, AuthorResponse } from "../../types/author"

interface AuthorState {
  authors: Author[]
  loading: boolean
  error: string | null
  searchResults: Author[]
}

// Fetch authors from API
export const fetchAuthors = createAsyncThunk("authors/fetchAuthors", async () => {
  try {
    const response = await fetchFromAPI("Author")
    return response as AuthorResponse
  } catch (error) {
    throw new Error("Failed to fetch authors")
  }
})

// Search authors by name
export const searchAuthors = createAsyncThunk(
  "authors/searchAuthors",
  async (searchTerm: string, { rejectWithValue }) => {
    try {
      console.log("Searching authors with term:", searchTerm)
      const API_URL = import.meta.env.REACT_APP_API_URL || "http://localhost:5083/api"

      // Use the search endpoint or add a query parameter
      const response = await fetch(`${API_URL}/Author?keyword=${encodeURIComponent(searchTerm)}`)

      if (!response.ok) {
        const errorText = await response.text()
        console.error(`API request failed with status ${response.status}:`, errorText)
        return rejectWithValue(`API request failed with status ${response.status}: ${errorText}`)
      }

      const data = await response.json()
      console.log("Author search response:", data)

      // Handle different response formats
      let authors
      if (data && data.isSuccess && data.data) {
        authors = data.data
      } else {
        authors = data
      }

      return authors as Author[]
    } catch (error) {
      console.error("Error searching authors:", error)
      return rejectWithValue(error instanceof Error ? error.message : "Failed to search authors")
    }
  },
)

// Update the createAuthor function to match the exact API structure
export const createAuthor = createAsyncThunk(
  "authors/createAuthor",
  async (authorData: Partial<Author>, { rejectWithValue }) => {
    try {
      console.log("Creating author with data:", authorData)
      const API_URL = import.meta.env.REACT_APP_API_URL || "http://localhost:5083/api"

      // Prepare the request body to exactly match the API structure
      const requestBody = {
        fullName: authorData.fullName || null,
        bio: authorData.bio || null,
        imageUrl: authorData.imageUrl || null,
      }

      console.log("Request body:", requestBody)

      const response = await fetch(`${API_URL}/Author`, {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify(requestBody),
      })

      const responseData = await response.json()
      console.log("Create author response:", responseData)

      if (!response.ok) {
        return rejectWithValue(responseData.exceptionMessage || responseData.message || "Failed to create author")
      }

      if (responseData && typeof responseData.isSuccess !== "undefined" && !responseData.isSuccess) {
        return rejectWithValue(responseData.exceptionMessage || responseData.message || "Failed to create author")
      }

      // Return the created author data
      return responseData.data || responseData
    } catch (error) {
      console.error("Error creating author:", error)
      return rejectWithValue(error instanceof Error ? error.message : "Failed to create author")
    }
  },
)

const initialState: AuthorState = {
  authors: [],
  loading: true,
  error: null,
  searchResults: [],
}

const authorSlice = createSlice({
  name: "authors",
  initialState,
  reducers: {
    clearAuthorSearch: (state) => {
      state.searchResults = []
    },
  },
  extraReducers: (builder) => {
    builder
      .addCase(fetchAuthors.pending, (state) => {
        state.loading = true
        state.error = null
      })
      .addCase(fetchAuthors.fulfilled, (state, action: PayloadAction<AuthorResponse>) => {
        state.loading = false
        // Extract authors from the data field of the response
        state.authors = action.payload.isSuccess ? action.payload.data : []
      })
      .addCase(fetchAuthors.rejected, (state, action) => {
        state.loading = false
        state.error = action.error.message || "Failed to fetch authors"
      })
      .addCase(searchAuthors.pending, (state) => {
        state.loading = true
        state.error = null
      })
      .addCase(searchAuthors.fulfilled, (state, action: PayloadAction<Author[]>) => {
        state.loading = false
        state.searchResults = action.payload
      })
      .addCase(searchAuthors.rejected, (state, action) => {
        state.loading = false
        state.error = action.error.message || "Failed to search authors"
      })
      .addCase(createAuthor.fulfilled, (state, action: PayloadAction<Author>) => {
        state.authors.push(action.payload)
      })
  },
})

export const { clearAuthorSearch } = authorSlice.actions
export default authorSlice.reducer
