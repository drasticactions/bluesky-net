namespace Bluesky.Net.Models;

using Internals;
using System;
using System.Linq;
using System.Text.RegularExpressions;

public class AtUri
{
    private string _host;
    private static readonly Regex _atpUriRegex = new Regex(
        @"^(at:\/\/)?((?:did:[a-z0-9:%-]+)|(?:[a-z][a-z0-9.:-]*))(\/[^?#\s]*)?(\?[^#\s]+)?(#[^\s]+)?$",
        RegexOptions.IgnoreCase
    );

    public AtUri(string uri)
    {
        Match match = _atpUriRegex.Match(uri);

        if (match == null || !match.Success)
        {
            throw new FormatException($"Invalid at uri: {uri}");
        }

        _host = match.Groups[2].Value ?? "";
        Pathname = match.Groups[3].Value ?? "";
        Hash = match.Groups[5].Value ?? "";
    }

    public string Hash { get; private set; }
    public string Pathname { get; private set; }
    public string Protocol => "at:";
    public string Origin => $"at://{_host}";
    public string Hostname => _host;
    public string Collection => Pathname.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries)[0];
    public string Rkey => Pathname.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries).ElementAtOrDefault(1);
    public string Href => this.ToString();

    public string ToString()
    {
        var buffer = new System.Text.StringBuilder();
        buffer.Append("at://");
        buffer.Append(_host);

        if (!Pathname.StartsWith("/"))
        {
            buffer.Append($"/{Pathname}");
        }
        else
        {
            buffer.Append(Pathname);
        }

        if (!string.IsNullOrEmpty(Hash) && !Hash.StartsWith("#"))
        {
            buffer.Append($"#{Hash}");
        }
        else
        {
            buffer.Append(Hash);
        }

        return buffer.ToString();
    }
}
