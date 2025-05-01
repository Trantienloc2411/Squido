import type React from "react"
import { Box, Flex, Text, Progress, useColorModeValue, Image } from "@chakra-ui/react"

const books = [
  {
    id: 1,
    title: "The Great Gatsby",
    author: "F. Scott Fitzgerald",
    sales: 120,
    image: "/open-book-library.png",
  },
  {
    id: 2,
    title: "To Kill a Mockingbird",
    author: "Harper Lee",
    sales: 95,
    image: "/open-book-library.png",
  },
  {
    id: 3,
    title: "1984",
    author: "George Orwell",
    sales: 85,
    image: "/open-book-library.png",
  },
  {
    id: 4,
    title: "The Hobbit",
    author: "J.R.R. Tolkien",
    sales: 75,
    image: "/open-book-library.png",
  },
  {
    id: 5,
    title: "Pride and Prejudice",
    author: "Jane Austen",
    sales: 65,
    image: "/open-book-library.png",
  },
]

const maxSales = Math.max(...books.map((book) => book.sales))

const TopSellingBooks: React.FC = () => {
  const progressColorScheme = useColorModeValue("brand", "brand")

  return (
    <Box>
      {books.map((book) => (
        <Flex key={book.id} mb={4} align="center">
          <Image
            src={book.image || "/placeholder.svg"}
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
              <Text fontWeight="bold">{book.sales}</Text>
            </Flex>
            <Text fontSize="sm" color="gray.500" mb={1} noOfLines={1}>
              {book.author}
            </Text>
            <Progress
              value={(book.sales / maxSales) * 100}
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
