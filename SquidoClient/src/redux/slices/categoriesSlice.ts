import { createSlice, createAsyncThunk } from "@reduxjs/toolkit";
import { baseApiRequest } from "./baseApi";

// API response types
interface ApiCategory {
  categoryId: string | number;
  name: string;
  description: string;
  bookCount: number | null;
}

// Application state types
interface Category {
  id: string;
  name: string;
  description: string;
  bookCount: number;
}

interface CategoriesState {
  categories: Category[];
  loading: boolean;
  error: string | null;
}

// Initial state
const initialState: CategoriesState = {
  categories: [],
  loading: false,
  error: null,
};

// Helper functions
function transformApiCategory(apiCategory: ApiCategory): Category {
  return {
    id: apiCategory.categoryId.toString(),
    name: apiCategory.name,
    description: apiCategory.description,
    bookCount: apiCategory.bookCount || 0,
  };
}

// Async thunks
export const fetchCategories = createAsyncThunk<
  Category[],
  void,
  { rejectValue: string }
>("categories/fetchCategories", async (_, { rejectWithValue }) => {
  try {
    // The API returns an array directly, not an object with a categories property
    const response = await baseApiRequest<ApiCategory[]>("/Category");
    
    return response.map(transformApiCategory);
  } catch (error) {
    const errorMessage = error instanceof Error ? error.message : "Failed to fetch categories";
    console.error("Error fetching categories:", error);
    return rejectWithValue(errorMessage);
  }
});

export const addCategory = createAsyncThunk<
  Category,
  Omit<Category, "id" | "bookCount">,
  { rejectValue: string }
>("categories/addCategory", async (category, { rejectWithValue }) => {
  try {
    const response = await baseApiRequest<ApiCategory>("/Category", {
      method: "POST",
      body: JSON.stringify(category),
    });
    return transformApiCategory(response);
  } catch (error) {
    const errorMessage = error instanceof Error ? error.message : "Failed to add category";
    console.error("Error adding category:", error);
    return rejectWithValue(errorMessage);
  }
});

export const updateCategory = createAsyncThunk<
  Category,
  Omit<Category, "bookCount">,
  { rejectValue: string }
>("categories/updateCategory", async (category, { rejectWithValue }) => {
  try {
    const response = await baseApiRequest<ApiCategory>(`/Category/${category.id}`, {
      method: "PUT",
      body: JSON.stringify({
        categoryId: category.id,
        name: category.name,
        description: category.description
      }),
    });
    return transformApiCategory(response);
  } catch (error) {
    const errorMessage = error instanceof Error ? error.message : "Failed to update category";
    console.error("Error updating category:", error);
    return rejectWithValue(errorMessage);
  }
});

export const deleteCategory = createAsyncThunk<
  string,
  string,
  { rejectValue: string }
>("categories/deleteCategory", async (id, { rejectWithValue }) => {
  try {
    await baseApiRequest(`/Category/${id}`, {
      method: "DELETE",
    });
    return id;
  } catch (error) {
    const errorMessage = error instanceof Error ? error.message : "Failed to delete category";
    console.error("Error deleting category:", error);
    return rejectWithValue(errorMessage);
  }
});

// Slice
const categoriesSlice = createSlice({
  name: "categories",
  initialState,
  reducers: {
    clearCategories: (state) => {
      state.categories = [];
      state.error = null;
    },
  },
  extraReducers: (builder) => {
    builder
      // Fetch categories
      .addCase(fetchCategories.pending, (state) => {
        state.loading = true;
        state.error = null;
      })
      .addCase(fetchCategories.fulfilled, (state, action) => {
        state.loading = false;
        state.categories = action.payload;
      })
      .addCase(fetchCategories.rejected, (state, action) => {
        state.loading = false;
        state.error = action.payload || "Failed to fetch categories";
      })
      
      // Add category
      .addCase(addCategory.pending, (state) => {
        state.loading = true;
        state.error = null;
      })
      .addCase(addCategory.fulfilled, (state, action) => {
        state.loading = false;
        state.categories.push(action.payload);
      })
      .addCase(addCategory.rejected, (state, action) => {
        state.loading = false;
        state.error = action.payload || "Failed to add category";
      })
      
      // Update category
      .addCase(updateCategory.pending, (state) => {
        state.loading = true;
        state.error = null;
      })
      .addCase(updateCategory.fulfilled, (state, action) => {
        state.loading = false;
        const index = state.categories.findIndex((category) => category.id === action.payload.id);
        if (index !== -1) {
          state.categories[index] = action.payload;
        }
      })
      .addCase(updateCategory.rejected, (state, action) => {
        state.loading = false;
        state.error = action.payload || "Failed to update category";
      })
      
      // Delete category
      .addCase(deleteCategory.pending, (state) => {
        state.loading = true;
        state.error = null;
      })
      .addCase(deleteCategory.fulfilled, (state, action) => {
        state.loading = false;
        state.categories = state.categories.filter((category) => category.id !== action.payload);
      })
      .addCase(deleteCategory.rejected, (state, action) => {
        state.loading = false;
        state.error = action.payload || "Failed to delete category";
      });
  },
});

export const { clearCategories } = categoriesSlice.actions;
export default categoriesSlice.reducer;