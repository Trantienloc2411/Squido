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
  Skeleton,
  SkeletonText,
  Image,
  Progress,
} from "@chakra-ui/react"
import { FiBook, FiUsers, FiDollarSign, FiTag } from "react-icons/fi"
import { useDispatch, useSelector } from "react-redux"
import { fetchDashboardStats } from "../redux/slices/dashboardSlice"
import type { AppDispatch, RootState } from "../redux/store"
import SalesChart from "../components/dashboard/SalesChart"
import RecentOrdersTable from "../components/dashboard/RecentOrdersTable"

const StatCard: React.FC<{
  title: string
  value: number | string
  icon: React.ElementType
  change?: number
  changeLabel?: string
  isLoading?: boolean
}> = ({ title, value, icon, change, changeLabel, isLoading = false }) => {
  const bgColor = useColorModeValue("white", "gray.800")
  const iconBg = useColorModeValue("brand.50", "brand.900")

  return (
    <Box p={5} bg={bgColor} rounded="lg" boxShadow="sm">
      <Flex justifyContent="space-between">
        <Stat>
          <StatLabel fontWeight="medium">{title}</StatLabel>
          {isLoading ? (
            <Skeleton height="30px" width="100px" my={1} />
          ) : (
            <StatNumber fontSize="2xl" fontWeight="bold">
              {value}
            </StatNumber>
          )}
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

const TopSellingBooks: React.FC<{ books: any[]; isLoading: boolean }> = ({ books, isLoading }) => {
  const progressColorScheme = useColorModeValue("brand", "brand")
  const maxQuantity = Math.max(...(books?.map((book) => book.quantity) || [1]))

  if (isLoading) {
    return (
      <Box>
        {[1, 2, 3, 4, 5].map((i) => (
          <Flex key={i} mb={4} align="center">
            <Skeleton height="40px" width="40px" mr={3} />
            <Box flex="1">
              <SkeletonText noOfLines={2} spacing="2" />
              <Skeleton height="8px" mt={1} />
            </Box>
          </Flex>
        ))}
      </Box>
    )
  }

  return (
    <Box>
      {books?.map((book) => (
        <Flex key={book.bookId} mb={4} align="center">
          <Image
            src={book.imageUrls?.[0] || "/placeholder.svg?height=40&width=30&query=book"}
            alt={book.title}
            boxSize="40px"
            objectFit="cover"
            mr={3}
            borderRadius="md"
          />
          <Box flex="1">
            <Flex justify="space-between" mb={1}>
              <Text fontWeight="medium" noOfLines={1}>
                {book.title}
              </Text>
              <Text fontWeight="bold">{book.quantity}</Text>
            </Flex>
            <Text fontSize="sm" color="gray.500" mb={1} noOfLines={1}>
              {book.categoryName}
            </Text>
            <Progress
              value={(book.quantity / maxQuantity) * 100}
              size="sm"
              colorScheme={progressColorScheme}
              borderRadius="full"
            />
          </Box>
        </Flex>
      ))}
    </Box>
  )
}

const TopCategories: React.FC<{ categories: any[]; isLoading: boolean }> = ({ categories, isLoading }) => {
  const progressColorScheme = useColorModeValue("brand", "brand")
  const maxCount = Math.max(...(categories?.map((category) => category.bookCount) || [1]))

  if (isLoading) {
    return (
      <Box>
        {[1, 2, 3, 4, 5].map((i) => (
          <Flex key={i} mb={4} align="center">
            <Skeleton height="40px" width="40px" mr={3} />
            <Box flex="1">
              <SkeletonText noOfLines={2} spacing="2" />
              <Skeleton height="8px" mt={1} />
            </Box>
          </Flex>
        ))}
      </Box>
    )
  }

  return (
    <Box>
      {categories?.map((category) => (
        <Flex key={category.categoryId} mb={4} align="center">
          <Flex alignItems="center" justifyContent="center" rounded="full" bg="brand.50" h="40px" w="40px" mr={3}>
            <Icon as={FiTag} color="brand.500" boxSize="20px" />
          </Flex>
          <Box flex="1">
            <Flex justify="space-between" mb={1}>
              <Text fontWeight="medium" noOfLines={1}>
                {category.name}
              </Text>
              <Text fontWeight="bold">{category.bookCount} books</Text>
            </Flex>
            <Progress
              value={(category.bookCount / maxCount) * 100}
              size="sm"
              colorScheme={progressColorScheme}
              borderRadius="full"
            />
          </Box>
        </Flex>
      ))}
    </Box>
  )
}

const Dashboard: React.FC = () => {
  const dispatch = useDispatch<AppDispatch>()
  const { stats, loading } = useSelector((state: RootState) => state.dashboard)
  const bgColor = useColorModeValue("white", "gray.800")

  useEffect(() => {
    dispatch(fetchDashboardStats())
  }, [dispatch])

  return (
    <Box>
      <Heading size="lg" mb={6}>
        Dashboard
      </Heading>

      <SimpleGrid columns={{ base: 1, md: 2, lg: 4 }} spacing={5} mb={8}>
        <StatCard title="Total Books" value={stats?.totalBooks || 0} icon={FiBook} isLoading={loading} />
        <StatCard title="Total Categories" value={stats?.totalCategories || 0} icon={FiTag} isLoading={loading} />
        <StatCard title="Total Customers" value={stats?.totalCustomers || 0} icon={FiUsers} isLoading={loading} />
        <StatCard
          title="Total Revenue"
          value={`$${stats?.totalRevenues?.toLocaleString() || 0}`}
          icon={FiDollarSign}
          isLoading={loading}
        />
      </SimpleGrid>

      <SimpleGrid columns={{ base: 1, lg: 2 }} spacing={5} mb={8}>
        <Box bg={bgColor} p={5} rounded="lg" boxShadow="sm">
          <Text fontSize="lg" fontWeight="medium" mb={4}>
            Sales Overview
          </Text>
          <SalesChart />
        </Box>

        <Box bg={bgColor} p={5} rounded="lg" boxShadow="sm">
          <Text fontSize="lg" fontWeight="medium" mb={4}>
            Top Books
          </Text>
          <TopSellingBooks books={stats?.topBooks || []} isLoading={loading} />
        </Box>
      </SimpleGrid>

      <SimpleGrid columns={{ base: 1, lg: 2 }} spacing={5} mb={8}>
        <Box bg={bgColor} p={5} rounded="lg" boxShadow="sm">
          <Text fontSize="lg" fontWeight="medium" mb={4}>
            Top Categories
          </Text>
          <TopCategories categories={stats?.topCategories || []} isLoading={loading} />
        </Box>

        <Box bg={bgColor} p={5} rounded="lg" boxShadow="sm">
          <Text fontSize="lg" fontWeight="medium" mb={4}>
            Recent Orders
          </Text>
          <RecentOrdersTable />
        </Box>
      </SimpleGrid>
    </Box>
  )
}

export default Dashboard
