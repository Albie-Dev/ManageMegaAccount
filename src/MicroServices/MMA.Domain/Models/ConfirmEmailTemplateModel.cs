namespace MMA.Domain
{
    public class ConfirmEmailTemplateModel : BaseEmailTemplateModel
    {
        public string CustomerName { get; set; } = string.Empty;
        public string ConfirmationLink { get; set; } = string.Empty;
    }

    public class BaseEmailTemplateModel : ClientApp
    {
        public string ReceiverEmail { get; set; } = string.Empty;
    }
}