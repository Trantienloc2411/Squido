using AutoMapper;
using SharedViewModal.ViewModels;
using WebApplication1.DAOs.Interfaces;
using WebApplication1.Models.Entities;
using WebApplication1.Services.Interfaces;

namespace WebApplication1.Services.Services;

public class AuthorService(IUnitOfWork unitOfWork, IMapper mapper) : IAuthorService
{
    public Task<ResponseMessage<AuthorViewModel>> CreateAuthor(Author author)
    {
        throw new NotImplementedException();
    }

    public Task<ResponseMessage<AuthorViewModel>> DeleteAuthor(string id)
    {
        throw new NotImplementedException();
    }

    public Task<AuthorViewModel> GetAuthorById(string id)
    {
        throw new NotImplementedException();
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


    public Task<ResponseMessage<AuthorViewModel>> UpdateAuthor(string id, Author author)
    {
        throw new NotImplementedException();
    }
}