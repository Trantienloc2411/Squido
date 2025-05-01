"use client"

import type React from "react"
import {
  Box,
  Flex,
  IconButton,
  useColorMode,
  useColorModeValue,
  Text,
  HStack,
  Menu,
  MenuButton,
  MenuList,
  MenuItem,
  Avatar,
  Divider,
  Badge,
} from "@chakra-ui/react"
import { HamburgerIcon, MoonIcon, SunIcon, BellIcon, SettingsIcon } from "@chakra-ui/icons"
import { useDispatch, useSelector } from "react-redux"
import { useNavigate } from "react-router-dom"
import type { RootState } from "../../redux/store"
import { logout } from "../../redux/slices/authSlice"

interface HeaderProps {
  toggleSidebar: () => void
}

const Header: React.FC<HeaderProps> = ({ toggleSidebar }) => {
  const { colorMode, toggleColorMode } = useColorMode()
  const bgColor = useColorModeValue("white", "gray.800")
  const borderColor = useColorModeValue("gray.200", "gray.700")
  const dispatch = useDispatch()
  const navigate = useNavigate()
  const { user } = useSelector((state: RootState) => state.auth)

  const handleLogout = () => {
    dispatch(logout())
    navigate("/login")
  }

  return (
    <Box
      as="header"
      position="sticky"
      top={0}
      zIndex={10}
      bg={bgColor}
      borderBottom="1px"
      borderColor={borderColor}
      px={4}
      py={2}
      boxShadow="sm"
    >
      <Flex alignItems="center" justifyContent="space-between">
        <Flex alignItems="center">
          <IconButton
            aria-label="Toggle Sidebar"
            icon={<HamburgerIcon />}
            variant="ghost"
            onClick={toggleSidebar}
            mr={2}
          />
          <Text fontSize="xl" fontWeight="bold" color="brand.500">
            Squido Admin
          </Text>
        </Flex>

        <HStack spacing={3}>
          <IconButton
            aria-label="Toggle color mode"
            icon={colorMode === "light" ? <MoonIcon /> : <SunIcon />}
            onClick={toggleColorMode}
            variant="ghost"
          />

          <Menu>
            <MenuButton
              as={IconButton}
              aria-label="Notifications"
              icon={
                <Box position="relative">
                  <BellIcon />
                  <Badge position="absolute" top="-6px" right="-6px" colorScheme="red" borderRadius="full" size="xs">
                    3
                  </Badge>
                </Box>
              }
              variant="ghost"
            />
            <MenuList>
              <MenuItem>New Order #1234</MenuItem>
              <MenuItem>New User Registration</MenuItem>
              <MenuItem>Low Stock Alert</MenuItem>
            </MenuList>
          </Menu>

          <Menu>
            <MenuButton>
              <Avatar size="sm" name={user?.name || "Admin User"} src="" />
            </MenuButton>
            <MenuList>
              <Text px={3} py={2} fontWeight="medium">
                {user?.name || "Admin User"}
              </Text>
              <Text px={3} pb={2} fontSize="sm" color="gray.500">
                {user?.email || "admin@squido.com"}
              </Text>
              <Divider />
              <MenuItem icon={<SettingsIcon />}>Settings</MenuItem>
              <MenuItem onClick={handleLogout}>Logout</MenuItem>
            </MenuList>
          </Menu>
        </HStack>
      </Flex>
    </Box>
  )
}

export default Header
