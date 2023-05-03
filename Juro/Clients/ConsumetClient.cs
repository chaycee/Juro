﻿using System;
using System.Net.Http;
using Juro.Providers.Consumet;
using Juro.Utils;

namespace Juro.Clients;

/// <summary>
/// Client for interacting with consumet api.
/// </summary>
public class ConsumetClient
{
    /// <summary>
    /// Operations related to NineAnime.
    /// </summary>
    public NineAnime NineAnime { get; }

    /// <summary>
    /// Initializes an instance of <see cref="ConsumetClient"/>.
    /// </summary>
    public ConsumetClient(Func<HttpClient> httpClientProvider)
    {
        NineAnime = new(httpClientProvider);
    }

    /// <summary>
    /// Initializes an instance of <see cref="ConsumetClient"/>.
    /// </summary>
    public ConsumetClient() : this(Http.ClientProvider)
    {
    }
}