import type React from "react"
import { Table, Thead, Tbody, Tr, Th, Td, Badge, Box, useColorModeValue, Button } from "@chakra-ui/react"

const orders = [
  {
    id: "ORD-001",
    customer: "John Doe",
    date: "2023-05-01",
    amount: 125.99,
    status: "Completed",
  },
  {
    id: "ORD-002",
    customer: "Jane Smith",
    date: "2023-05-02",
    amount: 85.5,
    status: "Processing",
  },
  {
    id: "ORD-003",
    customer: "Robert Johnson",
    date: "2023-05-03",
    amount: 210.75,
    status: "Completed",
  },
  {
    id: "ORD-004",
    customer: "Emily Davis",
    date: "2023-05-04",
    amount: 45.25,
    status: "Pending",
  },
  {
    id: "ORD-005",
    customer: "Michael Wilson",
    date: "2023-05-05",
    amount: 150.0,
    status: "Cancelled",
  },
]

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

const RecentOrdersTable: React.FC = () => {
  const borderColor = useColorModeValue("gray.200", "gray.700")

  return (
    <Box overflowX="auto">
      <Table variant="simple">
        <Thead>
          <Tr>
            <Th>Order ID</Th>
            <Th>Customer</Th>
            <Th>Date</Th>
            <Th isNumeric>Amount</Th>
            <Th>Status</Th>
            <Th>Action</Th>
          </Tr>
        </Thead>
        <Tbody>
          {orders.map((order) => (
            <Tr key={order.id}>
              <Td fontWeight="medium">{order.id}</Td>
              <Td>{order.customer}</Td>
              <Td>{order.date}</Td>
              <Td isNumeric>${order.amount.toFixed(2)}</Td>
              <Td>
                <Badge colorScheme={getStatusColor(order.status)}>{order.status}</Badge>
              </Td>
              <Td>
                <Button size="sm" variant="outline">
                  View
                </Button>
              </Td>
            </Tr>
          ))}
        </Tbody>
      </Table>
      <Box mt={4} textAlign="center">
        <Button variant="link" color="brand.500">
          View All Orders
        </Button>
      </Box>
    </Box>
  )
}

export default RecentOrdersTable
