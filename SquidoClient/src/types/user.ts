export interface User {
  id: string
  email: string
  username: string
  firstName: string
  lastName: string
  homeAddress: string | null
  wardName: string | null
  city: string | null
  district: string | null
  phone: string | null
  roleId: number
  gender: number
  isDeleted: boolean
  role: {
    id: number
    roleName: string
  }
  token: string | null
}

export interface UserResponse {
  records: User[]
  currentPage: number
  pageSize: number
  totalRecords: number
}

export interface UserParams {
  page?: number
  pageSize?: number
  keyword?: string
}
