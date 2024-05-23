using System.Collections.Generic;
using System.IO;
using RealDream.Network;

namespace RealDream
{
    public class FileTransferUtil
    {
        public enum EFileType
        {
            Unknown,
            Video,
            Model,
        }

        static HashSet<string> _modelsExt = new HashSet<string>() { "fbx" };
        static HashSet<string> _videosExt = new HashSet<string>() { "mp4" };

        public static string VideoExtPattern => "*.mp4";
        public static string ModelExtPattern => "*.fbx|*.FBX";
        public static bool IsVideo(string path) => GetFileType(path) == EFileType.Video;
        public static bool IsModel(string path) => GetFileType(path) == EFileType.Model;
        public static EFileType GetFileType(string path)
        {
            var fileName = Path.GetFileName(path);
            if (!fileName.Contains(".")) return EFileType.Unknown;
            var ext = fileName.Substring(fileName.LastIndexOf(".")+1);
            ext = ext.ToLower();
            if (_modelsExt.Contains(ext)) return EFileType.Model;
            if (_videosExt.Contains(ext)) return EFileType.Video;
            return EFileType.Unknown;
        }

        public static void UploadFile(string filePath, int idx = 0)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                Debug.LogError("filePath is Empty  ");
                return;
            }

            if (!File.Exists(filePath))
            {
                Debug.LogError("File not exits " + filePath);
                return;
            }

            Debug.Log($"UploadFile: {filePath}  idx={idx}");
            ClientSend.UpdateLoadFile(filePath, idx);
        }
    }
}