
using MMA.Domain;

namespace MMA.BlazorWasm.Pages.CET.Cloud.MegaAccount
{
    public partial class Index
    {
        private bool _isLoading { get; set; } = false;
        private bool _isUploadModalOpen { get; set; } = false;
        private NotificationResponse? _notificationResponse { get; set; }
        private List<ErrorDetailDto> _errors { get; set; } = new List<ErrorDetailDto>();
        private MegaAccountFilterProperty _megaAccountFilterProperty { get; set; } = new MegaAccountFilterProperty();
        private List<Guid> _megaAccountIds { get; set; } = new List<Guid>();
        private TableParam<MegaAccountFilterProperty> _requestDto { get; set; } = new TableParam<MegaAccountFilterProperty>();

        private bool ShowUpdateMegaAccount => _megaAccountIds.Count == 1;
        private bool ShowDeactiveMegaAccount => _megaAccountIds.Count > 0;

        private async Task ImportMegaAccountsAsync()
        {
            await Task.CompletedTask;
        }

        private async Task ExportMegaAccountsAsync()
        {
            await Task.CompletedTask;
        }

        private async Task HandleOpenUploadModal()
        {
            await Task.CompletedTask;
        }

        private async Task DownloadTemplateFileAsync()
        {
            await Task.CompletedTask;
        }

        public async Task CreateMegaAccountAsync()
        {
            await Task.CompletedTask;
        }

        private async Task UpdateMegaAccountAsync()
        {
            await Task.CompletedTask;
        }

        private async Task DeactiveMegaAccountsAsync()
        {
            await Task.CompletedTask;
        }

        private async Task DeleteMegaAccountsAsync()
        {
            await Task.CompletedTask;
        }

    }
}