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
  Avatar,
  Badge,
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
} from "@chakra-ui/react"
import { AddIcon, SearchIcon } from "@chakra-ui/icons"
import { FiMoreVertical, FiEdit, FiTrash2, FiEye, FiLock } from "react-icons/fi"
import { useDispatch, useSelector } from "react-redux"
import type { AppDispatch, RootState } from "../redux/store"
import { fetchUsers, deleteUser } from "../redux/slices/userSlice"
import UserModal from "../components/users/UserModal"
import DeleteConfirmationModal from "../components/common/DeleteConfirmationModal"
import type { User } from "../types/user"

const UserManagement: React.FC = () => {
  const dispatch = useDispatch<AppDispatch>()
  const toast = useToast()
  const { users, loading } = useSelector((state: RootState) => state.users)
  const [searchTerm, setSearchTerm] = useState("")
  const [selectedUser, setSelectedUser] = useState<User | null>(null)
  const { isOpen, onOpen, onClose } = useDisclosure()
  const { isOpen: isDeleteOpen, onOpen: onDeleteOpen, onClose: onDeleteClose } = useDisclosure()
  const [userToDelete, setUserToDelete] = useState<User | null>(null)
  const [isDeleting, setIsDeleting] = useState(false)
  const bgColor = useColorModeValue("white", "gray.800")

  useEffect(() => {
    dispatch(fetchUsers({ search: searchTerm }))
  }, [dispatch, searchTerm])

  const handleSearch = (e: React.ChangeEvent<HTMLInputElement>) => {
    setSearchTerm(e.target.value)
  }

  const handleAddUser = () => {
    setSelectedUser(null)
    onOpen()
  }

  const handleEditUser = (user: User) => {
    setSelectedUser({ ...user })
    onOpen()
  }

  const handleDeleteUser = (user: User) => {
    setUserToDelete(user)
    onDeleteOpen()
  }

  const confirmDelete = async () => {
    if (!userToDelete) return

    setIsDeleting(true)
    try {
      await dispatch(deleteUser(userToDelete.id))
      toast({
        title: "User deleted",
        description: `${userToDelete.name} has been deleted successfully.`,
        status: "success",
        duration: 5000,
        isClosable: true,
      })
      onDeleteClose()
    } catch (error) {
      toast({
        title: "Error",
        description: "Failed to delete the user. Please try again.",
        status: "error",
        duration: 5000,
        isClosable: true,
      })
    } finally {
      setIsDeleting(false)
    }
  }

  const getRoleColor = (role: string) => {
    switch (role.toLowerCase()) {
      case "admin":
        return "red"
      case "manager":
        return "purple"
      case "editor":
        return "blue"
      default:
        return "green"
    }
  }

  const getStatusColor = (status: string) => {
    switch (status.toLowerCase()) {
      case "active":
        return "green"
      case "inactive":
        return "gray"
      case "suspended":
        return "red"
      default:
        return "gray"
    }
  }

  return (
    <Box>
      <Flex justify="space-between" align="center" mb={6}>
        <Heading size="lg">User Management</Heading>
        <Button leftIcon={<AddIcon />} colorScheme="blue" onClick={handleAddUser}>
          Add User
        </Button>
      </Flex>

      <Box mb={6}>
        <InputGroup maxW="320px">
          <InputLeftElement pointerEvents="none">
            <SearchIcon color="gray.400" />
          </InputLeftElement>
          <Input placeholder="Search users..." value={searchTerm} onChange={handleSearch} />
        </InputGroup>
      </Box>

      <Box bg={bgColor} rounded="lg" overflow="hidden" boxShadow="sm">
        <Box overflowX="auto">
          <Table variant="simple">
            <Thead>
              <Tr>
                <Th>User</Th>
                <Th>Email</Th>
                <Th>Role</Th>
                <Th>Status</Th>
                <Th>Last Login</Th>
                <Th width="50px">Actions</Th>
              </Tr>
            </Thead>
            <Tbody>
              {users.map((user) => (
                <Tr key={user.id}>
                  <Td>
                    <Flex align="center">
                      <Avatar size="sm" name={user.name} src={user.avatar} mr={3} />
                      <Box>
                        <Text fontWeight="medium">{user.name}</Text>
                        <Text fontSize="xs" color="gray.500">
                          ID: {user.id}
                        </Text>
                      </Box>
                    </Flex>
                  </Td>
                  <Td>{user.email}</Td>
                  <Td>
                    <Badge colorScheme={getRoleColor(user.role)}>{user.role}</Badge>
                  </Td>
                  <Td>
                    <Badge colorScheme={getStatusColor(user.status)}>{user.status}</Badge>
                  </Td>
                  <Td>{user.lastLogin}</Td>
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
                        <MenuItem icon={<FiEye />}>View Details</MenuItem>
                        <MenuItem icon={<FiEdit />} onClick={() => handleEditUser(user)}>
                          Edit
                        </MenuItem>
                        <MenuItem icon={<FiLock />}>Reset Password</MenuItem>
                        <MenuItem icon={<FiTrash2 />} onClick={() => handleDeleteUser(user)}>
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

      <UserModal isOpen={isOpen} onClose={onClose} user={selectedUser} />

      <DeleteConfirmationModal
        isOpen={isDeleteOpen}
        onClose={onDeleteClose}
        onConfirm={confirmDelete}
        title="Delete User"
        itemName={userToDelete?.name || ""}
        isDeleting={isDeleting}
      />
    </Box>
  )
}

export default UserManagement
