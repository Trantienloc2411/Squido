"use client"

import type React from "react"
import { useEffect, useState } from "react"
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
  Textarea,
  FormErrorMessage,
  VStack,
  useToast,
  Spinner,
} from "@chakra-ui/react"
import { useDispatch, useSelector } from "react-redux"
import type { AppDispatch, RootState } from "../../redux/store"
import { createCategory, updateCategory, fetchCategoryById } from "../../redux/slices/categorySlice"
import type { Category } from "../../types/category"

interface CategoryFormModalProps {
  isOpen: boolean
  onClose: () => void
  categoryId: number | null
}

const CategoryFormModal: React.FC<CategoryFormModalProps> = ({ isOpen, onClose, categoryId }) => {
  const dispatch = useDispatch<AppDispatch>()
  const toast = useToast()
  const { currentCategory, loading } = useSelector((state: RootState) => state.categories)

  const [formData, setFormData] = useState<Partial<Category>>({
    name: "",
    description: "",
  })

  const [errors, setErrors] = useState<Record<string, string>>({})
  const [isSubmitting, setIsSubmitting] = useState(false)

  useEffect(() => {
    if (isOpen && categoryId) {
      dispatch(fetchCategoryById(categoryId))
    } else if (isOpen && !categoryId) {
      // Reset form for new category
      setFormData({
        name: "",
        description: "",
      })
    }
  }, [isOpen, categoryId, dispatch])

  useEffect(() => {
    if (currentCategory && categoryId) {
      setFormData({
        name: currentCategory.name || "",
        description: currentCategory.description || "",
      })
    }
  }, [currentCategory, categoryId])

  const validateForm = () => {
    const newErrors: Record<string, string> = {}

    if (!formData.name) newErrors.name = "Category name is required"

    setErrors(newErrors)
    return Object.keys(newErrors).length === 0
  }

  const handleChange = (e: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement>) => {
    const { name, value } = e.target
    setFormData((prev) => ({ ...prev, [name]: value }))
  }

  const handleSubmit = async () => {
    if (!validateForm()) return

    setIsSubmitting(true)

    try {
      if (categoryId) {
        // Update existing category
        await dispatch(updateCategory({ id: categoryId, categoryData: formData })).unwrap()
        toast({
          title: "Category updated",
          description: "The category has been updated successfully.",
          status: "success",
          duration: 5000,
          isClosable: true,
        })
      } else {
        // Create new category
        await dispatch(createCategory(formData)).unwrap()
        toast({
          title: "Category created",
          description: "The category has been created successfully.",
          status: "success",
          duration: 5000,
          isClosable: true,
        })
      }
      onClose()
    } catch (error) {
      console.error("Error submitting category:", error)
      toast({
        title: "Error",
        description: typeof error === "string" ? error : "An error occurred. Please try again.",
        status: "error",
        duration: 5000,
        isClosable: true,
      })
    } finally {
      setIsSubmitting(false)
    }
  }

  if (loading && categoryId && !currentCategory) {
    return (
      <Modal isOpen={isOpen} onClose={onClose}>
        <ModalOverlay />
        <ModalContent>
          <ModalHeader>{categoryId ? "Edit Category" : "Add New Category"}</ModalHeader>
          <ModalCloseButton />
          <ModalBody>
            <Spinner size="xl" mx="auto" my={8} display="block" />
          </ModalBody>
        </ModalContent>
      </Modal>
    )
  }

  return (
    <Modal isOpen={isOpen} onClose={onClose}>
      <ModalOverlay />
      <ModalContent>
        <ModalHeader>{categoryId ? "Edit Category" : "Add New Category"}</ModalHeader>
        <ModalCloseButton />
        <ModalBody>
          <VStack spacing={4}>
            <FormControl isInvalid={!!errors.name}>
              <FormLabel>Category Name</FormLabel>
              <Input name="name" value={formData.name} onChange={handleChange} placeholder="Enter category name" />
              <FormErrorMessage>{errors.name}</FormErrorMessage>
            </FormControl>

            <FormControl>
              <FormLabel>Description</FormLabel>
              <Textarea
                name="description"
                value={formData.description || ""}
                onChange={handleChange}
                placeholder="Enter category description"
                rows={4}
              />
            </FormControl>
          </VStack>
        </ModalBody>
        <ModalFooter>
          <Button variant="ghost" mr={3} onClick={onClose}>
            Cancel
          </Button>
          <Button colorScheme="blue" onClick={handleSubmit} isLoading={isSubmitting}>
            {categoryId ? "Update" : "Create"}
          </Button>
        </ModalFooter>
      </ModalContent>
    </Modal>
  )
}

export default CategoryFormModal
