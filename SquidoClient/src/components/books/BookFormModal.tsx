"use client"

import type React from "react"
import { useEffect, useState, useRef } from "react"
import {
  Modal,
  ModalOverlay,
  ModalContent,
  ModalHeader,
  ModalFooter,
  ModalBody,
  ModalCloseButton,
  Button,
  FormControl,
  FormLabel,
  Input,
  Select,
  NumberInput,
  NumberInputField,
  NumberInputStepper,
  NumberIncrementStepper,
  NumberDecrementStepper,
  FormErrorMessage,
  Flex,
  Image,
  IconButton,
  useToast,
  VStack,
  Box,
  Text,
  Textarea,
  Progress,
  SimpleGrid,
  Spinner,
  InputGroup,
  InputRightElement,
  List,
  ListItem,
  useDisclosure,
} from "@chakra-ui/react"
import { FiUpload, FiX, FiAlertCircle, FiSearch, FiPlus } from "react-icons/fi"
import { useDispatch, useSelector } from "react-redux"
import type { AppDispatch, RootState } from "../../redux/store"
import { createBook, updateBook, fetchCategories, fetchBooks, fetchBookById } from "../../redux/slices/bookSlice"
import { fetchAuthors, searchAuthors, clearAuthorSearch } from "../../redux/slices/authorSlice"
import type { Book } from "../../types/book"
import type { Author } from "../../types/author"
import { validateImage, uploadImage } from "../../utils/supabaseStorage"
import CreateAuthorModal from "../authors/CreateAuthorModal"

interface BookFormModalProps {
  isOpen: boolean
  onClose: () => void
  book: Book | null
}

// Update the BookFormData interface
interface BookFormData {
  title: string
  categoryId: number | null
  authorId: string | null
  description: string
  quantity: number
  price: number
  imageUrl: string | null
}

const BookFormModal: React.FC<BookFormModalProps> = ({ isOpen, onClose, book }) => {
  const dispatch = useDispatch<AppDispatch>()
  const toast = useToast()
  const { categories, categoriesLoading, bookDetail } = useSelector((state: RootState) => state.books)
  const { authors, loading: authorsLoading, searchResults } = useSelector((state: RootState) => state.authors)
  const authorSearchRef = useRef<HTMLInputElement>(null)

  // Create author modal
  const { isOpen: isCreateAuthorOpen, onOpen: onCreateAuthorOpen, onClose: onCreateAuthorClose } = useDisclosure()

  // Update the initial form data
  const [formData, setFormData] = useState<BookFormData>({
    title: "",
    categoryId: null,
    authorId: null,
    description: "",
    quantity: 0,
    price: 0,
    imageUrl: null,
  })

  const [errors, setErrors] = useState<Record<string, string>>({})
  const [imagePreview, setImagePreview] = useState<string>("")
  const [isSubmitting, setIsSubmitting] = useState(false)
  const [uploadProgress, setUploadProgress] = useState(0)
  const [isUploading, setIsUploading] = useState(false)
  const [isLoadingDetails, setIsLoadingDetails] = useState(false)

  // Author search state
  const [authorSearchTerm, setAuthorSearchTerm] = useState("")
  const [showAuthorResults, setShowAuthorResults] = useState(false)
  const [selectedAuthorName, setSelectedAuthorName] = useState("")

  // Fetch categories and authors when the modal opens
  useEffect(() => {
    if (isOpen) {
      dispatch(fetchCategories())
      dispatch(fetchAuthors())

      // If editing a book, fetch its full details
      if (book) {
        setIsLoadingDetails(true)
        dispatch(fetchBookById(book.id))
          .unwrap()
          .then(() => {
            setIsLoadingDetails(false)
          })
          .catch((err) => {
            console.error("Error fetching book details:", err)
            setIsLoadingDetails(false)
            toast({
              title: "Error",
              description: "Failed to load book details. Please try again.",
              status: "error",
              duration: 5000,
              isClosable: true,
            })
          })
      }
    } else {
      // Clear search results when modal closes
      dispatch(clearAuthorSearch())
      setAuthorSearchTerm("")
      setShowAuthorResults(false)
    }
  }, [dispatch, isOpen, book, toast])

  // Update the useEffect that sets form data when editing a book to properly handle authorId
  useEffect(() => {
    if (book && bookDetail) {
      // Find the category ID based on category name
      const categoryId = categories.find((cat) => cat.name === book.categoryName)?.id || null

      // Find the author ID if available in the book detail
      // Make sure we're getting authorId from the right place
      const authorId = book.authorId || bookDetail.book?.authorId || null

      // Set the selected author name for display
      setSelectedAuthorName(book.authorName || "")

      console.log("Setting form data with authorId:", authorId)
      console.log("Book data:", book)
      console.log("Book detail:", bookDetail)

      setFormData({
        title: book.title || "",
        categoryId,
        authorId,
        description: bookDetail.bookDescription || "",
        quantity: book.quantity || 0,
        price: book.price || 0,
        imageUrl: book.imageUrl || null,
      })

      // Set image preview if available
      if (book.imageUrl) {
        setImagePreview(book.imageUrl)
      } else {
        setImagePreview("")
      }
    } else if (!book) {
      // Reset form for new book
      setFormData({
        title: "",
        categoryId: null,
        authorId: null,
        description: "",
        quantity: 0,
        price: 0,
        imageUrl: null,
      })
      setSelectedAuthorName("")
      setImagePreview("")
    }
  }, [book, bookDetail, categories])

  // Handle author search
  useEffect(() => {
    if (authorSearchTerm.trim().length >= 2) {
      const delaySearch = setTimeout(() => {
        dispatch(searchAuthors(authorSearchTerm))
        setShowAuthorResults(true)
      }, 500)

      return () => clearTimeout(delaySearch)
    } else {
      setShowAuthorResults(false)
    }
  }, [authorSearchTerm, dispatch])

  // Click outside to close author search results
  useEffect(() => {
    function handleClickOutside(event: MouseEvent) {
      if (authorSearchRef.current && !authorSearchRef.current.contains(event.target as Node)) {
        setShowAuthorResults(false)
      }
    }

    document.addEventListener("mousedown", handleClickOutside)
    return () => {
      document.removeEventListener("mousedown", handleClickOutside)
    }
  }, [])

  const validateForm = () => {
    const newErrors: Record<string, string> = {}

    if (!formData.title) newErrors.title = "Title is required"
    if (!formData.categoryId) newErrors.categoryId = "Category is required"
    if (!formData.authorId) newErrors.authorId = "Author is required"
    if (formData.price === undefined || formData.price < 0) {
      newErrors.price = "Price must be a positive number"
    }

    setErrors(newErrors)
    return Object.keys(newErrors).length === 0
  }

  const handleChange = (e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement | HTMLTextAreaElement>) => {
    const { name, value } = e.target
    setFormData((prev) => ({
      ...prev,
      [name]:
        name === "categoryId" || name === "authorId"
          ? value
            ? name === "categoryId"
              ? Number.parseInt(value, 10)
              : value
            : null
          : value,
    }))
  }

  const handleAuthorSearch = (e: React.ChangeEvent<HTMLInputElement>) => {
    setAuthorSearchTerm(e.target.value)
    if (e.target.value === "") {
      setSelectedAuthorName("")
    }
  }

  const handleSelectAuthor = (author: Author) => {
    const authorId = author.id || author.authorId
    setFormData((prev) => ({
      ...prev,
      authorId,
    }))
    setSelectedAuthorName(author.fullName)
    setAuthorSearchTerm("")
    setShowAuthorResults(false)
  }

  const handleAuthorCreated = (authorId: string, fullName: string) => {
    setFormData((prev) => ({
      ...prev,
      authorId,
    }))
    setSelectedAuthorName(fullName)
    setAuthorSearchTerm("")
  }

  const handleNumberChange = (name: string, value: number) => {
    setFormData((prev) => ({ ...prev, [name]: value }))
  }

  // Handle direct price input to allow decimal values
  const handlePriceChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const value = Number.parseFloat(e.target.value)
    if (!isNaN(value)) {
      setFormData((prev) => ({ ...prev, price: value }))
    } else if (e.target.value === "") {
      setFormData((prev) => ({ ...prev, price: 0 }))
    }
  }

  const handleImageChange = async (e: React.ChangeEvent<HTMLInputElement>) => {
    const file = e.target.files?.[0]
    if (!file) return

    // Validate the image
    const validationError = validateImage(file)
    if (validationError) {
      toast({
        title: "Invalid image",
        description: validationError,
        status: "error",
        duration: 5000,
        isClosable: true,
      })
      return
    }

    try {
      setIsUploading(true)
      setUploadProgress(10)

      // Create a preview
      const reader = new FileReader()
      reader.onloadend = () => {
        const result = reader.result as string
        setImagePreview(result)
        setUploadProgress(30)
      }
      reader.readAsDataURL(file)

      // Update the handleImageChange function to set imageUrl instead of imageUrls
      // Upload to Supabase
      setUploadProgress(50)
      const imageUrl = await uploadImage(file)
      setUploadProgress(100)

      // Update form data with the new image URL
      setFormData((prev) => ({
        ...prev,
        imageUrl: imageUrl, // Store a single image URL
      }))

      toast({
        title: "Image uploaded",
        description: "The image has been uploaded successfully.",
        status: "success",
        duration: 3000,
        isClosable: true,
      })
    } catch (error) {
      toast({
        title: "Upload failed",
        description: error instanceof Error ? error.message : "Failed to upload image",
        status: "error",
        duration: 5000,
        isClosable: true,
      })
    } finally {
      setIsUploading(false)
      setUploadProgress(0)
    }
  }

  // Update the clearImage function
  const clearImage = () => {
    setImagePreview("")
    setFormData((prev) => ({ ...prev, imageUrl: null }))
  }

  // Update the handleSubmit function to log the data being sent
  const handleSubmit = async () => {
    if (!validateForm()) return

    setIsSubmitting(true)

    try {
      if (book) {
        // Ensure we're sending the correct data structure for the update
        const updateData = {
          title: formData.title,
          categoryId: formData.categoryId,
          authorId: formData.authorId,
          description: formData.description,
          quantity: formData.quantity,
          price: formData.price,
          imageUrl: formData.imageUrl,
        }

        console.log("Submitting update with data:", updateData)
        console.log("authorId type:", typeof updateData.authorId)

        const result = await dispatch(updateBook({ id: book.id, bookData: updateData })).unwrap()
        if (result) {
          toast({
            title: "Book updated",
            description: "The book has been updated successfully.",
            status: "success",
            duration: 5000,
            isClosable: true,
          })
          // Refresh the book list after successful update
          dispatch(fetchBooks({}))
          onClose()
        }
      } else {
        // For creating a new book
        const createData = {
          title: formData.title,
          categoryId: formData.categoryId,
          authorId: formData.authorId,
          description: formData.description,
          quantity: formData.quantity,
          price: formData.price,
          imageUrl: formData.imageUrl,
        }

        console.log("Submitting create with data:", createData)

        const result = await dispatch(createBook(createData)).unwrap()
        if (result) {
          toast({
            title: "Book created",
            description: "The book has been created successfully.",
            status: "success",
            duration: 5000,
            isClosable: true,
          })
          // Refresh the book list after successful creation
          dispatch(fetchBooks({}))
          onClose()
        }
      }
    } catch (error) {
      console.error("Error submitting form:", error)
      toast({
        title: "Error",
        description: error instanceof Error ? error.message : "An error occurred. Please try again.",
        status: "error",
        duration: 5000,
        isClosable: true,
      })
    } finally {
      setIsSubmitting(false)
    }
  }

  // Show loading state while fetching book details
  if (isLoadingDetails) {
    return (
      <Modal isOpen={isOpen} onClose={onClose} size="xl">
        <ModalOverlay />
        <ModalContent>
          <ModalHeader>{book ? "Edit Book" : "Add New Book"}</ModalHeader>
          <ModalCloseButton />
          <ModalBody>
            <Flex direction="column" align="center" justify="center" py={10}>
              <Spinner size="xl" mb={4} color="blue.500" />
              <Text>Loading book details...</Text>
            </Flex>
          </ModalBody>
        </ModalContent>
      </Modal>
    )
  }

  return (
    <>
      <Modal isOpen={isOpen} onClose={onClose} size="xl">
        <ModalOverlay />
        <ModalContent>
          <ModalHeader>{book ? "Edit Book" : "Add New Book"}</ModalHeader>
          <ModalCloseButton />
          <ModalBody>
            <VStack spacing={4}>
              <FormControl isInvalid={!!errors.title}>
                <FormLabel>Title</FormLabel>
                <Input name="title" value={formData.title} onChange={handleChange} placeholder="Book title" />
                <FormErrorMessage>{errors.title}</FormErrorMessage>
              </FormControl>

              <FormControl isInvalid={!!errors.categoryId}>
                <FormLabel>Category</FormLabel>
                <Select
                  name="categoryId"
                  value={formData.categoryId?.toString() || ""}
                  onChange={handleChange}
                  placeholder="Select category"
                  isDisabled={categoriesLoading}
                >
                  {categoriesLoading ? (
                    <option disabled>Loading categories...</option>
                  ) : (
                    categories.map((cat) => (
                      <option key={cat.id} value={cat.id.toString()}>
                        {cat.name}
                      </option>
                    ))
                  )}
                </Select>
                {categoriesLoading && (
                  <Flex align="center" mt={2}>
                    <Spinner size="sm" mr={2} />
                    <Text fontSize="sm">Loading categories...</Text>
                  </Flex>
                )}
                <FormErrorMessage>{errors.categoryId}</FormErrorMessage>
              </FormControl>

              <FormControl isInvalid={!!errors.authorId}>
                <FormLabel>Author</FormLabel>
                <Box position="relative" ref={authorSearchRef}>
                  <InputGroup>
                    <Input
                      placeholder="Search for author..."
                      value={authorSearchTerm}
                      onChange={handleAuthorSearch}
                      onClick={() => authorSearchTerm.length >= 2 && setShowAuthorResults(true)}
                    />
                    <InputRightElement>{authorsLoading ? <Spinner size="sm" /> : <FiSearch />}</InputRightElement>
                  </InputGroup>

                  {/* Display selected author */}
                  {selectedAuthorName && !authorSearchTerm && (
                    <Flex align="center" mt={2} bg="blue.50" p={2} borderRadius="md">
                      <Text>{selectedAuthorName}</Text>
                    </Flex>
                  )}

                  {/* Author search results */}
                  {showAuthorResults && (
                    <Box
                      position="absolute"
                      top="100%"
                      left={0}
                      right={0}
                      bg="white"
                      boxShadow="md"
                      borderRadius="md"
                      maxH="200px"
                      overflowY="auto"
                      zIndex={10}
                    >
                      <List spacing={0}>
                        {searchResults.length > 0 ? (
                          searchResults.map((author) => (
                            <ListItem
                              key={author.id || author.authorId}
                              p={2}
                              cursor="pointer"
                              _hover={{ bg: "gray.100" }}
                              onClick={() => handleSelectAuthor(author)}
                            >
                              {author.fullName}
                            </ListItem>
                          ))
                        ) : (
                          <ListItem p={2}>
                            <Flex justify="space-between" align="center">
                              <Text>No authors found</Text>
                              <Button
                                size="sm"
                                leftIcon={<FiPlus />}
                                colorScheme="blue"
                                variant="ghost"
                                onClick={onCreateAuthorOpen}
                              >
                                Create New
                              </Button>
                            </Flex>
                          </ListItem>
                        )}
                      </List>
                    </Box>
                  )}
                </Box>

                {/* Create new author button */}
                <Button mt={2} size="sm" leftIcon={<FiPlus />} onClick={onCreateAuthorOpen} variant="outline">
                  Create New Author
                </Button>

                <FormErrorMessage>{errors.authorId}</FormErrorMessage>
              </FormControl>

              <FormControl>
                <FormLabel>Description</FormLabel>
                <Textarea
                  name="description"
                  value={formData.description}
                  onChange={handleChange}
                  placeholder="Book description"
                  rows={4}
                />
              </FormControl>

              <SimpleGrid columns={2} spacing={4}>
                <FormControl isInvalid={!!errors.price}>
                  <FormLabel>Price ($)</FormLabel>
                  <Input
                    type="number"
                    step="0.01"
                    min="0"
                    value={formData.price}
                    onChange={handlePriceChange}
                    placeholder="0.00"
                  />
                  <FormErrorMessage>{errors.price}</FormErrorMessage>
                </FormControl>

                <FormControl>
                  <FormLabel>Quantity</FormLabel>
                  <NumberInput
                    min={0}
                    value={formData.quantity}
                    onChange={(_, value) => handleNumberChange("quantity", value)}
                  >
                    <NumberInputField />
                    <NumberInputStepper>
                      <NumberIncrementStepper />
                      <NumberDecrementStepper />
                    </NumberInputStepper>
                  </NumberInput>
                </FormControl>
              </SimpleGrid>

              <FormControl>
                <FormLabel>Cover Image</FormLabel>
                <Flex direction="column" gap={2}>
                  {imagePreview ? (
                    <Box position="relative" width="fit-content">
                      <Image
                        src={imagePreview || "/placeholder.svg"}
                        alt="Cover preview"
                        maxH="150px"
                        borderRadius="md"
                      />
                      <IconButton
                        aria-label="Remove image"
                        icon={<FiX />}
                        size="sm"
                        position="absolute"
                        top={1}
                        right={1}
                        onClick={clearImage}
                      />
                    </Box>
                  ) : (
                    <Button
                      as="label"
                      htmlFor="cover-upload"
                      leftIcon={<FiUpload />}
                      variant="outline"
                      cursor="pointer"
                      isDisabled={isUploading}
                    >
                      Upload Cover (JPG only, max 200KB)
                      <input
                        id="cover-upload"
                        type="file"
                        accept="image/jpeg,image/jpg"
                        onChange={handleImageChange}
                        style={{ display: "none" }}
                        disabled={isUploading}
                      />
                    </Button>
                  )}
                  {isUploading && (
                    <Box width="100%">
                      <Progress value={uploadProgress} size="sm" colorScheme="blue" mb={2} />
                      <Text fontSize="sm">Uploading: {uploadProgress}%</Text>
                    </Box>
                  )}
                  <Text fontSize="xs" color="gray.500">
                    <Box as="span" display="inline-flex" alignItems="center" mr={1}>
                      <FiAlertCircle />
                    </Box>
                    Only one JPG image up to 200KB is allowed
                  </Text>
                </Flex>
              </FormControl>
            </VStack>
          </ModalBody>
          <ModalFooter>
            <Button variant="ghost" mr={3} onClick={onClose}>
              Cancel
            </Button>
            <Button colorScheme="blue" onClick={handleSubmit} isLoading={isSubmitting}>
              {book ? "Update" : "Create"}
            </Button>
          </ModalFooter>
        </ModalContent>
      </Modal>

      {/* Create Author Modal */}
      <CreateAuthorModal
        isOpen={isCreateAuthorOpen}
        onClose={onCreateAuthorClose}
        onAuthorCreated={handleAuthorCreated}
      />
    </>
  )
}

export default BookFormModal
