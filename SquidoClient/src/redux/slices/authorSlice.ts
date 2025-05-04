import { createSlice, createAsyncThunk, type PayloadAction } from "@reduxjs/toolkit"
import { fetchFromAPI } from "../../utils/api"
import type { Author, AuthorResponse } from "../../types/author"

interface AuthorState {
  authors: Author[]
  loading: boolean
  error: string | null
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

const initialState: AuthorState = {
  authors: [],
  loading: true, // Start with loading true
  error: null,
}

const authorSlice = createSlice({
  name: "authors",
  initialState,
  reducers: {},
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
  },
})

export default authorSlice.reducer
