using Microsoft.Extensions.Configuration;
using MMA.Domain;

namespace MMA.Service
{
    public static class RuntimeContext
    {
        public static AppSettings AppSettings { get; private set; }
        private static AsyncLocal<UserEntity?> _currentUser = new AsyncLocal<UserEntity?>();
        private static AsyncLocal<string> _currentAccessToken = new AsyncLocal<string>();
        private static AsyncLocal<string> _ipAddress = new AsyncLocal<string>();
        private static AsyncLocal<Guid> _currentUserId = new AsyncLocal<Guid>();
        private static AsyncLocal<LinkHelperEntity?> _linkHelper = new AsyncLocal<LinkHelperEntity?>();
        public static IServiceProvider? ServiceProvider { get; set; }

        static RuntimeContext()
        {
            var directory = new DirectoryInfo(Directory.GetCurrentDirectory());
            while (directory != null && !directory.GetFiles("*.sln").Any())
            {
                directory = directory.Parent;
            }
            var builder = new ConfigurationBuilder()
                .SetBasePath(directory?.FullName ?? string.Empty)
                .AddJsonFile(Path.Combine("configuration.development.json"), optional: true, reloadOnChange: true);

            var configuration = builder.Build();
            AppSettings = new AppSettings();
            configuration.GetSection(nameof(AppSettings)).Bind(AppSettings);
        }

        public static string IpAddress
        {
            get
            {
                return _ipAddress.Value ?? string.Empty;
            }
            set
            {
                _ipAddress.Value = value;
            }
        }

        public static UserEntity? CurrentUser
        {
            get
            {
                return _currentUser.Value;
            }
            set
            {
                _currentUser.Value = value;
            }
        }

        public static Guid CurrentUserId
        {
            get
            {
                return _currentUserId.Value;
            }
            set
            {
                _currentUserId.Value = value;
            }
        }

        public static LinkHelperEntity? LinkHelper
        {
            get
            {
                return _linkHelper.Value;
            }
            set
            {
                _linkHelper.Value = value;
            }
        }

        public static string CurrentAccessToken
        {
            get
            {
                return _currentAccessToken.Value ?? string.Empty;
            }
            set
            {
                _currentAccessToken.Value = value;
            }
        }
    }
}