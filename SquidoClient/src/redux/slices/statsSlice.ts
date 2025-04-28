import { createSlice, createAsyncThunk } from "@reduxjs/toolkit";
import { baseApiRequest } from "./baseApi";

// API response types
interface ApiStats {
  totalBooks: number;
  totalCategories: number;
  totalCustomers: number;
  totalRevenues: number;
  topBooks: ApiBook[];
  topCategories: ApiCategory[];
}

interface ApiBook {
  bookId: string;
  title: string;
  createdDate: string;
  imagesUrls?: string[];
}

interface ApiCategory {
  categoryId: string;
  name: string;
  bookCount: number;
}

// Application state types
interface Book {
  id: string;
  title: string;
  createdDate: string;
  imageUrl: string;
}

interface Category {
  id: string;
  name: string;
  bookCount: number;
}

interface StatsState {
  totalProducts: number;
  totalCategories: number;
  totalCustomers: number;
  totalRevenue: number;
  recentBooks: Book[];
  topCategories: Category[];
  loading: boolean;
  error: string | null;
}

// Initial state
const initialState: StatsState = {
  totalProducts: 0,
  totalCategories: 0,
  totalCustomers: 0,
  totalRevenue: 0,
  recentBooks: [],
  topCategories: [],
  loading: false,
  error: null,
};

// Helper functions
function transformBook(book: ApiBook): Book {
  return {
    id: book.bookId,
    title: book.title,
    createdDate: book.createdDate,
    imageUrl: book.imagesUrls?.length ? book.imagesUrls[0] : "/placeholder.svg",
  };
}

function transformCategory(category: ApiCategory): Category {
  return {
    id: category.categoryId,
    name: category.name,
    bookCount: category.bookCount,
  };
}

// Async thunk
export const fetchStats = createAsyncThunk<StatsState, void, { rejectValue: string }>(
  "stats/fetchStats",
  async (_, { rejectWithValue }) => {
    try {
      const response = await baseApiRequest<ApiStats>("/Stats/");
      
      
      return {
        totalProducts: response.totalBooks,
        totalCategories: response.totalCategories,
        totalCustomers: response.totalCustomers,
        totalRevenue: response.totalRevenues,
        recentBooks: response.topBooks.map(transformBook),
        topCategories: response.topCategories.map(transformCategory),
        loading: false,
        error: null,
      };
    } catch (error) {
      const errorMessage = error instanceof Error ? error.message : "Failed to fetch stats";
      console.error("Error fetching stats:", error);
      return rejectWithValue(errorMessage);
    }
  }
);

// Slice
const statsSlice = createSlice({
  name: "stats",
  initialState,
  reducers: {
    clearStats: () => initialState,
  },
  extraReducers: (builder) => {
    builder
      .addCase(fetchStats.pending, (state) => {
        state.loading = true;
        state.error = null;
      })
      .addCase(fetchStats.fulfilled, (state, action) => {
        return { ...action.payload, loading: false, error: null };
      })
      .addCase(fetchStats.rejected, (state, action) => {
        state.loading = false;
        state.error = action.payload || "Unknown error occurred";
      });
  },
});

export const { clearStats } = statsSlice.actions;
export default statsSlice.reducer;