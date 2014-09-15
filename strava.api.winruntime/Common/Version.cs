using System;
using System.Reflection;

namespace com.strava.api.Common
{
    /// <summary>
    /// Contains information about the framework.
    /// </summary>
    public static class Framework
    {
        /// <summary>
        /// Contains information about the version of the framework.
        /// </summary>
        public static Version Version
        {
            get
            {
                var version = typeof (Framework).GetTypeInfo().Assembly.GetName().Version;

                return new Version(
                    version.Major,
                    version.Minor,
                    version.Build);
            }
        }
    }
}
