using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Serialization;

namespace RealDream.AI
{
    public class BVHPreview : MonoBehaviour
    {
        public CachedInitRotation TemplateRots;
        public GameObject PreviewPrefab;
        public string FilePath;
        public BVHParser Parser;

        [HideInInspector] //
        public List<GameObject> Children = new List<GameObject>();

        public bool IsBlender => _anim == null;
        [HideInInspector] public float Scale = 1;
        [HideInInspector] public int FrameRate = 30;
        [HideInInspector] public int MaxCount = 10;
        [HideInInspector] public int Frame;

        private Animator _anim;

        void OnFrameChanged()
        {
            UpdateFrame(Frame);
        }

        private void Start()
        {
            Parse();
        }

        private void Update()
        {
            if (_anim == null) return;
            transform.eulerAngles = new Vector3(IsBlender ? 90 : 0, 0, 0);
            UpdateByFrame();
        }

        public void Clear()
        {
            foreach (var child in Children)
            {
                if (child != null)
                    GameObject.DestroyImmediate(child);
            }

            Children.Clear();
        }

        public void Parse()
        {
            if (!File.Exists(FilePath))
            {
                UnityEngine.Debug.Log("Can not find target file " + FilePath);
                return;
            }

            if (_anim != null)
            {
                GameObject.DestroyImmediate(_anim.gameObject);
            }
            if (PreviewPrefab != null)
            {
                var go = GameObject.Instantiate(PreviewPrefab);
                _anim = go.GetComponentInChildren<Animator>();
            }

            _initRotation = GetInitRotations(_anim);

            Clear();
            var text = File.ReadAllText(FilePath);
            Parser = new BVHParser(text);
            Children = new List<GameObject>();
            Iterate(Parser.root, null, (bone, parent) =>
            {
                var go = new GameObject(bone.name);
                Children.Add(go);
                bone.extraData = go;
                if (parent != null && parent.extraData != null)
                {
                    go.transform.SetParent((parent.extraData as GameObject).transform, false);
                }

                if (parent == null)
                {
                    go.transform.SetParent(transform, false);
                }

                var pos = GetOffset(new Vector3(bone.offsetX, bone.offsetY, bone.offsetZ));
                go.transform.localPosition = pos;
            });
            MaxCount = Parser.frames;
        }

        Dictionary<HumanBodyBones, Quaternion> GetInitRotations(Animator anim)
        {
            var rotations = new List<Quaternion>();
            rotations.Clear();
            for (int i = 0; i < (int)HumanBodyBones.LastBone; i++)
            {
                var boneEnum = (HumanBodyBones)i;
                var dstBone = anim.GetBoneTransform(boneEnum);
                if (dstBone == null)
                {
                    rotations.Add(Quaternion.identity);
                    continue;
                }

                rotations.Add(dstBone.rotation);
            }

            var initRot = new Dictionary<HumanBodyBones, Quaternion>();
            for (int i = 0; i < rotations.Count; i++)
            {
                initRot[(HumanBodyBones)i] = rotations[i];
            }

            return initRot;
        }

        Dictionary<HumanBodyBones, Quaternion> _initRotation = new Dictionary<HumanBodyBones, Quaternion>();


        private void UpdateByFrame()
        {
            var frameCount = MaxCount;
            var duration = frameCount * 1.0f / FrameRate;
            var animTime = Time.timeSinceLevelLoad % duration;
            var idx = (int)(animTime * FrameRate);
            UpdateFrame(idx);
        }

        void UpdateFrame(int frame)
        {
            Frame = frame % MaxCount;
            Iterate(Parser.root, null, (bone, parent) =>
            {
                var tran = (bone.extraData as GameObject).transform;
                var eulerBVH = GetBoneData(bone, 3);
                Quaternion rot = GetRotation(eulerBVH);
                tran.localRotation = rot;
                if (_anim != null)
                {
                    if (Enum.TryParse<HumanBodyBones>(bone.name, out var boneType))
                    {
                        UpdateRot(_anim, boneType, tran.rotation);
                    }
                }

                _anim.transform.localEulerAngles = new Vector3(90, 0, 0);
            });

            var offset = GetBoneData(Parser.root, 0);
            var root = (Parser.root.extraData as GameObject).transform;
            root.localPosition = GetOffset(offset);
        }

        private Vector3 GetBoneData(BVHParser.BVHBone bone, int offset)
        {
            Vector3 eulerBVH = Vector3.zero;
            for (int i = 0; i < 3; i++)
                eulerBVH[i] = bone.channels[offset + i].values[Frame];
            return eulerBVH;
        }

        private Vector3 GetOffset(Vector3 offset)
        {
            offset *= Scale;
            Vector3 offset2 = new Vector3(-offset.x, offset.y, offset.z);
            if (IsBlender)
            {
                offset2 = new Vector3(-offset.x, offset.z, -offset.y);
                //offset2 = new Vector3(-offset.x,  offset.y, offset.z);
            }

            return offset2;
        }

        private Quaternion GetRotation(Vector3 euler)
        {
            Quaternion rot = fromEulerZXY(euler);
            Quaternion rot2 = rot;
            if (IsBlender)
                rot2 = new Quaternion(rot.x, -rot.z, rot.y, rot.w);
            else
                rot2 = new Quaternion(rot.x, -rot.y, -rot.z, rot.w);
            return rot2;
        }

        private Quaternion fromEulerZXY(Vector3 euler)
        {
            return Quaternion.AngleAxis(euler.z, Vector3.forward) * Quaternion.AngleAxis(euler.x, Vector3.right) *
                   Quaternion.AngleAxis(euler.y, Vector3.up);
        }


        private void OnDrawGizmos()
        {
            if (Children == null || Children.Count == null) return;
            foreach (var child in Children)
            {
                if (child == null) continue;
                var parent = child.transform.parent;
                if (parent != null && parent != transform)
                {
                    Gizmos.DrawLine(child.transform.position, parent.position);
                }
            }
        }

        void Iterate(BVHParser.BVHBone bone, BVHParser.BVHBone parent,
            System.Action<BVHParser.BVHBone, BVHParser.BVHBone> callback)
        {
            callback(bone, parent);
            foreach (var child in bone.children)
            {
                Iterate(child, bone, callback);
            }
        }


        private void UpdateRot(Animator anim, HumanBodyBones boneEnum, Quaternion rotation)
        {
            var dstBone = anim.GetBoneTransform(boneEnum);
            if (dstBone == null) return;
            var initSrcRot = TemplateRots.Rots[(int)boneEnum];
            var initDstRot = _initRotation[boneEnum];
            var dstRot = rotation * Quaternion.Inverse(Quaternion.Euler(initSrcRot));
            dstBone.rotation = dstRot * initDstRot;
        }
    }
}