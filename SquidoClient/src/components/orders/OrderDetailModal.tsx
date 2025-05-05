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
  Table,
  Thead,
  Tbody,
  Tr,
  Th,
  Td,
  Image,
  VStack,
  HStack,
} from "@chakra-ui/react"
import { FiPrinter, FiMail } from "react-icons/fi"
import type { Order } from "../../types/order"

interface OrderDetailModalProps {
  isOpen: boolean
  onClose: () => void
  order: Order | null
}

const OrderDetailModal: React.FC<OrderDetailModalProps> = ({ isOpen, onClose, order }) => {
  if (!order) return null

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
    <Modal isOpen={isOpen} onClose={onClose} size="xl">
      <ModalOverlay />
      <ModalContent>
        <ModalHeader>Order Details - {order.id}</ModalHeader>
        <ModalCloseButton />
        <ModalBody>
          <Flex
            justify="space-between"
            align={{ base: "flex-start", md: "center" }}
            direction={{ base: "column", md: "row" }}
            mb={4}
            gap={2}
          >
            <Box>
              <Text fontWeight="bold">Order Date:</Text>
              <Text>{new Date(order.date).toLocaleDateString()}</Text>
            </Box>

            <Badge colorScheme={getStatusColor(order.status)} fontSize="md" px={3} py={1} borderRadius="md">
              {order.status}
            </Badge>
          </Flex>

          <Divider my={4} />

          <Box mb={4}>
            <Text fontWeight="bold" mb={2}>
              Customer Information:
            </Text>
            <VStack align="flex-start" spacing={1}>
              <Text>{order.customer.name}</Text>
              <Text>{order.customer.email}</Text>
              <Text>{order.customer.phone}</Text>
            </VStack>
          </Box>

          <Box mb={4}>
            <Text fontWeight="bold" mb={2}>
              Shipping Address:
            </Text>
            <VStack align="flex-start" spacing={1}>
              <Text>{order.shippingAddress.street}</Text>
              <Text>
                {order.shippingAddress.city}, {order.shippingAddress.state} {order.shippingAddress.zip}
              </Text>
              <Text>{order.shippingAddress.country}</Text>
            </VStack>
          </Box>

          <Divider my={4} />

          <Text fontWeight="bold" mb={2}>
            Order Items:
          </Text>
          <Box overflowX="auto">
            <Table variant="simple" size="sm">
              <Thead>
                <Tr>
                  <Th>Item</Th>
                  <Th isNumeric>Qty</Th>
                  <Th isNumeric>Price</Th>
                  <Th isNumeric>Total</Th>
                </Tr>
              </Thead>
              <Tbody>
                {order.items.map((item) => (
                  <Tr key={item.id}>
                    <Td>
                      <Flex align="center">
                        <Image
                          src={item.image || "/placeholder.svg?height=30&width=20&query=book"}
                          alt={item.name}
                          boxSize="30px"
                          objectFit="cover"
                          mr={2}
                          borderRadius="md"
                        />
                        <Text>{item.name}</Text>
                      </Flex>
                    </Td>
                    <Td isNumeric>{item.quantity}</Td>
                    <Td isNumeric>${item.price.toFixed(2)}</Td>
                    <Td isNumeric>${(item.quantity * item.price).toFixed(2)}</Td>
                  </Tr>
                ))}
              </Tbody>
            </Table>
          </Box>

          <Divider my={4} />

          <Flex justify="space-between" mb={2}>
            <Text>Subtotal:</Text>
            <Text>${order.subtotal.toFixed(2)}</Text>
          </Flex>
          <Flex justify="space-between" mb={2}>
            <Text>Shipping:</Text>
            <Text>${order.shipping.toFixed(2)}</Text>
          </Flex>
          <Flex justify="space-between" mb={2}>
            <Text>Tax:</Text>
            <Text>${order.tax.toFixed(2)}</Text>
          </Flex>
          <Flex justify="space-between" fontWeight="bold">
            <Text>Total:</Text>
            <Text>${order.amount.toFixed(2)}</Text>
          </Flex>

          <Divider my={4} />

          <Box>
            <Text fontWeight="bold" mb={2}>
              Payment Information:
            </Text>
            <Text>Method: {order.paymentMethod}</Text>
            <Text>Transaction ID: {order.transactionId}</Text>
          </Box>
        </ModalBody>
        <ModalFooter>
          <HStack spacing={3}>
            <Button leftIcon={<FiPrinter />} variant="outline">
              Print
            </Button>
            <Button leftIcon={<FiMail />} variant="outline">
              Email
            </Button>
            <Button onClick={onClose}>Close</Button>
          </HStack>
        </ModalFooter>
      </ModalContent>
    </Modal>
  )
}

export default OrderDetailModal
