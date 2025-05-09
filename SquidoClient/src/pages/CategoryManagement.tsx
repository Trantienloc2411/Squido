

"use client"

import type React from "react"
import { useEffect, useState } from "react"
import {
  Box,
  Heading,
  Button,
  Flex,
  Table,
  Thead,
  Tbody,
  Tr,
  Th,
  Td,
  Menu,
  MenuButton,
  MenuList,
  MenuItem,
  IconButton,
  InputGroup,
  InputLeftElement,
  Input,
  useColorModeValue,
  useDisclosure,
  Text,
  useToast,
  Spinner,
  Badge,
  Alert,
  AlertIcon,
  AlertTitle,
  AlertDescription,
} from "@chakra-ui/react"
import { SearchIcon, AddIcon, RepeatIcon } from "@chakra-ui/icons"
import { FiMoreVertical, FiEdit, FiTrash2 } from "react-icons/fi"
import { useDispatch, useSelector } from "react-redux"
import type { AppDispatch, RootState } from "../redux/store"
import { fetchCategories, deleteCategory, clearCategoryError } from "../redux/slices/categorySlice"
import CategoryFormModal from "../components/categories/CategoryFormModal"
import DeleteConfirmationModal from "../components/common/DeleteConfirmationModal"
import type { CategoryParams } from "../types/category"

const CategoryManagement: React.FC = () => {
  const dispatch = useDispatch<AppDispatch>()
  const toast = useToast()
  const { categories, loading, error } = useSelector((state: RootState) => state.categories)
  const [searchTerm, setSearchTerm] = useState("")
  const [debouncedSearchTerm, setDebouncedSearchTerm] = useState("")
  const [selectedCategoryId, setSelectedCategoryId] = useState<number | null>(null)
  const bgColor = useColorModeValue("white", "gray.800")

  // For form modal
  const { isOpen: isFormOpen, onOpen: onFormOpen, onClose: onFormClose } = useDisclosure()

  // For delete confirmation modal
  const { isOpen: isDeleteOpen, onOpen: onDeleteOpen, onClose: onDeleteClose } = useDisclosure()
  const [categoryToDelete, setCategoryToDelete] = useState<{ id: number; name: string } | null>(null)
  const [isDeleting, setIsDeleting] = useState(false)

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
  const searchParams: CategoryParams = {
    keyword: debouncedSearchTerm,
  }

  // Fetch categories when search params change
  useEffect(() => {
    dispatch(fetchCategories(searchParams))
  }, [dispatch, debouncedSearchTerm])

  const handleSearch = (e: React.ChangeEvent<HTMLInputElement>) => {
    setSearchTerm(e.target.value)
  }

  const handleAddCategory = () => {
    setSelectedCategoryId(null)
    onFormOpen()
  }

  const handleEditCategory = (id: number) => {
    setSelectedCategoryId(id)
    onFormOpen()
  }

  const handleDeleteCategory = (id: number, name: string) => {
    setCategoryToDelete({ id, name })
    onDeleteOpen()
  }

  const confirmDelete = async () => {
    if (!categoryToDelete) return

    setIsDeleting(true)
    try {
      await dispatch(deleteCategory(categoryToDelete.id)).unwrap()
      toast({
        title: "Category deleted",
        description: `"${categoryToDelete.name}" has been deleted successfully.`,
        status: "success",
        duration: 5000,
        isClosable: true,
      })
      onDeleteClose()
    } catch (error) {
      toast({
        title: "Error",
        description: typeof error === "string" ? error : "Failed to delete the category. Please try again.",
        status: "error",
        duration: 5000,
        isClosable: true,
      })
    } finally {
      setIsDeleting(false)
    }
  }

  // Function to retry fetching categories
  const handleRetry = () => {
    dispatch(clearCategoryError())
    dispatch(fetchCategories(searchParams))
  }

  return (
    <Box>
      <Flex justify="space-between" align="center" mb={6}>
        <Heading size="lg">Category Management</Heading>
        <Button leftIcon={<AddIcon />} colorScheme="blue" onClick={handleAddCategory}>
          Add Category
        </Button>
      </Flex>

      <Flex mb={6} direction={{ base: "column", md: "row" }} gap={4} align={{ base: "stretch", md: "center" }}>
        <InputGroup maxW={{ base: "100%", md: "320px" }}>
          <InputLeftElement pointerEvents="none">
            <SearchIcon color="gray.400" />
          </InputLeftElement>
          <Input placeholder="Search categories..." value={searchTerm} onChange={handleSearch} />
        </InputGroup>

        <Button
          leftIcon={<RepeatIcon />}
          onClick={handleRetry}
          isLoading={loading}
          loadingText="Refreshing"
          size="sm"
          colorScheme="blue"
          variant="outline"
        >
          Refresh
        </Button>
      </Flex>

      {error && (
        <Alert status="error" mb={4} borderRadius="md">
          <AlertIcon />
          <Box flex="1">
            <AlertTitle>Error fetching categories</AlertTitle>
            <AlertDescription display="block">
              {error}
              <Text mt={2}>Check your API connection and try again.</Text>
            </AlertDescription>
          </Box>
        </Alert>
      )}

      {loading && !categories.length ? (
        <Flex direction="column" align="center" justify="center" py={10}>
          <Spinner size="xl" mb={4} color="blue.500" />
          <Text>Loading categories...</Text>
        </Flex>
      ) : (
        <Box bg={bgColor} rounded="lg" overflow="hidden" boxShadow="sm">
          <Box overflowX="auto">
            <Table variant="simple">
              <Thead>
                <Tr>
                  <Th>ID</Th>
                  <Th>Name</Th>
                  <Th>Description</Th>
                  <Th>Book Count</Th>
                  <Th width="50px">Actions</Th>
                </Tr>
              </Thead>
              <Tbody>
                {categories.map((category) => (
                  <Tr key={category.id}>
                    <Td>{category.id}</Td>
                    <Td fontWeight="medium">{category.name}</Td>
                    <Td>
                      <Text noOfLines={2}>{category.description || "—"}</Text>
                    </Td>
                    <Td>
                      {category.bookCount ? (
                        <Badge colorScheme="blue">{category.bookCount}</Badge>
                      ) : (
                        <Text color="gray.500">—</Text>
                      )}
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
                          <MenuItem icon={<FiEdit />} onClick={() => handleEditCategory(category.id)}>
                            Edit
                          </MenuItem>
                          <MenuItem
                            icon={<FiTrash2 />}
                            onClick={() => handleDeleteCategory(category.id, category.name)}
                          >
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

          {!loading && categories.length === 0 && (
            <Box p={8} textAlign="center">
              <Text>No categories found. Try adjusting your search or add a new category.</Text>
            </Box>
          )}
        </Box>
      )}

      {/* Category Form Modal */}
      <CategoryFormModal isOpen={isFormOpen} onClose={onFormClose} categoryId={selectedCategoryId} />

      {/* Delete Confirmation Modal */}
      <DeleteConfirmationModal
        isOpen={isDeleteOpen}
        onClose={onDeleteClose}
        onConfirm={confirmDelete}
        title="Delete Category"
        itemName={categoryToDelete?.name || ""}
        isDeleting={isDeleting}
      />
    </Box>
  )
}

export default CategoryManagement
