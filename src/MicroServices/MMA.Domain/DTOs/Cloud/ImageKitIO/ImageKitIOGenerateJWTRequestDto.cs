namespace MMA.Domain
{
    public class ImageKitIOGenerateJWTRequestDto
    {
        public int Expire { get; set; }
        public string PublicKey { get; set; } = string.Empty;
        public ImageKitIOUploadPayloadRequest UploadPayload { get; set; } = null!;

    }

    public class ImageKitIOUploadPayloadRequest
    {
        public string FileName { get; set; } = string.Empty;
        public bool UseUniqueFileName { get; set; }
        public List<string> Tags { get; set; } = new List<string>();
    }
}