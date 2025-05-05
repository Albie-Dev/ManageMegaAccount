using MMA.Domain;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components;


namespace MMA.BlazorWasm.Components.Upload
{
    public partial class ImageUploader
    {
        private bool isUploading;
        private double progress = 0;

        [Parameter]
        public List<ImageKitIOFileResponseDto> UploadedFiles { get; set; } = new List<ImageKitIOFileResponseDto>();

        [Parameter]
        public EventCallback<List<ImageKitIOFileResponseDto>> UploadedFilesChanged { get; set; }

        [Parameter]
        public List<string> Tags { get; set; } = new List<string>()
        {

        };

        private DotNetObjectReference<ImageUploader>? _dotNetObjRef;

        private string _errorMessage { get; set; } = string.Empty;

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
            var fileType = FileHelper.GetFileTypeFromMimeType(mimeType: file.ContentType);
            if (fileType != CFileType.Image)
            {
                _errorMessage = $"File type is not allowed. Allowed file types are images.";
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
                endpoint: authenticationEndpoint,
                data: new ImageKitIOGenerateJWTRequestDto
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

                if (apiResponse != null)
                {
                    if (!apiResponse.Errors.IsNullOrEmpty())
                    {
                        isUploading = false;
                        _toastService.ShowError(message: apiResponse.Errors.Select(s => s.Error).ToList().ToMultilineString());
                    }
                    else
                    {
                        var token = apiResponse.Data ?? string.Empty;
                        _dotNetObjRef = DotNetObjectReference.Create(this);
                        var fileBuffer = new byte[file.Size];
                        await file.OpenReadStream(maxAllowedSize: 1024 * 1024 * 50).ReadAsync(fileBuffer);
                        await _jsRuntime.InvokeVoidAsync("uploadFile", fileBuffer, file.Name, token, string.Join(",", Tags), _dotNetObjRef);
                    }
                }
                else
                {
                    isUploading = false;
                    _toastService.ShowError($"Cannot connect to Server: {CPortalType.CET.ToDescription()}");
                }
            }
            catch (Exception ex)
            {
                isUploading = false;
                _toastService.ShowError($"An error occured while upload image. FileName = {file.Name}. Error = {ex.Message} {CPortalType.CET.ToDescription()}");
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
                var result = jsonResponse?.FromJson<ImageKitIOFileResponseDto>() ?? new ImageKitIOFileResponseDto();
                UploadedFiles.Add(result);
                await UploadedFilesChanged.InvokeAsync(UploadedFiles);
                isUploading = false;
                try
                {
                    var apiResponse = await _httpClientHelper.PostAsync<CreateFileRequestDto, int>(
                        endpoint: Path.Combine(EndpointConstant.Cloud_Base_Url, EndpointConstant.Cloud_File_Create),
                        data: new CreateFileRequestDto()
                        {
                            CloudType = CCloudType.ImageKitIO,
                            FileIds = UploadedFiles.Select(s => s.FileId).ToList()
                        },
                        requestType: CHttpClientType.Private,
                        portalType: CPortalType.CET);
                    if (apiResponse != null && apiResponse.Success)
                    {
                        Console.WriteLine($"Create files successful. Count = {apiResponse.Data}");
                    }
                }
                catch(Exception ex)
                {
                    _toastService.ShowError(message: $"{ex.Message}. Host = {CPortalType.CET.ToDescription()}");
                }
                finally
                {
                    isUploading = false;
                }
            }
            else
            {
                _toastService.ShowWarning(message: $"An error occured while uploading file. Error = {jsonResponse}");
            }
            isUploading = false;
            StateHasChanged();
        }

        private async Task DeleteImage(string fileId)
        {
            string deleteEndpoint = Path.Combine(CPortalType.CET.ToDescription(), EndpointConstant.Cloud_Base_Url, $"{EndpointConstant.Cloud_ImageKitIO}/{fileId}");
            try
            {
                var apiResponse = await _httpClientHelper.DeleteAsync<bool>(endpoint: deleteEndpoint);
                var fileToRemove = UploadedFiles.FirstOrDefault(f => f.FileId == fileId);
                if (apiResponse != null && apiResponse.Success)
                {
                    if (fileToRemove != null)
                    {
                        UploadedFiles.Remove(fileToRemove);
                        await UploadedFilesChanged.InvokeAsync(UploadedFiles);
                    }
                }
                else
                {
                    if (apiResponse == null)
                    {
                        _toastService.ShowError($"Cannot connect to Server: {CPortalType.CET.ToDescription()}");
                    }
                    else
                    {
                        if (apiResponse.Errors.IsNullOrEmpty())
                        {
                            _toastService.ShowError($"An error occured while delete image. FileID = {fileId}");
                        }
                        else
                        {
                            _toastService.ShowWarning(message: apiResponse.Errors.Select(s => s.Error).ToList().ToMultilineString());
                        }
                    }
                    if (fileToRemove != null)
                    {
                        UploadedFiles.Remove(fileToRemove);
                    }
                    await UploadedFilesChanged.InvokeAsync(UploadedFiles);
                }
            }
            catch (Exception ex)
            {
                _toastService.ShowError($"An error occured while delete image. FileID = {fileId}. Error = {ex.Message}");
            }
            StateHasChanged();
        }

        private async Task ViewImage(string url)
        {
            await _jsRuntime.InvokeVoidAsync("open", url, "_blank");
        }
    }
}