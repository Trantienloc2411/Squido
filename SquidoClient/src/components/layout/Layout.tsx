"use client"

import React from "react"
import { Outlet } from "react-router-dom"
import { Box, Flex } from "@chakra-ui/react"
import Sidebar from "./Sidebar"
import Header from "./Header"

const Layout: React.FC = () => {
  const [isSidebarOpen, setSidebarOpen] = React.useState(true)

  const toggleSidebar = () => {
    setSidebarOpen(!isSidebarOpen)
  }

  return (
    <Flex h="100vh" flexDirection="column">
      <Header toggleSidebar={toggleSidebar} />
      <Flex flex="1" overflow="hidden">
        <Sidebar isOpen={isSidebarOpen} />
        <Box
          flex="1"
          p={4}
          overflowY="auto"
          transition="margin-left 0.3s"
          ml={isSidebarOpen ? { base: 0, md: "250px" } : 0}
        >
          <Outlet />
        </Box>
      </Flex>
    </Flex>
  )
}

export default Layout
