import { configureStore } from "@reduxjs/toolkit"
import authReducer from "./slices/authSlice"
import bookReducer from "./slices/bookSlice"
import userReducer from "./slices/userSlice"
import orderReducer from "./slices/orderSlice"
import dashboardReducer from "./slices/dashboardSlice"
import authorReducer from "./slices/authorSlice"
import categoryReducer from "./slices/categorySlice"

export const store = configureStore({
  reducer: {
    auth: authReducer,
    books: bookReducer,
    users: userReducer,
    orders: orderReducer,
    dashboard: dashboardReducer,
    authors: authorReducer,
    categories: categoryReducer,
  },
  middleware: (getDefaultMiddleware) =>
    getDefaultMiddleware({
      serializableCheck: false,
    }),
})

export type RootState = ReturnType<typeof store.getState>
export type AppDispatch = typeof store.dispatch
