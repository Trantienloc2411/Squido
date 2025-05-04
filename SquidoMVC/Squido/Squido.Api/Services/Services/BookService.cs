using AutoMapper;
using SharedViewModal.ViewModels;
using WebApplication1.Models.Entities;
using WebApplication1.DAOs.Interfaces;
using WebApplication1.Services.Interfaces;

namespace WebApplication1.Services.Services;

public class BookService(IUnitOfWork unitOfWork, IMapper mapper) : IBookService
{
    public async Task<ICollection<BookViewModel>> GetBooks(string? keyword = null)
    {
        var bookList = await unitOfWork.BookRepository.GetAllWithIncludeAsync(
            p => p.IsDeleted == false,
            p => p.Category,
            p => p.Author,
            p => p.ImageBooks);

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            keyword = keyword.ToLower();
            bookList = bookList.Where(b =>
                (!string.IsNullOrEmpty(b.Title) && b.Title.ToLower().Contains(keyword))).ToList();
        }

        var bookReturn = mapper.Map<ICollection<BookViewModel>>(bookList);
        
        return bookReturn;
    }



    public async Task<ICollection<BookViewModel>> GetBooksPaginated(int currentPage, int pageSize = 10, string? keyword = null)
    {
        var data = await GetBooks(keyword); // Pass the keyword here
        return data.Skip((currentPage - 1) * pageSize).Take(pageSize).ToList();
    }


    public async Task<Book> GetBookById(string id)
    {
        try
        {
            
            var book = await unitOfWork.BookRepository.GetSingleWithIncludeAsync(t => t.BookId.ToLower() == id.ToLower(), t => t.Category,
                t => t.Author, t => t.ImageBooks);
            return book;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
    

    public async Task<ICollection<BookViewModel>> GetBookByAuthorId(string authorId, string? currentBook)
    {
        try
        {
            if (!Guid.TryParse(authorId, out var parsedAuthorId))
            {
                throw new ArgumentException("Invalid authorId format");
            }

            // Fetch books
            var booksList = await unitOfWork.BookRepository.GetAllWithIncludeAsync(
                b => b.AuthorId == parsedAuthorId && b.IsDeleted == false,
                b => b.Author,
                b => b.Category,
                b => b.ImageBooks
            );

            // If there's a currentBook, exclude it
            if (!string.IsNullOrEmpty(currentBook))
            {
                var toExclude = booksList.FirstOrDefault(c => c.BookId == currentBook);
                if (toExclude != null)
                {
                    booksList = booksList.Except(new[] { toExclude }).ToList();
                }
            }

            // Map and return
            var bookReturn = mapper.Map<ICollection<BookViewModel>>(booksList);
            return bookReturn;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task<ICollection<BookViewModel>> GetBooksByCategoryId(List<int> categoryIds)
    {
        try
        {
            var books = await unitOfWork.BookRepository.GetAllWithIncludeAsync(
                c => c.IsDeleted == false,
                c => c.Category,
                c => c.Author,
                c => c.ImageBooks);
            var result = books.Where(
                b => categoryIds.Contains(b.CategoryId)).ToList();
            var bookReturn = mapper.Map<ICollection<BookViewModel>>(result);
            return bookReturn;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task<ResponseMessage<BookViewModel>> CreateBook(CreateBookViewModel bookViewModel)
    {
        try
        {
            var book = mapper.Map<Book>(bookViewModel);
            book.BookId = GenerateBookId();
            book.CreatedDate = DateTime.Now;
            book.IsDeleted = false;
            await unitOfWork.BookRepository.AddAsync(book);

            List<ImageBook> imageBooks = new List<ImageBook>();
            foreach (var image in bookViewModel.ImageUrls)
            {
                var imageBook = new ImageBook
                {
                    BookId = book.BookId,
                    UrlImage = image,
                };
                imageBooks.Add(imageBook);
            }
            unitOfWork.ImageBookRepository.AddRange(imageBooks);
            unitOfWork.Save();
            var result = mapper.Map<BookViewModel>(book);
            return new ResponseMessage<BookViewModel>
            {
                IsSuccess = true,
                Message = "Create Book Success",
                Data = result
            };
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return new ResponseMessage<BookViewModel>
            {
                IsSuccess = false,
                Message = "Create Book Failed",
                Data = null
            };
        }
    }

    public Task<ResponseMessage<BookViewModel>> UpdateBook(string id, BookViewModel bookViewModel)
    {
        throw new NotImplementedException();
    }

    public Task<ResponseMessage<BookViewModel>> DeleteBook(string id)
    {
        throw new NotImplementedException();
    }

    private static string GenerateBookId()
    {
        var bookId = Guid.NewGuid().ToString();
        return bookId;
    }
}