import { createSlice, createAsyncThunk, type PayloadAction } from "@reduxjs/toolkit"
import { fetchFromAPI } from "../../utils/api"
import type { Book, BookResponse, BookParams } from "../../types/book"
import type { Category } from "../../types/category"
import type { BookDetail } from "../../types/book"

// Update the BookState interface to include error
interface BookState {
  books: Book[]
  currentBook: Book | null
  bookDetail: BookDetail | null
  categories: Category[]
  loading: boolean
  categoriesLoading: boolean
  error: string | null
  pagination: {
    currentPage: number
    pageCount: number
    totalCount: number
    hasMore: boolean
  }
}

// Update the fetchBooks function in bookSlice.ts

// Fetch books from API with pagination
export const fetchBooks = createAsyncThunk("books/fetchBooks", async (params: BookParams = {}, { rejectWithValue }) => {
  const { page = 1, pageSize = 20, keyword, categoryId } = params

  try {
    console.log("Fetching books with params:", params)
    const API_URL = import.meta.env.REACT_APP_API_URL || "http://localhost:5083/api"
    console.log("Using API URL:", API_URL)

    let url
    let queryParams

    // If categoryId is provided, use the Filter endpoint
    if (categoryId) {
      queryParams = `?page=${page}&pageSize=${pageSize}`
      if (keyword) {
        queryParams += `&keyword=${encodeURIComponent(keyword)}`
      }
      url = `${API_URL}/Book/Filter/${categoryId}${queryParams}`
    } else {
      // Otherwise use the regular endpoint with query parameters
      queryParams = `?page=${page}&pageSize=${pageSize}`
      if (keyword) {
        queryParams += `&keyword=${encodeURIComponent(keyword)}`
      }
      url = `${API_URL}/Book${queryParams}`
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

    // Log the response status and headers for debugging
    console.log(`API Response Status: ${response.status} ${response.statusText}`)
    console.log("API Response Headers:", Object.fromEntries([...response.headers.entries()]))

    if (!response.ok) {
      let errorText
      try {
        errorText = await response.text()
        console.error(`API request failed with status ${response.status}:`, errorText)
      } catch (e) {
        errorText = `Could not read error response: ${e}`
        console.error(`API request failed with status ${response.status}, could not read error:`, e)
      }
      return rejectWithValue(`API request failed with status ${response.status}: ${errorText}`)
    }

    // Try to parse the response as JSON
    let data
    try {
      data = await response.json()
      console.log("Fetch books response:", data)
    } catch (e) {
      console.error("Error parsing JSON response:", e)
      return rejectWithValue(`Error parsing JSON response: ${e}`)
    }

    // Check if the API returned a success response
    if (data && typeof data.isSuccess !== "undefined" && !data.isSuccess) {
      console.error("API returned error:", data.exceptionMessage || data.message)
      return rejectWithValue(data.exceptionMessage || data.message || "Failed to fetch books")
    }

    // Handle different response formats
    let bookResponse
    if (data && data.isSuccess && data.data) {
      // If the response is wrapped in a data property
      bookResponse = data.data
    } else {
      // If the response is the direct data
      bookResponse = data
    }

    return bookResponse as BookResponse
  } catch (error) {
    // Check if it's an abort error (timeout)
    if (error.name === "AbortError") {
      console.error("Request timed out")
      return rejectWithValue("Request timed out after 30 seconds")
    }

    console.error("Error fetching books:", error)
    return rejectWithValue(error instanceof Error ? error.message : "Failed to fetch books")
  }
})

// Fetch categories from API
export const fetchCategories = createAsyncThunk("books/fetchCategories", async () => {
  try {
    const response = await fetchFromAPI("Category")
    return response as Category[]
  } catch (error) {
    throw new Error("Failed to fetch categories")
  }
})

// Fetch more books (for pagination)
export const fetchMoreBooks = createAsyncThunk("books/fetchMoreBooks", async (params: BookParams = {}) => {
  const { page = 1, pageSize = 20, keyword, categoryId } = params

  try {
    let response

    // If categoryId is provided, use the Filter endpoint
    if (categoryId) {
      let queryParams = `?page=${page}&pageSize=${pageSize}`

      if (keyword) {
        queryParams += `&keyword=${encodeURIComponent(keyword)}`
      }

      response = await fetchFromAPI(`Book/Filter/${categoryId}${queryParams}`)
    } else {
      // Otherwise use the regular endpoint with query parameters
      let queryParams = `page=${page}&pageSize=${pageSize}`

      if (keyword) {
        queryParams += `&keyword=${encodeURIComponent(keyword)}`
      }

      response = await fetchFromAPI(`Book?${queryParams}`)
    }

    return response as BookResponse
  } catch (error) {
    throw new Error("Failed to fetch more books")
  }
})

// Fetch a single book by ID - FIXED VERSION
export const fetchBookById = createAsyncThunk("books/fetchBookById", async (id: string, { rejectWithValue }) => {
  try {
    console.log(`Fetching book with ID: ${id}`)
    const API_URL = import.meta.env.REACT_APP_API_URL || "http://localhost:5083/api"

    const response = await fetch(`${API_URL}/Book/${id}`)

    if (!response.ok) {
      const errorText = await response.text()
      console.error(`API request failed with status ${response.status}:`, errorText)
      return rejectWithValue(`API request failed with status ${response.status}: ${errorText}`)
    }

    const data = await response.json()
    console.log("Book API response:", data)

    // Log author information specifically
    if (data && data.book) {
      console.log("Author information in response:", {
        authorId: data.book.authorId,
        authorName: data.book.authorName,
      })
    } else if (data && data.data && data.data.book) {
      console.log("Author information in response.data:", {
        authorId: data.data.book.authorId,
        authorName: data.data.book.authorName,
      })
    } else if (data) {
      console.log("Author information in direct data:", {
        authorId: data.authorId,
        authorName: data.authorName,
      })
    }

    // Handle different response formats
    let bookDetail

    // Check if the response has isSuccess property
    if (data && typeof data.isSuccess !== "undefined") {
      // If isSuccess is false, reject with error message
      if (!data.isSuccess) {
        console.error("API returned error:", data.exceptionMessage || data.message)
        return rejectWithValue(data.exceptionMessage || data.message || "Failed to fetch book details")
      }

      // If isSuccess is true, extract data
      bookDetail = data.data
    } else {
      // If the response doesn't have isSuccess property, use the data directly
      bookDetail = data
    }

    // If bookDetail is null or undefined, reject
    if (!bookDetail) {
      console.error("Invalid response format: bookDetail is null or undefined")
      return rejectWithValue("Invalid response format: bookDetail is null or undefined")
    }

    // If bookDetail doesn't have a book property, check if it is the book itself
    if (!bookDetail.book) {
      // If the response has properties that look like a book, wrap it
      if (bookDetail.id || bookDetail.title) {
        bookDetail = { book: bookDetail }
      } else {
        console.error("Invalid response format: no book data found", bookDetail)
        return rejectWithValue("Invalid response format: no book data found")
      }
    }

    console.log("Processed book detail:", bookDetail)
    return bookDetail
  } catch (error) {
    console.error("Error fetching book:", error)
    return rejectWithValue(error instanceof Error ? error.message : "Failed to fetch book")
  }
})

// Update the createBook function in bookSlice.ts

// Also update the createBook function to match the API structure
// Find the createBook function and replace it with this implementation:

export const createBook = createAsyncThunk("books/createBook", async (bookData: any, { rejectWithValue }) => {
  try {
    console.log("Creating book with data:", bookData)
    const API_URL = import.meta.env.REACT_APP_API_URL || "http://localhost:5083/api"

    // Prepare the request body to match the API structure
    const requestBody = {
      title: bookData.title,
      categoryId: bookData.categoryId,
      authorId: bookData.authorId,
      description: bookData.description,
      quantity: bookData.quantity,
      price: bookData.price,
      imageUrl: bookData.imageUrl,
    }

    console.log("Request body:", requestBody)

    const response = await fetch(`${API_URL}/Book`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify(requestBody),
    })

    const responseData = await response.json()
    console.log("Create book response:", responseData)

    if (!response.ok) {
      return rejectWithValue(responseData.exceptionMessage || responseData.message || "Failed to create book")
    }

    if (responseData && typeof responseData.isSuccess !== "undefined" && !responseData.isSuccess) {
      return rejectWithValue(responseData.exceptionMessage || responseData.message || "Failed to create book")
    }

    // Return the created book data
    return responseData.data || responseData
  } catch (error) {
    console.error("Error creating book:", error)
    return rejectWithValue(error instanceof Error ? error.message : "Failed to create book")
  }
})

// Update the updateBook function to match the API structure
// Find the updateBook function and replace it with this implementation:

// Find the updateBook function and check how it's handling authorId

// Update the updateBook function to ensure authorId is properly included
export const updateBook = createAsyncThunk(
  "books/updateBook",
  async ({ id, bookData }: { id: string; bookData: any }, { rejectWithValue }) => {
    try {
      console.log("Updating book with ID:", id, "Data:", bookData)
      const API_URL = import.meta.env.REACT_APP_API_URL || "http://localhost:5083/api"

      // Ensure authorId is included and not null
      if (!bookData.authorId) {
        console.warn("Warning: authorId is null or undefined in update request")
      }

      // Prepare the request body to match the API structure
      const requestBody = {
        title: bookData.title,
        categoryId: bookData.categoryId,
        authorId: bookData.authorId || "", // Ensure it's not null
        description: bookData.description,
        quantity: bookData.quantity,
        price: bookData.price,
        imageUrl: bookData.imageUrl,
      }

      console.log("Request body:", requestBody)

      const response = await fetch(`${API_URL}/Book/${id}`, {
        method: "PUT",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify(requestBody),
      })

      const responseData = await response.json()
      console.log("Update book response:", responseData)

      if (!response.ok) {
        return rejectWithValue(responseData.exceptionMessage || responseData.message || "Failed to update book")
      }

      if (responseData && typeof responseData.isSuccess !== "undefined" && !responseData.isSuccess) {
        return rejectWithValue(responseData.exceptionMessage || responseData.message || "Failed to update book")
      }

      // Return the updated book data
      return responseData.data || responseData
    } catch (error) {
      console.error("Error updating book:", error)
      return rejectWithValue(error instanceof Error ? error.message : "Failed to update book")
    }
  },
)

// Delete a book
export const deleteBook = createAsyncThunk("books/deleteBook", async (id: string) => {
  try {
    const response = await fetch(`${import.meta.env.REACT_APP_API_URL}/Book/${id}`, {
      method: "DELETE",
    })

    if (!response.ok) {
      throw new Error("Failed to delete book")
    }

    return id
  } catch (error) {
    throw new Error("Failed to delete book")
  }
})

// Update the initialState to include error
const initialState: BookState = {
  books: [],
  currentBook: null,
  bookDetail: null,
  categories: [],
  loading: false,
  categoriesLoading: false,
  error: null,
  pagination: {
    currentPage: 1,
    pageCount: 1,
    totalCount: 0,
    hasMore: false,
  },
}

// Update the extraReducers to handle errors properly
const bookSlice = createSlice({
  name: "books",
  initialState,
  reducers: {
    clearBookError: (state) => {
      state.error = null
    },
  },
  extraReducers: (builder) => {
    builder
      // Fetch books
      .addCase(fetchBooks.pending, (state) => {
        state.loading = true
        state.error = null
      })
      .addCase(fetchBooks.fulfilled, (state, action: PayloadAction<BookResponse>) => {
        state.loading = false
        state.books = action.payload.data
        state.pagination = {
          currentPage: action.payload.currentPage,
          pageCount: action.payload.pageCount,
          totalCount: action.payload.totalCount,
          hasMore: action.payload.currentPage < action.payload.pageCount,
        }
      })
      .addCase(fetchBooks.rejected, (state, action) => {
        state.loading = false
        state.error = action.error.message || "Failed to fetch books"
      })

      // Fetch categories
      .addCase(fetchCategories.pending, (state) => {
        state.categoriesLoading = true
      })
      .addCase(fetchCategories.fulfilled, (state, action: PayloadAction<Category[]>) => {
        state.categoriesLoading = false
        state.categories = action.payload
      })
      .addCase(fetchCategories.rejected, (state) => {
        state.categoriesLoading = false
      })

      // Fetch more books (pagination)
      .addCase(fetchMoreBooks.pending, (state) => {
        state.loading = true
        state.error = null
      })
      .addCase(fetchMoreBooks.fulfilled, (state, action: PayloadAction<BookResponse>) => {
        state.loading = false
        // Append new books to existing books
        state.books = [...state.books, ...action.payload.data]
        state.pagination = {
          currentPage: action.payload.currentPage,
          pageCount: action.payload.pageCount,
          totalCount: action.payload.totalCount,
          hasMore: action.payload.currentPage < action.payload.pageCount,
        }
      })
      .addCase(fetchMoreBooks.rejected, (state, action) => {
        state.loading = false
        state.error = action.error.message || "Failed to fetch more books"
      })

      // Fetch book by ID
      .addCase(fetchBookById.pending, (state) => {
        state.loading = true
        state.error = null
        state.bookDetail = null
      })
      .addCase(fetchBookById.fulfilled, (state, action: PayloadAction<BookDetail>) => {
        state.loading = false
        state.currentBook = action.payload.book
        state.bookDetail = action.payload
        state.error = null
      })
      .addCase(fetchBookById.rejected, (state, action) => {
        state.loading = false
        state.error = (action.payload as string) || action.error.message || "Failed to fetch book"
        console.error("Book fetch rejected:", action.payload || action.error.message)
      })

      // Create book
      .addCase(createBook.fulfilled, (state, action: PayloadAction<Book>) => {
        state.books.unshift(action.payload)
      })
      .addCase(createBook.rejected, (state, action) => {
        state.error = (action.payload as string) || action.error.message || "Failed to create book"
      })

      // Update book
      .addCase(updateBook.fulfilled, (state, action: PayloadAction<Book>) => {
        const index = state.books.findIndex((book) => book.id === action.payload.id)
        if (index !== -1) {
          state.books[index] = action.payload
        }
        if (state.currentBook?.id === action.payload.id) {
          state.currentBook = action.payload
        }
      })
      .addCase(updateBook.rejected, (state, action) => {
        state.error = (action.payload as string) || action.error.message || "Failed to update book"
      })

      // Delete book
      .addCase(deleteBook.fulfilled, (state, action: PayloadAction<string>) => {
        state.books = state.books.filter((book) => book.id !== action.payload)
        if (state.currentBook?.id === action.payload) {
          state.currentBook = null
        }
      })
      .addCase(deleteBook.rejected, (state, action) => {
        state.error = action.error.message || "Failed to delete book"
      })
  },
})

export const { clearBookError } = bookSlice.actions
export default bookSlice.reducer
