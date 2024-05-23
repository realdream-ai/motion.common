using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace RealDream {
    public static partial class PathUtil {
        
        private static string _relPath;

        private static string RelPath {
            get {
                if (_relPath == null) {
#if UNITY_5_3_OR_NEWER
                    _relPath = UnityEngine.Application.dataPath.Substring(0, UnityEngine.Application.dataPath.Length - 7);
#else
                    _relPath = ".";
#endif
                }

                return _relPath;
            }
        }

        public static string GetAbsPath(string assetPath) {
            assetPath = assetPath.Replace("\\", "/");
            var paths = assetPath.Split('/');
            List<string> strs = new List<string>();
            foreach (var path in paths) {
                if (path == ".") {
                    continue;
                }
                if (path == "..") {
                    try {
                        strs.RemoveAt(strs.Count - 1); }
                    catch (Exception e) {
                        Console.WriteLine(e);
                        throw;
                    }
                    continue;
                }

                strs.Add(path);
            }

            var finalPath = string.Join("/", strs);
            return finalPath;
        }
        public static string GetInputDir(string path)
        {
#if UNITY_5_3_OR_NEWER
            var fileName = Path.GetFileName(path);
            var dir = "../Input/";
            #if UNITY_STANDALONE_OSX && !UNITY_EDITOR
                dir = "../"+dir;
            #endif
            var dstPath = Path.GetFullPath(Path.Combine(UnityEngine. Application.dataPath, dir+fileName));
            return dstPath;
#else
            return Path.Combine("./Input",path);
#endif
        }

        public static string GetInputDir()
        {
            return Path.GetDirectoryName(PathUtil.GetInputDir("a.txt"));
        }

       

        public static string GetFullPath(string assetPath) {
            return Path.Combine(RelPath, assetPath);
        }

        public static List<string> GetAllPath(string path, string exts, bool isRelPath = true)
        {
            var set = new HashSet<string>();
            PathUtil.Walk(path,exts,(p)=>set.Add(p));
            var paths = set.ToList();
            paths.Sort();
            if (isRelPath)
                paths = paths.Select(a => PathUtil.GetAssetPath(a)).ToList();
            else
                paths = paths.Select(a => PathUtil.GetFullPath(a)).ToList();
            return paths;
        }
        public static string GetParentDir(string dir) {
            dir = dir.Replace("\\", "/");
            if (dir.EndsWith("/")) {
                dir = dir.Substring(0, dir.Length - 1);
            }

            dir = dir.Substring(0, dir.LastIndexOf("/"));
            return dir;
        }

        public static string GetAssetPath(string fullPath) {
            fullPath = fullPath.Replace("\\", "/");
            if (fullPath.StartsWith(RelPath)) {
                return fullPath.Substring(RelPath.Length + 1);
            }

            return fullPath;
        }

        public static void CreateDir(string dir,bool isDelete = false)
        {
            if (isDelete&& Directory.Exists(dir))
            {
                Directory.Delete(dir,true);
            }

            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
        }
        public static void Walk(string path, string exts, System.Action<string> callback, bool isIncludeDir = false, bool isSaveAssets = false, bool isAllDirs = true) {
            bool isAll = string.IsNullOrEmpty(exts) || exts == "*" || exts == "*.*";
            string[] extList = exts.Replace("*", "").Split('|');
            if (Directory.Exists(path)) {
                var searchOption = isAllDirs ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
                List<string> paths = new List<string>();
                string[] lst = exts.Split('|');
                foreach (var ext in lst) {
                    // 如果选择的是文件夹
                    string[] files = Directory.GetFiles(path, ext, searchOption);
                    paths.AddRange(files);
                    if (isIncludeDir) {
                        string[] directories = Directory.GetDirectories(path, ext, searchOption);
                        paths.AddRange(directories);
                    }
                }

                foreach (var item in paths) {
                    if (callback != null) {
                        callback(item);
                    }
                }

                if (isSaveAssets) { }
            }
            else {
                if (isAll) {
                    if (callback != null) {
                        callback(path);
                    }
                }
                else {
                    // 如果选择的是文件
                    foreach (var ext in extList) {
                        if (path.EndsWith(ext, StringComparison.OrdinalIgnoreCase)) {
                            if (callback != null) {
                                callback(path);
                            }
                        }
                    }
                }

                if (isSaveAssets) {
                }
            }
        }
    }
}