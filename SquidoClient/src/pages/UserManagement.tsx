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
  Spinner,
  Switch,
  Alert,
  AlertIcon,
  AlertTitle,
  AlertDescription,
} from "@chakra-ui/react"
import { SearchIcon, RepeatIcon } from "@chakra-ui/icons"
import { FiMoreVertical, FiEdit, FiTrash2, FiEye, FiUserPlus } from "react-icons/fi"
import { useDispatch, useSelector } from "react-redux"
import type { AppDispatch, RootState } from "../redux/store"
import { fetchUsers, deleteUser, fetchMoreUsers, clearUserError } from "../redux/slices/userSlice"
import UserDetailModal from "../components/users/UserDetailModal"
import UserFormModal from "../components/users/UserFormModal"
import DeleteConfirmationModal from "../components/common/DeleteConfirmationModal"
import type { UserParams } from "../types/user"

const UserManagement: React.FC = () => {
  const dispatch = useDispatch<AppDispatch>()
  const toast = useToast()
  const { users, loading, pagination, error } = useSelector((state: RootState) => state.users)
  const [searchTerm, setSearchTerm] = useState("")
  const [debouncedSearchTerm, setDebouncedSearchTerm] = useState("")
  const [selectedUserId, setSelectedUserId] = useState<string | null>(null)
  const bgColor = useColorModeValue("white", "gray.800")

  // For detail modal
  const { isOpen: isDetailOpen, onOpen: onDetailOpen, onClose: onDetailClose } = useDisclosure()

  // For edit modal
  const { isOpen: isEditOpen, onOpen: onEditOpen, onClose: onEditClose } = useDisclosure()

  // For delete confirmation modal
  const { isOpen: isDeleteOpen, onOpen: onDeleteOpen, onClose: onDeleteClose } = useDisclosure()
  const [userToDelete, setUserToDelete] = useState<string | null>(null)
  const [isDeleting, setIsDeleting] = useState(false)

  // For loading more users
  const [loadingMore, setLoadingMore] = useState(false)

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
  const searchParams: UserParams = {
    page: 1,
    pageSize: 10,
    keyword: debouncedSearchTerm,
  }

  // Fetch users when search params change
  useEffect(() => {
    dispatch(fetchUsers(searchParams))
  }, [dispatch, debouncedSearchTerm])

  const handleSearch = (e: React.ChangeEvent<HTMLInputElement>) => {
    setSearchTerm(e.target.value)
  }

  const handleViewUser = (userId: string) => {
    setSelectedUserId(userId)
    onDetailOpen()
  }

  const handleEditUser = (userId: string) => {
    setSelectedUserId(userId)
    onEditOpen()
  }

  const handleDeleteUser = (userId: string, name: string) => {
    setUserToDelete(userId)
    onDeleteOpen()
  }

  const confirmDelete = async () => {
    if (!userToDelete) return

    setIsDeleting(true)
    try {
      await dispatch(deleteUser(userToDelete)).unwrap()
      toast({
        title: "User deleted",
        description: "The user has been deleted successfully.",
        status: "success",
        duration: 5000,
        isClosable: true,
      })
      onDeleteClose()
    } catch (error) {
      toast({
        title: "Error",
        description: typeof error === "string" ? error : "Failed to delete the user. Please try again.",
        status: "error",
        duration: 5000,
        isClosable: true,
      })
    } finally {
      setIsDeleting(false)
    }
  }

  const handleLoadMore = async () => {
    if (!pagination.hasMore) return

    setLoadingMore(true)
    try {
      await dispatch(
        fetchMoreUsers({
          ...searchParams,
          page: pagination.currentPage + 1,
        }),
      )
    } catch (error) {
      toast({
        title: "Error",
        description: "Failed to load more users. Please try again.",
        status: "error",
        duration: 5000,
        isClosable: true,
      })
    } finally {
      setLoadingMore(false)
    }
  }

  // Function to retry fetching users
  const handleRetry = () => {
    dispatch(clearUserError())
    dispatch(fetchUsers(searchParams))
  }

  return (
    <Box>
      <Flex justify="space-between" align="center" mb={6}>
        <Heading size="lg">User Management</Heading>
        <Button leftIcon={<FiUserPlus />} colorScheme="blue">
          Add User
        </Button>
      </Flex>

      <Flex mb={6} direction={{ base: "column", md: "row" }} gap={4} align={{ base: "stretch", md: "center" }}>
        <InputGroup maxW={{ base: "100%", md: "320px" }}>
          <InputLeftElement pointerEvents="none">
            <SearchIcon color="gray.400" />
          </InputLeftElement>
          <Input placeholder="Search users..." value={searchTerm} onChange={handleSearch} />
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
            <AlertTitle>Error fetching users</AlertTitle>
            <AlertDescription display="block">
              {error}
              <Text mt={2}>Check your API connection and try again.</Text>
            </AlertDescription>
          </Box>
        </Alert>
      )}

      {loading && !users.length ? (
        <Flex direction="column" align="center" justify="center" py={10}>
          <Spinner size="xl" mb={4} color="blue.500" />
          <Text>Loading users...</Text>
        </Flex>
      ) : (
        <>
          {pagination && (
            <Text mb={4} color="gray.500">
              Showing {users.length} of {pagination.totalCount} users
            </Text>
          )}

          <Box bg={bgColor} rounded="lg" overflow="hidden" boxShadow="sm">
            <Box overflowX="auto">
              <Table variant="simple">
                <Thead>
                  <Tr>
                    <Th>Email</Th>
                    <Th>Full Name</Th>
                    <Th>Phone</Th>
                    <Th>Role</Th>
                    <Th>Status</Th>
                    <Th width="50px">Actions</Th>
                  </Tr>
                </Thead>
                <Tbody>
                  {users.map((user) => (
                    <Tr key={user.id}>
                      <Td>{user.email}</Td>
                      <Td>
                        {user.firstName} {user.lastName}
                      </Td>
                      <Td>{user.phone || "—"}</Td>
                      <Td>
                        <Badge colorScheme="blue">{user.role.roleName}</Badge>
                      </Td>
                      <Td>
                        <Switch isChecked={!user.isDeleted} colorScheme="green" size="sm" isReadOnly />
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
                            <MenuItem icon={<FiEye />} onClick={() => handleViewUser(user.id)}>
                              View Details
                            </MenuItem>
                            <MenuItem icon={<FiEdit />} onClick={() => handleEditUser(user.id)}>
                              Edit
                            </MenuItem>
                            <MenuItem
                              icon={<FiTrash2 />}
                              onClick={() => handleDeleteUser(user.id, `${user.firstName} ${user.lastName}`)}
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

            {pagination.hasMore && (
              <Flex justify="center" p={4}>
                <Button
                  onClick={handleLoadMore}
                  isLoading={loadingMore}
                  loadingText="Loading more..."
                  variant="outline"
                >
                  Load More ({users.length} of {pagination.totalCount})
                </Button>
              </Flex>
            )}

            {!loading && users.length === 0 && (
              <Box p={8} textAlign="center">
                <Text>No users found. Try adjusting your search.</Text>
              </Box>
            )}
          </Box>
        </>
      )}

      {/* User Detail Modal */}
      <UserDetailModal isOpen={isDetailOpen} onClose={onDetailClose} userId={selectedUserId} />

      {/* User Edit Modal */}
      <UserFormModal isOpen={isEditOpen} onClose={onEditClose} userId={selectedUserId} />

      {/* Delete Confirmation Modal */}
      <DeleteConfirmationModal
        isOpen={isDeleteOpen}
        onClose={onDeleteClose}
        onConfirm={confirmDelete}
        title="Delete User"
        itemName="this user"
        isDeleting={isDeleting}
      />
    </Box>
  )
}

export default UserManagement
