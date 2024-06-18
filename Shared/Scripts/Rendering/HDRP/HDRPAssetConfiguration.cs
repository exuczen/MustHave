#if UNITY_PIPELINE_HDRP
using System;
using System.Reflection;
using UnityEngine.Rendering.HighDefinition;

namespace MustHave
{
    public static class HDRPAssetConfiguration
    {
        private const BindingFlags InternalFieldFlags = BindingFlags.Instance | BindingFlags.Default | BindingFlags.Public | BindingFlags.NonPublic;

        private static readonly Type AssetType = typeof(HDRenderPipelineAsset);

        private static readonly FieldInfo m_RenderPipelineSettings = GetFieldInfo("m_RenderPipelineSettings");

        private static FieldInfo GetFieldInfo(string name) => AssetType.GetField(name, InternalFieldFlags);

        public static void ModifySettings(this HDRenderPipelineAsset asset, ActionRef<RenderPipelineSettings> modify)
        {
            var settings = asset.currentPlatformRenderPipelineSettings;
            modify(ref settings);
            m_RenderPipelineSettings.SetValue(asset, settings);
#if UNITY_EDITOR
            AssetUtils.SaveAsset(asset);
#endif
        }
    }
}
#endif