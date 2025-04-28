import React from "react"
import ReactDOM from "react-dom/client"
import { BrowserRouter } from "react-router-dom"
import App from "./App"
import { ThemeProvider } from "./components/theme-provider"
import { Providers } from "./redux/provider"
import "./index.css"

ReactDOM.createRoot(document.getElementById("root")!).render(
  <React.StrictMode>
    <BrowserRouter>
      <ThemeProvider defaultTheme="light" storageKey="edox-theme">
        <Providers>
          <App />
        </Providers>
      </ThemeProvider>
    </BrowserRouter>
  </React.StrictMode>,
)
