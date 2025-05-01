export interface OrderItem {
  id: string
  name: string
  price: number
  quantity: number
  image?: string
}

export interface Customer {
  name: string
  email: string
  phone: string
}

export interface Address {
  street: string
  city: string
  state: string
  zip: string
  country: string
}

export interface Order {
  id: string
  customer: Customer
  date: string
  items: OrderItem[]
  subtotal: number
  shipping: number
  tax: number
  amount: number
  status: string
  paymentMethod: string
  transactionId: string
  shippingAddress: Address
}
