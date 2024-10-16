#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace TestAvatars.BlendShapePerformanceTest
{
    internal static class AnimationGenerator
    {
        [MenuItem("Tools/TestAvatars/BlendShapePerformanceTest/Run Animation Generator")]
        public static void RunAnimationGenerator()
        {
            var destDir = AssetDatabase.GUIDToAssetPath("d693bde4b00c44a29d8ca3c08a0ca6de");

            if (string.IsNullOrEmpty(destDir))
            {
                Debug.LogError("Could not find destination directory");
                return;
            }

            Debug.Log($"Found destination directory: {destDir}");

            var onAnimation = new AnimationClip();
            var offAnimation = new AnimationClip();

            foreach (var objectName in GameObjectNames())
            {
                onAnimation.SetCurve(objectName, 
                    typeof(SkinnedMeshRenderer),
                    "blendShape.ShapeKey",
                    AnimationCurve.Constant(0, 1, 100));
                offAnimation.SetCurve(objectName, 
                    typeof(SkinnedMeshRenderer),
                    "blendShape.ShapeKey",
                    AnimationCurve.Constant(0, 1, 0));
            }

            AssetDatabase.CreateAsset(onAnimation, $"{destDir}/OnAnimation.anim");
            AssetDatabase.CreateAsset(offAnimation, $"{destDir}/OffAnimation.anim");
        }

        private static IEnumerable<string> GameObjectNames()
        {
            return Enumerable.Range(0, 1000).Select(i =>
            {
                var x = (i / 1) % 10;
                var y = (i / 10) % 10;
                var z = (i / 100) % 10;

                return $"Boxes/Plane ({x})/Stick ({y})/BlendShapeCube ({z})";
            });
        }

        [MenuItem("Tools/TestAvatars/BlendShapePerformanceTest/Generate Merge Skinned Mesh")]
        public static void GenerateMergeSkinnedMesh()
        {
            var target = Selection.activeGameObject;
            if (target == null) return;
            var mergeSkinnedMeshType = Type.GetType("Anatawa12.AvatarOptimizer.MergeSkinnedMesh, com.anatawa12.avatar-optimizer.runtime");

            if (mergeSkinnedMeshType == null)
            {
                Debug.LogError("Could not find MergeSkinnedMesh type");
                return;
            }

            var renderers = target.GetComponentsInChildren<SkinnedMeshRenderer>(true);

            var mergeSMRGO = new GameObject("Merge Skinned Mesh");
            mergeSMRGO.transform.parent = target.transform.parent;
            mergeSMRGO.transform.localPosition = Vector3.zero;
            mergeSMRGO.transform.localRotation = Quaternion.identity;
            mergeSMRGO.transform.localScale = Vector3.one;

            var mergeSMR = (MonoBehaviour)mergeSMRGO.AddComponent(mergeSkinnedMeshType);
            dynamic mergeSMRDynamic = mergeSMR;
            mergeSMRDynamic.Initialize(2);
            mergeSMRDynamic.MergeBlendShapes = false;
            mergeSMRDynamic.SourceSkinnedMeshRenderers.UnionWith(renderers);

            Selection.activeGameObject = mergeSMRGO;
        }
    }
}

#endif