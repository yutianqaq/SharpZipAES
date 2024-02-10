using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SharpZipAES
{
    class Program
    {
        [DllImport("kernel32.dll", EntryPoint = "RtlZeroMemory")]
        public static extern bool RtlZeroMemory(IntPtr Destination, int Length);

        public static byte[] GenerateSalt()
        {
            byte[] data = new byte[32];
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            for (int i = 0; i < 10; i++)
            {
                rng.GetBytes(data);
            }
            return data;
        }

        public static void Encrypter(string inputFile, string password)
        {

            FileStream fsCrypt = new FileStream(Path.GetFileNameWithoutExtension(inputFile) + ".aes.zip", FileMode.Create); //Output.aes.zip
            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);

            //Setup AES256 CFB
            RijndaelManaged AES = new RijndaelManaged();
            AES.KeySize = 256;
            AES.BlockSize = 128;
            AES.Padding = PaddingMode.PKCS7;
            byte[] salt = GenerateSalt();
            var key = new Rfc2898DeriveBytes(passwordBytes, salt, 50000); //PBKDF2
            AES.Key = key.GetBytes(AES.KeySize / 8);
            AES.IV = key.GetBytes(AES.BlockSize / 8);
            AES.Mode = CipherMode.CFB;

            fsCrypt.Write(salt, 0, salt.Length);
            CryptoStream cs = new CryptoStream(fsCrypt, AES.CreateEncryptor(), CryptoStreamMode.Write);
            FileStream fs = new FileStream(inputFile, FileMode.Open);

            byte[] buffer = new byte[1048576]; //Allocate 1MB instead of the whole target file
            int read;

            try
            {
                while ((read = fs.Read(buffer, 0, buffer.Length)) > 0)
                {
                    cs.Write(buffer, 0, read);
                }
                fs.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e.Message);
            }
            finally
            {
                cs.Close();
                fsCrypt.Close();
            }

        }

        public static void Decrypter(string inputFile, string outputFile, string password)
        {
            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
            byte[] salt = new byte[32];

            FileStream fsCrypt = new FileStream(inputFile, FileMode.Open);
            fsCrypt.Read(salt, 0, salt.Length);
            RijndaelManaged AES = new RijndaelManaged();
            AES.KeySize = 256;
            AES.BlockSize = 128;
            var key = new Rfc2898DeriveBytes(passwordBytes, salt, 50000);
            AES.Key = key.GetBytes(AES.KeySize / 8);
            AES.IV = key.GetBytes(AES.BlockSize / 8);
            AES.Padding = PaddingMode.PKCS7;
            AES.Mode = CipherMode.CFB;

            CryptoStream cs = new CryptoStream(fsCrypt, AES.CreateDecryptor(), CryptoStreamMode.Read);
            FileStream fs = new FileStream(outputFile, FileMode.Create);

            byte[] buffer = new byte[1048576];
            int read;

            try
            {
                while ((read = cs.Read(buffer, 0, buffer.Length)) > 0)
                {
                    fs.Write(buffer, 0, read);
                }
            }
            catch (CryptographicException ex_CryptographicException)
            {
                Console.WriteLine("[-] Error decrypting the archive: " + ex_CryptographicException.Message);
                return; //Usually due to invalid key
            }
            catch (Exception e)
            {
                Console.WriteLine("[-] Error: " + e.Message);
                return;
            }

            try
            {
                cs.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine("[-] Error closing the crypto stream: " + e.Message);
                return;
            }
            finally
            {
                fs.Close();
                fsCrypt.Close();
            }
        }

        public static void Zipfiles(List<string> filepathlist, string zipnamepath, string passwordd)
        {
            try
            {
                using (var zip = ZipFile.Open(zipnamepath, ZipArchiveMode.Create))
                {

                    foreach (string filepath in filepathlist)
                    {
                        string filename = Path.GetFileName(filepath);
                        zip.CreateEntryFromFile(filepath, filename);
                    }
                }
                Encrypter(zipnamepath, passwordd);

                Console.WriteLine("[+] Packed compressed file to {0} succeeded", zipnamepath);
                Console.WriteLine("[+] Wrote encrypted archive " + Path.GetFileNameWithoutExtension(zipnamepath) + ".aes.zip to disk!");
                File.Delete(zipnamepath);


            }
            catch (Exception ex)
            {
                Console.WriteLine(" [-] Failed with error info: {0}", ex.Message);
            }

        }

        public static void CreateFromDirectory(string sourceDirectoryName, string destinationArchiveFileName, string password)
        {
            try
            {
                ZipFile.CreateFromDirectory(sourceDirectoryName, destinationArchiveFileName);
                Console.WriteLine("[+] Packed compressed directory to {0} succeeded", destinationArchiveFileName);
                Encrypter(destinationArchiveFileName, password);

                Console.WriteLine("[+] Wrote encrypted archive " + Path.GetFileNameWithoutExtension(destinationArchiveFileName) + ".aes.zip to disk!");
                File.Delete(destinationArchiveFileName);
            }
            catch (Exception ex)
            {
                Console.WriteLine(" [-] Failed with error info: {0}", ex.Message);
            }
        }

        static void Main(string[] args)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();


            Console.WriteLine("Author: Yutian");
            Console.WriteLine("Github: https://github.com/yutianqaq/SharpZipAES");
            string usage = "[-] Usage:\n" +
                "  SharpZipAES.exe encrypt <encryption key> <path to compress> \n" +
                "  SharpZipAES.exe decrypt <encryption key> <path to encrypted ZIP> ";

            if (args.Length < 3)
            {
                Console.WriteLine(usage);
                return;
            }

            if (args[0].ToLower() == "encrypt")
            {
                Console.WriteLine("[+] Wrote encrypted archive ");
                //Generate a random filename for the archive
                Random random = new Random();
                string characters = "0123456789";
                characters += "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
                characters += "abcdefghijklmnopqrstuvwxyz";
                StringBuilder randomName = new StringBuilder(8);
                for (int i = 0; i < 8; i++)
                {
                    randomName.Append(characters[random.Next(characters.Length)]);
                }
                string archiveName = randomName.ToString() + ".zip";
                string passwd = args[1];
                GCHandle handle = GCHandle.Alloc(passwd, GCHandleType.Pinned); //Pin the password

                if (!Path.HasExtension(args[2]))
                {
                    CreateFromDirectory(args[2], archiveName, passwd);
                }
                else
                {
                    Console.WriteLine("[+] Packed compressed file");
                    List<string> list = new List<string>();
                    for (int i = 2; i < args.Length; i++)
                    {
                        list.Add(args[i]);
                    }
                    Zipfiles(list, archiveName, passwd);
                }

                //Cleanup
                RtlZeroMemory(handle.AddrOfPinnedObject(), passwd.Length * 2); //Zero out the pinned password on the heap
                handle.Free();
                Console.WriteLine("[+] Removed encryption key from memory");
                Console.WriteLine("[+] Deleted unecrypted archive");

                Console.WriteLine("[+] Ready for exfil");


            }
            else if (args[0].ToLower() == "decrypt")
            {
                Console.WriteLine("[+] Decrypting " + args[2]);
                string[] fileName = (Path.GetFileNameWithoutExtension(args[2])).Split('.');
                string outFile = fileName[0] + ".zip";
                try
                {
                    Decrypter(args[2], outFile, args[1]);
                    Console.WriteLine("[+] Decrypted {0} successfully!", outFile);
                }
                catch
                {
                    Console.WriteLine("[-] Something went wrong decrypting the file.");
                    return;
                }
            }
            else
            {
                Console.WriteLine(usage);
                return;
            }


            stopwatch.Stop();
            Console.WriteLine("[+] Program run time is: " + stopwatch.Elapsed);

        }
    }
}
