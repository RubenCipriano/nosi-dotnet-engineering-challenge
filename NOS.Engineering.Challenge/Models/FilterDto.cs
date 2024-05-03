namespace NOS.Engineering.Challenge.Models;

public sealed record FilterDto
{
    public FilterDto(string? title, string? genre)
    {
        Title = title ?? string.Empty;
        Genre = genre ?? string.Empty;
    }

    public string Title { get; init; }
    public string Genre { get; init; }
}