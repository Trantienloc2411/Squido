"use client"

import type React from "react"
import { useEffect } from "react"
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
} from "@chakra-ui/react"
import { FiEdit, FiArrowLeft } from "react-icons/fi"
import { useNavigate, useParams } from "react-router-dom"
import { useDispatch, useSelector } from "react-redux"
import type { AppDispatch, RootState } from "../../redux/store"
import { fetchBookById } from "../../redux/slices/bookSlice"

const BookDetail: React.FC = () => {
  const { id } = useParams<{ id: string }>()
  const navigate = useNavigate()
  const dispatch = useDispatch<AppDispatch>()
  const { currentBook, loading } = useSelector((state: RootState) => state.books)
  const bgColor = useColorModeValue("white", "gray.800")

  useEffect(() => {
    if (id) {
      dispatch(fetchBookById(id))
    }
  }, [dispatch, id])

  if (loading || !currentBook) {
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

  return (
    <Box>
      <Flex align="center" justify="space-between" mb={6}>
        <Flex align="center">
          <Button leftIcon={<FiArrowLeft />} variant="ghost" onClick={() => navigate("/books")} mr={4}>
            Back
          </Button>
          <Heading size="lg">{currentBook.title}</Heading>
        </Flex>
        <Button leftIcon={<FiEdit />} onClick={() => navigate(`/books/edit/${id}`)}>
          Edit
        </Button>
      </Flex>

      <Box bg={bgColor} p={6} rounded="lg" boxShadow="sm">
        <SimpleGrid columns={{ base: 1, md: 2 }} spacing={8}>
          <Box>
            <Image
              src={currentBook.coverImage || "/placeholder.svg?height=400&width=300&query=book"}
              alt={currentBook.title}
              maxH="400px"
              objectFit="contain"
              borderRadius="md"
              mx="auto"
            />
          </Box>

          <Box>
            <Heading size="md" mb={2}>
              {currentBook.title}
            </Heading>
            <Text color="gray.500" mb={4}>
              by {currentBook.author}
            </Text>

            <Badge colorScheme="blue" mb={4}>
              {currentBook.category}
            </Badge>

            <Text mb={4}>{currentBook.description}</Text>

            <Divider my={4} />

            <SimpleGrid columns={2} spacing={4} mb={4}>
              <Stat>
                <StatLabel>Price</StatLabel>
                <StatNumber>${currentBook.price.toFixed(2)}</StatNumber>
              </Stat>

              <Stat>
                <StatLabel>Stock</StatLabel>
                <StatNumber>{currentBook.stock}</StatNumber>
                <StatHelpText>
                  <Badge colorScheme={currentBook.stock > 0 ? "green" : "red"} variant="subtle">
                    {currentBook.stock > 0 ? "In Stock" : "Out of Stock"}
                  </Badge>
                </StatHelpText>
              </Stat>
            </SimpleGrid>

            <SimpleGrid columns={2} spacing={4}>
              <Box>
                <Text fontWeight="bold">ISBN:</Text>
                <Text>{currentBook.isbn}</Text>
              </Box>

              <Box>
                <Text fontWeight="bold">Publisher:</Text>
                <Text>{currentBook.publisher}</Text>
              </Box>

              <Box>
                <Text fontWeight="bold">Published Date:</Text>
                <Text>{currentBook.publishedDate}</Text>
              </Box>

              <Box>
                <Text fontWeight="bold">Language:</Text>
                <Text>{currentBook.language}</Text>
              </Box>

              <Box>
                <Text fontWeight="bold">Pages:</Text>
                <Text>{currentBook.pages}</Text>
              </Box>
            </SimpleGrid>
          </Box>
        </SimpleGrid>
      </Box>
    </Box>
  )
}

export default BookDetail
