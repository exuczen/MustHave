using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace MustHave
{
    [ExecuteInEditMode]
    public class OutlineObject : MonoBehaviour
    {
        private static class ShaderData
        {
            public static readonly int ColorID = Shader.PropertyToID("_Color");
            public static readonly int OneMinusDepthID = Shader.PropertyToID("_OneMinusDepth");
            public static readonly int DepthID = Shader.PropertyToID("_Depth");
            public static readonly int MinDepthID = Shader.PropertyToID("_MinDepth");
        }

        public Color32 Color32 { get => color; set => color = value; }
        public Color Color { get => color; set => color = value; }
        public float Depth { get; set; }
        public float CameraDistanceSqr => cameraDistanceSqr;

        [SerializeField]
        private Color color = Color.white;

        private static readonly ObjectPool<RendererData> rendererDataPool = new(() => new RendererData(), null, data => data.Clear());

        private readonly List<Renderer> renderers = new();
        private readonly List<RendererData> renderersData = new();

        private OutlineObjectCamera objectCamera = null;

        private float cameraDistanceSqr;

        public void GetDistanceFromCamera(Vector3 camPos)
        {
            cameraDistanceSqr = (transform.position - camPos).sqrMagnitude;
        }

        public void SetupRenderers(Material material, int layer, float minDepth)
        {
            material.SetColor(ShaderData.ColorID, Color);
            material.SetFloat(ShaderData.DepthID, Depth);
            material.SetFloat(ShaderData.MinDepthID, minDepth);

            foreach (var data in renderersData)
            {
                data.Setup(material, layer);
            }
        }

        public void SetColorWithDepth(float depth, float minDepth)
        {
            Depth = depth;
            Color = GetColorWithAlphaDepth(depth, minDepth);

            foreach (var data in renderersData)
            {
                data.Color = Color;
                data.Depth = Depth;
            }
        }

        public void DrawBBoxGizmo()
        {
            foreach (var data in renderersData)
            {
                var bounds = data.Renderer.bounds;
                Gizmos.matrix = Matrix4x4.identity;
                Gizmos.color = new Color(color.r, color.g, color.b, 0.5f);
                Gizmos.DrawCube(bounds.center, bounds.size);
                Gizmos.color = Color.green;
                Gizmos.DrawWireCube(bounds.center, bounds.size);
            }
        }

        public void RestoreRenderers()
        {
            foreach (var data in renderersData)
            {
                data.Restore();
            }
        }

        public void ForEachRendererData(Action<RendererData> action)
        {
            foreach (var data in renderersData)
            {
                action(data);
            }
        }

        private Color GetColorWithAlphaDepth(float depth, float minDepth)
        {
            var color = Color;
            color.a = Mathf.Clamp01(1f - depth + minDepth);
            return color;
        }

        private void OnEnable()
        {
            GetComponentsInChildren(renderers);
            foreach (var renderer in renderers)
            {
                var data = rendererDataPool.Get();
                data.SetRenderer(renderer);
                renderersData.Add(data);
            }
            var camera = CameraUtils.MainOrCurrent;
            if (camera)
            {
                var outlineCamera = camera.GetComponent<OutlineCamera>();
                if (outlineCamera)
                {
                    objectCamera = outlineCamera.ObjectCamera;
                    objectCamera.AddOutlineObject(this);
                }
            }
        }

        private void OnDisable()
        {
            if (objectCamera)
            {
                objectCamera.RemoveOutlineObject(this);
            }
            foreach (var data in renderersData)
            {
                rendererDataPool.Release(data);
            }
            renderersData.Clear();
            renderers.Clear();
        }
    }
}
