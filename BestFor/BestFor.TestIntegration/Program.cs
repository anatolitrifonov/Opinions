using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BestFor.TestIntegration
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //BlobTests.TestResizeUserFile();
            BlobTests.TestUploadUserFile();
            //BlobTests.TestListAllBlobs();
            // BlobTests.TestClearAllBlobs();
            BlobTests.TestLoadUserFile();
            Console.WriteLine("Done.");
            Console.ReadLine();
        }
    }
}
