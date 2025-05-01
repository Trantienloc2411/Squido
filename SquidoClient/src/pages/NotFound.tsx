"use client"

import type React from "react"
import { Box, Heading, Text, Button, VStack, useColorModeValue } from "@chakra-ui/react"
import { useNavigate } from "react-router-dom"

const NotFound: React.FC = () => {
  const navigate = useNavigate()

  return (
    <Box
      minH="100vh"
      display="flex"
      alignItems="center"
      justifyContent="center"
      bg={useColorModeValue("gray.50", "gray.900")}
    >
      <VStack spacing={6} textAlign="center" p={8}>
        <Heading size="4xl">404</Heading>
        <Heading size="xl">Page Not Found</Heading>
        <Text fontSize="lg" color={useColorModeValue("gray.600", "gray.400")}>
          The page you're looking for doesn't exist or has been moved.
        </Text>
        <Button colorScheme="blue" size="lg" onClick={() => navigate("/dashboard")}>
          Go to Dashboard
        </Button>
      </VStack>
    </Box>
  )
}

export default NotFound
