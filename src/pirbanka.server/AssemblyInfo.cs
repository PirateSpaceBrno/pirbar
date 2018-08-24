using System;
using System.IO;
using System.Reflection;

namespace PirBanka.Server
{
    /// <summary>
    /// Information about the executing assembly.
    /// </summary>
    public static class AssemblyInfo
    {
        private static DateTime? _Date = null;

        /// <summary>
        /// Gets the linker date from the assembly header.
        /// </summary>
        public static DateTime Date
        {
            get
            {
                if (_Date == null)
                {
                    _Date = GetLinkerTime(Assembly.GetExecutingAssembly());
                }
                return _Date.Value;
            }
        }

        /// <summary>
        /// Gets the linker date of the assembly.
        /// </summary>
        /// <param name="assembly"></param>
        /// <returns></returns>
        /// <remarks>https://blog.codinghorror.com/determining-build-date-the-hard-way/</remarks>
        private static DateTime GetLinkerTime(Assembly assembly)
        {
            var filePath = assembly.Location;
            const int c_PeHeaderOffset = 60;
            const int c_LinkerTimestampOffset = 8;

            var buffer = new byte[2048];

            using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                stream.Read(buffer, 0, 2048);

            var offset = BitConverter.ToInt32(buffer, c_PeHeaderOffset);
            var secondsSince1970 = BitConverter.ToInt32(buffer, offset + c_LinkerTimestampOffset);
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            var linkTimeUtc = epoch.AddSeconds(secondsSince1970);

            return linkTimeUtc;
        }

        internal static string Version
        {
            get
            {
                var buildDate = PirBanka.Server.AssemblyInfo.Date;
                var buildVersion = new Version(buildDate.Year, buildDate.Month, buildDate.Day, Convert.ToInt32(buildDate.TimeOfDay.TotalSeconds));

                /*  Codename is based on https://en.wikipedia.org/wiki/List_of_cat_breed
                * Alphabetically for v1: starts with A, v2: starts with B...
                * For Hotfixes, subversions etc, use next alphabetical name starting with same letter as major version
                * -------------
                * Examples
                * -------------
                * v1:
                *   Abyssinian
                * v1 HF1:
                *   Aegean
                * v2:
                *   Balinese
                * etc.
                */
                var codename = "abyssinian";


                return $"pirbanka.server.{codename}.{buildVersion}";
            }
        }
    }
}