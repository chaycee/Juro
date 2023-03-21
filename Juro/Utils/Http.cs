﻿using System;
using System.Net.Http;
using System.Net.Security;

namespace Juro.Utils;

public static class Http
{
    private static readonly Lazy<HttpClient> HttpClientLazy = new(() =>
    {
        var handler = new HttpClientHandler
        {
            //UseCookies = false
            //AllowAutoRedirect = true
        };

        //handler.MaxAutomaticRedirections = 2;

        //if (handler.SupportsAutomaticDecompression)
        //    handler.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

        //handler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };

        var httpClient = new HttpClient(handler, true);

        if (!httpClient.DefaultRequestHeaders.Contains("User-Agent"))
        {
            httpClient.DefaultRequestHeaders.Add(
                "User-Agent",
                Http.ChromeUserAgent()
            );
        }

        return httpClient;
    });

    public static HttpClient Client => HttpClientLazy.Value;

    #region User Agent

    /// <summary>
    /// Generates a random User-Agent from the IE browser.
    /// </summary>
    /// <returns>Random User-Agent from IE browser.</returns>
    public static string IEUserAgent()
    {
        var windowsVersion = RandomWindowsVersion();

        string version;
        string mozillaVersion;
        string trident;
        string otherParams;

        #region Random version generation

        if (windowsVersion.Contains("NT 5.1"))
        {
            version = "9.0";
            mozillaVersion = "5.0";
            trident = "5.0";
            otherParams = ".NET CLR 2.0.50727; .NET CLR 3.5.30729";
        }
        else if (windowsVersion.Contains("NT 6.0"))
        {
            version = "9.0";
            mozillaVersion = "5.0";
            trident = "5.0";
            otherParams = ".NET CLR 2.0.50727; Media Center PC 5.0; .NET CLR 3.5.30729";
        }
        else
        {
            switch (Randomizer.Instance.Next(3))
            {
                case 0:
                    version = "10.0";
                    trident = "6.0";
                    mozillaVersion = "5.0";
                    break;

                case 1:
                    version = "10.6";
                    trident = "6.0";
                    mozillaVersion = "5.0";
                    break;

                default:
                    version = "11.0";
                    trident = "7.0";
                    mozillaVersion = "5.0";
                    break;
            }

            otherParams = ".NET CLR 2.0.50727; .NET CLR 3.5.30729; .NET CLR 3.0.30729; Media Center PC 6.0; .NET4.0C; .NET4.0E";
        }

        #endregion

        return
            $"Mozilla/{mozillaVersion} (compatible; MSIE {version}; {windowsVersion}; Trident/{trident}; {otherParams})";
    }

    /// <summary>
    /// Generates a random User-Agent from the Opera browser.
    /// </summary>
    /// <returns>A random User-Agent from the Opera browser.</returns>
    public static string OperaUserAgent()
    {
        string version;
        string presto;

        #region Random version generation

        switch (Randomizer.Instance.Next(4))
        {
            case 0:
                version = "12.16";
                presto = "2.12.388";
                break;

            case 1:
                version = "12.14";
                presto = "2.12.388";
                break;

            case 2:
                version = "12.02";
                presto = "2.10.289";
                break;

            default:
                version = "12.00";
                presto = "2.10.181";
                break;
        }

        #endregion

        return $"Opera/9.80 ({RandomWindowsVersion()}); U) Presto/{presto} Version/{version}";
    }

    /// <summary>
    /// Generates a random User-Agent from the Chrome browser.
    /// </summary>
    /// <returns>Random User-Agent from Chrome browser.</returns>
    public static string ChromeUserAgent()
    {
        var major = Randomizer.Instance.Next(62, 70);
        var build = Randomizer.Instance.Next(2100, 3538);
        var branchBuild = Randomizer.Instance.Next(170);

        return $"Mozilla/5.0 ({RandomWindowsVersion()}) AppleWebKit/537.36 (KHTML, like Gecko) " +
            $"Chrome/{major}.0.{build}.{branchBuild} Safari/537.36";
    }


    private static readonly byte[] FirefoxVersions = { 64, 63, 62, 60, 58, 52, 51, 46, 45 };

    /// <summary>
    /// Generates a random User-Agent from the Firefox browser.
    /// </summary>
    /// <returns>Random User-Agent from the Firefox browser.</returns>
    public static string FirefoxUserAgent()
    {
        var version = FirefoxVersions[Randomizer.Instance.Next(FirefoxVersions.Length - 1)];

        return $"Mozilla/5.0 ({RandomWindowsVersion()}; rv:{version}.0) Gecko/20100101 Firefox/{version}.0";
    }

    /// <summary>
    /// Generates a random User-Agent from the Opera mobile browser.
    /// </summary>
    /// <returns>Random User-Agent from Opera mobile browser.</returns>
    public static string OperaMiniUserAgent()
    {
        string os;
        string miniVersion;
        string version;
        string presto;

        #region Random version generation

        switch (Randomizer.Instance.Next(3))
        {
            case 0:
                os = "iOS";
                miniVersion = "7.0.73345";
                version = "11.62";
                presto = "2.10.229";
                break;

            case 1:
                os = "J2ME/MIDP";
                miniVersion = "7.1.23511";
                version = "12.00";
                presto = "2.10.181";
                break;

            default:
                os = "Android";
                miniVersion = "7.5.54678";
                version = "12.02";
                presto = "2.10.289";
                break;
        }

        #endregion

        return $"Opera/9.80 ({os}; Opera Mini/{miniVersion}/28.2555; U; ru) Presto/{presto} Version/{version}";
    }

    /// <summary>
    /// Returns a random Chrome / Firefox / Opera User-Agent based on their popularity.
    /// </summary>
    /// <returns>User-Agent header value string</returns>
    public static string RandomUserAgent()
    {
        var rand = Randomizer.Instance.Next(99) + 1;

        // TODO: edge, yandex browser, safari

        // Chrome = 70%
        if (rand >= 1 && rand <= 70)
            return ChromeUserAgent();

        // Firefox = 15%
        if (rand > 70 && rand <= 85)
            return FirefoxUserAgent();

        // IE = 6%
        if (rand > 85 && rand <= 91)
            return IEUserAgent();

        // Opera 12 = 5%
        if (rand > 91 && rand <= 96)
            return OperaUserAgent();

        // Opera mini = 4%
        return OperaMiniUserAgent();
    }

    #endregion

    #region Static methods (private)

    private static bool AcceptAllCertifications(object sender,
        System.Security.Cryptography.X509Certificates.X509Certificate certification,
        System.Security.Cryptography.X509Certificates.X509Chain chain,
        SslPolicyErrors sslPolicyErrors) => true;

    private static string RandomWindowsVersion()
    {
        var windowsVersion = "Windows NT ";
        var random = Randomizer.Instance.Next(99) + 1;

        // Windows 10 = 45% popularity
        if (random >= 1 && random <= 45)
            windowsVersion += "10.0";

        // Windows 7 = 35% popularity
        else if (random > 45 && random <= 80)
            windowsVersion += "6.1";

        // Windows 8.1 = 15% popularity
        else if (random > 80 && random <= 95)
            windowsVersion += "6.3";

        // Windows 8 = 5% popularity
        else
            windowsVersion += "6.2";

        // Append WOW64 for X64 system
        if (Randomizer.Instance.NextDouble() <= 0.65)
            windowsVersion += Randomizer.Instance.NextDouble() <= 0.5 ? "; WOW64" : "; Win64; x64";

        return windowsVersion;
    }

    #endregion
}