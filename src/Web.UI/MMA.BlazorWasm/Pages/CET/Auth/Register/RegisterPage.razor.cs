using MMA.Domain;

namespace MMA.BlazorWasm.Pages.CET.Auth.Register
{
    public partial class RegisterPage
    {
        private bool _isLoading { get; set; } = false;
        private NotificationResponse? _notificationResponse { get; set; } = null;
        private List<ErrorDetailDto> _errors { get; set; } = new List<ErrorDetailDto>();

        private RegisterRequestDto _requestDto { get; set; } = new RegisterRequestDto();

        private async Task RegisterAsync()
        {
            try
            {
                _isLoading = true;
                _errors.Clear();
                var apiResponse = await _httpClientHelper.PostAsync<RegisterRequestDto, NotificationResponse>(
                    endpoint: Path.Combine(EndpointConstant.CET_Base_Url, EndpointConstant.CET_Auth_Register),
                    data: _requestDto,
                    requestType: CHttpClientType.Public,
                    portalType: CPortalType.CET);
                
                if (apiResponse == null)
                {
                    _toastService.ShowError($"Cannot connect to server. Host = {CPortalType.CET.ToDescription()}");
                }
                else
                {
                    if (!apiResponse.Errors.IsNullOrEmpty())
                    {
                        _errors = apiResponse.Errors;
                    }
                    else if (apiResponse.Data == null)
                    {
                        _toastService.ShowError($"An error occured while fetch data. Server no response data.");
                    }
                    else
                    {
                        _notificationResponse = apiResponse.Data;
                    }
                }
            }
            catch(Exception ex)
            {
                _toastService.ShowError($"{ex.Message}. Host = {CPortalType.CET.ToDescription()}");
            }
            finally
            {
                _isLoading = false;
            }
        }
    }
}