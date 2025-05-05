// API Test Utility
export const testApiConnection = async () => {
    const API_URL = import.meta.env.REACT_APP_API_URL || "http://localhost:5083/api"
  
    try {
      console.log(`Testing API connection to: ${API_URL}`)
  
      // Try to fetch a simple endpoint with a timeout
      const controller = new AbortController()
      const timeoutId = setTimeout(() => controller.abort(), 10000) // 10 second timeout
  
      const response = await fetch(`${API_URL}/health`, {
        signal: controller.signal,
        method: "GET",
        headers: {
          Accept: "application/json",
        },
      })
  
      clearTimeout(timeoutId)
  
      console.log(`API Test Response Status: ${response.status} ${response.statusText}`)
  
      if (!response.ok) {
        const errorText = await response.text()
        console.error(`API test failed with status ${response.status}:`, errorText)
        return {
          success: false,
          status: response.status,
          message: `API returned status ${response.status}: ${errorText}`,
        }
      }
  
      // Try to parse the response
      try {
        const data = await response.json()
        console.log("API Test Response Data:", data)
        return {
          success: true,
          status: response.status,
          data,
        }
      } catch (e) {
        console.error("Error parsing API test response:", e)
        return {
          success: false,
          status: response.status,
          message: `Error parsing response: ${e}`,
        }
      }
    } catch (error) {
      // Check if it's an abort error (timeout)
      if (error.name === "AbortError") {
        console.error("API test request timed out")
        return {
          success: false,
          status: "timeout",
          message: "API request timed out after 10 seconds",
        }
      }
  
      console.error("API test error:", error)
      return {
        success: false,
        status: "error",
        message: error instanceof Error ? error.message : "Unknown error",
      }
    }
  }
  
  // Add a function to check if the API URL is valid
  export const validateApiUrl = (url: string): boolean => {
    try {
      new URL(url)
      return true
    } catch (e) {
      return false
    }
  }
  
  // Add a function to check the current API URL
  export const checkApiUrl = (): { isValid: boolean; url: string } => {
    const apiUrl = process.env.REACT_APP_API_URL || "http://localhost:5083/api"
    return {
      isValid: validateApiUrl(apiUrl),
      url: apiUrl,
    }
  }
  