namespace MMA.Domain
{
    public class EndpointConstant
    {
        public const string CET_Base_Url = "api/v1/cet";
        public const string CET_Auth_Microsoft_Generate_Login_Url = "auth/microsoft/generateloginurl";
        public const string CET_Auth_Google_Generate_Login_Url = "auth/google/generateloginurl";
        public const string CET_Auth_Microsoft_Login = "auth/microsoft/login";
        public const string CET_Auth_Google_Login = "auth/google/login";
        public const string CET_Auth_Protected = "auth/protected";
        public const string CET_Auth_SystemLogin = "auth/systemlogin";
        public const string CET_Auth_RefreshToken = "auth/refreshtoken";
        public const string CET_Auth_Register = "auth/register";
        public const string CET_Auth_Register_Confirm = "auth/register/confirm";
        public const string CET_Auth_TwoFactor_Confirm = "auth/towfactor/confirm";
        public const string CET_Auth_Logout = "auth/logout";

        #region mega account
        public const string CET_Mega_Account_Paging = "megaaccount/paging";
        #endregion mega account

        public const string CET_Role_Paging = "roles/paging";



        #region movie
        public const string Movie_Base_Url = "api/v1/movie";
        public const string Movie_Actor_Paging = "actor/paging";
        public const string Movie_Actor_AllInfo = "actor/allinfos";
        public const string Movie_Actor_Detail = "actor/detail";
        public const string Movie_Actor_Create = "actor/delete";
        public const string Movie_Actor_Update = "actor/update";
        public const string Movie_Actor_Delete = "actor/delete";

        #endregion movie
    }
}