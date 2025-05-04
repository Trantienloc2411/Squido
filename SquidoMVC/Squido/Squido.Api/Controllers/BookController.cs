using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SharedViewModal.ViewModels;
using WebApplication1.Services.Interfaces;

namespace WebApplication1.Controllers;

[ApiController]
public class BookController(IBookService bookService, IMapper mapper) : ControllerBase
{

    [HttpGet("api/Book")]
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
            if (book == null)
            {
                return NotFound();
            }
            var bookRelated = await bookService.GetBookByAuthorId(book.AuthorId.ToString(), bookId);
            var listBookRelated = bookRelated.ToList();
            var bookViewModel = new BookViewModel();
            var categoryViewModel = new CategoryViewModel();
            
            var bookResult = mapper.Map(book, bookViewModel);
            var categoryResult = mapper.Map(book.Category, categoryViewModel);
            var details = new ViewBookDetailViewModel
            {
                Book = bookResult,
                Category = categoryResult,
                Bio = book.Author.Bio,
                RatingValueAverage = 4.5,
                BookDescription = book.Description,
                BookRelated = listBookRelated,
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


    [HttpPost("api/Book")]
    public async Task<IActionResult> CreateBook([FromBody] CreateBookViewModel bookViewModel)
    {
        try
        {
            var result = await bookService.CreateBook(bookViewModel);
            if (result.IsSuccess)
            {
                return CreatedAtAction(nameof(GetBook), new { id = result!.Data!.BookId }, result);
            }
            return BadRequest(result);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    

}