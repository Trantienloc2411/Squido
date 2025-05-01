import { extendTheme, type ThemeConfig } from "@chakra-ui/react"

const config: ThemeConfig = {
  initialColorMode: "light",
  useSystemColorMode: false,
}

const colors = {
  brand: {
    50: "#e6f1ff",
    100: "#cce3ff",
    200: "#99c7ff",
    300: "#66aaff",
    400: "#338eff",
    500: "#0072ff", // Primary brand color
    600: "#005bcc",
    700: "#004499",
    800: "#002e66",
    900: "#001733",
  },
}

const theme = extendTheme({
  config,
  colors,
  fonts: {
    heading: `'Inter', sans-serif`,
    body: `'Inter', sans-serif`,
  },
  components: {
    Button: {
      baseStyle: {
        fontWeight: "semibold",
      },
      variants: {
        primary: {
          bg: "brand.500",
          color: "white",
          _hover: {
            bg: "brand.600",
          },
        },
      },
    },
  },
})

export default theme
