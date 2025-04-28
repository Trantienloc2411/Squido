 

import type React from "react"

import { useState, useEffect } from "react"
import { useNavigate } from "react-router-dom"
import { useAppDispatch, useAppSelector } from "../../redux/hooks"
import { addProduct, updateProduct } from "../../redux/slices/productsSlice"
import { fetchCategories } from "../../redux/slices/categoriesSlice"
import { Button } from "../ui/button"
import { Input } from "../ui/input"
import { Label } from "../ui/label"
import { Textarea } from "../ui/textarea"
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "../ui/select"
import { toast } from "../ui/use-toast"
import { X, Upload } from "lucide-react"

interface ProductFormProps {
  product?: any
}

function ProductForm({ product }: ProductFormProps) {
  const navigate = useNavigate()
  const dispatch = useAppDispatch()
  const { categories } = useAppSelector((state) => state.categories)
  const [isSubmitting, setIsSubmitting] = useState(false)
  const [formData, setFormData] = useState({
    id: "",
    name: "",
    category: "",
    description: "",
    price: "",
    images: [] as string[],
    createdDate: "",
    updatedDate: "",
  })

  useEffect(() => {
    dispatch(fetchCategories())

    if (product) {
      setFormData({
        id: product.id,
        name: product.name,
        category: product.category,
        description: product.description,
        price: product.price.toString(),
        images: product.images || [],
        createdDate: product.createdDate,
        updatedDate: new Date().toISOString(),
      })
    } else {
      setFormData({
        ...formData,
        createdDate: new Date().toISOString(),
        updatedDate: new Date().toISOString(),
      })
    }
  }, [dispatch, product])

  const handleChange = (e: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement>) => {
    const { name, value } = e.target
    setFormData((prev) => ({ ...prev, [name]: value }))
  }

  const handleSelectChange = (name: string, value: string) => {
    setFormData((prev) => ({ ...prev, [name]: value }))
  }

  const handleImageUpload = (e: React.ChangeEvent<HTMLInputElement>) => {
    if (e.target.files && e.target.files.length > 0) {
      const file = e.target.files[0]
      const reader = new FileReader()

      reader.onload = (event) => {
        if (event.target && event.target.result) {
          setFormData((prev) => ({
            ...prev,
            images: [...prev.images, event.target.result as string],
          }))
        }
      }

      reader.readAsDataURL(file)
    }
  }

  const handleRemoveImage = (index: number) => {
    setFormData((prev) => ({
      ...prev,
      images: prev.images.filter((_, i) => i !== index),
    }))
  }

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault()
    setIsSubmitting(true)

    try {
      const productData = {
        ...formData,
        price: Number.parseFloat(formData.price),
      }

      if (product) {
        await dispatch(updateProduct(productData)).unwrap()
        toast({
          title: "Product updated",
          description: "The product has been updated successfully.",
        })
      } else {
        await dispatch(addProduct(productData)).unwrap()
        toast({
          title: "Product added",
          description: "The product has been added successfully.",
        })
      }

      navigate("/products")
    } catch (error) {
      toast({
        title: "Error",
        description: `Failed to ${product ? "update" : "add"} product. Please try again.`,
        variant: "destructive",
      })
    } finally {
      setIsSubmitting(false)
    }
  }

  return (
    <form onSubmit={handleSubmit} className="space-y-6">
      <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
        <div className="space-y-2">
          <Label htmlFor="name">Product Name</Label>
          <Input id="name" name="name" value={formData.name} onChange={handleChange} required />
        </div>

        <div className="space-y-2">
          <Label htmlFor="category">Category</Label>
          <Select value={formData.category} onValueChange={(value) => handleSelectChange("category", value)}>
            <SelectTrigger>
              <SelectValue placeholder="Select a category" />
            </SelectTrigger>
            <SelectContent>
              {categories.map((category) => (
                <SelectItem key={category.id} value={category.name}>
                  {category.name}
                </SelectItem>
              ))}
            </SelectContent>
          </Select>
        </div>
      </div>

      <div className="space-y-2">
        <Label htmlFor="description">Description</Label>
        <Textarea id="description" name="description" value={formData.description} onChange={handleChange} rows={4} />
      </div>

      <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
        <div className="space-y-2">
          <Label htmlFor="price">Price ($)</Label>
          <Input
            id="price"
            name="price"
            type="number"
            step="0.01"
            min="0"
            value={formData.price}
            onChange={handleChange}
            required
          />
        </div>
      </div>

      <div className="space-y-2">
        <Label>Product Images</Label>
        <div className="grid grid-cols-2 md:grid-cols-4 gap-4">
          {formData.images.map((image, index) => (
            <div key={index} className="relative">
              <img
                src={image || "/placeholder.svg"}
                alt={`Product image ${index + 1}`}
                className="h-24 w-full object-cover rounded-md"
              />
              <Button
                type="button"
                variant="destructive"
                size="icon"
                className="absolute -top-2 -right-2 h-6 w-6 rounded-full"
                onClick={() => handleRemoveImage(index)}
              >
                <X className="h-3 w-3" />
              </Button>
            </div>
          ))}
          <div className="h-24 border-2 border-dashed rounded-md flex items-center justify-center">
            <label className="cursor-pointer flex flex-col items-center justify-center w-full h-full">
              <Upload className="h-6 w-6 text-muted-foreground" />
              <span className="text-xs text-muted-foreground mt-1">Upload</span>
              <input type="file" accept="image/*" className="hidden" onChange={handleImageUpload} />
            </label>
          </div>
        </div>
      </div>

      <div className="flex justify-end gap-4">
        <Button type="button" variant="outline" onClick={() => navigate("/products")}>
          Cancel
        </Button>
        <Button type="submit" disabled={isSubmitting}>
          {isSubmitting ? (product ? "Updating..." : "Adding...") : product ? "Update Product" : "Add Product"}
        </Button>
      </div>
    </form>
  )
}

export default ProductForm
