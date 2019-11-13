using System;
using kio_windows_integration.Exceptions;

namespace kio_windows_integration.Services
{
    public partial class WindowsIntegrationFocusApi
    {
        /// <summary>
        /// Wraps any expression in a <see cref="WindowsIntegrationFocusApiException"/>
        /// </summary>
        private static T WrapEx<T>(Func<T> expression)
        {
            try
            {
                return expression();
            }
            catch (Exception e)
            {
                throw new WindowsIntegrationFocusApiException(e.Message, e);
            }
        }

        /// <inheritdoc cref="WrapEx{T}"/>
        private static void WrapEx(Action statement)
        {
            try
            {
                statement();
            }
            catch (Exception e)
            {
                throw new WindowsIntegrationFocusApiException(e.Message, e);
            }
        }
    }
}