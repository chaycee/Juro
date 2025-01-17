﻿using System.Collections.Generic;

namespace Juro.Models.Manga;

public interface IMangaChapterPage
{
    public string Image { get; set; }

    public int Page { get; set; }

    public string? Title { get; set; }

    public Dictionary<string, string> HeaderForImage { get; set; }
}