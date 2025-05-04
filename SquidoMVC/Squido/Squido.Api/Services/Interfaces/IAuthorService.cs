namespace WebApplication1.Services.Interfaces;

using Microsoft.AspNetCore.Mvc;
using SharedViewModal.ViewModels;
using WebApplication1.Models.Entities;
public interface IAuthorService
{
    Task<List<AuthorViewModel>> GetAuthors(string? keyword = null,int currentPage = 1, int pageSize = 10);
    Task<AuthorViewModel> GetAuthorById(string id);
    Task<ResponseMessage<AuthorViewModel>> CreateAuthor(Author author);
    Task<ResponseMessage<AuthorViewModel>> UpdateAuthor(string id, Author author);
    Task<ResponseMessage<AuthorViewModel>> DeleteAuthor(string id);
}