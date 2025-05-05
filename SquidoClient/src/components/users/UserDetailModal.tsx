"use client"

import type React from "react"
import {
  Modal,
  ModalOverlay,
  ModalContent,
  ModalHeader,
  ModalFooter,
  ModalBody,
  ModalCloseButton,
  Button,
  Box,
  Flex,
  Text,
  Badge,
  Divider,
  SimpleGrid,
  Spinner,
} from "@chakra-ui/react"
import { useDispatch, useSelector } from "react-redux"
import type { AppDispatch, RootState } from "../../redux/store"
import { fetchUserById } from "../../redux/slices/userSlice"
import { useEffect } from "react"

interface UserDetailModalProps {
  isOpen: boolean
  onClose: () => void
  userId: string | null
}

const UserDetailModal: React.FC<UserDetailModalProps> = ({ isOpen, onClose, userId }) => {
  const dispatch = useDispatch<AppDispatch>()
  const { currentUser, loading, error } = useSelector((state: RootState) => state.users)

  useEffect(() => {
    if (isOpen && userId) {
      dispatch(fetchUserById(userId))
    }
  }, [isOpen, userId, dispatch])

  if (!isOpen) return null

  if (loading) {
    return (
      <Modal isOpen={isOpen} onClose={onClose} size="lg">
        <ModalOverlay />
        <ModalContent>
          <ModalHeader>User Details</ModalHeader>
          <ModalCloseButton />
          <ModalBody>
            <Flex justify="center" align="center" minH="200px">
              <Spinner size="xl" />
            </Flex>
          </ModalBody>
        </ModalContent>
      </Modal>
    )
  }

  if (error || !currentUser) {
    return (
      <Modal isOpen={isOpen} onClose={onClose} size="lg">
        <ModalOverlay />
        <ModalContent>
          <ModalHeader>User Details</ModalHeader>
          <ModalCloseButton />
          <ModalBody>
            <Box p={4} textAlign="center" color="red.500">
              {error || "Failed to load user details"}
            </Box>
          </ModalBody>
          <ModalFooter>
            <Button onClick={onClose}>Close</Button>
          </ModalFooter>
        </ModalContent>
      </Modal>
    )
  }

  const getGenderText = (gender: number) => {
    switch (gender) {
      case 0:
        return "Female"
      case 1:
        return "Male"
      default:
        return "Other"
    }
  }

  return (
    <Modal isOpen={isOpen} onClose={onClose} size="lg">
      <ModalOverlay />
      <ModalContent>
        <ModalHeader>User Details</ModalHeader>
        <ModalCloseButton />
        <ModalBody>
          <Box mb={4}>
            <Flex justify="space-between" align="center">
              <Text fontSize="xl" fontWeight="bold">
                {currentUser.firstName} {currentUser.lastName}
              </Text>
              <Badge colorScheme="blue">{currentUser.role.roleName}</Badge>
            </Flex>
            <Text color="gray.500">{currentUser.email}</Text>
          </Box>

          <Divider my={4} />

          <SimpleGrid columns={2} spacing={4} mb={4}>
            <Box>
              <Text fontWeight="bold">Username</Text>
              <Text>{currentUser.username}</Text>
            </Box>
            <Box>
              <Text fontWeight="bold">Phone</Text>
              <Text>{currentUser.phone || "Not provided"}</Text>
            </Box>
            <Box>
              <Text fontWeight="bold">Gender</Text>
              <Text>{getGenderText(currentUser.gender)}</Text>
            </Box>
            <Box>
              <Text fontWeight="bold">Role</Text>
              <Text>{currentUser.role.roleName}</Text>
            </Box>
          </SimpleGrid>

          <Divider my={4} />

          <Box mb={4}>
            <Text fontWeight="bold" mb={2}>
              Address Information
            </Text>
            <SimpleGrid columns={1} spacing={2}>
              <Box>
                <Text fontWeight="medium">Home Address</Text>
                <Text>{currentUser.homeAddress || "Not provided"}</Text>
              </Box>
              <Box>
                <Text fontWeight="medium">Ward</Text>
                <Text>{currentUser.wardName || "Not provided"}</Text>
              </Box>
              <Box>
                <Text fontWeight="medium">District</Text>
                <Text>{currentUser.district || "Not provided"}</Text>
              </Box>
              <Box>
                <Text fontWeight="medium">City</Text>
                <Text>{currentUser.city || "Not provided"}</Text>
              </Box>
            </SimpleGrid>
          </Box>

          <Divider my={4} />

          <Box>
            <Text fontWeight="bold" mb={2}>
              Account Information
            </Text>
            <SimpleGrid columns={2} spacing={2}>
              <Box>
                <Text fontWeight="medium">User ID</Text>
                <Text fontSize="sm" isTruncated>
                  {currentUser.id}
                </Text>
              </Box>
              <Box>
                <Text fontWeight="medium">Status</Text>
                <Badge colorScheme={currentUser.isDeleted ? "red" : "green"}>
                  {currentUser.isDeleted ? "Deleted" : "Active"}
                </Badge>
              </Box>
            </SimpleGrid>
          </Box>
        </ModalBody>
        <ModalFooter>
          <Button onClick={onClose}>Close</Button>
        </ModalFooter>
      </ModalContent>
    </Modal>
  )
}

export default UserDetailModal
