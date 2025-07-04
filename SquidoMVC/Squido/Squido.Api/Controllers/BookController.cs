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
                    Data = paginatedBooks.ToList(),
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

            if (book.AuthorId == Guid.Empty)
            {
                return BadRequest("Invalid author ID");
            }

            var bookRelated = await bookService.GetBookByAuthorId(book.AuthorId.ToString(), bookId);
            var listBookRelated = bookRelated.ToList();
            
            var details = mapper.Map<ViewBookDetailViewModel>(book);
            details.BookRelated = listBookRelated;
            details.RatingValueAverage = 4.5;

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
                ? await bookService.GetBooks()
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
            if (bookViewModel == null)
            {
                return BadRequest("Invalid book data");
            }

            var result = await bookService.CreateBook(bookViewModel);
            if (result.IsSuccess && result.Data != null)
            {
                return CreatedAtAction(nameof(GetBook), new { id = result.Data.Id }, result);
            }
            return BadRequest(result);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    [HttpPut("api/Book/{id}")]
    public async Task<IActionResult> UpdateBook([FromRoute(Name = "id")] string bookId, [FromBody] CreateBookViewModel bookViewModel)
    {
        try
        {
            if (bookViewModel == null)
            {
                return BadRequest("Invalid book data");
            }

            var result = await bookService.UpdateBook(bookId, bookViewModel);
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    [HttpDelete("api/Book/{id}")]
    public async Task<IActionResult> DeleteBook([FromRoute(Name = "id")] string id)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return BadRequest("Invalid book ID");
            }

            var result = await bookService.DeleteBook(id);
            if (result.IsSuccess) return Ok(result.IsSuccess);
            else return BadRequest(result.Message);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }        
    }

    

}