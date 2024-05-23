using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RealDream.AI
{
    public class CacheInfo
    {
        public enum EState
        {
            Loaded,
            InQueue,
            Dealing,
            Done
        }

        public int Idx;
        public string Path;
        public string Hash;
        public Texture2D Texture;
        public EState State = EState.Loaded;
        public bool IsLoaded => State == EState.Loaded;
        public bool IsInQueue => State == EState.InQueue;
        public bool IsDealing => State == EState.Dealing;
        public bool IsDone => State == EState.Done;

        public string FileName => System.IO.Path.GetFileName(Path);

        public void OnResult()
        {
            State = EState.Done;
        }

        public override string ToString()
        {
            return $"Idx:{Idx} Path:{Path} ";
        }
    }

    public class CacheManager
    {
        private List<CacheInfo> _videoData = new List<CacheInfo>();
        private List<CacheInfo> _modelData = new List<CacheInfo>();
        public static CacheManager Instance { get; private set; }

        public static List<CacheInfo> VideoData => Instance._videoData;
        public static List<CacheInfo> ModelData => Instance._modelData;

        public static void Init()
        {
            Instance = new CacheManager();
            CacheUtil.InitCache();
            EventUtil.Trigger(EGameEvent.OnCacheLoadStart);
            EditorCoroutineUtil.StartCoroutine(Instance._Init());
        }

        public static void OnResult(string hash, bool isVideo)
        {
            var datas = isVideo ? VideoData : ModelData;
            var data = datas.Where(a => a.Hash == hash).FirstOrDefault();
            if (data != null)
            {
                data.OnResult();
            }
            else
            {
                Debug.LogError("can not find result " + hash);
            }
        }

        public static void OnAdd(string path, System.Action callback = null)
        {
            EventUtil.Trigger(EGameEvent.OnCacheAdd,path);
        }

        private IEnumerator _Init()
        {
            var paths = PathUtil.GetAllPath(PathUtil.GetInputDir(),
                $"{FileTransferUtil.VideoExtPattern}|{FileTransferUtil.ModelExtPattern}");
            for (int i = 0; i < paths.Count; i++)
            {
                EventUtil.Trigger(EGameEvent.OnCacheLoadProgress, i * 1.0f / paths.Count);
                yield return LoadCache(paths[i]);
            }

            EventUtil.Trigger(EGameEvent.OnCacheLoadDone);
        }

        private IEnumerator LoadCache(string path, System.Action callback = null)
        {
            var isModel = FileTransferUtil.IsModel(path);
            var data = isModel ? _modelData : _videoData;
            var list = new List<Texture2D>();
            var hashes = new List<string>();

            yield return SnapshotUtil.GetOrCreateCache(path, list, hashes);
            data.Add(new CacheInfo()
            {
                Idx = data.Count,
                Path = path,
                Hash = hashes[0],
                Texture = list[0],
            });
            callback?.Invoke();
        }
    }
}