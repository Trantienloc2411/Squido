"use client"

import type React from "react"
import { useState } from "react"
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
  Box,
  Image,
  IconButton,
  Progress,
  Text,
  useToast,
  Flex,
} from "@chakra-ui/react"
import { FiUpload, FiX, FiAlertCircle } from "react-icons/fi"
import { useDispatch } from "react-redux"
import type { AppDispatch } from "../../redux/store"
import { createAuthor } from "../../redux/slices/authorSlice"
import { validateImage, uploadImage } from "../../utils/supabaseStorage"

interface CreateAuthorModalProps {
  isOpen: boolean
  onClose: () => void
  onAuthorCreated: (authorId: string, fullName: string) => void
}

interface AuthorFormData {
  fullName: string
  bio: string
  imageUrl: string | null
}

const CreateAuthorModal: React.FC<CreateAuthorModalProps> = ({ isOpen, onClose, onAuthorCreated }) => {
  const dispatch = useDispatch<AppDispatch>()
  const toast = useToast()

  const [formData, setFormData] = useState<AuthorFormData>({
    fullName: "",
    bio: "",
    imageUrl: null,
  })

  const [errors, setErrors] = useState<Record<string, string>>({})
  const [imagePreview, setImagePreview] = useState<string>("")
  const [isSubmitting, setIsSubmitting] = useState(false)
  const [uploadProgress, setUploadProgress] = useState(0)
  const [isUploading, setIsUploading] = useState(false)

  const validateForm = () => {
    const newErrors: Record<string, string> = {}

    if (!formData.fullName) newErrors.fullName = "Author name is required"

    setErrors(newErrors)
    return Object.keys(newErrors).length === 0
  }

  const handleChange = (e: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement>) => {
    const { name, value } = e.target
    setFormData((prev) => ({ ...prev, [name]: value }))
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

      // Upload to Supabase
      setUploadProgress(50)
      const imageUrl = await uploadImage(file, "authors") // Use "authors" folder
      setUploadProgress(100)

      // Update form data with the new image URL
      setFormData((prev) => ({
        ...prev,
        imageUrl: imageUrl,
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

  const clearImage = () => {
    setImagePreview("")
    setFormData((prev) => ({ ...prev, imageUrl: null }))
  }

  const handleSubmit = async () => {
    if (!validateForm()) return

    setIsSubmitting(true)

    try {
      // Create author data
      const authorData = {
        fullName: formData.fullName,
        bio: formData.bio,
        imageUrl: formData.imageUrl,
      }

      console.log("Creating author with data:", authorData)

      const result = await dispatch(createAuthor(authorData)).unwrap()
      console.log("Author created:", result)

      toast({
        title: "Author created",
        description: `${formData.fullName} has been created successfully.`,
        status: "success",
        duration: 5000,
        isClosable: true,
      })

      // Call the callback with the new author ID and name
      onAuthorCreated(result.id || result.authorId, formData.fullName)

      // Reset form and close modal
      setFormData({
        fullName: "",
        bio: "",
        imageUrl: null,
      })
      setImagePreview("")
      onClose()
    } catch (error) {
      console.error("Error creating author:", error)
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

  return (
    <Modal isOpen={isOpen} onClose={onClose} size="lg">
      <ModalOverlay />
      <ModalContent>
        <ModalHeader>Create New Author</ModalHeader>
        <ModalCloseButton />
        <ModalBody>
          <VStack spacing={4}>
            <FormControl isInvalid={!!errors.fullName}>
              <FormLabel>Full Name</FormLabel>
              <Input
                name="fullName"
                value={formData.fullName}
                onChange={handleChange}
                placeholder="Author's full name"
              />
              <FormErrorMessage>{errors.fullName}</FormErrorMessage>
            </FormControl>

            <FormControl>
              <FormLabel>Bio</FormLabel>
              <Textarea
                name="bio"
                value={formData.bio}
                onChange={handleChange}
                placeholder="Author's biography"
                rows={4}
              />
            </FormControl>

            <FormControl>
              <FormLabel>Author Image</FormLabel>
              <Flex direction="column" gap={2}>
                {imagePreview ? (
                  <Box position="relative" width="fit-content">
                    <Image
                      src={imagePreview || "/placeholder.svg"}
                      alt="Author preview"
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
                    htmlFor="author-image-upload"
                    leftIcon={<FiUpload />}
                    variant="outline"
                    cursor="pointer"
                    isDisabled={isUploading}
                  >
                    Upload Image (JPG only, max 200KB)
                    <input
                      id="author-image-upload"
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
            Create Author
          </Button>
        </ModalFooter>
      </ModalContent>
    </Modal>
  )
}

export default CreateAuthorModal
