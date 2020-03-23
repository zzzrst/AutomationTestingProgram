using System;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;

namespace UpdatePackager
{
    class UpdatePackager
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Getting all files that are zipped...");
            string[] zip = Directory.GetFiles(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),"*.zip");

            if (File.Exists("Contents"))
            {
                File.Delete("Contents");
            }

            Console.WriteLine("Hashing files...");
            using (var file = new StreamWriter("Contents"))
            {
                foreach (string toHash in zip)
                {
                    Console.WriteLine($"Hashing {Path.GetFileName(toHash)}");
                    file.WriteLine($"{Path.GetFileName(toHash)}\t{GetSHA3HashFromFile(toHash)}");
                }
            }
            File.SetAttributes("Contents", File.GetAttributes("Contents") | FileAttributes.ReadOnly);

            Console.WriteLine("Hashing complete...");
        }

        // <summary>
        /// Gets the SHA3 hash from file.
        /// Adapted from https://stackoverflow.com/a/16318156/1460422
        /// </summary>
        /// <param name="fileName">The filename to hash.</param>
        /// <returns>The SHA3 hash from file.</returns>
        private static string GetSHA3HashFromFile(string fileName)
        {
            FileStream file = new FileStream(fileName, FileMode.Open);
            SHA384 sha3 = new SHA384CryptoServiceProvider();
            byte[] byteHash = sha3.ComputeHash(file);
            file.Close();

            StringBuilder hashString = new StringBuilder();
            for (int i = 0; i < byteHash.Length; i++)
                hashString.Append(byteHash[i].ToString("x2"));
            return hashString.ToString();
        }
    }
}
