import { createSlice, createAsyncThunk, type PayloadAction } from "@reduxjs/toolkit"
import type { Order } from "../../types/order"

interface OrderState {
  orders: Order[]
  loading: boolean
  error: string | null
}

interface FetchOrdersParams {
  search?: string
  status?: string
}

// Mock data
const mockOrders: Order[] = [
  {
    id: "ORD-001",
    customer: {
      name: "John Doe",
      email: "john.doe@example.com",
      phone: "555-123-4567",
    },
    date: "2023-05-01T10:30:00Z",
    items: [
      {
        id: "1",
        name: "The Great Gatsby",
        price: 12.99,
        quantity: 1,
        image: "/open-book-library.png",
      },
      {
        id: "2",
        name: "To Kill a Mockingbird",
        price: 14.99,
        quantity: 2,
        image: "/open-book-library.png",
      },
    ],
    subtotal: 42.97,
    shipping: 5.99,
    tax: 3.92,
    amount: 52.88,
    status: "Completed",
    paymentMethod: "Credit Card",
    transactionId: "TXN123456",
    shippingAddress: {
      street: "123 Main St",
      city: "Anytown",
      state: "CA",
      zip: "12345",
      country: "USA",
    },
  },
  {
    id: "ORD-002",
    customer: {
      name: "Jane Smith",
      email: "jane.smith@example.com",
      phone: "555-987-6543",
    },
    date: "2023-05-02T14:45:00Z",
    items: [
      {
        id: "3",
        name: "1984",
        price: 10.99,
        quantity: 1,
        image: "/open-book-library.png",
      },
    ],
    subtotal: 10.99,
    shipping: 4.99,
    tax: 1.28,
    amount: 17.26,
    status: "Processing",
    paymentMethod: "PayPal",
    transactionId: "TXN789012",
    shippingAddress: {
      street: "456 Oak Ave",
      city: "Somewhere",
      state: "NY",
      zip: "67890",
      country: "USA",
    },
  },
  {
    id: "ORD-003",
    customer: {
      name: "Robert Johnson",
      email: "robert.johnson@example.com",
      phone: "555-456-7890",
    },
    date: "2023-05-03T09:15:00Z",
    items: [
      {
        id: "1",
        name: "The Great Gatsby",
        price: 12.99,
        quantity: 1,
        image: "/open-book-library.png",
      },
      {
        id: "3",
        name: "1984",
        price: 10.99,
        quantity: 1,
        image: "/open-book-library.png",
      },
      {
        id: "4",
        name: "The Hobbit",
        price: 11.99,
        quantity: 1,
        image: "/open-book-library.png",
      },
    ],
    subtotal: 35.97,
    shipping: 5.99,
    tax: 3.36,
    amount: 45.32,
    status: "Completed",
    paymentMethod: "Credit Card",
    transactionId: "TXN345678",
    shippingAddress: {
      street: "789 Pine St",
      city: "Elsewhere",
      state: "TX",
      zip: "54321",
      country: "USA",
    },
  },
  {
    id: "ORD-004",
    customer: {
      name: "Emily Davis",
      email: "emily.davis@example.com",
      phone: "555-789-0123",
    },
    date: "2023-05-04T16:20:00Z",
    items: [
      {
        id: "5",
        name: "Pride and Prejudice",
        price: 9.99,
        quantity: 1,
        image: "/open-book-library.png",
      },
    ],
    subtotal: 9.99,
    shipping: 4.99,
    tax: 1.2,
    amount: 16.18,
    status: "Pending",
    paymentMethod: "PayPal",
    transactionId: "TXN901234",
    shippingAddress: {
      street: "321 Maple Rd",
      city: "Nowhere",
      state: "FL",
      zip: "13579",
      country: "USA",
    },
  },
  {
    id: "ORD-005",
    customer: {
      name: "Michael Wilson",
      email: "michael.wilson@example.com",
      phone: "555-321-6547",
    },
    date: "2023-05-05T11:10:00Z",
    items: [
      {
        id: "6",
        name: "The Catcher in the Rye",
        price: 11.99,
        quantity: 1,
        image: "/open-book-library.png",
      },
      {
        id: "7",
        name: "Lord of the Flies",
        price: 10.99,
        quantity: 1,
        image: "/open-book-library.png",
      },
    ],
    subtotal: 22.98,
    shipping: 5.99,
    tax: 2.32,
    amount: 31.29,
    status: "Cancelled",
    paymentMethod: "Credit Card",
    transactionId: "TXN567890",
    shippingAddress: {
      street: "654 Birch Ln",
      city: "Anyplace",
      state: "WA",
      zip: "97531",
      country: "USA",
    },
  },
]

// Mock API calls
export const fetchOrders = createAsyncThunk("orders/fetchOrders", async (params: FetchOrdersParams = {}) => {
  // Simulate API delay
  await new Promise((resolve) => setTimeout(resolve, 1000))

  let filteredOrders = [...mockOrders]

  if (params.search) {
    const searchLower = params.search.toLowerCase()
    filteredOrders = filteredOrders.filter(
      (order) =>
        order.id.toLowerCase().includes(searchLower) ||
        order.customer.name.toLowerCase().includes(searchLower) ||
        order.customer.email.toLowerCase().includes(searchLower),
    )
  }

  if (params.status) {
    filteredOrders = filteredOrders.filter((order) => order.status.toLowerCase() === params.status?.toLowerCase())
  }

  return filteredOrders
})

export const updateOrderStatus = createAsyncThunk(
  "orders/updateStatus",
  async ({ id, status }: { id: string; status: string }) => {
    // Simulate API delay
    await new Promise((resolve) => setTimeout(resolve, 1000))

    // In a real app, this would make an API call
    return { id, status }
  },
)

export const cancelOrder = createAsyncThunk("orders/cancelOrder", async (id: string) => {
  // Simulate API delay
  await new Promise((resolve) => setTimeout(resolve, 1000))

  // In a real app, this would make an API call
  return id
})

const initialState: OrderState = {
  orders: [],
  loading: false,
  error: null,
}

const orderSlice = createSlice({
  name: "orders",
  initialState,
  reducers: {},
  extraReducers: (builder) => {
    builder
      .addCase(fetchOrders.pending, (state) => {
        state.loading = true
        state.error = null
      })
      .addCase(fetchOrders.fulfilled, (state, action: PayloadAction<Order[]>) => {
        state.loading = false
        state.orders = action.payload
      })
      .addCase(fetchOrders.rejected, (state, action) => {
        state.loading = false
        state.error = action.error.message || "Failed to fetch orders"
      })
      .addCase(updateOrderStatus.fulfilled, (state, action: PayloadAction<{ id: string; status: string }>) => {
        const { id, status } = action.payload
        const orderIndex = state.orders.findIndex((order) => order.id === id)
        if (orderIndex !== -1) {
          state.orders[orderIndex].status = status
        }
      })
      .addCase(cancelOrder.fulfilled, (state, action: PayloadAction<string>) => {
        const id = action.payload
        const orderIndex = state.orders.findIndex((order) => order.id === id)
        if (orderIndex !== -1) {
          state.orders[orderIndex].status = "Cancelled"
        }
      })
  },
})

export default orderSlice.reducer
