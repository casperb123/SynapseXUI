using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace SynapseXUI.Entities
{
    public class Version : IEquatable<Version>
    {
        /// <summary>
        /// If it's the current version
        /// </summary>
        public bool IsCurrentVersion { get; private set; }
        /// <summary>
        /// The major part of the version
        /// </summary>
        public int Major { get; private set; }
        /// <summary>
        /// The minor part of the version
        /// </summary>
        public int Minor { get; private set; }
        /// <summary>
        /// The build part of the version
        /// </summary>
        public int Build { get; private set; }
        /// <summary>
        /// The revision part of the version
        /// </summary>
        public int Revision { get; private set; }

        /// <summary>
        /// Creates a new version
        /// </summary>
        /// <param name="major">The major part of the version</param>
        /// <param name="isCurrentVersion">If it's the current version</param>
        public Version(int major, bool isCurrentVersion = false)
        {
            Major = major;
            IsCurrentVersion = isCurrentVersion;
        }

        /// <summary>
        /// Creates a new version
        /// </summary>
        /// <param name="major">The major part of the version</param>
        /// <param name="minor">The minor part of the version</param>
        /// <param name="isCurrentVersion">If it's the current version</param>
        public Version(int major, int minor, bool isCurrentVersion = false) : this(major, isCurrentVersion)
        {
            Minor = minor;
        }

        /// <summary>
        /// Creates a new version
        /// </summary>
        /// <param name="major">The major part of the version</param>
        /// <param name="minor">The minor part of the version</param>
        /// <param name="build">The build part of the version</param>
        /// <param name="isCurrentVersion">If it's the current version</param>
        public Version(int major, int minor, int build, bool isCurrentVersion = false) : this(major, minor, isCurrentVersion)
        {
            Build = build;
        }

        /// <summary>
        /// Creates a new version
        /// </summary>
        /// <param name="major">The major part of the version</param>
        /// <param name="minor">The minor part of the version</param>
        /// <param name="build">The build part of the version</param>
        /// <param name="revision">The revision part of the version</param>
        /// <param name="isCurrentVersion">If it's the current version</param>
        public Version(int major, int minor, int build, int revision, bool isCurrentVersion = false) : this(major, minor, build, isCurrentVersion)
        {
            Revision = revision;
        }

        /// <summary>
        /// Checks if the first version is newer than the second version
        /// </summary>
        /// <param name="first">The first version to check</param>
        /// <param name="second">The second version to check</param>
        /// <returns>true if the first version is newer than the seconds version, false otherwise</returns>
        public static bool operator >(Version first, Version second)
        {
            if (first.Major > second.Major)
                return true;
            else if (first.Minor > second.Minor &&
                first.Major == second.Major)
            {
                return true;
            }
            else if (first.Build > second.Build &&
                first.Major == second.Major &&
                first.Minor == second.Minor)
            {
                return true;
            }
            else if (first.Revision > second.Revision &&
                first.Major == second.Major &&
                first.Minor == second.Minor &&
                first.Build == second.Build)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Checks if the first version is older than the second version
        /// </summary>
        /// <param name="first">The first version to check</param>
        /// <param name="second">The second version to check</param>
        /// <returns>true if the first version is older than the second version, false otherwise</returns>
        public static bool operator <(Version first, Version second)
        {
            if (first.Major < second.Major)
                return true;
            else if (first.Minor < second.Minor &&
                second.Major == first.Major)
            {
                return true;
            }
            else if (second.Build > first.Build &&
                second.Major == first.Major &&
                second.Minor == first.Minor)
            {
                return true;
            }
            else if (second.Revision > first.Revision &&
                second.Major == first.Major &&
                second.Minor == first.Minor &&
                second.Build == first.Build)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Checks if the first and second version is the same
        /// </summary>
        /// <param name="first">The first version to check</param>
        /// <param name="second">The second version to check</param>
        /// <returns>true if the first and second version is the same, false otherwise</returns>
        public static bool operator ==(Version first, Version second)
        {
            if (first.Major == second.Major &&
                first.Minor == second.Minor &&
                first.Build == second.Build &&
                first.Revision == second.Revision)
                return true;

            return false;
        }

        /// <summary>
        /// Checks if the first and second version isn't the same
        /// </summary>
        /// <param name="first">The first version to check</param>
        /// <param name="second">The second version to check</param>
        /// <returns>true if the first and second version isn't the same, false otherwise</returns>
        public static bool operator !=(Version first, Version second)
        {
            if (first.Major != second.Major &&
                first.Minor != second.Minor &&
                first.Build != second.Build &&
                first.Revision != second.Revision)
                return true;

            return false;
        }

        /// <summary>
        /// Gets the version string
        /// </summary>
        /// <returns>The version string</returns>
        public override string ToString()
        {
            string versionTxt = $"{Major}.{Minor}";

            if (Build > 0)
            {
                versionTxt += $".{Build}";
                if (Revision > 0)
                    versionTxt += $".{Revision}";
            }

            return versionTxt;
        }

        /// <summary>
        /// Converts a string to a version
        /// </summary>
        /// <param name="version">The version string to convert</param>
        /// <param name="isCurrentVersion">If it's the current version</param>
        /// <returns>The converted string version</returns>
        public static Version ConvertToVersion(string version, bool isCurrentVersion = false)
        {
            version = version.Replace("v", "").Split('-')[0];

            Regex regex = new Regex(@"\d+(?:\.\d+)+");

            if (regex.IsMatch(version))
            {
                var splitted = version.Split('.').Select(int.Parse).ToArray();

                if (splitted.Length == 1)
                    return new Version(splitted[0], isCurrentVersion);
                else if (splitted.Length == 2)
                    return new Version(splitted[0], splitted[1], isCurrentVersion);
                else if (splitted.Length == 3)
                    return new Version(splitted[0], splitted[1], splitted[2], isCurrentVersion);
                else if (splitted.Length >= 4)
                    return new Version(splitted[0], splitted[1], splitted[2], splitted[3], isCurrentVersion);
            }

            throw new FormatException("Version was in a invalid format");
        }

        /// <summary>
        /// Checks if the object is the same version
        /// </summary>
        /// <param name="obj">The object to check</param>
        /// <returns>true if the object is the same version, false otherwise</returns>
        public override bool Equals(object obj)
        {
            return Equals(obj as Version);
        }

        /// <summary>
        /// Checks if the version is the same
        /// </summary>
        /// <param name="other">The version to check</param>
        /// <returns>true if the version is the same, false otherwise</returns>
        public bool Equals(Version other)
        {
            return other != null &&
                   Major == other.Major &&
                   Minor == other.Minor &&
                   Build == other.Build &&
                   Revision == other.Revision;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
