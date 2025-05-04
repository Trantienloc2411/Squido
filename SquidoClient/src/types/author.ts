export interface Author {
    authorId: string
    fullName: string
    bio?: string
    imageUrl?: string
    isDeleted: boolean
    books: any[]
  }
  
  export interface AuthorResponse {
    isSuccess: boolean
    message: string
    errorCode: string | null
    data: Author[]
    exceptionMessage: string | null
  }
  