using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SharedViewModal.ViewModels;
using WebApplication1.Models.Entities;
using WebApplication1.Services.Interfaces;

namespace WebApplication1.Controllers;
[ApiController]
[Route("api/[controller]")]
public class AuthorController(IAuthorService authorService, IMapper mapper) : ControllerBase
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

    [HttpGet("{id}")]
    public async Task<IActionResult> GetAuthor(Guid id)
    {
        try
        {
            var author = await authorService.GetAuthorById(id.ToString());
            if (author.IsSuccess == false)
            {
                return NotFound();
            }
            else
                return Ok(
                    new ResponseMessage<AuthorViewModel>()
                    {
                        Data = author.Data,
                        IsSuccess = true,
                    });

        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpPost]
    public async Task<IActionResult> CreateAuthor([FromBody] CreateAuthorViewModel authorRequest)
    {
        try
        {
            var newAuthor = new Author();
            var author = mapper.Map(authorRequest, newAuthor);
            var result = await authorService.CreateAuthor(author);
            if (result.IsSuccess == true)
            {
                return Ok(new
                {
                    Data = result,
                });
            }
            else
            {
                return BadRequest(result.Message);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateAuthor(Guid id, [FromBody] CreateAuthorViewModel authorRequest)
    {
        try
        {
            var result = await authorService.UpdateAuthor(id.ToString(), mapper.Map<Author>(authorRequest));
            if (result.IsSuccess == true)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(result.Message);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
    
    
}