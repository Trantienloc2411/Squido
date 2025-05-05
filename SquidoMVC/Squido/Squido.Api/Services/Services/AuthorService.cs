using AutoMapper;
using SharedViewModal.ViewModels;
using WebApplication1.DAOs.Interfaces;
using WebApplication1.Models.Entities;
using WebApplication1.Services.Interfaces;

namespace WebApplication1.Services.Services;

public class AuthorService(IUnitOfWork unitOfWork, IMapper mapper) : IAuthorService
{
    public async Task<ResponseMessage<AuthorViewModel>> CreateAuthor(Author author)
    {
        try
        {
            await unitOfWork.AuthorRepository.AddAsync(author);
            unitOfWork.Save();
            return new ResponseMessage<AuthorViewModel>()
            {
                IsSuccess = true,
                Data = mapper.Map<AuthorViewModel>(author),

            };
        }   
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task<ResponseMessage<AuthorViewModel>> DeleteAuthor(string id)
    {
        try
        {
            var author = await
                unitOfWork.AuthorRepository.GetSingleWithIncludeAsync(t => t.Id.ToString().ToLower() == id.ToLower());
            if(author != null) author.IsDeleted = true;
            unitOfWork.Save();
            return new ResponseMessage<AuthorViewModel>()
            {
                IsSuccess = true,
            };
        }
        catch (Exception e)
        {
            return new ResponseMessage<AuthorViewModel>()
            {
                IsSuccess = false,
                Message = e.Message,
                Data = null,
            };
        }
    }

    public async Task<ResponseMessage<AuthorViewModel>> GetAuthorById(string id)
    {
        try
        {
            var author = await unitOfWork.AuthorRepository.GetSingleWithIncludeAsync(t => t.Id.ToString().ToLower() == id.ToLower());
            if (author != null)
                return new ResponseMessage<AuthorViewModel>()
                {
                    IsSuccess = true,
                    Data = mapper.Map<AuthorViewModel>(author),
                };
            else
                return new ResponseMessage<AuthorViewModel>()
                {
                    IsSuccess = false,
                    Data = null,

                };
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task<List<AuthorViewModel>> GetAuthors(string? keyword = null, int currentPage = 1, int pageSize = 10)
    {
        try
        {
            var authorList = await unitOfWork.AuthorRepository.GetAllWithIncludeAsync(
            p => p.IsDeleted == false);
            authorList = [.. authorList];

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                keyword = keyword.ToLower();
                authorList = [.. authorList.Where(b =>
                    !string.IsNullOrEmpty(b.FullName) && b.FullName.ToLower().Contains(keyword))];
            }

            var authorReturn = mapper.Map<ICollection<AuthorViewModel>>(authorList);
            return [.. authorReturn.Skip((currentPage - 1) * pageSize).Take(pageSize)];
        }
        catch (System.Exception)
        {

            throw;
        }
    }


    public async Task<ResponseMessage<AuthorViewModel>> UpdateAuthor(string id, Author author)
    {
        try
        {
            var authorOld = await unitOfWork.AuthorRepository.GetSingleWithIncludeAsync(t => t.Id.ToString().ToLower() == id.ToLower());
            if (authorOld == null)
            {
                return new ResponseMessage<AuthorViewModel>
                {
                    IsSuccess = false,
                    Message = "Author not found"
                };
            }

            var idAuthor = authorOld.Id;
            var newAuthor = mapper.Map(author, authorOld); // map new data onto old entity
            newAuthor.Id = idAuthor;
            await unitOfWork.AuthorRepository.UpdateAsync(newAuthor);
            unitOfWork.Save();

            return new ResponseMessage<AuthorViewModel>
            {
                IsSuccess = true,
                Data = mapper.Map<AuthorViewModel>(newAuthor),
                Message = "Author updated successfully"
            };
        }
        catch (Exception e)
        {
            return new ResponseMessage<AuthorViewModel>
            {
                IsSuccess = false,
                Message = e.Message
            };
        }
    }

}