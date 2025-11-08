using UnityEngine;
using VRC.Dynamics;
using VRC.SDKBase;

namespace TestAvatars.VRCSDK3_9_1_beta_2_breaking_changes
{
    public class GlobalPBSettings : MonoBehaviour, IEditorOnly
    {
        public VRCPhysBoneBase.PermissionFilter allowCollision;
        public VRCPhysBoneBase.PermissionFilter allowGrabbing;
        public VRCPhysBoneBase.PermissionFilter allowPosing;
    }
}

#if UNITY_EDITOR
namespace TestAvatars.VRCSDK3_9_1_beta_2_breaking_changes
{
    using VRC.SDKBase.Editor.BuildPipeline;
    using UnityEditor;

    [CustomEditor(typeof(GlobalPBSettings))]
    class GlobalPBSettingsEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            var settings = (GlobalPBSettings)target;
            Undo.RecordObject(settings, "Settings Inspector Changes");
            EditorGUI.BeginChangeCheck();
            DrawPermissionFilter(ref settings.allowCollision, "Allow Collision");
            DrawPermissionFilter(ref settings.allowGrabbing, "Allow Grabbing");
            DrawPermissionFilter(ref settings.allowPosing, "Allow Posing");
            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(settings);
            }
        }

        private static void DrawPermissionFilter(ref VRCPhysBoneBase.PermissionFilter filter, string label)
        {
            EditorGUILayout.LabelField(label, EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            filter.allowOthers = EditorGUILayout.Toggle("Allow Others", filter.allowOthers);
            filter.allowSelf = EditorGUILayout.Toggle("Allow Self", filter.allowSelf);
            EditorGUI.indentLevel--;
        }
    }

    class GlobalPBSettingsProcessor : IVRCSDKPreprocessAvatarCallback
    {
        public int callbackOrder => -8192;

        public bool OnPreprocessAvatar(GameObject avatarGameObject)
        {
            if (!avatarGameObject.TryGetComponent<GlobalPBSettings>(out var settings)) return true;

            foreach (var physBone in avatarGameObject.GetComponentsInChildren<VRCPhysBoneBase>(true))
            {
                physBone.allowCollision = VRCPhysBoneBase.AdvancedBool.Other;
                physBone.collisionFilter = settings.allowCollision;
                physBone.allowGrabbing = VRCPhysBoneBase.AdvancedBool.Other;
                physBone.grabFilter = settings.allowGrabbing;
                physBone.allowPosing = VRCPhysBoneBase.AdvancedBool.Other;
                physBone.poseFilter = settings.allowPosing;
            }

            Object.DestroyImmediate(settings);

            return true;
        }
    }
}
#endif
