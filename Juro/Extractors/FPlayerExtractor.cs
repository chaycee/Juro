﻿using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;
using Juro.Models.Videos;
using Juro.Utils;
using Juro.Utils.Extensions;

namespace Juro.Extractors;

/// <summary>
/// Extractor for FPlayer.
/// </summary>
public class FPlayerExtractor : IVideoExtractor
{
    private readonly IHttpClientFactory _httpClientFactory;

    /// <inheritdoc />
    public string ServerName => "FPlayer";

    /// <summary>
    /// Initializes an instance of <see cref="FPlayerExtractor"/>.
    /// </summary>
    public FPlayerExtractor(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    /// <summary>
    /// Initializes an instance of <see cref="FPlayerExtractor"/>.
    /// </summary>
    public FPlayerExtractor(Func<HttpClient> httpClientProvider)
        : this(new HttpClientFactory(httpClientProvider))
    {
    }

    /// <summary>
    /// Initializes an instance of <see cref="FPlayerExtractor"/>.
    /// </summary>
    public FPlayerExtractor() : this(Http.ClientProvider)
    {
    }

    /// <inheritdoc />
    public async ValueTask<List<VideoSource>> ExtractAsync(
        string url,
        CancellationToken cancellationToken = default)
    {
        var http = _httpClientFactory.CreateClient();

        var apiLink = url.Replace("/v/", "/api/source/");

        var list = new List<VideoSource>();

        try
        {
            var headers = new Dictionary<string, string>()
            {
                { "Referer", url }
            };

            var json = await http.PostAsync(apiLink, headers, cancellationToken);
            if (!string.IsNullOrWhiteSpace(json))
            {
                var data = JsonNode.Parse(JsonNode.Parse(json)!["data"]!.ToString())!.AsArray();
                for (var i = 0; i < data.Count; i++)
                {
                    list.Add(new()
                    {
                        VideoUrl = data[i]!["file"]!.ToString(),
                        Resolution = data[i]!["label"]!.ToString(),
                        Format = VideoType.Container,
                        FileType = data[i]!["type"]!.ToString(),
                    });
                }

                return list;
            }
        }
        catch
        {
            // Ignore
        }

        return list;
    }
}