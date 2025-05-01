"use client"

import type React from "react"
import { useEffect, useState } from "react"
import {
  Box,
  Heading,
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
  Select,
  HStack,
  useColorModeValue,
  useDisclosure,
  useToast,
} from "@chakra-ui/react"
import { SearchIcon } from "@chakra-ui/icons"
import { FiMoreVertical, FiEye, FiEdit, FiPrinter, FiXCircle } from "react-icons/fi"
import { useDispatch, useSelector } from "react-redux"
import type { AppDispatch, RootState } from "../redux/store"
import { fetchOrders, cancelOrder } from "../redux/slices/orderSlice"
import OrderDetailModal from "../components/orders/OrderDetailModal"
import OrderStatusModal from "../components/orders/OrderStatusModal"
import DeleteConfirmationModal from "../components/common/DeleteConfirmationModal"
import type { Order } from "../types/order"

const OrderManagement: React.FC = () => {
  const dispatch = useDispatch<AppDispatch>()
  const toast = useToast()
  const { orders, loading } = useSelector((state: RootState) => state.orders)
  const [searchTerm, setSearchTerm] = useState("")
  const [statusFilter, setStatusFilter] = useState("")
  const [selectedOrder, setSelectedOrder] = useState<Order | null>(null)

  // For detail modal
  const { isOpen, onOpen, onClose } = useDisclosure()

  // For status update modal
  const { isOpen: isStatusOpen, onOpen: onStatusOpen, onClose: onStatusClose } = useDisclosure()

  // For cancel confirmation modal
  const { isOpen: isCancelOpen, onOpen: onCancelOpen, onClose: onCancelClose } = useDisclosure()

  const [orderToCancel, setOrderToCancel] = useState<Order | null>(null)
  const [isCancelling, setIsCancelling] = useState(false)
  const bgColor = useColorModeValue("white", "gray.800")

  useEffect(() => {
    dispatch(fetchOrders({ search: searchTerm, status: statusFilter }))
  }, [dispatch, searchTerm, statusFilter])

  const handleSearch = (e: React.ChangeEvent<HTMLInputElement>) => {
    setSearchTerm(e.target.value)
  }

  const handleStatusChange = (e: React.ChangeEvent<HTMLSelectElement>) => {
    setStatusFilter(e.target.value)
  }

  const handleViewOrder = (order: Order) => {
    setSelectedOrder(order)
    onOpen()
  }

  const handleUpdateStatus = (order: Order) => {
    setSelectedOrder(order)
    onStatusOpen()
  }

  const handleCancelOrder = (order: Order) => {
    setOrderToCancel(order)
    onCancelOpen()
  }

  const confirmCancel = async () => {
    if (!orderToCancel) return

    setIsCancelling(true)
    try {
      await dispatch(cancelOrder(orderToCancel.id))
      toast({
        title: "Order cancelled",
        description: `Order ${orderToCancel.id} has been cancelled.`,
        status: "success",
        duration: 5000,
        isClosable: true,
      })
      onCancelClose()
    } catch (error) {
      toast({
        title: "Error",
        description: "Failed to cancel the order. Please try again.",
        status: "error",
        duration: 5000,
        isClosable: true,
      })
    } finally {
      setIsCancelling(false)
    }
  }

  const getStatusColor = (status: string) => {
    switch (status.toLowerCase()) {
      case "completed":
        return "green"
      case "processing":
        return "blue"
      case "pending":
        return "yellow"
      case "cancelled":
        return "red"
      default:
        return "gray"
    }
  }

  return (
    <Box>
      <Heading size="lg" mb={6}>
        Order Management
      </Heading>

      <Flex mb={6} direction={{ base: "column", md: "row" }} gap={4} align={{ base: "stretch", md: "center" }}>
        <InputGroup maxW={{ base: "100%", md: "320px" }}>
          <InputLeftElement pointerEvents="none">
            <SearchIcon color="gray.400" />
          </InputLeftElement>
          <Input placeholder="Search orders..." value={searchTerm} onChange={handleSearch} />
        </InputGroup>

        <HStack spacing={4}>
          <Select
            placeholder="All Statuses"
            value={statusFilter}
            onChange={handleStatusChange}
            maxW={{ base: "100%", md: "200px" }}
          >
            <option value="pending">Pending</option>
            <option value="processing">Processing</option>
            <option value="completed">Completed</option>
            <option value="cancelled">Cancelled</option>
          </Select>

          <Select placeholder="Sort By" maxW={{ base: "100%", md: "150px" }}>
            <option value="date-desc">Date (Newest)</option>
            <option value="date-asc">Date (Oldest)</option>
            <option value="amount-desc">Amount (High-Low)</option>
            <option value="amount-asc">Amount (Low-High)</option>
          </Select>
        </HStack>
      </Flex>

      <Box bg={bgColor} rounded="lg" overflow="hidden" boxShadow="sm">
        <Box overflowX="auto">
          <Table variant="simple">
            <Thead>
              <Tr>
                <Th>Order ID</Th>
                <Th>Customer</Th>
                <Th>Date</Th>
                <Th isNumeric>Amount</Th>
                <Th>Items</Th>
                <Th>Status</Th>
                <Th width="50px">Actions</Th>
              </Tr>
            </Thead>
            <Tbody>
              {orders.map((order) => (
                <Tr key={order.id}>
                  <Td fontWeight="medium">{order.id}</Td>
                  <Td>{order.customer.name}</Td>
                  <Td>{new Date(order.date).toLocaleDateString()}</Td>
                  <Td isNumeric>${order.amount.toFixed(2)}</Td>
                  <Td>{order.items.length}</Td>
                  <Td>
                    <Badge colorScheme={getStatusColor(order.status)}>{order.status}</Badge>
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
                        <MenuItem icon={<FiEye />} onClick={() => handleViewOrder(order)}>
                          View Details
                        </MenuItem>
                        <MenuItem icon={<FiEdit />} onClick={() => handleUpdateStatus(order)}>
                          Update Status
                        </MenuItem>
                        <MenuItem icon={<FiPrinter />}>Print Invoice</MenuItem>
                        {order.status.toLowerCase() !== "cancelled" && (
                          <MenuItem icon={<FiXCircle />} onClick={() => handleCancelOrder(order)}>
                            Cancel Order
                          </MenuItem>
                        )}
                      </MenuList>
                    </Menu>
                  </Td>
                </Tr>
              ))}
            </Tbody>
          </Table>
        </Box>
      </Box>

      {/* Order Detail Modal */}
      <OrderDetailModal isOpen={isOpen} onClose={onClose} order={selectedOrder} />

      {/* Order Status Update Modal */}
      <OrderStatusModal isOpen={isStatusOpen} onClose={onStatusClose} order={selectedOrder} />

      {/* Cancel Order Confirmation Modal */}
      <DeleteConfirmationModal
        isOpen={isCancelOpen}
        onClose={onCancelClose}
        onConfirm={confirmCancel}
        title="Cancel Order"
        itemName={`Order ${orderToCancel?.id || ""}`}
        isDeleting={isCancelling}
      />
    </Box>
  )
}

export default OrderManagement
