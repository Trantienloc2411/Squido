using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SharedViewModal.ViewModels;
using WebApplication1.Services.Interfaces;

namespace WebApplication1.Controllers;

[ApiController]
public class BookController(IBookService bookService) : ControllerBase
{

    [HttpGet("api/Book")]
    [Authorize]
    public async Task<IActionResult> GetBooks(string? keyword, [FromQuery] int? page = null, [FromQuery] int pageSize = 10)
    {
        try
        {
            if (page.HasValue)
            {
                if (page < 1) page = 1;
                if (pageSize < 1) pageSize = 10;

                // Get filtered books by keyword
                var filteredBooks = await bookService.GetBooks(keyword);
                var booksCount = filteredBooks.Count;
                var pageCount = (int)Math.Ceiling((double)booksCount / pageSize);

                // Get paginated version of filtered books
                var paginatedBooks = await bookService.GetBooksPaginated(page.Value, pageSize, keyword);

                var result = new PaginationViewModel<BookViewModel>
                {
                    CurrentPage = page.Value,
                    PageCount = pageCount,
                    Data = (List<BookViewModel>)paginatedBooks,
                    TotalCount = booksCount,
                };

                return Ok(result);
            }
            else
            {
                // No pagination, just filtered books
                var allBooks = await bookService.GetBooks(keyword);
                return Ok(allBooks);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }


    [HttpGet("api/Book/{id}")]
    public async Task<IActionResult> GetBook([FromRoute(Name = "id")] string bookId)
    {
        try
        {
            var book = await bookService.GetBookById(bookId);
            var bookRelated = await bookService.GetBookByAuthorId(book.AuthorId.ToString(), bookId);

            var details = new ViewBookDetailViewModel
            {
                Book =
                {
                    AuthorName = book.Author.FullName,
                    Title = book.Title,
                    BookId = book.BookId,
                    BuyCount = book.BuyCount,
                    CategoryName = book.Category.Name,
                    CreatedDate = book.CreatedDate,
                    Price = book.Price,
                    Quantity = book.Quantity,
                    UpdatedDate = (DateTime)book.UpdatedDate,
                },
                Category =
                {
                    CategoryId = book.CategoryId,
                    Description = book.Description,
                    Name = book.Category.Name
                },
                Bio = book.Author.Bio,
                ImageUrl = "",
                RatingValueAverage = 4.5,
                BookDescription = book.Description,
                BookRelated = (List<BookViewModel>)bookRelated,
            };

            return Ok(details);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    [HttpGet("api/Book/Filter/{categoryId?}")]
    public async Task<IActionResult> GetBooksByCategory(
        [FromRoute(Name = "categoryId")] string? categoryIds,
        [FromQuery] int? page = null,
        [FromQuery] int pageSize = 9)
    {
        try
        {
            List<int> ids = new();

            if (!string.IsNullOrWhiteSpace(categoryIds))
            {
                ids = categoryIds.Split(',').Select(int.Parse).ToList();
            }

            var filteredBooks = string.IsNullOrWhiteSpace(categoryIds)
                ? await bookService.GetBooks() // <-- you need this method in your service
                : await bookService.GetBooksByCategoryId(ids);

            if (page.HasValue)
            {
                if (page < 1) page = 1;
                if (pageSize < 1) pageSize = 10;

                var booksCount = filteredBooks.Count;
                var pageCount = (int)Math.Ceiling((double)booksCount / pageSize);

                var paginatedBooks = filteredBooks
                    .Skip((page.Value - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                var result = new PaginationViewModel<BookViewModel>
                {
                    CurrentPage = page.Value,
                    PageCount = pageCount,
                    Data = paginatedBooks,
                    TotalCount = booksCount
                };

                return Ok(result);
            }

            return Ok(filteredBooks);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }


}