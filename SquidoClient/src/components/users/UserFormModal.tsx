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
  Select,
  FormErrorMessage,
  VStack,
  useToast,
  SimpleGrid,
  Spinner,
} from "@chakra-ui/react"
import { useDispatch, useSelector } from "react-redux"
import type { AppDispatch, RootState } from "../../redux/store"
import { updateUser, fetchUserById } from "../../redux/slices/userSlice"

interface UserFormModalProps {
  isOpen: boolean
  onClose: () => void
  userId: string | null
}

interface UserFormData {
  email: string
  username: string
  firstName: string
  lastName: string
  homeAddress: string
  wardName: string
  city: string
  district: string
  phone: string
  roleId: number
  gender: number
}

const UserFormModal: React.FC<UserFormModalProps> = ({ isOpen, onClose, userId }) => {
  const dispatch = useDispatch<AppDispatch>()
  const toast = useToast()
  const { currentUser, loading } = useSelector((state: RootState) => state.users)

  const [formData, setFormData] = useState<UserFormData>({
    email: "",
    username: "",
    firstName: "",
    lastName: "",
    homeAddress: "",
    wardName: "",
    city: "",
    district: "",
    phone: "",
    roleId: 1,
    gender: 1,
  })

  const [errors, setErrors] = useState<Record<string, string>>({})
  const [isSubmitting, setIsSubmitting] = useState(false)

  useEffect(() => {
    if (isOpen && userId) {
      dispatch(fetchUserById(userId))
    }
  }, [isOpen, userId, dispatch])

  useEffect(() => {
    if (currentUser) {
      setFormData({
        email: currentUser.email || "",
        username: currentUser.username || "",
        firstName: currentUser.firstName || "",
        lastName: currentUser.lastName || "",
        homeAddress: currentUser.homeAddress || "",
        wardName: currentUser.wardName || "",
        city: currentUser.city || "",
        district: currentUser.district || "",
        phone: currentUser.phone || "",
        roleId: currentUser.roleId || 1,
        gender: currentUser.gender || 1,
      })
    }
  }, [currentUser])

  const validateForm = () => {
    const newErrors: Record<string, string> = {}

    if (!formData.email) newErrors.email = "Email is required"
    if (!formData.firstName) newErrors.firstName = "First name is required"
    if (!formData.lastName) newErrors.lastName = "Last name is required"
    if (!formData.username) newErrors.username = "Username is required"

    // Email validation
    if (formData.email && !/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(formData.email)) {
      newErrors.email = "Invalid email format"
    }

    // Phone validation (optional)
    if (formData.phone && !/^\d{10,15}$/.test(formData.phone)) {
      newErrors.phone = "Phone number should be 10-15 digits"
    }

    setErrors(newErrors)
    return Object.keys(newErrors).length === 0
  }

  const handleChange = (e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement>) => {
    const { name, value } = e.target
    setFormData((prev) => ({
      ...prev,
      [name]: name === "roleId" || name === "gender" ? Number(value) : value,
    }))
  }

  const handleSubmit = async () => {
    if (!validateForm() || !userId) return

    setIsSubmitting(true)

    try {
      const result = await dispatch(updateUser({ id: userId, userData: formData })).unwrap()

      toast({
        title: "User updated",
        description: "User information has been updated successfully.",
        status: "success",
        duration: 5000,
        isClosable: true,
      })

      onClose()
    } catch (error) {
      console.error("Error updating user:", error)
      toast({
        title: "Error",
        description: typeof error === "string" ? error : "Failed to update user. Please try again.",
        status: "error",
        duration: 5000,
        isClosable: true,
      })
    } finally {
      setIsSubmitting(false)
    }
  }

  if (loading && !currentUser) {
    return (
      <Modal isOpen={isOpen} onClose={onClose} size="lg">
        <ModalOverlay />
        <ModalContent>
          <ModalHeader>Edit User</ModalHeader>
          <ModalCloseButton />
          <ModalBody>
            <Spinner size="xl" mx="auto" my={8} display="block" />
          </ModalBody>
        </ModalContent>
      </Modal>
    )
  }

  return (
    <Modal isOpen={isOpen} onClose={onClose} size="lg">
      <ModalOverlay />
      <ModalContent>
        <ModalHeader>Edit User</ModalHeader>
        <ModalCloseButton />
        <ModalBody>
          <VStack spacing={4}>
            <SimpleGrid columns={{ base: 1, md: 2 }} spacing={4} width="100%">
              <FormControl isInvalid={!!errors.firstName}>
                <FormLabel>First Name</FormLabel>
                <Input name="firstName" value={formData.firstName} onChange={handleChange} />
                <FormErrorMessage>{errors.firstName}</FormErrorMessage>
              </FormControl>

              <FormControl isInvalid={!!errors.lastName}>
                <FormLabel>Last Name</FormLabel>
                <Input name="lastName" value={formData.lastName} onChange={handleChange} />
                <FormErrorMessage>{errors.lastName}</FormErrorMessage>
              </FormControl>
            </SimpleGrid>

            <FormControl isInvalid={!!errors.email}>
              <FormLabel>Email</FormLabel>
              <Input name="email" type="email" value={formData.email} onChange={handleChange} />
              <FormErrorMessage>{errors.email}</FormErrorMessage>
            </FormControl>

            <FormControl isInvalid={!!errors.username}>
              <FormLabel>Username</FormLabel>
              <Input name="username" value={formData.username} onChange={handleChange} />
              <FormErrorMessage>{errors.username}</FormErrorMessage>
            </FormControl>

            <FormControl isInvalid={!!errors.phone}>
              <FormLabel>Phone</FormLabel>
              <Input name="phone" value={formData.phone} onChange={handleChange} />
              <FormErrorMessage>{errors.phone}</FormErrorMessage>
            </FormControl>

            <SimpleGrid columns={{ base: 1, md: 2 }} spacing={4} width="100%">
              <FormControl>
                <FormLabel>Gender</FormLabel>
                <Select name="gender" value={formData.gender} onChange={handleChange}>
                  <option value={1}>Male</option>
                  <option value={0}>Female</option>
                  <option value={2}>Other</option>
                </Select>
              </FormControl>

              <FormControl>
                <FormLabel>Role</FormLabel>
                <Select name="roleId" value={formData.roleId} onChange={handleChange}>
                  <option value={1}>Customer</option>
                  <option value={2}>Admin</option>
                  <option value={3}>Staff</option>
                </Select>
              </FormControl>
            </SimpleGrid>

            <FormControl>
              <FormLabel>Home Address</FormLabel>
              <Input name="homeAddress" value={formData.homeAddress} onChange={handleChange} />
            </FormControl>

            <SimpleGrid columns={{ base: 1, md: 3 }} spacing={4} width="100%">
              <FormControl>
                <FormLabel>Ward</FormLabel>
                <Input name="wardName" value={formData.wardName} onChange={handleChange} />
              </FormControl>

              <FormControl>
                <FormLabel>District</FormLabel>
                <Input name="district" value={formData.district} onChange={handleChange} />
              </FormControl>

              <FormControl>
                <FormLabel>City</FormLabel>
                <Input name="city" value={formData.city} onChange={handleChange} />
              </FormControl>
            </SimpleGrid>
          </VStack>
        </ModalBody>
        <ModalFooter>
          <Button variant="ghost" mr={3} onClick={onClose}>
            Cancel
          </Button>
          <Button colorScheme="blue" onClick={handleSubmit} isLoading={isSubmitting}>
            Save Changes
          </Button>
        </ModalFooter>
      </ModalContent>
    </Modal>
  )
}

export default UserFormModal
