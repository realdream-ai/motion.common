using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using RealDream.Network;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace RealDream.AI
{
    public class CacheResult
    {
        public string FileName;
        public string Hash;
        public byte[] Bytes;
    }

    public class CacheUtil
    {
        // TODO support other platform eg :mac
        public static string CacheDir = "./Cache";
        public const string UpdateDir = "./Upload";
        public static Dictionary<string, CacheResult> Caches = new Dictionary<string, CacheResult>();

        public static string CacheSplitTag = "@@";
        public static void InitCache()
        {
            Caches.Clear();
            CreateDir(UpdateDir);
            CreateDir(CacheDir);

            var files = Directory.GetFiles(CacheDir, "*.png");
            MsgUtil.Log("Init Cache count " + files.Length);
            foreach (var path in files)
            {
                var strs = Path.GetFileNameWithoutExtension(path).Split(CacheUtil.CacheSplitTag);
                if (strs.Length != 2)
                {
                    Debug.LogError("Cache file illegal: " + path);
                    continue;
                }

                var hash = strs[0];
                var fileName = strs[1];
                var bytes = File.ReadAllBytes(path);
                Caches[hash] = new CacheResult()
                {
                    Hash = hash,
                    FileName = fileName,
                    Bytes = bytes
                };
            }

            MsgUtil.Log("InitCache Done");
        }

        public static string GetCachePath(string path)
        {
            var hash = HashUtil. CalcHash(path);
            var fileName = Path.GetFileNameWithoutExtension(path);
            return $"{hash}@@{fileName}.png";
        }

        private static void CreateDir(string dir)
        {
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
        }

        public static void AddCache(CacheResult result)
        {
            Caches[result.Hash] = result;
        }

        public static bool TryGetCache(string hash, out CacheResult result)
        {
            if (Caches.TryGetValue(hash, out var value))
            {
                result = value;
                return true;
            }

            result = null;
            return false;
        }

    }
}