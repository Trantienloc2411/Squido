import type React from "react"
import { Box, Flex, Text, Progress, useColorModeValue, Image, Skeleton, SkeletonText } from "@chakra-ui/react"

interface TopBook {
  bookId: string
  title: string
  categoryName: string
  authorName: string | null
  quantity: number
  price: number
  buyCount: number
  imageUrls: string[]
  createdDate: string
  updatedDate: string | null
}

interface TopSellingBooksProps {
  books?: TopBook[]
  isLoading?: boolean
}

const TopSellingBooks: React.FC<TopSellingBooksProps> = ({ books = [], isLoading = false }) => {
  const progressColorScheme = useColorModeValue("brand", "brand")
  const maxQuantity = books.length > 0 ? Math.max(...books.map((book) => book.quantity)) : 1

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
      {books.map((book) => (
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

export default TopSellingBooks
