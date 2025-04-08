using System.Collections;

namespace MMA.Domain
{
    public class ImageKitUploadFileRequestDto
    {

        public string FileName { get; set; } = string.Empty;

        public bool useUniqueFileName { get; set; }

        public List<string> Tags { get; set; } = new();

        public string Folder { get; set; } = string.Empty;

        public string CustomCoordinates { get; set; } = string.Empty;

        public List<string> ResponseFields { get; set; } = new List<string>();

        public List<Extension> Extensions { get; set; } = new List<Extension>();

        public string WebhookUrl { get; set; } = string.Empty;

        public bool OverwriteFile { get; set; }

        public bool OverwriteAITags { get; set; }

        public bool OverwriteTags { get; set; }

        public bool OverwriteCustomMetadata { get; set; }

        public Hashtable? CustomMetadata { get; set; }

        public object? File { get; set; }

        public bool isPrivateFile { get; set; }

        public UploadTransformation? Transformation { get; set; }

        public string Checks { get; set; } = string.Empty;

        public bool? IsPublished { get; set; }
    }

    public class Extension
    {
        public string Name { get; set; } = string.Empty;
    }

    public class UploadTransformation
    {
        public string? Pre { get; set; } = string.Empty;

        public List<PostTransformation>? Post { get; set; }
    }

    public class PostTransformation
    {
        public string Type { get; set; } = string.Empty;

        public string Value { get; set; } = string.Empty;
    }
}