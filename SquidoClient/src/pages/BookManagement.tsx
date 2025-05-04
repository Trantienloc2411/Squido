"use client"

import type React from "react"
import { useEffect, useState, useCallback, useRef } from "react"
import { Routes, Route, useNavigate } from "react-router-dom"
import {
  Box,
  Heading,
  Button,
  Flex,
  useDisclosure,
  Input,
  InputGroup,
  InputLeftElement,
  HStack,
  Select,
  Text,
  useToast,
  Alert,
  AlertIcon,
  AlertTitle,
  AlertDescription,
  CloseButton,
  Spinner,
} from "@chakra-ui/react"
import { AddIcon, SearchIcon, RepeatIcon } from "@chakra-ui/icons"
import { useDispatch, useSelector } from "react-redux"
import type { AppDispatch, RootState } from "../redux/store"
import { fetchBooks, fetchCategories, clearBookError } from "../redux/slices/bookSlice"
import BookList from "../components/books/BookList"
import BookDetail from "../components/books/BookDetail"
import BookFormModal from "../components/books/BookFormModal"
import type { BookParams } from "../types/book"

const BookManagement: React.FC = () => {
  const dispatch = useDispatch<AppDispatch>()
  const navigate = useNavigate()
  const toast = useToast()
  const { isOpen, onOpen, onClose } = useDisclosure()
  const [searchTerm, setSearchTerm] = useState("")
  const [categoryId, setCategoryId] = useState<number | undefined>(undefined)
  const [debouncedSearchTerm, setDebouncedSearchTerm] = useState("")
  const { books, loading, pagination, categories, categoriesLoading, error } = useSelector(
    (state: RootState) => state.books,
  )

  // Keep track of fetch attempts
  const fetchAttemptsRef = useRef(0)
  const [isRetrying, setIsRetrying] = useState(false)

  // Fetch categories on component mount
  useEffect(() => {
    dispatch(fetchCategories())
  }, [dispatch])

  // Debounce search term to avoid too many API calls
  useEffect(() => {
    const handler = setTimeout(() => {
      setDebouncedSearchTerm(searchTerm)
    }, 500)

    return () => {
      clearTimeout(handler)
    }
  }, [searchTerm])

  // Search params for API
  const searchParams = useRef<BookParams>({
    page: 1,
    pageSize: 20,
    keyword: debouncedSearchTerm,
    categoryId: categoryId,
  }).current

  // Update search params when inputs change
  useEffect(() => {
    searchParams.keyword = debouncedSearchTerm
    searchParams.categoryId = categoryId
  }, [debouncedSearchTerm, categoryId, searchParams])

  // Function to fetch books with current search params
  const fetchBooksList = useCallback(() => {
    setIsRetrying(true)
    fetchAttemptsRef.current += 1
    console.log(`Fetching books (attempt ${fetchAttemptsRef.current})...`)

    return dispatch(fetchBooks(searchParams))
      .unwrap()
      .then((result) => {
        console.log("Books fetched successfully:", result)
        setIsRetrying(false)
        return result
      })
      .catch((err) => {
        console.error("Error fetching books:", err)
        setIsRetrying(false)
        toast({
          title: "Error fetching books",
          description: typeof err === "string" ? err : "Failed to fetch books. Please try again.",
          status: "error",
          duration: 5000,
          isClosable: true,
        })
        throw err
      })
  }, [dispatch, searchParams, toast])

  // Fetch books when search params change
  useEffect(() => {
    fetchBooksList().catch((err) => console.error("Failed to fetch books:", err))
  }, [fetchBooksList, debouncedSearchTerm, categoryId])

  const handleSearch = (e: React.ChangeEvent<HTMLInputElement>) => {
    setSearchTerm(e.target.value)
  }

  const handleCategoryChange = (e: React.ChangeEvent<HTMLSelectElement>) => {
    const value = e.target.value
    setCategoryId(value ? Number.parseInt(value, 10) : undefined)
  }

  const handleAddBook = () => {
    onOpen()
  }

  // Function to handle modal close and refresh book list
  const handleModalClose = () => {
    onClose()
    // Refresh the book list after modal closes
    fetchBooksList().catch((err) => console.error("Failed to fetch books after modal close:", err))
  }

  // Function to retry fetching books
  const handleRetry = () => {
    dispatch(clearBookError())
    fetchBooksList().catch((err) => console.error("Failed to retry fetching books:", err))
  }

  return (
    <Box>
      <Routes>
        <Route
          path="/"
          element={
            <>
              <Flex justify="space-between" align="center" mb={6}>
                <Heading size="lg">Book Management</Heading>
                <Button leftIcon={<AddIcon />} colorScheme="blue" onClick={handleAddBook}>
                  Add Book
                </Button>
              </Flex>

              <Flex mb={6} direction={{ base: "column", md: "row" }} gap={4} align={{ base: "stretch", md: "center" }}>
                <InputGroup maxW={{ base: "100%", md: "320px" }}>
                  <InputLeftElement pointerEvents="none">
                    <SearchIcon color="gray.400" />
                  </InputLeftElement>
                  <Input placeholder="Search books..." value={searchTerm} onChange={handleSearch} />
                </InputGroup>

                <HStack spacing={4}>
                  <Select
                    placeholder="All Categories"
                    value={categoryId?.toString() || ""}
                    onChange={handleCategoryChange}
                    maxW={{ base: "100%", md: "200px" }}
                    isDisabled={categoriesLoading}
                  >
                    {categoriesLoading ? (
                      <option value="">Loading categories...</option>
                    ) : (
                      categories.map((cat) => (
                        <option key={cat.categoryId} value={cat.categoryId.toString()}>
                          {cat.name}
                        </option>
                      ))
                    )}
                  </Select>

                  <Button
                    leftIcon={<RepeatIcon />}
                    onClick={handleRetry}
                    isLoading={isRetrying}
                    loadingText="Retrying"
                    size="sm"
                    colorScheme="blue"
                    variant="outline"
                  >
                    Refresh
                  </Button>
                </HStack>
              </Flex>

              {error && (
                <Alert status="error" mb={4} borderRadius="md">
                  <AlertIcon />
                  <Box flex="1">
                    <AlertTitle>Error fetching books</AlertTitle>
                    <AlertDescription display="block">
                      {error}
                      <Text mt={2}>Check your API connection and try again.</Text>
                    </AlertDescription>
                  </Box>
                  <CloseButton position="absolute" right="8px" top="8px" onClick={() => dispatch(clearBookError())} />
                </Alert>
              )}

              {loading && !books.length ? (
                <Flex direction="column" align="center" justify="center" py={10}>
                  <Spinner size="xl" mb={4} color="blue.500" />
                  <Text>Loading books...</Text>
                </Flex>
              ) : (
                <>
                  {pagination && (
                    <Text mb={4} color="gray.500">
                      Showing {books.length} of {pagination.totalCount} books
                    </Text>
                  )}

                  <BookList books={books} isLoading={loading} pagination={pagination} searchParams={searchParams} />
                </>
              )}

              {/* Add Book Modal */}
              <BookFormModal isOpen={isOpen} onClose={handleModalClose} book={null} />
            </>
          }
        />
        <Route path="/:id" element={<BookDetail />} />
      </Routes>
    </Box>
  )
}

export default BookManagement
