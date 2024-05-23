using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace RealDream.AI
{
    public static class BoneUtil
    {
        public static HumanBodyBones GetParentBone(HumanBodyBones key)
        {
            return _humanBodyChild2Parent[key];
        }

        public static Transform CloneBoneHierarchy(Transform originalBone, Transform parentOfCloned)
        {
            GameObject clonedBone = new GameObject();
            var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.transform.SetParent(clonedBone.transform, true);
            cube.transform.localScale = Vector3.one * 0.01f;
            clonedBone.name = originalBone.name;
            clonedBone.transform.parent = parentOfCloned;
            clonedBone.transform.position = originalBone.position;
            clonedBone.transform.rotation = originalBone.rotation; // set rotation to identity
            return clonedBone.transform;
        }

        private static Dictionary<HumanBodyBones, HumanBodyBones> _humanBodyChild2Parent =
            new Dictionary<HumanBodyBones, HumanBodyBones>
            {
                { HumanBodyBones.Hips, HumanBodyBones.LastBone },
                { HumanBodyBones.LeftUpperLeg, HumanBodyBones.Hips },
                { HumanBodyBones.RightUpperLeg, HumanBodyBones.Hips },
                { HumanBodyBones.LeftLowerLeg, HumanBodyBones.LeftUpperLeg },
                { HumanBodyBones.RightLowerLeg, HumanBodyBones.RightUpperLeg },
                { HumanBodyBones.LeftFoot, HumanBodyBones.LeftLowerLeg },
                { HumanBodyBones.RightFoot, HumanBodyBones.RightLowerLeg },
                { HumanBodyBones.Spine, HumanBodyBones.Hips },
                { HumanBodyBones.Chest, HumanBodyBones.Spine },
                { HumanBodyBones.Neck, HumanBodyBones.UpperChest },
                { HumanBodyBones.Head, HumanBodyBones.Neck },
                { HumanBodyBones.LeftShoulder, HumanBodyBones.UpperChest },
                { HumanBodyBones.RightShoulder, HumanBodyBones.UpperChest },
                { HumanBodyBones.LeftUpperArm, HumanBodyBones.LeftShoulder },
                { HumanBodyBones.RightUpperArm, HumanBodyBones.RightShoulder },
                { HumanBodyBones.LeftLowerArm, HumanBodyBones.LeftUpperArm },
                { HumanBodyBones.RightLowerArm, HumanBodyBones.RightUpperArm },
                { HumanBodyBones.LeftHand, HumanBodyBones.LeftLowerArm },
                { HumanBodyBones.RightHand, HumanBodyBones.RightLowerArm },
                { HumanBodyBones.LeftToes, HumanBodyBones.LeftFoot },
                { HumanBodyBones.RightToes, HumanBodyBones.RightFoot },
                { HumanBodyBones.LeftThumbProximal, HumanBodyBones.LeftHand },
                { HumanBodyBones.LeftThumbIntermediate, HumanBodyBones.LeftThumbProximal },
                { HumanBodyBones.LeftThumbDistal, HumanBodyBones.LeftThumbIntermediate },
                { HumanBodyBones.LeftIndexProximal, HumanBodyBones.LeftHand },
                { HumanBodyBones.LeftIndexIntermediate, HumanBodyBones.LeftIndexProximal },
                { HumanBodyBones.LeftIndexDistal, HumanBodyBones.LeftIndexIntermediate },
                { HumanBodyBones.LeftMiddleProximal, HumanBodyBones.LeftHand },
                { HumanBodyBones.LeftMiddleIntermediate, HumanBodyBones.LeftMiddleProximal },
                { HumanBodyBones.LeftMiddleDistal, HumanBodyBones.LeftMiddleIntermediate },
                { HumanBodyBones.LeftRingProximal, HumanBodyBones.LeftHand },
                { HumanBodyBones.LeftRingIntermediate, HumanBodyBones.LeftRingProximal },
                { HumanBodyBones.LeftRingDistal, HumanBodyBones.LeftRingIntermediate },
                { HumanBodyBones.LeftLittleProximal, HumanBodyBones.LeftHand },
                { HumanBodyBones.LeftLittleIntermediate, HumanBodyBones.LeftLittleProximal },
                { HumanBodyBones.LeftLittleDistal, HumanBodyBones.LeftLittleIntermediate },
                { HumanBodyBones.RightThumbProximal, HumanBodyBones.RightHand },
                { HumanBodyBones.RightThumbIntermediate, HumanBodyBones.RightThumbProximal },
                { HumanBodyBones.RightThumbDistal, HumanBodyBones.RightThumbIntermediate },
                { HumanBodyBones.RightIndexProximal, HumanBodyBones.RightHand },
                { HumanBodyBones.RightIndexIntermediate, HumanBodyBones.RightIndexProximal },
                { HumanBodyBones.RightIndexDistal, HumanBodyBones.RightIndexIntermediate },
                { HumanBodyBones.RightMiddleProximal, HumanBodyBones.RightHand },
                { HumanBodyBones.RightMiddleIntermediate, HumanBodyBones.RightMiddleProximal },
                { HumanBodyBones.RightMiddleDistal, HumanBodyBones.RightMiddleIntermediate },
                { HumanBodyBones.RightRingProximal, HumanBodyBones.RightHand },
                { HumanBodyBones.RightRingIntermediate, HumanBodyBones.RightRingProximal },
                { HumanBodyBones.RightRingDistal, HumanBodyBones.RightRingIntermediate },
                { HumanBodyBones.RightLittleProximal, HumanBodyBones.RightHand },
                { HumanBodyBones.RightLittleIntermediate, HumanBodyBones.RightLittleProximal },
                { HumanBodyBones.RightLittleDistal, HumanBodyBones.RightLittleIntermediate },
                { HumanBodyBones.UpperChest, HumanBodyBones.Chest },
                { HumanBodyBones.LastBone, HumanBodyBones.LastBone },
            };
    }
}