import type React from "react"
import { Box, useColorModeValue } from "@chakra-ui/react"
import { BarChart, Bar, XAxis, YAxis, CartesianGrid, Tooltip, ResponsiveContainer } from "recharts"

const data = [
  { name: "Jan", sales: 4000 },
  { name: "Feb", sales: 3000 },
  { name: "Mar", sales: 5000 },
  { name: "Apr", sales: 2780 },
  { name: "May", sales: 1890 },
  { name: "Jun", sales: 2390 },
  { name: "Jul", sales: 3490 },
  { name: "Aug", sales: 4000 },
  { name: "Sep", sales: 3000 },
  { name: "Oct", sales: 5000 },
  { name: "Nov", sales: 2780 },
  { name: "Dec", sales: 3890 },
]

const SalesChart: React.FC = () => {
  const barColor = useColorModeValue("brand.500", "brand.300")
  const textColor = useColorModeValue("gray.600", "gray.300")

  return (
    <Box h="300px" w="100%">
      <ResponsiveContainer width="100%" height="100%">
        <BarChart data={data} margin={{ top: 5, right: 30, left: 20, bottom: 5 }}>
          <CartesianGrid strokeDasharray="3 3" stroke={useColorModeValue("gray.200", "gray.700")} />
          <XAxis dataKey="name" tick={{ fill: textColor }} />
          <YAxis tick={{ fill: textColor }} />
          <Tooltip
            contentStyle={{
              backgroundColor: useColorModeValue("#fff", "#2D3748"),
              borderColor: useColorModeValue("gray.200", "gray.700"),
              color: textColor,
            }}
          />
          <Bar dataKey="sales" fill={barColor} radius={[4, 4, 0, 0]} />
        </BarChart>
      </ResponsiveContainer>
    </Box>
  )
}

export default SalesChart
