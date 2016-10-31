﻿using System;
using System.IO;
using BestFor.Dto;
using BestFor.Services.Blobs;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BestFor.TestIntegration
{
    public class BlobTests
    {
        public static void TestUploadUserFile()
        {
            var settings = Common.AppSettings.ReadSettings();
            var currentDirectory = Directory.GetCurrentDirectory();
            using (var fileStream = File.OpenRead(currentDirectory + "\\TestData\\mister.jpg"))
            {
                var blobService = new BlobService(settings, null);

                var blobData = new BlobDataDto() { FileName = "mister.jpg", Stream = fileStream };

                blobService.SaveUserProfilePicture("Anatoli", blobData);
            }
        }

        public static void TestLoadUserFile()
        {
            var settings = Common.AppSettings.ReadSettings();

            var blobService = new BlobService(settings, null);

            string path = @"C:\Temp\z.jpg";
            if (File.Exists(path)) File.Delete(path);

            var result = blobService.FindUserProfilePicture("Anatoli2");

 
            using (var fileStream = File.OpenWrite(path))
            {
                result.Stream.Position = 0;
                result.Stream.CopyTo(fileStream);
            }
        }

        public static void TestListAllBlobs()
        {
            var settings = Common.AppSettings.ReadSettings();

            var blobService = new BlobService(settings, null);

            var result = blobService.ListAllBlobs();

            foreach(var item in result)
            {
                Console.WriteLine(item);
            }
        }

        public static void TestClearAllBlobs()
        {
            var settings = Common.AppSettings.ReadSettings();

            var blobService = new BlobService(settings, null);

            var result = blobService.ClearAllBlobs();

            foreach (var item in result)
            {
                Console.WriteLine(item);
            }
        }
    }
}