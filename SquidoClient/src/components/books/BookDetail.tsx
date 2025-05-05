"use client"

import type React from "react"
import { useEffect, useState } from "react"
import {
  Box,
  Heading,
  Text,
  Image,
  SimpleGrid,
  Stat,
  StatLabel,
  StatNumber,
  StatHelpText,
  Badge,
  Button,
  Flex,
  Divider,
  useColorModeValue,
  Skeleton,
  SkeletonText,
  HStack,
  Icon,
  Alert,
  AlertIcon,
  AlertTitle,
  AlertDescription,
} from "@chakra-ui/react"
import { FiEdit, FiArrowLeft, FiStar, FiRefreshCw } from "react-icons/fi"
import { useNavigate, useParams } from "react-router-dom"
import { useDispatch, useSelector } from "react-redux"
import type { AppDispatch, RootState } from "../../redux/store"
import { fetchBookById } from "../../redux/slices/bookSlice"

const BookDetail: React.FC = () => {
  const { id } = useParams<{ id: string }>()
  const navigate = useNavigate()
  const dispatch = useDispatch<AppDispatch>()
  const { bookDetail, loading, error } = useSelector((state: RootState) => state.books)
  const bgColor = useColorModeValue("white", "gray.800")
  const [loadAttempts, setLoadAttempts] = useState(0)

  useEffect(() => {
    if (id) {
      console.log(`Loading book with ID: ${id}, attempt: ${loadAttempts + 1}`)
      dispatch(fetchBookById(id))
    }
  }, [dispatch, id, loadAttempts])

  const handleRetry = () => {
    setLoadAttempts((prev) => prev + 1)
  }

  // Show loading state
  if (loading) {
    return (
      <Box>
        <Flex align="center" mb={6}>
          <Button leftIcon={<FiArrowLeft />} variant="ghost" onClick={() => navigate("/books")} mr={4}>
            Back
          </Button>
          <Skeleton height="30px" width="200px" />
        </Flex>

        <Box bg={bgColor} p={6} rounded="lg" boxShadow="sm">
          <SimpleGrid columns={{ base: 1, md: 2 }} spacing={8}>
            <Skeleton height="300px" />
            <Box>
              <SkeletonText mt="4" noOfLines={8} spacing="4" skeletonHeight="2" />
            </Box>
          </SimpleGrid>
        </Box>
      </Box>
    )
  }

  // Show error state
  if (error || !bookDetail) {
    return (
      <Box>
        <Flex align="center" mb={6}>
          <Button leftIcon={<FiArrowLeft />} variant="ghost" onClick={() => navigate("/books")} mr={4}>
            Back
          </Button>
          <Heading size="lg">Book Details</Heading>
        </Flex>

        <Alert
          status="error"
          variant="subtle"
          flexDirection="column"
          alignItems="center"
          justifyContent="center"
          textAlign="center"
          height="200px"
          bg={bgColor}
          rounded="lg"
          boxShadow="sm"
        >
          <AlertIcon boxSize="40px" mr={0} />
          <AlertTitle mt={4} mb={1} fontSize="lg">
            Failed to load book details
          </AlertTitle>
          <AlertDescription maxWidth="sm">{error || "There was an error loading the book details."}</AlertDescription>
          <Button leftIcon={<FiRefreshCw />} colorScheme="red" variant="outline" mt={4} onClick={handleRetry}>
            Retry
          </Button>
        </Alert>
      </Box>
    )
  }

  // Ensure book data exists
  if (!bookDetail.book) {
    return (
      <Box>
        <Flex align="center" mb={6}>
          <Button leftIcon={<FiArrowLeft />} variant="ghost" onClick={() => navigate("/books")} mr={4}>
            Back
          </Button>
          <Heading size="lg">Book Details</Heading>
        </Flex>

        <Alert
          status="warning"
          variant="subtle"
          flexDirection="column"
          alignItems="center"
          justifyContent="center"
          textAlign="center"
          height="200px"
          bg={bgColor}
          rounded="lg"
          boxShadow="sm"
        >
          <AlertIcon boxSize="40px" mr={0} />
          <AlertTitle mt={4} mb={1} fontSize="lg">
            Book Not Found
          </AlertTitle>
          <AlertDescription maxWidth="sm">
            The requested book could not be found or the data is incomplete.
          </AlertDescription>
          <Button
            leftIcon={<FiArrowLeft />}
            colorScheme="blue"
            variant="outline"
            mt={4}
            onClick={() => navigate("/books")}
          >
            Back to Books
          </Button>
        </Alert>
      </Box>
    )
  }

  const { book, category, bookDescription, bio, imageUrl, ratingValueAverage } = bookDetail

  return (
    <Box>
      <Flex align="center" justify="space-between" mb={6}>
        <Flex align="center">
          <Button leftIcon={<FiArrowLeft />} variant="ghost" onClick={() => navigate("/books")} mr={4}>
            Back
          </Button>
          <Heading size="lg">{book.title || "Untitled Book"}</Heading>
        </Flex>
        <Button leftIcon={<FiEdit />} onClick={() => navigate(`/books/edit/${id}`)}>
          Edit
        </Button>
      </Flex>

      <Box bg={bgColor} p={6} rounded="lg" boxShadow="sm" mb={6}>
        <SimpleGrid columns={{ base: 1, md: 2 }} spacing={8}>
          <Box>
            <Image
              src={imageUrl || book.imageUrl || "/placeholder.svg?height=400&width=300&query=book"}
              alt={book.title || "Book Cover"}
              maxH="400px"
              objectFit="contain"
              borderRadius="md"
              mx="auto"
            />
          </Box>

          <Box>
            <Heading size="md" mb={2}>
              {book.title || "Untitled Book"}
            </Heading>
            <Text color="gray.500" mb={4}>
              by {book.authorName || "Unknown Author"}
            </Text>

            <HStack mb={4}>
              {category && category.name && <Badge colorScheme="blue">{category.name}</Badge>}
              {typeof ratingValueAverage !== "undefined" && (
                <Flex align="center">
                  <Icon as={FiStar} color="yellow.400" mr={1} />
                  <Text fontWeight="bold">{ratingValueAverage}</Text>
                </Flex>
              )}
            </HStack>

            {bookDescription && <Text mb={4}>{bookDescription}</Text>}

            <Divider my={4} />

            <SimpleGrid columns={2} spacing={4} mb={4}>
              <Stat>
                <StatLabel>Price</StatLabel>
                <StatNumber>${typeof book.price === "number" ? book.price.toFixed(2) : "0.00"}</StatNumber>
              </Stat>

              <Stat>
                <StatLabel>Stock</StatLabel>
                <StatNumber>{typeof book.quantity === "number" ? book.quantity : 0}</StatNumber>
                <StatHelpText>
                  <Badge
                    colorScheme={typeof book.quantity === "number" && book.quantity > 0 ? "green" : "red"}
                    variant="subtle"
                  >
                    {typeof book.quantity === "number" && book.quantity > 0 ? "In Stock" : "Out of Stock"}
                  </Badge>
                </StatHelpText>
              </Stat>
            </SimpleGrid>

            <SimpleGrid columns={2} spacing={4}>
              <Box>
                <Text fontWeight="bold">Book ID:</Text>
                <Text>{book.id || "N/A"}</Text> {/* Changed from bookId to id */}
              </Box>

              <Box>
                <Text fontWeight="bold">Buy Count:</Text>
                <Text>{typeof book.buyCount === "number" ? book.buyCount : 0}</Text>
              </Box>

              <Box>
                <Text fontWeight="bold">Created Date:</Text>
                <Text>{book.createdDate ? new Date(book.createdDate).toLocaleDateString() : "N/A"}</Text>
              </Box>

              {book.updatedDate && (
                <Box>
                  <Text fontWeight="bold">Last Updated:</Text>
                  <Text>{new Date(book.updatedDate).toLocaleDateString()}</Text>
                </Box>
              )}
            </SimpleGrid>
          </Box>
        </SimpleGrid>
      </Box>

      {/* Author Bio Section */}
      {bio && (
        <Box bg={bgColor} p={6} rounded="lg" boxShadow="sm" mb={6}>
          <Heading size="md" mb={4}>
            About the Author
          </Heading>
          <Text>{bio}</Text>
        </Box>
      )}

      {/* Category Description */}
      {category && category.description && (
        <Box bg={bgColor} p={6} rounded="lg" boxShadow="sm" mb={6}>
          <Heading size="md" mb={4}>
            About {category.name} Books
          </Heading>
          <Text>{category.description}</Text>
        </Box>
      )}
    </Box>
  )
}

export default BookDetail
