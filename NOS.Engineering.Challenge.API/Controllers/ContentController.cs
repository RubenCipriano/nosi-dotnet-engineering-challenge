using System.Net;
using Microsoft.AspNetCore.Mvc;
using NOS.Engineering.Challenge.API.Models;
using NOS.Engineering.Challenge.Managers;
using NOS.Engineering.Challenge.Models;
using NOS.Engineering.Challenge.Services;

namespace NOS.Engineering.Challenge.API.Controllers;

[Route("api/v1/[controller]")]
[ApiController]
public class ContentController : Controller
{
    private readonly IContentsManager _manager;
    private readonly ILogger<ContentController> _logger;
    private readonly ICacheService<Content> _cacheService;
    public ContentController(IContentsManager manager, ILogger<ContentController> logger, ICacheService<Content> cacheService)
    {
        _manager = manager;
        _logger = logger;
        _cacheService = cacheService;
    }

    [HttpGet]
    public async Task<IActionResult> GetManyContents()
    {
        _logger.LogInformation("[GET] api/v1/Content");

        var contents = await _manager.GetManyContents().ConfigureAwait(false);

        if (!contents.Any())
            return NotFound();

        foreach (Content? content in contents)
            if (content != null) 
                await _cacheService.Set(content.Id, content).ConfigureAwait(false);

        return Ok(contents);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetContent(Guid id)
    {
        _logger.LogInformation($"[GET] api/v1/Content/{id}");

        var content = await _manager.GetContent(id).ConfigureAwait(false);

        if (content == null)
            return NotFound();

        await _cacheService.Set(content.Id, content).ConfigureAwait(false);

        return Ok(content);
    }
    
    [HttpPost]
    public async Task<IActionResult> CreateContent(
        [FromBody] ContentInput content
        )
    {
        _logger.LogInformation($"[POST] api/v1/Content");

        var createdContent = await _manager.CreateContent(content.ToDto()).ConfigureAwait(false);

        if (createdContent == null)
            return Problem();

        await _cacheService.Set(createdContent.Id, createdContent).ConfigureAwait(false);

        return Ok(createdContent);
    }
    
    [HttpPatch("{id}")]
    public async Task<IActionResult> UpdateContent(
        Guid id,
        [FromBody] ContentInput content
        )
    {
        _logger.LogInformation($"[PATCH] api/v1/Content/{id}");

        var updatedContent = await _manager.UpdateContent(id, content.ToDto()).ConfigureAwait(false);

        if (updatedContent == null)
            return NotFound();

        await _cacheService.Set(id, updatedContent).ConfigureAwait(false);

        return  Ok(updatedContent);
    }
    
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteContent(
        Guid id
    )
    {
        _logger.LogInformation($"[DELETE] api/v1/Content/{id}");

        var deletedId = await _manager.DeleteContent(id).ConfigureAwait(false);

        await _cacheService.Remove(id);

        return Ok(deletedId);
    }
    
    [HttpPost("{id}/genre")]
    public async Task<IActionResult> AddGenres(
        Guid id,
        [FromBody] IEnumerable<string> genres
    )
    {
        _logger.LogInformation($"[POST] api/v1/Content/{id}/genre");

        var content = await _manager.AddGenreAsync(id, genres).ConfigureAwait(false);

        if (content == null)
            return NotFound();

        await _cacheService.Set(id, content).ConfigureAwait(false);

        return Ok(content);
    }
    
    [HttpDelete("{id}/genre")]
    public async Task<IActionResult> RemoveGenres(
        Guid id,
        [FromBody] IEnumerable<string> genres
    )
    {
        var content = await _manager.RemoveGenreAsync(id, genres).ConfigureAwait(false);

        if (content == null)
            return NotFound();

        await _cacheService.Set(id, content).ConfigureAwait(false);

        return Ok(content);
    }
}