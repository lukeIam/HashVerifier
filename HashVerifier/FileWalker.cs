using System;
using System.IO;
using System.Text;


namespace HashVerifier
{
    class FileWalker
    {
        public static bool Check(DirectoryInfo folder, FileInfo hashFile, int ignoreDepth, out string log)
        {
            StringBuilder logbuilder = new StringBuilder();

            if (folder == null || !folder.Exists)
            {
                throw new ArgumentException("Folder does not exists.");
            }

            bool result = true;

            StoredHashes knownHashes = new StoredHashes(hashFile);
            knownHashes.Load();

            string[] files = Directory.GetFiles(folder.ToString(), "*", SearchOption.AllDirectories);

            foreach (string file in files)
            {
                FileInfo fi = new FileInfo(file);

                if (fi.FullName == hashFile.FullName)
                {
                    continue;
                }

                //Make absolute path relative
                string fileName = fi.FullName.Replace(folder.FullName + "\\", "");

                string currentHash = HashProvider.GetHashSha256(fi.OpenRead());
                string knownHash = knownHashes.GetHash(fileName);

                if (String.IsNullOrEmpty(knownHash) || currentHash != knownHash)
                {
                    //miss
                    result = false;
                    logbuilder.AppendLine("Fail: " + fileName);
                }
                else
                {
                    //hit
                    logbuilder.AppendLine("Success: " + fileName);
                }
            }

            log = logbuilder.ToString();

            return result;
        }

        public static void Add(DirectoryInfo folder, FileInfo hashFile, int ignoreDepth, out string log)
        {
            StringBuilder logbuilder = new StringBuilder();

            if (folder == null || !folder.Exists)
            {
                throw new ArgumentException("Folder does not exists.");
            }

            StoredHashes knownHashes = new StoredHashes(hashFile);

            string[] files = Directory.GetFiles(folder.ToString(), "*", SearchOption.AllDirectories);

            foreach (string file in files)
            {
                FileInfo fi = new FileInfo(file);

                if (fi.FullName == hashFile.FullName)
                {
                    continue;
                }
                
                //Make absolute path relative
                string fileName = fi.FullName.Replace(folder.FullName+"\\", "");

                string currentHash = HashProvider.GetHashSha256(fi.OpenRead());
                string knownHash = knownHashes.GetHash(fileName);

                if (String.IsNullOrEmpty(knownHash))
                {
                    //miss
                    knownHashes.SetHash(fileName, currentHash);
                    logbuilder.AppendLine("Added: " + fileName);
                }
                else if(currentHash != knownHash)
                {
                    //hit
                    knownHashes.SetHash(fileName, currentHash);
                    logbuilder.AppendLine("Updated: " + fileName);
                }
            }
            knownHashes.Save();
            log = logbuilder.ToString();
        }
    }
}
