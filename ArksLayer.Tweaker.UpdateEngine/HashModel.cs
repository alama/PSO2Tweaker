using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace ArksLayer.Tweaker.UpdateEngine
{
    /// <summary>
    /// A class containing values required for mass hashing operation.
    /// </summary>
    internal class HashModel
    {
        /// <summary>
        /// The full path to the file to be hashed.
        /// </summary>
        public string FileName { set; get; }

        /// <summary>
        /// Spawns a background process for processing the file content for this hashing class, 
        /// which can later be accessed from the ComputeTask property.
        /// At the same time, creates patch Key and human-readable MD5 Hash from the result.
        /// </summary>
        public void ComputeHash(byte[] file, string gameDirectory)
        {
            this.ComputeTask = Task.Run(() =>
            {
                using (var ram = new MemoryStream(file))
                using (var md5 = MD5.Create())
                {
                    var hashBinary = md5.ComputeHash(ram);
                    this.Key = this.FileName.Remove(0, gameDirectory.Length + 1).Replace('\\', '/');
                    this.Hash = BitConverter.ToString(hashBinary).Replace("-", "").ToLower();
                }
            });
        }

        /// <summary>
        /// Patch key to be compared to the patchlist.
        /// </summary>
        public string Key { set; get; }

        /// <summary>
        /// Human-readable MD5 hash for the patch file.
        /// </summary>
        public string Hash { set; get; }

        /// <summary>
        /// Compute job for generating the Key and Hash property in background.
        /// </summary>
        public Task ComputeTask { get; private set; }
    }
}
