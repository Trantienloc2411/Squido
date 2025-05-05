// API utility functions
const API_URL = import.meta.env.REACT_APP_API_URL || "http://localhost:5083/api"
export const fetchFromAPI = async (endpoint: string) => {
  try {
    console.log(`Fetching from API: ${API_URL}/${endpoint}`)

    // Add a timeout to the fetch request
    const controller = new AbortController()
    const timeoutId = setTimeout(() => controller.abort(), 30000) // 30 second timeout

    const response = await fetch(`${API_URL}/${endpoint}`, {
      signal: controller.signal,
      headers: {
        Accept: "application/json",
        "Content-Type": "application/json",
      },
    })

    clearTimeout(timeoutId)

    // Log the response status and headers for debugging
    console.log(`API Response Status: ${response.status} ${response.statusText}`)
    console.log("API Response Headers:", Object.fromEntries([...response.headers.entries()]))

    if (!response.ok) {
      let errorText
      try {
        errorText = await response.text()
        console.error(`API request failed with status ${response.status}:`, errorText)
      } catch (e) {
        errorText = `Could not read error response: ${e}`
        console.error(`API request failed with status ${response.status}, could not read error:`, e)
      }
      throw new Error(`API request failed with status ${response.status}: ${errorText}`)
    }

    // Try to parse the response as JSON
    try {
      const data = await response.json()
      console.log(`API Response Data for ${endpoint}:`, data)
      return data
    } catch (e) {
      console.error(`Error parsing JSON response from ${endpoint}:`, e)
      throw new Error(`Error parsing JSON response: ${e}`)
    }
  } catch (error) {
    // Check if it's an abort error (timeout)
    if (error.name === "AbortError") {
      console.error(`Request to ${endpoint} timed out`)
      throw new Error(`Request to ${endpoint} timed out after 30 seconds`)
    }

    console.error(`Error fetching from ${endpoint}:`, error)
    throw error
  }
}

// Helper function to handle the common response format
export const extractDataFromResponse = (response: any) => {
  if (response && response.isSuccess && response.data) {
    return response.data
  }
  throw new Error(response?.exceptionMessage || response?.message || "Failed to extract data from response")
}