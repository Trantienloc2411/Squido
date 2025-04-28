import { configureStore } from "@reduxjs/toolkit"
import categoriesReducer from "./slices/categoriesSlice"
import productsReducer from "./slices/productsSlice"
import customersReducer from "./slices/customersSlice"
import statsReducer from "./slices/statsSlice"

export const store = configureStore({
  reducer: {
    categories: categoriesReducer,
    products: productsReducer,
    customers: customersReducer,
    stats: statsReducer,
  },
})

export type RootState = ReturnType<typeof store.getState>
export type AppDispatch = typeof store.dispatch
