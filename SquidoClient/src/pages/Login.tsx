"use client"

import type React from "react"
import { useState, useEffect } from "react"
import {
  Box,
  Button,
  Checkbox,
  Container,
  Flex,
  FormControl,
  FormLabel,
  Heading,
  Input,
  Stack,
  Text,
  useColorMode,
  useColorModeValue,
  FormErrorMessage,
  Icon,
} from "@chakra-ui/react"
import { FiMoon, FiSun, FiBook } from "react-icons/fi"
import { useDispatch } from "react-redux"
import { useNavigate } from "react-router-dom"
import { login } from "../redux/slices/authSlice"
import type { AppDispatch } from "../redux/store"

const Login: React.FC = () => {
  const { colorMode, toggleColorMode } = useColorMode()
  const dispatch = useDispatch<AppDispatch>()
  const navigate = useNavigate()

  useEffect(() => {
    // Automatically redirect to dashboard
    navigate("/dashboard")
  }, [navigate])

  const [email, setEmail] = useState("")
  const [password, setPassword] = useState("")
  const [errors, setErrors] = useState<{ email?: string; password?: string }>({})
  const [isLoading, setIsLoading] = useState(false)

  const validateForm = () => {
    const newErrors: { email?: string; password?: string } = {}

    if (!email) newErrors.email = "Email is required"
    if (!/^\S+@\S+\.\S+$/.test(email)) {
      newErrors.email = "Invalid email format"
    }

    if (!password) newErrors.password = "Password is required"
    if (password.length < 6) {
      newErrors.password = "Password must be at least 6 characters"
    }

    setErrors(newErrors)
    return Object.keys(newErrors).length === 0
  }

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault()

    if (!validateForm()) return

    setIsLoading(true)

    try {
      // In a real app, this would make an API call
      await dispatch(login({ email, password }))
      navigate("/dashboard")
    } catch (error) {
      console.error("Login failed:", error)
    } finally {
      setIsLoading(false)
    }
  }

  return (
    <Box minH="100vh" bg={useColorModeValue("gray.50", "gray.900")}>
      <Flex position="absolute" top={4} right={4}>
        <Button
          aria-label={`Switch to ${colorMode === "light" ? "dark" : "light"} mode`}
          variant="ghost"
          onClick={toggleColorMode}
        >
          {colorMode === "light" ? <FiMoon /> : <FiSun />}
        </Button>
      </Flex>

      <Container maxW="lg" py={{ base: "12", md: "24" }} px={{ base: "0", sm: "8" }}>
        <Stack spacing="8">
          <Stack spacing="6" align="center">
            <Icon as={FiBook} boxSize={12} color="brand.500" />
            <Stack spacing={{ base: "2", md: "3" }} textAlign="center">
              <Heading size={{ base: "md", md: "lg" }}>Squido Admin</Heading>
              <Text color="muted">Sign in to your account</Text>
            </Stack>
          </Stack>

          <Box
            py={{ base: "0", sm: "8" }}
            px={{ base: "4", sm: "10" }}
            bg={useColorModeValue("white", "gray.800")}
            boxShadow={{ base: "none", sm: "md" }}
            borderRadius={{ base: "none", sm: "xl" }}
          >
            <form onSubmit={handleSubmit}>
              <Stack spacing="6">
                <Stack spacing="5">
                  <FormControl isInvalid={!!errors.email}>
                    <FormLabel htmlFor="email">Email</FormLabel>
                    <Input id="email" type="email" value={email} onChange={(e) => setEmail(e.target.value)} />
                    <FormErrorMessage>{errors.email}</FormErrorMessage>
                  </FormControl>

                  <FormControl isInvalid={!!errors.password}>
                    <FormLabel htmlFor="password">Password</FormLabel>
                    <Input
                      id="password"
                      type="password"
                      value={password}
                      onChange={(e) => setPassword(e.target.value)}
                    />
                    <FormErrorMessage>{errors.password}</FormErrorMessage>
                  </FormControl>
                </Stack>

                <Stack spacing="6">
                  <Flex justify="space-between">
                    <Checkbox defaultChecked>Remember me</Checkbox>
                    <Button variant="link" colorScheme="blue" size="sm">
                      Forgot password?
                    </Button>
                  </Flex>

                  <Button type="submit" colorScheme="blue" isLoading={isLoading}>
                    Sign in
                  </Button>
                </Stack>
              </Stack>
            </form>
          </Box>
        </Stack>
      </Container>
    </Box>
  )
}

export default Login
