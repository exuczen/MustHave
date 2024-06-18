#if UNITY_PIPELINE_HDRP
using System.Reflection;
using UnityEngine.Rendering.HighDefinition;

namespace MustHave
{
    public abstract class HDRPAssetInfo : TypeInfo<HDRenderPipelineAsset>
    {
        public static readonly FieldInfo m_RenderPipelineSettings = GetFieldInfo("m_RenderPipelineSettings");
    }

    public static class HDRPAssetExtensionMethods
    {
        public static void ModifySettings(this HDRenderPipelineAsset asset, ActionRef<RenderPipelineSettings> modify)
        {
            var settings = asset.currentPlatformRenderPipelineSettings;
            modify(ref settings);
            HDRPAssetInfo.m_RenderPipelineSettings.SetValue(asset, settings);
#if UNITY_EDITOR
            AssetUtils.SaveAsset(asset);
#endif
        }
    }
}
#endif
