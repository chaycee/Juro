﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Juro.Models.Manga;
using Juro.Utils;
using Juro.Utils.Extensions;

namespace Juro.Providers.Manga;

/// <summary>
/// Client for interacting with MangaKatana.
/// </summary>
public class MangaKatana : IMangaProvider
{
    private readonly HttpClient _http;

    public string Name { get; set; } = "MangaKatana";

    public string BaseUrl => "https://mangakatana.com";

    public string Logo => "";

    /// <summary>
    /// Initializes an instance of <see cref="MangaKatana"/>.
    /// </summary>
    public MangaKatana(Func<HttpClient> httpClientProvider)
    {
        _http = httpClientProvider();
    }

    /// <summary>
    /// Initializes an instance of <see cref="MangaKatana"/>.
    /// </summary>
    public MangaKatana() : this(Http.ClientProvider)
    {
    }

    /// <summary>
    /// Search for manga.
    /// </summary>
    /// <param name="query"></param>
    /// <returns>An interface of type <see cref="IMangaResult"/> from an instance of <see cref="MangaResult"/>.</returns>
    public async Task<List<IMangaResult>> SearchAsync(
        string query,
        CancellationToken cancellationToken = default!)
    {
        var url = $"{BaseUrl}/?search={Uri.EscapeDataString(query)}";
        var response = await _http.ExecuteAsync(url, cancellationToken);

        var document = Html.Parse(response);

        var gg = document.GetElementbyId("book_list");

        return document.GetElementbyId("book_list")?
            .SelectNodes(".//div[contains(@class, 'media-left')]")?
            .Select(el => (IMangaResult)new MangaResult()
            {
                Id = el.SelectSingleNode(".//a").Attributes["href"].Value,
                Title = el.SelectSingleNode(".//img")?.Attributes["alt"]?.Value,
                Image = el.SelectSingleNode(".//img")?.Attributes["src"]?.Value
            }).ToList() ?? new();
    }

    /// <summary>
    /// Gets the manga info by Id.
    /// </summary>
    /// <param name="mangaId">The Id of the manga</param>
    /// <returns>An interface of type <see cref="IMangaResult"/> from an instance of <see cref="MangaInfo"/>.</returns>
    public async Task<IMangaInfo> GetMangaInfoAsync(
        string mangaId,
        CancellationToken cancellationToken = default!)
    {
        var url = BaseUrl + mangaId;
        var response = await _http.ExecuteAsync(url, cancellationToken);

        var document = Html.Parse(response);

        var mangaInfo = new MangaInfo
        {
            Id = mangaId
        };

        mangaInfo.Description = document.DocumentNode.SelectSingleNode(".//div[@class='flex flex-col']/div[2]/p")?.InnerText.Trim();
        mangaInfo.Genres = document.DocumentNode.SelectNodes(".//div[@class='flex flex-col']/div[4]/a")?
            .Select(el => el.InnerText).ToList() ?? new();

        var statusText = document.DocumentNode.SelectSingleNode(".//div[@class='flex flex-col']/div[3]/div[2]/div")?.InnerText.Trim();
        mangaInfo.Status = statusText switch
        {
            "finished" => MediaStatus.Completed,
            "publishing" => MediaStatus.Ongoing,
            _ => MediaStatus.Unknown,
        };

        mangaInfo.Chapters = document.DocumentNode.SelectNodes(".//div[@id='chapters']/div/a")?
            .Reverse()?.Select(el => (IMangaChapter)new MangaChapter()
            {
                Id = el.Attributes["href"].Value,
                Title = el.InnerText
            }).ToList() ?? new();

        return mangaInfo;
    }

    /// <summary>
    /// Gets chapter pages for manga.
    /// </summary>
    public async Task<List<IMangaChapterPage>> GetChapterPagesAsync(
        string chapterId,
        CancellationToken cancellationToken = default!)
    {
        var url = BaseUrl + chapterId;
        var response = await _http.ExecuteAsync(url, cancellationToken);

        var document = Html.Parse(response);

        var i = 1;

        return document.DocumentNode.SelectNodes(".//img[@class='js-page']")
            .Select(el => (IMangaChapterPage)new MangaChapterPage()
            {
                Image = el.Attributes["data-src"]!.Value,
                Page = i++
            }).ToList();
    }
}