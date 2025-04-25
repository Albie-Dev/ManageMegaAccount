using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MMA.Domain;

namespace MMA.BlazorWasm.Components.Upload
{
    public partial class FileUploader
    {
        [Parameter]
        public bool IsOpen { get; set; } = false;
        [Parameter]
        public string Title { get; set; } = "Import";
        [Parameter]
        public int MaxFileSize { get; set; } = 1024 * 1024 * 15;
        [Parameter]
        public List<CFileType> FileTypes { get; set; } = new List<CFileType>();


        private string _errorMessage { get; set; } = string.Empty;
        private IBrowserFile? _selectedFile { get; set; } = null;
        private bool _isSelected => _selectedFile != null;
        private CFileType _fileType { get; set; } = CFileType.None;

        [Parameter]
        public EventCallback<IBrowserFile> OnFileUploaded { get; set; }

        [Parameter]
        public EventCallback OnDownloadTemplateFile { get; set; }

        [Parameter]
        public EventCallback<bool> IsOpenChanged { get; set; }

        private async Task CloseModal()
        {
            IsOpen = false;
            _errorMessage = string.Empty;
            _selectedFile = null;
            await ChangeOpenModal(IsOpen);
        }

        private void HandleFileSelected(InputFileChangeEventArgs inputFileChangeEvent)
        {
            var file = inputFileChangeEvent.File;
            if (file.Size > MaxFileSize)
            {
                _errorMessage = $"File size exceeds the maximum allowed size of {MaxFileSize / 1024 / 1024} MB.";
                _selectedFile = null;
                return;
            }
            string fileMimeType = file.ContentType;
            _fileType = FileHelper.GetFileTypeFromMimeType(mimeType: fileMimeType);
            if (FileTypes != null && FileTypes.Count > 0 && !FileTypes.Contains(_fileType))
            {
                _errorMessage = $"File type is not allowed. Allowed file types are: {string.Join(", ", FileTypes)}";
                _selectedFile = null;
                return;
            }

            _selectedFile = file;
            _errorMessage = string.Empty;
        }

        private async Task UploadFile()
        {
            if (_selectedFile != null)
            {
                await OnFileUploaded.InvokeAsync(_selectedFile);
            }
        }

        private async Task DownloadImporFileAsync()
        {
            await OnDownloadTemplateFile.InvokeAsync();
        }

        private async Task ChangeOpenModal(bool isOpen)
        {
            IsOpen = isOpen;
            await IsOpenChanged.InvokeAsync(isOpen);
        }
    }
}