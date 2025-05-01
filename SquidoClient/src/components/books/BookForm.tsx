"use client"

import type React from "react"
import { useEffect, useState } from "react"
import {
  Box,
  Button,
  FormControl,
  FormLabel,
  Input,
  Textarea,
  Select,
  NumberInput,
  NumberInputField,
  NumberInputStepper,
  NumberIncrementStepper,
  NumberDecrementStepper,
  SimpleGrid,
  Heading,
  useToast,
  FormErrorMessage,
  Flex,
  Image,
  IconButton,
  useColorModeValue,
} from "@chakra-ui/react"
import { FiUpload, FiX } from "react-icons/fi"
import { useNavigate, useParams } from "react-router-dom"
import { useDispatch, useSelector } from "react-redux"
import type { AppDispatch, RootState } from "../../redux/store"
import { fetchBookById, createBook, updateBook } from "../../redux/slices/bookSlice"
import type { Book } from "../../types/book"

const BookForm: React.FC = () => {
  const { id } = useParams<{ id: string }>()
  const navigate = useNavigate()
  const dispatch = useDispatch<AppDispatch>()
  const toast = useToast()
  const { currentBook, loading } = useSelector((state: RootState) => state.books)
  const bgColor = useColorModeValue("white", "gray.800")

  const [formData, setFormData] = useState<Partial<Book>>({
    title: "",
    author: "",
    isbn: "",
    description: "",
    category: "",
    price: 0,
    stock: 0,
    publisher: "",
    publishedDate: "",
    language: "English",
    pages: 0,
    coverImage: "",
  })

  const [errors, setErrors] = useState<Record<string, string>>({})
  const [imagePreview, setImagePreview] = useState<string>("")

  useEffect(() => {
    if (id && id !== "new") {
      dispatch(fetchBookById(id))
    }
  }, [dispatch, id])

  useEffect(() => {
    if (currentBook && id !== "new") {
      setFormData(currentBook)
      setImagePreview(currentBook.coverImage || "")
    }
  }, [currentBook, id])

  const validateForm = () => {
    const newErrors: Record<string, string> = {}

    if (!formData.title) newErrors.title = "Title is required"
    if (!formData.author) newErrors.author = "Author is required"
    if (!formData.isbn) newErrors.isbn = "ISBN is required"
    if (!formData.category) newErrors.category = "Category is required"
    if (formData.price === undefined || formData.price < 0) {
      newErrors.price = "Price must be a positive number"
    }

    setErrors(newErrors)
    return Object.keys(newErrors).length === 0
  }

  const handleChange = (e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement | HTMLTextAreaElement>) => {
    const { name, value } = e.target
    setFormData((prev) => ({ ...prev, [name]: value }))
  }

  const handleNumberChange = (name: string, value: number) => {
    setFormData((prev) => ({ ...prev, [name]: value }))
  }

  const handleImageChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const file = e.target.files?.[0]
    if (file) {
      const reader = new FileReader()
      reader.onloadend = () => {
        const result = reader.result as string
        setImagePreview(result)
        setFormData((prev) => ({ ...prev, coverImage: result }))
      }
      reader.readAsDataURL(file)
    }
  }

  const clearImage = () => {
    setImagePreview("")
    setFormData((prev) => ({ ...prev, coverImage: "" }))
  }

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault()

    if (!validateForm()) return

    try {
      if (id && id !== "new") {
        await dispatch(updateBook({ id, bookData: formData as Book }))
        toast({
          title: "Book updated",
          description: "The book has been updated successfully.",
          status: "success",
          duration: 5000,
          isClosable: true,
        })
      } else {
        await dispatch(createBook(formData as Book))
        toast({
          title: "Book created",
          description: "The book has been created successfully.",
          status: "success",
          duration: 5000,
          isClosable: true,
        })
      }
      navigate("/books")
    } catch (error) {
      toast({
        title: "Error",
        description: "An error occurred. Please try again.",
        status: "error",
        duration: 5000,
        isClosable: true,
      })
    }
  }

  return (
    <Box>
      <Heading size="lg" mb={6}>
        {id && id !== "new" ? "Edit Book" : "Add New Book"}
      </Heading>

      <Box as="form" onSubmit={handleSubmit} bg={bgColor} p={6} rounded="lg" boxShadow="sm">
        <SimpleGrid columns={{ base: 1, md: 2 }} spacing={6}>
          <Box>
            <FormControl isInvalid={!!errors.title} mb={4}>
              <FormLabel>Title</FormLabel>
              <Input name="title" value={formData.title} onChange={handleChange} placeholder="Book title" />
              <FormErrorMessage>{errors.title}</FormErrorMessage>
            </FormControl>

            <FormControl isInvalid={!!errors.author} mb={4}>
              <FormLabel>Author</FormLabel>
              <Input name="author" value={formData.author} onChange={handleChange} placeholder="Author name" />
              <FormErrorMessage>{errors.author}</FormErrorMessage>
            </FormControl>

            <FormControl isInvalid={!!errors.isbn} mb={4}>
              <FormLabel>ISBN</FormLabel>
              <Input name="isbn" value={formData.isbn} onChange={handleChange} placeholder="ISBN number" />
              <FormErrorMessage>{errors.isbn}</FormErrorMessage>
            </FormControl>

            <FormControl isInvalid={!!errors.category} mb={4}>
              <FormLabel>Category</FormLabel>
              <Select name="category" value={formData.category} onChange={handleChange} placeholder="Select category">
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
              <FormErrorMessage>{errors.category}</FormErrorMessage>
            </FormControl>

            <SimpleGrid columns={2} spacing={4} mb={4}>
              <FormControl isInvalid={!!errors.price}>
                <FormLabel>Price ($)</FormLabel>
                <NumberInput
                  min={0}
                  precision={2}
                  value={formData.price}
                  onChange={(_, value) => handleNumberChange("price", value)}
                >
                  <NumberInputField />
                  <NumberInputStepper>
                    <NumberIncrementStepper />
                    <NumberDecrementStepper />
                  </NumberInputStepper>
                </NumberInput>
                <FormErrorMessage>{errors.price}</FormErrorMessage>
              </FormControl>

              <FormControl>
                <FormLabel>Stock</FormLabel>
                <NumberInput min={0} value={formData.stock} onChange={(_, value) => handleNumberChange("stock", value)}>
                  <NumberInputField />
                  <NumberInputStepper>
                    <NumberIncrementStepper />
                    <NumberDecrementStepper />
                  </NumberInputStepper>
                </NumberInput>
              </FormControl>
            </SimpleGrid>
          </Box>

          <Box>
            <FormControl mb={4}>
              <FormLabel>Description</FormLabel>
              <Textarea
                name="description"
                value={formData.description}
                onChange={handleChange}
                placeholder="Book description"
                rows={4}
              />
            </FormControl>

            <SimpleGrid columns={2} spacing={4} mb={4}>
              <FormControl>
                <FormLabel>Publisher</FormLabel>
                <Input name="publisher" value={formData.publisher} onChange={handleChange} placeholder="Publisher" />
              </FormControl>

              <FormControl>
                <FormLabel>Published Date</FormLabel>
                <Input name="publishedDate" type="date" value={formData.publishedDate} onChange={handleChange} />
              </FormControl>
            </SimpleGrid>

            <SimpleGrid columns={2} spacing={4} mb={4}>
              <FormControl>
                <FormLabel>Language</FormLabel>
                <Select name="language" value={formData.language} onChange={handleChange}>
                  <option value="English">English</option>
                  <option value="Spanish">Spanish</option>
                  <option value="French">French</option>
                  <option value="German">German</option>
                  <option value="Chinese">Chinese</option>
                  <option value="Japanese">Japanese</option>
                </Select>
              </FormControl>

              <FormControl>
                <FormLabel>Pages</FormLabel>
                <NumberInput min={1} value={formData.pages} onChange={(_, value) => handleNumberChange("pages", value)}>
                  <NumberInputField />
                  <NumberInputStepper>
                    <NumberIncrementStepper />
                    <NumberDecrementStepper />
                  </NumberInputStepper>
                </NumberInput>
              </FormControl>
            </SimpleGrid>

            <FormControl mb={4}>
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
                  <Button as="label" htmlFor="cover-upload" leftIcon={<FiUpload />} variant="outline" cursor="pointer">
                    Upload Cover
                    <input
                      id="cover-upload"
                      type="file"
                      accept="image/*"
                      onChange={handleImageChange}
                      style={{ display: "none" }}
                    />
                  </Button>
                )}
              </Flex>
            </FormControl>
          </Box>
        </SimpleGrid>

        <Flex justify="flex-end" mt={6} gap={4}>
          <Button variant="outline" onClick={() => navigate("/books")}>
            Cancel
          </Button>
          <Button type="submit" colorScheme="blue" isLoading={loading}>
            {id && id !== "new" ? "Update Book" : "Create Book"}
          </Button>
        </Flex>
      </Box>
    </Box>
  )
}

export default BookForm
