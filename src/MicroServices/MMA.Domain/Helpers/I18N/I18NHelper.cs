using System.Globalization;
using System.Reflection;
using System.Resources;

namespace MMA.Domain
{
    public static class I18NHelper
    {
        // ResourceManager to access the .resx file
        private static readonly ResourceManager ResourceManager =
            new ResourceManager("MMA.Domain.Resources.I18N.MMA.Domain", Assembly.GetExecutingAssembly());

        /// <summary>
        /// Gets the localized string for the given key.
        /// </summary>
        /// <param name="key">The key to look up in the resource file.</param>
        /// <param name="args">Optional parameters to format the string.</param>
        /// <returns>The localized string or null if the key is not found.</returns>
        public static string GetString(string key, params object[] args)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException(nameof(key));
            }

            string value = ResourceManager.GetString(key, CultureInfo.CurrentCulture) ?? string.Empty;

            if (value == null)
            {
                return string.Empty;
            }

            return args.Length > 0 ? string.Format(value, args) : value;
        }
    }
}