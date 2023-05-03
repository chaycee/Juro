﻿using System;
using System.Threading.Tasks;
using Httpz;
using Juro.Clients;
using Juro.Utils;
using Juro.Utils.Extensions;

namespace Juro.DemoConsole;

internal class Program
{
    static async Task Main()
    {
        Console.WriteLine("Hello, World!");

        await AnimeDemo();
        //await MangaDemo();
        //await MovieDemo();
    }

    private static async Task AnimeDemo()
    {
        //var test = "k".ToIntOrNull(36);

        var client = new AnimeClient();
        var animes = await client.NineAnime.SearchAsync("jujutsu kaisen");
        var animeInfo = await client.NineAnime.GetAnimeInfoAsync(animes[0].Id);
        var episodes = await client.NineAnime.GetEpisodesAsync(animes[0].Id);
        var videoServers = await client.NineAnime.GetVideoServersAsync(episodes[0].Id);
        var videos = await client.NineAnime.GetVideosAsync(videoServers[3]);

        var downloader = new HlsDownloader();
        var test = await downloader.GetHlsStreamMetadatasAsync(videos[0].VideoUrl, videos[0].Headers);
    }

    private static async Task MovieDemo()
    {
        var client = new MovieClient();
        var movies = await client.FlixHQ.SearchAsync("spongebob");

        var movieInfo = await client.FlixHQ.GetMediaInfoAsync(movies[0].Id);
        var servers = await client.FlixHQ.GetEpisodeServersAsync(movieInfo!.Episodes[0].Id, movieInfo!.Id);

        //Defaut
        var sources = await client.FlixHQ.GetEpisodeSourcesAsync(movieInfo!.Episodes[0].Id, movieInfo!.Id);
        //
        //var sources = await client.FlixHQ.GetEpisodeSourcesAsync(servers[0].Url, movieInfo!.Id);

        // Download the stream
        var fileName = $@"{Environment.CurrentDirectory}\test1.ts";

        //var downloader = new HlsDownloader();
        //
        //using var progress = new ConsoleProgress();
        //
        //var qualities = await downloader.GetHlsStreamMetadatasAsync(sources[0].VideoUrl, sources[0].Headers);
        //var stream = await qualities[0].Stream;
        //await downloader.DownloadAllTsThenMergeAsync(stream, sources[0].Headers, fileName, progress, 15);
    }

    private static async Task MangaDemo()
    {
        var client = new MangaClient();
        //var results = await client.MangaKakalot.SearchAsync("Tomodachi Game");
        var results = await client.MangaPill.SearchAsync("solo leveling");
        var mangaInfo = await client.MangaPill.GetMangaInfoAsync(results[0].Id);
        var pages = await client.MangaPill.GetChapterPagesAsync(mangaInfo.Chapters[0].Id);

        // Download the image
        var fileName = $@"{Environment.CurrentDirectory}\page1.png";

        var downloader = new Downloader();
        await downloader.DownloadAsync(
            pages[0].Image,
            fileName,
            headers: pages[0].HeaderForImage
        );
    }
}