using System;
using System.IO;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using RealDream;
using RealDream.AI;

using UnityEngine;
using UnityEngine.Video;

namespace RealDream.AI
{
    public class SnapshotUtil
    {
        public class VideoSnapshotTaker : MonoBehaviour
        {
            private VideoPlayer videoPlayer;

            public IEnumerator CreateSnapshot(string videoPath, string savePath)
            {
                videoPlayer = gameObject.AddComponent<VideoPlayer>();
                Debug.Log($"CreateSnapshot {PathUtil.GetInputDir(videoPath)} => {savePath}");

#if UNITY_STANDALONE_LINUX
                System.IO.File.WriteAllBytes(savePath,  CreateWhiteTex().EncodeToPNG());
                //yield  break;
#endif
                // Prepare the video player
                videoPlayer.url = PathUtil.GetInputDir(videoPath) ;
                videoPlayer.Prepare();
                while (!videoPlayer.isPrepared)
                {
                    yield return null;
                }

                videoPlayer.Play();
                yield return CaptureFrame(savePath);
            }

            private static Texture2D CreateWhiteTex()
            {
                Texture2D tex = new Texture2D(2, 2);
                Color[] colors = new Color[4];
        
                for (int i = 0; i < colors.Length; i++)
                {
                    colors[i] = Color.white;
                }
                tex.SetPixels(colors);
                tex.Apply();
                return tex;
            }


            private IEnumerator CaptureFrame(string savePath)
            {
                var renderTexture =  RenderTexture.GetTemporary((int)videoPlayer.width / 4, (int)videoPlayer.height / 4, 24);
                videoPlayer.targetTexture = renderTexture;
                yield return new WaitUntil(() => videoPlayer.isPlaying);
                yield return null;

                yield return new WaitForEndOfFrame();

                SaveRT(savePath, renderTexture);

                videoPlayer.Stop();
            }

            private void SaveRT(string savePath, RenderTexture renderTexture)
            {
                RenderTexture.active = renderTexture;

                Texture2D videoFrame =
                    new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGB24, false);
                videoFrame.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
                videoFrame.Apply();

                byte[] bytes = videoFrame.EncodeToPNG();
                System.IO.File.WriteAllBytes(savePath, bytes);

                videoPlayer.targetTexture = null;
                RenderTexture.active = null;
                Destroy(videoFrame);
                RenderTexture.ReleaseTemporary(renderTexture);
            }
        }

        public class ModelSnapshotTaker : MonoBehaviour
        {
            private const float ScalePercent = 0.85f;
            public IEnumerator CreateSnapshot(string path, string savePath)
            {
                List<GameObject> objs = new List<GameObject>();
                Debug.Log($"LoadMode {path}");
                if(objs.Count == 0) 
                    yield break;
                Debug.Log($"CreateSnapshot {path} => {savePath}");
                yield return CaptureFrame(savePath,objs[0],512);
            }

            private IEnumerator CaptureFrame(string savePath,GameObject targetObject,int textureSize)
            {
                Camera snapCamera = new GameObject("SnapshotCamera").AddComponent<Camera>();
                var rt = RenderTexture.GetTemporary(textureSize, textureSize);
                snapCamera.targetTexture = rt;
                snapCamera.orthographic = true;
                targetObject.transform.position = new Vector3(-1000, -1000, -1000);
                yield return new WaitForEndOfFrame();
                var rds = targetObject.GetComponentsInChildren<Renderer>();
                if (rds.Length > 0)
                {
                    Bounds bounds = rds[0].bounds;
                    for (int i = 1; i < rds.Length; i++)
                    {
                        var bd = rds[i].bounds;
                        bounds.Encapsulate(bd);
                    }
                    float desiredHeight = 2.0f * snapCamera.orthographicSize * ScalePercent;
                    var halfHeight = desiredHeight * 0.5f;
                    var scale = Mathf.Max(bounds.size.y, bounds.size.x);
                    float scaleFactor = desiredHeight /scale;
                    targetObject.transform.localScale = scaleFactor * Vector3.one;
                    targetObject.transform.eulerAngles = new Vector3(0, 180, 0);
                    snapCamera.transform.position =Vector3.up*halfHeight +  targetObject.transform.position - new Vector3(0, 0, bounds.size.magnitude * 2) ;
                    snapCamera.transform.LookAt(targetObject.transform.position + Vector3.up*halfHeight);
                }

                snapCamera.Render();

                RenderTexture.active = snapCamera.targetTexture;
                Texture2D snapshot = new Texture2D(textureSize, textureSize, TextureFormat.RGB24, false);
                snapshot.ReadPixels(new Rect(0, 0, textureSize, textureSize), 0, 0);
                snapshot.Apply();

                byte[] png = snapshot.EncodeToPNG();

                File.WriteAllBytes(savePath, png);
                RenderTexture.active = null;
                RenderTexture.ReleaseTemporary(rt);
                Destroy(snapshot);
                Destroy(snapCamera.gameObject);

            }
        }
        

        public static IEnumerator GetOrCreateCache(string path, List<Texture2D> textures,List<string> hashs)
        {
            var hash = HashUtil.CalcHash(path);
            hashs.Add(hash);
            byte[] bytes = null;
            if (CacheUtil.TryGetCache(hash, out var result))
            {
                bytes = result.Bytes;
                Debug.Log("Load cache " + result.FileName);
            }
            else
            {
                PathUtil.CreateDir(CacheUtil.CacheDir);
                var savePath = Path.Combine(CacheUtil.CacheDir, CacheUtil.GetCachePath(path));
                yield return CreateSnapshot(path, savePath);
                bytes = File.ReadAllBytes(savePath);
            }

            var texture = LoadTexture(bytes);
            textures.Add(texture);
        }

        private static IEnumerator CreateSnapshot(string videoPath, string savePath)
        {
            Debug.Log($"Create cache {videoPath} => {savePath}");
            var type = FileTransferUtil.GetFileType(videoPath);
            if (type == FileTransferUtil.EFileType.Model)
            {
                var go = new GameObject();
                yield return go.AddComponent<ModelSnapshotTaker>().CreateSnapshot(videoPath, savePath);
                GameObject.Destroy(go);
            }
            else
            {
                var go = new GameObject();
                yield return go.AddComponent<VideoSnapshotTaker>().CreateSnapshot(videoPath, savePath);
                GameObject.Destroy(go);
            }
        }

        private static Texture2D CreateSnapshot(GameObject obj, string path)
        {
            RenderTexture renderTexture = new RenderTexture(Screen.width, Screen.height, 24);
            Camera camera = Camera.main;

            RenderTexture currentRT = RenderTexture.active;
            CameraClearFlags clearFlags = camera.clearFlags;
            Color backgroundColor = camera.backgroundColor;

            camera.targetTexture = renderTexture;
            RenderTexture.active = renderTexture;

            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = Color.clear;

            camera.Render();

            Texture2D snapshot =
                new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGB24, false);

            snapshot.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
            snapshot.Apply();

            camera.targetTexture = null;
            RenderTexture.active = currentRT;
            camera.clearFlags = clearFlags;
            camera.backgroundColor = backgroundColor;

            byte[] bytes = snapshot.EncodeToPNG();

            File.WriteAllBytes(path, bytes);

            GameObject.Destroy(renderTexture);
            return snapshot;
        }

        public static Texture2D LoadTexture(byte[] fileData)
        {
            Texture2D tex = new Texture2D(2, 2);
            if (tex.LoadImage(fileData))
            {
                return tex;
            }

            return null;
        }

        public static Texture2D LoadTexture(string filePath)
        {
            if (File.Exists(filePath))
            {
                byte[] fileData = File.ReadAllBytes(filePath);
                return LoadTexture(fileData);
            }

            return null;
        }
    }
}