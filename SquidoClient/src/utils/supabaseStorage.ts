import { createClient } from "@supabase/supabase-js"

// Initialize Supabase client
const supabaseUrl = import.meta.env.VITE_API_SUPABASE_URL || ""
const supabaseKey = import.meta.env.VITE_APP_SUPABASE_ANON_KEY || ""
const supabase = createClient(supabaseUrl, supabaseKey)

// Maximum file size (200KB)
const MAX_FILE_SIZE = 200 * 1024 // 200KB in bytes

/**
 * Validates an image file for upload
 * @param file The file to validate
 * @returns An error message if validation fails, null if validation passes
 */
export const validateImage = (file: File): string | null => {
  // Check file size
  if (file.size > MAX_FILE_SIZE) {
    return `File size exceeds the maximum limit of 200KB. Current size: ${(file.size / 1024).toFixed(2)}KB`
  }

  // Check file type
  if (!file.type.includes("jpeg") && !file.type.includes("jpg")) {
    return "Only JPG/JPEG images are allowed"
  }

  return null
}

/**
 * Uploads an image to Supabase storage
 * @param file The file to upload
 * @param path The storage path (e.g., 'books')
 * @returns The URL of the uploaded image
 */
export const uploadImage = async (file: File, path = "books"): Promise<string> => {
  // Validate the image
  const validationError = validateImage(file)
  if (validationError) {
    throw new Error(validationError)
  }

  // Generate a unique filename
  const fileExt = file.name.split(".").pop()
  const fileName = `${Date.now()}-${Math.random().toString(36).substring(2, 15)}.${fileExt}`
  const filePath = `${path}/${fileName}`

  // Upload the file
  const { data, error } = await supabase.storage.from("bookimage").upload(filePath, file, {
    cacheControl: "3600",
    upsert: false,
  })

  if (error) {
    throw new Error(`Error uploading image: ${error.message}`)
  }

  // Get the public URL
  const {
    data: { publicUrl },
  } = supabase.storage.from("bookimage").getPublicUrl(data.path)

  return publicUrl
}
