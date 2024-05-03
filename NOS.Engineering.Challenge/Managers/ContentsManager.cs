using NOS.Engineering.Challenge.Database;
using NOS.Engineering.Challenge.Models;
using Microsoft.Extensions.Logging;

namespace NOS.Engineering.Challenge.Managers;

public class ContentsManager : IContentsManager
{
    private readonly IDatabase<Content?, ContentDto> _database;
    private readonly ILogger<ContentsManager> _logger;

    public ContentsManager(IDatabase<Content?, ContentDto> database, ILogger<ContentsManager> logger)
    {
        _database = database;
        _logger = logger;
    }

    public Task<IEnumerable<Content?>> GetManyContents()
    {
        return _database.ReadAll();
    }

    public async Task<IEnumerable<Content?>> GetFilteredContents(string? title, string? genre)
    {
        var contents = await _database.ReadAll().ConfigureAwait(false);

        if (!string.IsNullOrWhiteSpace(title))
        {
            contents = contents.Where(c => c?.Title?.Contains(title, StringComparison.OrdinalIgnoreCase) ?? false);
        }

        if (!string.IsNullOrWhiteSpace(genre))
        {
            contents = contents.Where(c => c?.GenreList?.Any(g => string.Equals(g, genre, StringComparison.OrdinalIgnoreCase)) ?? false);
        }

        return contents;
    }

    public Task<Content?> CreateContent(ContentDto content)
    {
        _logger.LogInformation($"Creting content");

        return _database.Create(content);
    }

    public Task<Content?> GetContent(Guid id)
    {
        _logger.LogInformation($"Getting content id: {id}");

        return _database.Read(id);
    }

    public Task<Content?> UpdateContent(Guid id, ContentDto content)
    {
        _logger.LogInformation($"Updating content id: {id}");

        return _database.Update(id, content);
    }

    public Task<Guid> DeleteContent(Guid id)
    {
        _logger.LogInformation($"Deleting content id: {id}");

        return _database.Delete(id);
    }

    public async Task<Content?> AddGenreAsync(Guid id, IEnumerable<string> genres)
    {
        var content = await _database.Read(id).ConfigureAwait(false);

        if(content == null) 
            return null;

        content.GenreList.ToHashSet().UnionWith(genres);

        _logger.LogInformation($"Added genres: {genres} on content id: {id}");

        return await _database.Update(id, content.ToDto());
    }

    public async Task<Content?> RemoveGenreAsync(Guid id, IEnumerable<string> genres)
    {
        var content = await _database.Read(id).ConfigureAwait(false);

        if (content == null)
            return null;

        content.GenreList.ToHashSet().Except(genres);

        _logger.LogInformation($"Delete genres: {genres} on content id: {id}");

        return await _database.Update(id, content.ToDto());
    }
}