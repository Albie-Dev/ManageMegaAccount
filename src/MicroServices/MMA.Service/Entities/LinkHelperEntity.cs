namespace MMA.Service
{
    public class LinkHelperEntity : BaseEntity
    {
        public string Server_AppEndpoint { get; set; } = string.Empty;
        public string Client_AppEndpoint { get; set; } = string.Empty;
        public string Client_ConfirmNewPasswordRoute { get; set; } = string.Empty;
    }
}