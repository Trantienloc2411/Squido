using Microsoft.AspNetCore.Mvc;
using SharedViewModal.ViewModels;
using WebApplication1.Services.Interfaces;

namespace WebApplication1.Controllers;
[ApiController]
[Route("api/[controller]")]
public class AuthorController(IAuthorService authorService) : ControllerBase
{

    [HttpGet]
    public async Task<ResponseMessage<List<AuthorViewModel>>> GetAllAuthors(string? keyword = null, int page = 1, int pageSize = 10)
    {
        try
        {
            var authors = await authorService.GetAuthors(keyword, page, pageSize);
            return new ResponseMessage<List<AuthorViewModel>>()
            {
                Data = authors,
                IsSuccess = true,
                Message = "Success"
            };
        }
        catch (System.Exception)
        {
            
            throw;
        }
        
    }
    
    
}