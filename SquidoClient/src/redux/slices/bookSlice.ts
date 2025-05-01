import { createSlice, createAsyncThunk, type PayloadAction } from "@reduxjs/toolkit"
import type { Book } from "../../types/book"

interface BookState {
  books: Book[]
  currentBook: Book | null
  loading: boolean
  error: string | null
}

interface FetchBooksParams {
  search?: string
  category?: string
}

// Mock data
const mockBooks: Book[] = [
  {
    id: "1",
    title: "The Great Gatsby",
    author: "F. Scott Fitzgerald",
    isbn: "9780743273565",
    description: "A classic novel about the American Dream.",
    category: "fiction",
    price: 12.99,
    stock: 25,
    publisher: "Scribner",
    publishedDate: "2004-09-30",
    language: "English",
    pages: 180,
    coverImage: "/open-book-library.png",
  },
  {
    id: "2",
    title: "To Kill a Mockingbird",
    author: "Harper Lee",
    isbn: "9780061120084",
    description: "A novel about racial injustice in the American South.",
    category: "fiction",
    price: 14.99,
    stock: 18,
    publisher: "HarperPerennial",
    publishedDate: "2006-05-23",
    language: "English",
    pages: 336,
    coverImage: "/open-book-library.png",
  },
  {
    id: "3",
    title: "1984",
    author: "George Orwell",
    isbn: "9780451524935",
    description: "A dystopian novel about totalitarianism.",
    category: "sci-fi",
    price: 10.99,
    stock: 0,
    publisher: "Signet Classic",
    publishedDate: "1961-01-01",
    language: "English",
    pages: 328,
    coverImage: "/open-book-library.png",
  },
]

// Mock API calls
export const fetchBooks = createAsyncThunk("books/fetchBooks", async (params: FetchBooksParams = {}) => {
  // Simulate API delay
  await new Promise((resolve) => setTimeout(resolve, 1000))

  let filteredBooks = [...mockBooks]

  if (params.search) {
    const searchLower = params.search.toLowerCase()
    filteredBooks = filteredBooks.filter(
      (book) =>
        book.title.toLowerCase().includes(searchLower) ||
        book.author.toLowerCase().includes(searchLower) ||
        book.isbn.includes(params.search),
    )
  }

  if (params.category) {
    filteredBooks = filteredBooks.filter((book) => book.category === params.category)
  }

  return filteredBooks
})

export const fetchBookById = createAsyncThunk("books/fetchBookById", async (id: string) => {
  // Simulate API delay
  await new Promise((resolve) => setTimeout(resolve, 1000))

  const book = mockBooks.find((book) => book.id === id)
  if (!book) {
    throw new Error("Book not found")
  }

  return book
})

export const createBook = createAsyncThunk("books/createBook", async (bookData: Book) => {
  // Simulate API delay
  await new Promise((resolve) => setTimeout(resolve, 1000))

  // In a real app, this would make an API call
  const newBook = {
    ...bookData,
    id: Date.now().toString(),
  }

  return newBook
})

export const updateBook = createAsyncThunk(
  "books/updateBook",
  async ({ id, bookData }: { id: string; bookData: Book }) => {
    // Simulate API delay
    await new Promise((resolve) => setTimeout(resolve, 1000))

    // In a real app, this would make an API call
    return {
      ...bookData,
      id,
    }
  },
)

export const deleteBook = createAsyncThunk("books/deleteBook", async (id: string) => {
  // Simulate API delay
  await new Promise((resolve) => setTimeout(resolve, 1000))

  // In a real app, this would make an API call
  return id
})

const initialState: BookState = {
  books: [],
  currentBook: null,
  loading: false,
  error: null,
}

const bookSlice = createSlice({
  name: "books",
  initialState,
  reducers: {},
  extraReducers: (builder) => {
    builder
      .addCase(fetchBooks.pending, (state) => {
        state.loading = true
        state.error = null
      })
      .addCase(fetchBooks.fulfilled, (state, action: PayloadAction<Book[]>) => {
        state.loading = false
        state.books = action.payload
      })
      .addCase(fetchBooks.rejected, (state, action) => {
        state.loading = false
        state.error = action.error.message || "Failed to fetch books"
      })
      .addCase(fetchBookById.pending, (state) => {
        state.loading = true
        state.error = null
      })
      .addCase(fetchBookById.fulfilled, (state, action: PayloadAction<Book>) => {
        state.loading = false
        state.currentBook = action.payload
      })
      .addCase(fetchBookById.rejected, (state, action) => {
        state.loading = false
        state.error = action.error.message || "Failed to fetch book"
      })
      .addCase(createBook.fulfilled, (state, action: PayloadAction<Book>) => {
        state.books.push(action.payload)
      })
      .addCase(updateBook.fulfilled, (state, action: PayloadAction<Book>) => {
        const index = state.books.findIndex((book) => book.id === action.payload.id)
        if (index !== -1) {
          state.books[index] = action.payload
        }
        if (state.currentBook?.id === action.payload.id) {
          state.currentBook = action.payload
        }
      })
      .addCase(deleteBook.fulfilled, (state, action: PayloadAction<string>) => {
        state.books = state.books.filter((book) => book.id !== action.payload)
        if (state.currentBook?.id === action.payload) {
          state.currentBook = null
        }
      })
  },
})

export default bookSlice.reducer
