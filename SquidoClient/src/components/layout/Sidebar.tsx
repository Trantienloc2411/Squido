import type React from "react"
import { Box, Flex, VStack, Text, Icon, useColorModeValue, Divider } from "@chakra-ui/react"
import { NavLink } from "react-router-dom"
import { FiHome, FiBook, FiUsers, FiShoppingCart, FiBarChart2, FiSettings, FiHelpCircle, FiTag } from "react-icons/fi"

interface SidebarProps {
  isOpen: boolean
}

interface NavItemProps {
  icon: React.ElementType
  children: React.ReactNode
  to: string
}

const NavItem: React.FC<NavItemProps> = ({ icon, children, to }) => {
  const activeBg = useColorModeValue("brand.50", "whiteAlpha.200")
  const hoverBg = useColorModeValue("gray.100", "whiteAlpha.100")

  return (
    <Box
      as={NavLink}
      to={to}
      w="full"
      borderRadius="md"
      _activeLink={{
        bg: activeBg,
        color: "brand.500",
        fontWeight: "semibold",
      }}
      _hover={{
        bg: hoverBg,
        textDecoration: "none",
      }}
    >
      <Flex align="center" p={3}>
        <Icon as={icon} fontSize="lg" mr={3} />
        <Text fontSize="sm">{children}</Text>
      </Flex>
    </Box>
  )
}

const Sidebar: React.FC<SidebarProps> = ({ isOpen }) => {
  const bgColor = useColorModeValue("white", "gray.800")
  const borderColor = useColorModeValue("gray.200", "gray.700")

  return (
    <Box
      position="fixed"
      left={0}
      w={{ base: "full", md: "250px" }}
      h="calc(100vh - 60px)"
      bg={bgColor}
      borderRight="1px"
      borderColor={borderColor}
      transform={{
        base: isOpen ? "translateX(0)" : "translateX(-100%)",
        md: "translateX(0)",
      }}
      transition="transform 0.3s ease"
      zIndex={9}
      overflowY="auto"
      display={isOpen ? "block" : { base: "none", md: "block" }}
    >
      <VStack align="stretch" spacing={1} p={3}>
        <Text px={3} py={2} fontSize="xs" fontWeight="bold" color="gray.500">
          MAIN
        </Text>
        <NavItem icon={FiHome} to="/dashboard">
          Dashboard
        </NavItem>
        <NavItem icon={FiBook} to="/books">
          Books
        </NavItem>
        <NavItem icon={FiTag} to="/categories">
          Categories
        </NavItem>
        <NavItem icon={FiUsers} to="/users">
          Users
        </NavItem>
        <NavItem icon={FiShoppingCart} to="/orders">
          Orders
        </NavItem>

        <Divider my={3} />

        <Text px={3} py={2} fontSize="xs" fontWeight="bold" color="gray.500">
          REPORTS
        </Text>
        <NavItem icon={FiBarChart2} to="/reports/sales">
          Sales
        </NavItem>
        <NavItem icon={FiBarChart2} to="/reports/inventory">
          Inventory
        </NavItem>

        <Divider my={3} />

        <Text px={3} py={2} fontSize="xs" fontWeight="bold" color="gray.500">
          SETTINGS
        </Text>
        <NavItem icon={FiSettings} to="/settings">
          General
        </NavItem>
        <NavItem icon={FiHelpCircle} to="/help">
          Help
        </NavItem>
      </VStack>
    </Box>
  )
}

export default Sidebar
