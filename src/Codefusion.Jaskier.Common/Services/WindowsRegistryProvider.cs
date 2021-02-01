namespace Codefusion.Jaskier.Common.Services
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Security;
    using Microsoft.Win32;

    public interface IWindowsRegistryProvider
    {
        string GetString(string key, string defaultValue);

        void SetString(string key, string value);

        bool GetBoolean(string key, bool defaultValue);

        void SetBoolean(string key, bool value);
    }

    public class WindowsRegistryProvider : IWindowsRegistryProvider
    {
        private readonly IErrorHandler errorHandler;

        public WindowsRegistryProvider(IErrorHandler errorHandler)
        {
            this.errorHandler = errorHandler;
        }     

        public string GetString(string key, string defaultValue)
        {
            try
            {
                var value = Registry.GetValue(GetRegistryPath(), key, null);

                if (value == null)
                {
                    this.SetString(key, defaultValue);
                    return defaultValue;
                }

                return value.ToString();
            }
            catch (IOException ioException)
            {
                this.HandleError(ioException);
                return string.Empty;
            }
            catch (SecurityException securityException)
            {
                this.HandleError(securityException);
                return string.Empty;
            }
        }

        public void SetString(string key, string value)
        {
            try
            {
                Registry.SetValue(GetRegistryPath(), key, value);
            }
            catch (IOException ioException)
            {
                this.HandleError(ioException);
            }
            catch (SecurityException securityException)
            {
                this.HandleError(securityException);
            }
        }

        public bool GetBoolean(string key, bool defaultValue)
        {
            var stringValue = this.GetString(key, defaultValue.ToString(CultureInfo.InvariantCulture));

            bool result;
            if (!string.IsNullOrEmpty(stringValue) && bool.TryParse(stringValue, out result))
            {
                return result;
            }

            return defaultValue;
        }

        public void SetBoolean(string key, bool value)
        {
            this.SetString(key, value.ToString(CultureInfo.InvariantCulture));
        }

        private static string GetRegistryPath()
        {
            return $"{Registry.CurrentUser.Name}\\SOFTWARE\\{Constants.RegistryCompanyName}\\{Constants.RegistryProductName}\\";
        }

        private void HandleError(Exception exception)
        {
            this.errorHandler.Handle("Failed to access to Windows registry.", exception);
        }
    }
}
