"use client"

import type React from "react"
import { useEffect, useState } from "react"
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
} from "@chakra-ui/react"
import { AddIcon, SearchIcon } from "@chakra-ui/icons"
import { useDispatch, useSelector } from "react-redux"
import type { AppDispatch, RootState } from "../redux/store"
import { fetchBooks } from "../redux/slices/bookSlice"
import BookList from "../components/books/BookList"
import BookDetail from "../components/books/BookDetail"
import BookFormModal from "../components/books/BookFormModal"

const BookManagement: React.FC = () => {
  const dispatch = useDispatch<AppDispatch>()
  const navigate = useNavigate()
  const { isOpen, onOpen, onClose } = useDisclosure()
  const [searchTerm, setSearchTerm] = useState("")
  const [category, setCategory] = useState("")
  const { books, loading } = useSelector((state: RootState) => state.books)

  useEffect(() => {
    dispatch(fetchBooks({ search: searchTerm, category }))
  }, [dispatch, searchTerm, category])

  const handleSearch = (e: React.ChangeEvent<HTMLInputElement>) => {
    setSearchTerm(e.target.value)
  }

  const handleCategoryChange = (e: React.ChangeEvent<HTMLSelectElement>) => {
    setCategory(e.target.value)
  }

  const handleAddBook = () => {
    onOpen()
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
                    value={category}
                    onChange={handleCategoryChange}
                    maxW={{ base: "100%", md: "200px" }}
                  >
                    <option value="fiction">Fiction</option>
                    <option value="non-fiction">Non-Fiction</option>
                    <option value="mystery">Mystery</option>
                    <option value="sci-fi">Science Fiction</option>
                    <option value="fantasy">Fantasy</option>
                    <option value="biography">Biography</option>
                    <option value="history">History</option>
                    <option value="romance">Romance</option>
                    <option value="horror">Horror</option>
                    <option value="children">Children's</option>
                    <option value="self-help">Self-Help</option>
                    <option value="education">Education</option>
                  </Select>

                  <Select placeholder="Sort By" maxW={{ base: "100%", md: "150px" }}>
                    <option value="title">Title</option>
                    <option value="author">Author</option>
                    <option value="price">Price</option>
                    <option value="date">Date Added</option>
                  </Select>
                </HStack>
              </Flex>

              <BookList books={books} isLoading={loading} />

              {/* Add Book Modal */}
              <BookFormModal isOpen={isOpen} onClose={onClose} book={null} />
            </>
          }
        />
        <Route path="/:id" element={<BookDetail />} />
      </Routes>
    </Box>
  )
}

export default BookManagement
