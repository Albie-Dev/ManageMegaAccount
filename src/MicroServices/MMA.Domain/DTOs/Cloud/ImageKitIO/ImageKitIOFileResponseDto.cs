using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;

namespace MMA.Domain
{
    public class ImageKitIOFileResponseDto
    {
        public string FileId { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public int Size { get; set; }

        public VersionInfo? VersionInfo { get; set; }

        public string FilePath { get; set; } = string.Empty;

        public string Url { get; set; } = string.Empty;

        public string FileType { get; set; } = string.Empty;

        public int Height { get; set; }

        public int Width { get; set; }

        public string ThumbnailUrl { get; set; } = string.Empty;

        public List<string> Tags { get; set; } = new List<string>();

        public object? AITags { get; set; }

        public bool IsPrivateFile { get; set; }

        public string CustomCoordinates { get; set; } = string.Empty;

        public Metadata? Metadata { get; set; }

        public ExtensionStatus? ExtensionStatus { get; set; }
    }


    public class Metadata
    {
        public int Height { get; set; }

        public int Width { get; set; }

        public int Size { get; set; }

        public string Format { get; set; } = string.Empty;

        public bool HasColorProfile { get; set; }

        public int Quality { get; set; }

        public int Density { get; set; }

        public bool HasTransparency { get; set; }

        public Exif? Exif { get; set; }

        public string PHash { get; set; } = string.Empty;

        public string Raw { get; set; } = string.Empty;

        public int HttpStatusCode { get; set; }
    }

    [ExcludeFromCodeCoverage]
    public class Exif
    {
    }

    public class ExtensionStatus
    {
        [JsonProperty("remove-bg")]
        public string RemoveBg { get; set; } = string.Empty;

        [JsonProperty("google-auto-tagging")]
        public string GoogleAutoTagging { get; set; } = string.Empty;
    }


    public class VersionInfo
    {
        public string Id { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;
    }
}