
import type React from "react"

import { useEffect, useState } from "react"
import { useAppDispatch, useAppSelector } from "../../redux/hooks"
import { fetchCategories, deleteCategory, updateCategory } from "../../redux/slices/categoriesSlice"
import { Table, TableBody, PaginatedTable, TableCell, TableHead, TableHeader, TableRow } from "../ui/table"
import { Button } from "../ui/button"
import { DropdownMenu, DropdownMenuContent, DropdownMenuItem, DropdownMenuTrigger } from "../ui/dropdown-menu"
import {
  Dialog,
  DialogContent,
  DialogHeader,
  DialogTitle,
  DialogTrigger,
  DialogFooter,
  DialogClose,
} from "../ui/dialog"
import { Input } from "../ui/input"
import { Label } from "../ui/label"
import { Textarea } from "../ui/textarea"
import { Edit, MoreVertical, Trash } from "lucide-react"
import { Skeleton } from "../ui/skeleton"

function CategoryList() {
  const dispatch = useAppDispatch()
  const { categories, loading } = useAppSelector((state) => state.categories)
  const [editCategory, setEditCategory] = useState<any>(null)

  useEffect(() => {
    dispatch(fetchCategories())
  }, [dispatch])

  const handleDelete = (id: string) => {
    if (window.confirm("Are you sure you want to delete this category?")) {
      dispatch(deleteCategory(id))
    }
  }

  const handleUpdate = (e: React.FormEvent) => {
    e.preventDefault()
    if (editCategory) {
      dispatch(updateCategory(editCategory))
    }
  }

  if (loading) {
    return (
      <div>
        <div className="rounded-md border">
          <Table>
            <TableHeader>
              <TableRow>
                <TableHead>Name</TableHead>
                <TableHead>Description</TableHead>
                <TableHead className="w-[100px]">Actions</TableHead>
              </TableRow>
            </TableHeader>
            <TableBody>
              {[...Array(5)].map((_, i) => (
                <TableRow key={i}>
                  <TableCell>
                    <Skeleton className="h-4 w-32" />
                  </TableCell>
                  <TableCell>
                    <Skeleton className="h-4 w-full" />
                  </TableCell>
                  <TableCell>
                    <Skeleton className="h-8 w-8" />
                  </TableCell>
                </TableRow>
              ))}
            </TableBody>
          </Table>
        </div>
      </div>
    )
  }

  return (
    <div>
      <div className="rounded-md border">
        <PaginatedTable data={categories} itemsPerPage={5}>
          <TableHeader>
            <TableRow>
              <TableHead>Name</TableHead>
              <TableHead>Description</TableHead>
              <TableHead className="w-[100px]">Actions</TableHead>
            </TableRow>
          </TableHeader>
          <TableBody>
            {categories.map((category) => (
              <TableRow key={category.id}>
                <TableCell className="font-medium">{category.name}</TableCell>
                <TableCell>{category.description}</TableCell>
                <TableCell>
                  <DropdownMenu>
                    <DropdownMenuTrigger asChild>
                      <Button variant="ghost" size="icon">
                        <MoreVertical className="h-4 w-4" />
                      </Button>
                    </DropdownMenuTrigger>
                    <DropdownMenuContent align="end">
                      <Dialog>
                        <DialogTrigger asChild>
                          <DropdownMenuItem
                            onSelect={(e) => {
                              e.preventDefault()
                              setEditCategory(category)
                            }}
                          >
                            <Edit className="mr-2 h-4 w-4" />
                            Edit
                          </DropdownMenuItem>
                        </DialogTrigger>
                        <DialogContent>
                          <DialogHeader>
                            <DialogTitle>Edit Category</DialogTitle>
                          </DialogHeader>
                          <form onSubmit={handleUpdate}>
                            <div className="grid gap-4 py-4">
                              <div className="grid gap-2">
                                <Label htmlFor="name">Name</Label>
                                <Input
                                  id="name"
                                  value={editCategory?.name || ""}
                                  onChange={(e) =>
                                    setEditCategory({
                                      ...editCategory,
                                      name: e.target.value,
                                    })
                                  }
                                  required
                                />
                              </div>
                              <div className="grid gap-2">
                                <Label htmlFor="description">Description</Label>
                                <Textarea
                                  id="description"
                                  value={editCategory?.description || ""}
                                  onChange={(e) =>
                                    setEditCategory({
                                      ...editCategory,
                                      description: e.target.value,
                                    })
                                  }
                                  rows={3}
                                />
                              </div>
                            </div>
                            <DialogFooter>
                              <DialogClose asChild>
                                <Button type="button" variant="outline">
                                  Cancel
                                </Button>
                              </DialogClose>
                              <Button type="submit">Save Changes</Button>
                            </DialogFooter>
                          </form>
                        </DialogContent>
                      </Dialog>
                      <DropdownMenuItem onSelect={() => handleDelete(category.id)} className="text-destructive">
                        <Trash className="mr-2 h-4 w-4" />
                        Delete
                      </DropdownMenuItem>
                    </DropdownMenuContent>
                  </DropdownMenu>
                </TableCell>
              </TableRow>
            ))}
          </TableBody>
        </PaginatedTable>
      </div>
    </div>
  )
}

export default CategoryList
