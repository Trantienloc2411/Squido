"use client"

import type React from "react"
import { useState } from "react"
import {
  Table,
  Thead,
  Tbody,
  Tr,
  Th,
  Td,
  Box,
  Image,
  Text,
  Badge,
  Menu,
  MenuButton,
  MenuList,
  MenuItem,
  IconButton,
  useColorModeValue,
  Skeleton,
  SkeletonText,
  Flex,
  useDisclosure,
  useToast,
} from "@chakra-ui/react"
import { FiMoreVertical, FiEdit, FiTrash2, FiEye } from "react-icons/fi"
import { useNavigate } from "react-router-dom"
import { useDispatch } from "react-redux"
import type { AppDispatch } from "../../redux/store"
import { deleteBook } from "../../redux/slices/bookSlice"
import type { Book } from "../../types/book"
import BookFormModal from "./BookFormModal"
import DeleteConfirmationModal from "../common/DeleteConfirmationModal"

interface BookListProps {
  books: Book[]
  isLoading: boolean
}

const BookList: React.FC<BookListProps> = ({ books, isLoading }) => {
  const navigate = useNavigate()
  const dispatch = useDispatch<AppDispatch>()
  const toast = useToast()
  const bgColor = useColorModeValue("white", "gray.800")

  // For edit modal
  const { isOpen: isEditOpen, onOpen: onEditOpen, onClose: onEditClose } = useDisclosure()
  const [selectedBook, setSelectedBook] = useState<Book | null>(null)

  // For delete modal
  const { isOpen: isDeleteOpen, onOpen: onDeleteOpen, onClose: onDeleteClose } = useDisclosure()
  const [bookToDelete, setBookToDelete] = useState<Book | null>(null)
  const [isDeleting, setIsDeleting] = useState(false)

  const handleView = (id: string) => {
    navigate(`/books/${id}`)
  }

  const handleEdit = (book: Book) => {
    setSelectedBook(book)
    onEditOpen()
  }

  const handleDelete = (book: Book) => {
    setBookToDelete(book)
    onDeleteOpen()
  }

  const confirmDelete = async () => {
    if (!bookToDelete) return

    setIsDeleting(true)
    try {
      await dispatch(deleteBook(bookToDelete.id))
      toast({
        title: "Book deleted",
        description: `"${bookToDelete.title}" has been deleted successfully.`,
        status: "success",
        duration: 5000,
        isClosable: true,
      })
      onDeleteClose()
    } catch (error) {
      toast({
        title: "Error",
        description: "Failed to delete the book. Please try again.",
        status: "error",
        duration: 5000,
        isClosable: true,
      })
    } finally {
      setIsDeleting(false)
    }
  }

  if (isLoading) {
    return (
      <Box bg={bgColor} rounded="lg" overflow="hidden" boxShadow="sm">
        <Skeleton height="40px" />
        <SkeletonText mt="4" noOfLines={10} spacing="4" skeletonHeight="2" />
      </Box>
    )
  }

  return (
    <>
      <Box bg={bgColor} rounded="lg" overflow="hidden" boxShadow="sm">
        <Box overflowX="auto">
          <Table variant="simple">
            <Thead>
              <Tr>
                <Th>Book</Th>
                <Th>Author</Th>
                <Th>Category</Th>
                <Th isNumeric>Price</Th>
                <Th>Stock</Th>
                <Th>Status</Th>
                <Th width="50px">Actions</Th>
              </Tr>
            </Thead>
            <Tbody>
              {books.map((book) => (
                <Tr key={book.id}>
                  <Td>
                    <Flex align="center">
                      <Image
                        src={book.coverImage || "/placeholder.svg?height=40&width=30&query=book"}
                        alt={book.title}
                        boxSize="40px"
                        objectFit="cover"
                        mr={3}
                        borderRadius="md"
                      />
                      <Box>
                        <Text fontWeight="medium" noOfLines={1}>
                          {book.title}
                        </Text>
                        <Text fontSize="xs" color="gray.500">
                          ISBN: {book.isbn}
                        </Text>
                      </Box>
                    </Flex>
                  </Td>
                  <Td>{book.author}</Td>
                  <Td>{book.category}</Td>
                  <Td isNumeric>${book.price.toFixed(2)}</Td>
                  <Td>{book.stock}</Td>
                  <Td>
                    <Badge colorScheme={book.stock > 0 ? "green" : "red"} variant="subtle" rounded="full" px={2}>
                      {book.stock > 0 ? "In Stock" : "Out of Stock"}
                    </Badge>
                  </Td>
                  <Td>
                    <Menu>
                      <MenuButton
                        as={IconButton}
                        icon={<FiMoreVertical />}
                        variant="ghost"
                        size="sm"
                        aria-label="Options"
                      />
                      <MenuList>
                        <MenuItem icon={<FiEye />} onClick={() => handleView(book.id)}>
                          View Details
                        </MenuItem>
                        <MenuItem icon={<FiEdit />} onClick={() => handleEdit(book)}>
                          Edit
                        </MenuItem>
                        <MenuItem icon={<FiTrash2 />} onClick={() => handleDelete(book)}>
                          Delete
                        </MenuItem>
                      </MenuList>
                    </Menu>
                  </Td>
                </Tr>
              ))}
            </Tbody>
          </Table>
        </Box>
      </Box>

      {/* Edit Modal */}
      <BookFormModal isOpen={isEditOpen} onClose={onEditClose} book={selectedBook} />

      {/* Delete Confirmation Modal */}
      <DeleteConfirmationModal
        isOpen={isDeleteOpen}
        onClose={onDeleteClose}
        onConfirm={confirmDelete}
        title="Delete Book"
        itemName={bookToDelete?.title || ""}
        isDeleting={isDeleting}
      />
    </>
  )
}

export default BookList
