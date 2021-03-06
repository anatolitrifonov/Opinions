﻿using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace BestFor.Common
{
    /// <summary>
    /// Represents Typed general application settings. Contains static methods to load appsettings.
    /// </summary>
    public class AppSettings
    {
        private class IOptionsImplementation : IOptions<AppSettings>
        {
            public AppSettings AppSettings { get; set; }

            public AppSettings Value { get { return AppSettings; } }
        }

        public string AmazonAccessKeyId { get; set; }

        public string AmazonAssociateId { get; set; }

        public string AmazonSecretKey { get; set; }

        public string EmailServerAddress { get; set; }

        public int EmailServerPort { get; set; }

        public string EmailServerUser { get; set; }

        public string EmailServerPassword { get; set; }

        public string EmailFromAddress { get; set; }

        public string MiscSetting { get; set; }

        public bool EnableFacebookSharing { get; set; }

        /// <summary>
        /// Used only for sharing on facebook.
        /// </summary>
        public string FullDomainAddress { get; set; }

        /// <summary>
        /// Default container name on the azure
        /// </summary>
        public string AzureBlobsContainerName { get; set; }

        /// <summary>
        /// Connection string to azure storage
        /// </summary>
        public string AzureBlobsConnectionString { get; set; }

        /// <summary>
        /// Blobs container Url
        /// </summary>
        public string AzureBlobsContainerUrl { get; set; }

        /// <summary>
        /// Default avatar width
        /// </summary>
        public int AvatarWidth { get; set; } = 100;

        /// <summary>
        /// Default avatar height
        /// </summary>
        public int AvatarHeight { get; set; } = 100;

        /// <summary>
        /// Used only for debugging to track when an instance of this class is created.
        /// </summary>
        /// <remarks>
        /// Put the break point here for debugging.
        /// </remarks>
        public AppSettings() { }

        /// <summary>
        /// This is used only for unit tests. Basically this reads the settings assuming the file in is the current folder.
        /// Usually instance is created by injection.
        /// </summary>
        /// <returns></returns>
        public static IOptions<AppSettings> ReadSettings()
        {
            var currentDirectory = Directory.GetCurrentDirectory();
            var builder = new ConfigurationBuilder()
                .SetBasePath(currentDirectory)
                .AddJsonFile("appsettings.json")
                .AddJsonFile("appsettings.secret.json", true);
            var appSettings = new AppSettings();

            builder.Build().GetSection("AppSettings").Bind(appSettings);

            return new IOptionsImplementation() { AppSettings = appSettings };
        }
    }
}
