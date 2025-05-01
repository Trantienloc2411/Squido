import { configureStore } from "@reduxjs/toolkit"
import authReducer from "./slices/authSlice"
import bookReducer from "./slices/bookSlice"
import userReducer from "./slices/userSlice"
import orderReducer from "./slices/orderSlice"
import dashboardReducer from "./slices/dashboardSlice"

export const store = configureStore({
  reducer: {
    auth: authReducer,
    books: bookReducer,
    users: userReducer,
    orders: orderReducer,
    dashboard: dashboardReducer,
  },
  middleware: (getDefaultMiddleware) =>
    getDefaultMiddleware({
      serializableCheck: false,
    }),
})

export type RootState = ReturnType<typeof store.getState>
export type AppDispatch = typeof store.dispatch
