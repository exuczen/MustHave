using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace MustHave
{
    public class ExampleRenderPipeline : RenderPipeline
    {
        private static readonly ShaderTagId[] LegacyShaderTagIDs = {
            new("Always"),
            new("ForwardBase"),
            new("ForwardAdd"),
            new("PrepassBase"),
            new("PrepassFinal"),
            new("Vertex"),
            new("VertexLMRGBM"),
            new("VertexLM"),
        };

        protected override void Render(ScriptableRenderContext context, List<Camera> cameras)
        {
            // https://docs.unity3d.com/ScriptReference/Rendering.RenderPipeline.BeginContextRendering.html - Required by MustHave.OutlineCamera.
            BeginContextRendering(context, cameras);

            // Create and schedule a command to clear the current render target
            CommandBuffer cmd = CommandBufferPool.Get();
            cmd.ClearRenderTarget(true, true, Color.clear);
            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);

            foreach (Camera camera in cameras)
            {
                // https://docs.unity3d.com/ScriptReference/Rendering.RenderPipeline.BeginCameraRendering.html - Required by MustHave.OutlineCamera.
                BeginCameraRendering(context, camera);

                if (camera.cullingMask == OutlineObjectCamera.Layer.OutlineMask)
                {
                    RenderWithMustHaveOutlineObjectCamera(context, camera);
                }
                else
                {
                    RenderWithYourCamera(context, camera);
                }
                // https://docs.unity3d.com/ScriptReference/Rendering.RenderPipeline.EndCameraRendering.html - Required by MustHave.OutlineCamera.
                EndCameraRendering(context, camera);
            }

            // Instruct the graphics API to perform all scheduled commands
            context.Submit();

            // https://docs.unity3d.com/ScriptReference/Rendering.RenderPipeline.EndContextRendering.html - Required by MustHave.OutlineCamera.
            EndContextRendering(context, cameras);
        }

        protected override void Render(ScriptableRenderContext context, Camera[] cameras) { }

        /// <summary>
        /// Default implementation of MustHave.OutlineObjectCamera rendering - it needs to stay that way.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="camera"></param>
        private void RenderWithMustHaveOutlineObjectCamera(ScriptableRenderContext context, Camera camera, string lightMode = "SRPDefaultUnlit")
        {
            // Get the culling parameters from the current Camera
            camera.TryGetCullingParameters(out var cullingParameters);

            // Use the culling parameters to perform a cull operation, and store the results
            var cullingResults = context.Cull(ref cullingParameters);

            // Update the value of built-in shader variables, based on the current Camera
            context.SetupCameraProperties(camera);

            // Tell Unity which geometry to draw, based on its LightMode Pass tag value
            var shaderTagId = new ShaderTagId(lightMode);

            // Tell Unity how to sort the geometry, based on the current Camera
            var sortingSettings = new SortingSettings(camera);

            // Create a DrawingSettings struct that describes which geometry to draw and how to draw it
            var drawingSettings = new DrawingSettings(shaderTagId, sortingSettings);

            // Tell Unity how to filter the culling results, to further specify which geometry to draw
            // Use FilteringSettings.defaultValue to specify no filtering
            var filteringSettings = FilteringSettings.defaultValue;

            // Schedule a command to draw the geometry, based on the settings you have defined
            context.DrawRenderers(cullingResults, ref drawingSettings, ref filteringSettings);

            // Instruct the graphics API to perform all scheduled commands
            context.Submit();
        }


        /// <summary>
        /// Example implementation of your custom rendering - change it as much as you want.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="camera"></param>
        private void RenderWithYourCamera(ScriptableRenderContext context, Camera camera)
        {
            // Get the culling parameters from the current Camera
            camera.TryGetCullingParameters(out var cullingParameters);

            // Use the culling parameters to perform a cull operation, and store the results
            var cullingResults = context.Cull(ref cullingParameters);

            // Update the value of built-in shader variables, based on the current Camera
            context.SetupCameraProperties(camera);

            // Tell Unity how to sort the geometry, based on the current Camera
            var sortingSettings = new SortingSettings(camera);

            // Create a DrawingSettings struct that describes which geometry to draw and how to draw it
            var drawingSettings = new DrawingSettings(LegacyShaderTagIDs[0], sortingSettings);

            for (int i = 1; i < LegacyShaderTagIDs.Length; i++)
            {
                drawingSettings.SetShaderPassName(i, LegacyShaderTagIDs[i]);
            }
            // Tell Unity how to filter the culling results, to further specify which geometry to draw
            // Use FilteringSettings.defaultValue to specify no filtering
            var filteringSettings = FilteringSettings.defaultValue;

            // Schedule a command to draw the geometry, based on the settings you have defined
            context.DrawRenderers(cullingResults, ref drawingSettings, ref filteringSettings);

            // Schedule a command to draw the Skybox if required
            if (camera.clearFlags == CameraClearFlags.Skybox && RenderSettings.skybox != null)
            {
                context.DrawSkybox(camera);
            }

            // Instruct the graphics API to perform all scheduled commands
            context.Submit();
        }
    }
}
