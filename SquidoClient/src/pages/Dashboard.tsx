"use client"

import type React from "react"
import { useEffect } from "react"
import {
  Box,
  SimpleGrid,
  Stat,
  StatLabel,
  StatNumber,
  StatHelpText,
  StatArrow,
  Flex,
  Text,
  Heading,
  useColorModeValue,
  Icon,
} from "@chakra-ui/react"
import { FiBook, FiUsers, FiShoppingCart, FiDollarSign } from "react-icons/fi"
import { useDispatch, useSelector } from "react-redux"
import { fetchDashboardStats } from "../redux/slices/dashboardSlice"
import type { AppDispatch, RootState } from "../redux/store"
import SalesChart from "../components/dashboard/SalesChart"
import RecentOrdersTable from "../components/dashboard/RecentOrdersTable"
import TopSellingBooks from "../components/dashboard/TopSellingBooks"

const StatCard: React.FC<{
  title: string
  value: number | string
  icon: React.ElementType
  change?: number
  changeLabel?: string
}> = ({ title, value, icon, change, changeLabel }) => {
  const bgColor = useColorModeValue("white", "gray.800")
  const iconBg = useColorModeValue("brand.50", "brand.900")

  return (
    <Box p={5} bg={bgColor} rounded="lg" boxShadow="sm">
      <Flex justifyContent="space-between">
        <Stat>
          <StatLabel fontWeight="medium">{title}</StatLabel>
          <StatNumber fontSize="2xl" fontWeight="bold">
            {value}
          </StatNumber>
          {change !== undefined && (
            <StatHelpText>
              <StatArrow type={change >= 0 ? "increase" : "decrease"} />
              {Math.abs(change)}% {changeLabel || "from last month"}
            </StatHelpText>
          )}
        </Stat>
        <Flex alignItems="center" justifyContent="center" rounded="full" bg={iconBg} h="45px" w="45px">
          <Icon as={icon} color="brand.500" boxSize="20px" />
        </Flex>
      </Flex>
    </Box>
  )
}

const Dashboard: React.FC = () => {
  const dispatch = useDispatch<AppDispatch>()
  const { stats, loading } = useSelector((state: RootState) => state.dashboard)

  useEffect(() => {
    dispatch(fetchDashboardStats())
  }, [dispatch])

  return (
    <Box>
      <Heading size="lg" mb={6}>
        Dashboard
      </Heading>

      <SimpleGrid columns={{ base: 1, md: 2, lg: 4 }} spacing={5} mb={8}>
        <StatCard title="Total Books" value={stats?.totalBooks || 0} icon={FiBook} change={5.2} />
        <StatCard title="Total Users" value={stats?.totalUsers || 0} icon={FiUsers} change={12.5} />
        <StatCard title="Total Orders" value={stats?.totalOrders || 0} icon={FiShoppingCart} change={-2.4} />
        <StatCard
          title="Revenue"
          value={`$${stats?.revenue?.toLocaleString() || 0}`}
          icon={FiDollarSign}
          change={8.1}
        />
      </SimpleGrid>

      <SimpleGrid columns={{ base: 1, lg: 2 }} spacing={5} mb={8}>
        <Box bg={useColorModeValue("white", "gray.800")} p={5} rounded="lg" boxShadow="sm">
          <Text fontSize="lg" fontWeight="medium" mb={4}>
            Sales Overview
          </Text>
          <SalesChart />
        </Box>

        <Box bg={useColorModeValue("white", "gray.800")} p={5} rounded="lg" boxShadow="sm">
          <Text fontSize="lg" fontWeight="medium" mb={4}>
            Top Selling Books
          </Text>
          <TopSellingBooks />
        </Box>
      </SimpleGrid>

      <Box bg={useColorModeValue("white", "gray.800")} p={5} rounded="lg" boxShadow="sm">
        <Text fontSize="lg" fontWeight="medium" mb={4}>
          Recent Orders
        </Text>
        <RecentOrdersTable />
      </Box>
    </Box>
  )
}

export default Dashboard
