using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;

namespace Hashes
{
    class Hashes
    {
        static void Main(string[] args)
        {
            string path = "";
            string result = new FolderHash().hashFolder(path);
            Console.WriteLine("Hash of the folder '{0}' is\n{1}", path, result);
        }
    }

    class FolderHash
    {
        public string hashFolder(string path)
        {
            // folderName: {file1, file2, ..., [folder1: folderHash1], [folder2: folderHash2]}
            string directory_struct = path + ": {";
            string[] folders = Directory.GetDirectories(path);
            string[] files = Directory.GetFiles(path);
            
            // Start task for folder's hashes
            Task<string>[] folders_hash = new Task<string>[folders.Length];
            if (folders.Length != 0)
            {
                for (int i = 0; i < folders.Length; ++i)
                {
                    int position = i;
                    folders_hash[position] = Task.Run(() =>
                    {
                        return hashFolder(folders[position]);
                    });
                }
            }

            // Add files from folder to struct 
            for (int i = 0; i < files.Length; ++i)
            {
                directory_struct += files[i];
                if (i != files.Length - 1)
                {
                    directory_struct += ", ";
                } 
            }

            // Get hashes from folders
            if (folders.Length != 0)
            {
                directory_struct += ", ";
                for (int i = 0; i < folders.Length; ++i)
                {
                    folders_hash[i].Wait();
                    directory_struct += "[" + folders_hash[i].Result + "]";
                    if (i != folders.Length - 1)
                    {
                        directory_struct += ", ";
                    }
                }
            }
            directory_struct += "}";  
            return makeHash(directory_struct);
        }

        private string makeHash(string input)
        {
            using (MD5 md5hash = MD5.Create())
            {
                byte[] data = md5hash.ComputeHash(Encoding.UTF8.GetBytes(input));
                StringBuilder sBuilder = new StringBuilder();
                for (int i = 0; i < data.Length; i++)
                {
                    sBuilder.Append(data[i].ToString("x2"));
                }
                return sBuilder.ToString();
            }
        }
    }
}