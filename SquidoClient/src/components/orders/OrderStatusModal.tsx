"use client"

import React from "react"
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
  Select,
  useToast,
} from "@chakra-ui/react"
import { useDispatch } from "react-redux"
import type { AppDispatch } from "../../redux/store"
import { updateOrderStatus } from "../../redux/slices/orderSlice"
import type { Order } from "../../types/order"

interface OrderStatusModalProps {
  isOpen: boolean
  onClose: () => void
  order: Order | null
}

const OrderStatusModal: React.FC<OrderStatusModalProps> = ({ isOpen, onClose, order }) => {
  const dispatch = useDispatch<AppDispatch>()
  const toast = useToast()
  const [status, setStatus] = React.useState<string>("")
  const [isSubmitting, setIsSubmitting] = React.useState(false)

  React.useEffect(() => {
    if (order) {
      setStatus(order.status)
    }
  }, [order])

  const handleStatusChange = (e: React.ChangeEvent<HTMLSelectElement>) => {
    setStatus(e.target.value)
  }

  const handleSubmit = async () => {
    if (!order) return

    setIsSubmitting(true)
    try {
      await dispatch(updateOrderStatus({ id: order.id, status }))
      toast({
        title: "Order updated",
        description: `Order ${order.id} status has been updated to ${status}.`,
        status: "success",
        duration: 5000,
        isClosable: true,
      })
      onClose()
    } catch (error) {
      toast({
        title: "Error",
        description: "Failed to update order status. Please try again.",
        status: "error",
        duration: 5000,
        isClosable: true,
      })
    } finally {
      setIsSubmitting(false)
    }
  }

  return (
    <Modal isOpen={isOpen} onClose={onClose}>
      <ModalOverlay />
      <ModalContent>
        <ModalHeader>Update Order Status</ModalHeader>
        <ModalCloseButton />
        <ModalBody>
          <FormControl>
            <FormLabel>Order ID</FormLabel>
            <Select isDisabled value={order?.id || ""}>
              <option value={order?.id || ""}>{order?.id || ""}</option>
            </Select>
          </FormControl>

          <FormControl mt={4}>
            <FormLabel>Status</FormLabel>
            <Select value={status} onChange={handleStatusChange}>
              <option value="Pending">Pending</option>
              <option value="Processing">Processing</option>
              <option value="Completed">Completed</option>
              <option value="Cancelled">Cancelled</option>
            </Select>
          </FormControl>
        </ModalBody>
        <ModalFooter>
          <Button variant="ghost" mr={3} onClick={onClose}>
            Cancel
          </Button>
          <Button colorScheme="blue" onClick={handleSubmit} isLoading={isSubmitting}>
            Update Status
          </Button>
        </ModalFooter>
      </ModalContent>
    </Modal>
  )
}

export default OrderStatusModal
