namespace MMA.Domain
{
    public enum CTokenType
    {
        None = 0,
        AuthToken = 1,
        EmailToken = 2,
        TwoFactorEnable = 3,
        PhoneToken = 4,
        DiscountToken = 5,
        Normal = 6,
    }
}