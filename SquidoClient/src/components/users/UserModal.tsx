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
} from "@chakra-ui/react"
import { useDispatch } from "react-redux"
import type { AppDispatch } from "../../redux/store"
import { createUser, updateUser } from "../../redux/slices/userSlice"
import type { User } from "../../types/user"

interface UserModalProps {
  isOpen: boolean
  onClose: () => void
  user: User | null
}

const UserModal: React.FC<UserModalProps> = ({ isOpen, onClose, user }) => {
  const dispatch = useDispatch<AppDispatch>()
  const toast = useToast()

  const [formData, setFormData] = useState<Partial<User>>({
    name: "",
    email: "",
    role: "user",
    status: "active",
  })

  const [errors, setErrors] = useState<Record<string, string>>({})

  useEffect(() => {
    if (user) {
      setFormData({
        name: user.name,
        email: user.email,
        role: user.role,
        status: user.status,
      })
    } else {
      setFormData({
        name: "",
        email: "",
        role: "user",
        status: "active",
      })
    }
  }, [user])

  const validateForm = () => {
    const newErrors: Record<string, string> = {}

    if (!formData.name) newErrors.name = "Name is required"
    if (!formData.email) newErrors.email = "Email is required"
    if (!/^\S+@\S+\.\S+$/.test(formData.email || "")) {
      newErrors.email = "Invalid email format"
    }

    setErrors(newErrors)
    return Object.keys(newErrors).length === 0
  }

  const handleChange = (e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement>) => {
    const { name, value } = e.target
    setFormData((prev) => ({ ...prev, [name]: value }))
  }

  const handleSubmit = async () => {
    if (!validateForm()) return

    try {
      if (user) {
        await dispatch(updateUser({ id: user.id, userData: { ...formData, id: user.id } as User }))
        toast({
          title: "User updated",
          description: "The user has been updated successfully.",
          status: "success",
          duration: 5000,
          isClosable: true,
        })
      } else {
        // Create a new user object without using constructor
        const newUser = {
          id: Date.now().toString(), // Temporary ID that will be replaced by the server
          name: formData.name || "",
          email: formData.email || "",
          role: formData.role || "user",
          status: formData.status || "active",
        }
        await dispatch(createUser(newUser))
        toast({
          title: "User created",
          description: "The user has been created successfully.",
          status: "success",
          duration: 5000,
          isClosable: true,
        })
      }
      onClose()
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
    <Modal isOpen={isOpen} onClose={onClose}>
      <ModalOverlay />
      <ModalContent>
        <ModalHeader>{user ? "Edit User" : "Add New User"}</ModalHeader>
        <ModalCloseButton />
        <ModalBody>
          <VStack spacing={4}>
            <FormControl isInvalid={!!errors.name}>
              <FormLabel>Name</FormLabel>
              <Input name="name" value={formData.name || ""} onChange={handleChange} placeholder="Full name" />
              <FormErrorMessage>{errors.name}</FormErrorMessage>
            </FormControl>

            <FormControl isInvalid={!!errors.email}>
              <FormLabel>Email</FormLabel>
              <Input
                name="email"
                type="email"
                value={formData.email || ""}
                onChange={handleChange}
                placeholder="Email address"
              />
              <FormErrorMessage>{errors.email}</FormErrorMessage>
            </FormControl>

            <FormControl>
              <FormLabel>Role</FormLabel>
              <Select name="role" value={formData.role || "user"} onChange={handleChange}>
                <option value="admin">Admin</option>
                <option value="manager">Manager</option>
                <option value="editor">Editor</option>
                <option value="user">User</option>
              </Select>
            </FormControl>

            <FormControl>
              <FormLabel>Status</FormLabel>
              <Select name="status" value={formData.status || "active"} onChange={handleChange}>
                <option value="active">Active</option>
                <option value="inactive">Inactive</option>
                <option value="suspended">Suspended</option>
              </Select>
            </FormControl>
          </VStack>
        </ModalBody>
        <ModalFooter>
          <Button variant="ghost" mr={3} onClick={onClose}>
            Cancel
          </Button>
          <Button colorScheme="blue" onClick={handleSubmit}>
            {user ? "Update" : "Create"}
          </Button>
        </ModalFooter>
      </ModalContent>
    </Modal>
  )
}

export default UserModal
