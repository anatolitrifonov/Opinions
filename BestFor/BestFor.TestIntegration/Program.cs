using BestFor.TestIntegration.Users;
using System;
using System.Threading.Tasks;

namespace BestFor.TestIntegration
{
    public class Program
    {
        public static void Main(string[] args)
        {
            UsersHelper usersHelper = new UsersHelper();
            Task t = usersHelper.AddTwentyUsers();
            t.Wait();


            //BlobTests.TestResizeUserFile();
            //BlobTests.TestUploadUserFile();
            //BlobTests.TestListAllBlobs();
            // BlobTests.TestClearAllBlobs();
            // BlobTests.TestLoadUserFile();
            Console.WriteLine("Done.");
            Console.ReadLine();
        }
    }
}
