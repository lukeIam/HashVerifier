using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace HashVerifier
{
    public class StoredHashes
    {
        private Dictionary<string, string> _knownHashes;
        private readonly FileInfo _file;

        public StoredHashes(FileInfo file)
        {
            if (file == null)
            {
                throw new ArgumentException("Invalid known hash file.");
            }

            this._file = file;
            _knownHashes = new Dictionary<string, string>();
        }

        public String GetHash(string fileName)
        {
            string result = "";
            _knownHashes.TryGetValue(fileName, out result);
            return result;
        }

        public void SetHash(string fileName, string hash)
        {
            if (_knownHashes.ContainsKey(fileName))
            {
                _knownHashes[fileName] = hash;
            }
            else
            {
                _knownHashes.Add(fileName, hash);
            }
        }

        public void Save()
        {
            if (_file.Exists)
            {
                _file.Delete();
            }
            SerializeDict(_knownHashes, _file);
        }

        public void Load()
        {
            _knownHashes = _file.Exists ? DeserializeDict(_file) : new Dictionary<string, string>();
        }

        private Dictionary<string, string> DeserializeDict(FileInfo file)
        {
            if (file == null || !file.Exists)
            {
                throw new ArgumentException("Invalid source hash file.");
            }

            XmlSerializer serializer = new XmlSerializer(typeof(List<FileHashPair>));

            List<FileHashPair> data = null;

            using (var fileStream = file.OpenRead())
            {
                try
                {
                    data = serializer.Deserialize(fileStream) as List<FileHashPair>;
                }
                catch (InvalidOperationException)
                {
                    data = null;
                }
            }

            if (data == null)
            {
                return new Dictionary<string, string>();
            }
            Dictionary<string, string> dic = new Dictionary<string, string>(data.Count);
            foreach (var pair in data)
            {
                dic.Add(pair.Key, pair.Value);
            }

            return dic;
        }

        public void SerializeDict(Dictionary<string, string> dict, FileInfo file)
        {

            if (file == null)
            {
                throw new ArgumentException("Invalid target hash file.");
            }

            using (var fileStream = file.OpenWrite())
            {
                List<FileHashPair> entries = new List<FileHashPair>(dict.Count);
                entries.AddRange(dict.Select(kvp => new FileHashPair() { Key = kvp.Key, Value = kvp.Value }));

                XmlSerializer serializer = new XmlSerializer(typeof(List<FileHashPair>));

                serializer.Serialize(fileStream, entries);
            }
            
        }

        public class FileHashPair
        {
            public string Key { get; set; }
            public string Value { get; set; }
            public FileHashPair()
            {
            }
        }
    }
}
