using MMA.Domain;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components;

namespace MMA.BlazorWasm.Components.Upload
{
    public partial class VideoUploader
    {
        private bool isUploading;
        private double progress = 0;

        [Parameter]
        public List<ImageKitIOFileResponseDto> UploadedFiles { get; set; } = new();

        [Parameter]
        public EventCallback<List<ImageKitIOFileResponseDto>> UploadedFilesChanged { get; set; }

        [Parameter]
        public List<string> Tags { get; set; } = new();

        private DotNetObjectReference<VideoUploader>? _dotNetObjRef;
        private string _errorMessage = string.Empty;

        private async Task OnFileSelected(InputFileChangeEventArgs e)
        {
            var files = e.GetMultipleFiles();
            foreach (var file in files)
            {
                await UploadSingleFile(file);
            }
        }

        private async Task UploadSingleFile(IBrowserFile file)
        {
            var fileType = FileHelper.GetFileTypeFromMimeType(file.ContentType);
            if (fileType != CFileType.Video)
            {
                _errorMessage = $"Only video files are allowed.";
                return;
            }

            isUploading = true;
            progress = 0;

            var authenticationEndpoint = Path.Combine(CPortalType.CET.ToDescription(),
                EndpointConstant.Cloud_Base_Url, EndpointConstant.Cloud_ImageKitIO_GenerateToken);

            try
            {
                _errorMessage = string.Empty;
                var apiResponse = await _httpClientHelper.PostAsync<ImageKitIOGenerateJWTRequestDto, string>(
                    authenticationEndpoint,
                    new ImageKitIOGenerateJWTRequestDto
                    {
                        UploadPayload = new ImageKitIOUploadPayloadRequest
                        {
                            FileName = file.Name,
                            UseUniqueFileName = true,
                            Tags = Tags
                        },
                        Expire = 3600,
                        PublicKey = _configuration["ImageKitIO:PublicKey"] ?? string.Empty
                    });

                if (apiResponse?.Errors?.Any() == true)
                {
                    isUploading = false;
                    _toastService.ShowError(apiResponse.Errors.Select(s => s.Error).ToList().ToMultilineString());
                    return;
                }

                var token = apiResponse?.Data ?? string.Empty;
                _dotNetObjRef = DotNetObjectReference.Create(this);
                var fileBuffer = new byte[file.Size];
                await file.OpenReadStream(maxAllowedSize: 1024 * 1024 * 200).ReadAsync(fileBuffer); // 200MB
                await _jsRuntime.InvokeVoidAsync("uploadFile", fileBuffer, file.Name, token, string.Join(",", Tags), _dotNetObjRef);
            }
            catch (Exception ex)
            {
                isUploading = false;
                _toastService.ShowError($"Error uploading video. FileName = {file.Name}. Error = {ex.Message}");
            }
        }

        [JSInvokable]
        public void UpdateProgress(double progressValue)
        {
            progress = progressValue;
            StateHasChanged();
        }

        [JSInvokable]
        public async Task UploadComplete(bool success, string? jsonResponse)
        {
            if (success)
            {
                var result = jsonResponse?.FromJson<ImageKitIOFileResponseDto>() ?? new();
                UploadedFiles.Add(result);
                await UploadedFilesChanged.InvokeAsync(UploadedFiles);
            }
            else
            {
                _toastService.ShowWarning($"Upload failed. Error = {jsonResponse}");
            }

            isUploading = false;
            StateHasChanged();
        }

        private async Task DeleteVideo(string fileId)
        {
            string deleteEndpoint = Path.Combine(CPortalType.CET.ToDescription(), EndpointConstant.Cloud_Base_Url, $"{EndpointConstant.Cloud_ImageKitIO}/{fileId}");
            try
            {
                var apiResponse = await _httpClientHelper.DeleteAsync<bool>(deleteEndpoint);
                var fileToRemove = UploadedFiles.FirstOrDefault(f => f.FileId == fileId);

                if (apiResponse?.Success == true && fileToRemove != null)
                {
                    UploadedFiles.Remove(fileToRemove);
                    await UploadedFilesChanged.InvokeAsync(UploadedFiles);
                }
                else
                {
                    _toastService.ShowError("Failed to delete video.");
                }
            }
            catch (Exception ex)
            {
                _toastService.ShowError($"Error deleting video. FileID = {fileId}. Error = {ex.Message}");
            }

            StateHasChanged();
        }

        private async Task ViewVideo(string url)
        {
            await _jsRuntime.InvokeVoidAsync("open", url, "_blank");
        }
    }
}
