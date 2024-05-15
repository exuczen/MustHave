using System;
using UnityEngine;

namespace MustHave
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(Camera))]
    public class CameraChangeListener : MonoBehaviour
    {
        public event Action PropertyChanged = delegate { };

        private struct CameraData
        {
            public bool orthographic;
            public float fieldOfView;

            public void CopyFrom(Camera camera)
            {
                orthographic = camera.orthographic;
                fieldOfView = camera.fieldOfView;
            }

            public readonly bool Equals(Camera camera)
            {
                return orthographic == camera.orthographic &&
                        fieldOfView == camera.fieldOfView;
            }
        }

        private CameraData prevData = default;
        private new Camera camera = null;

        private void OnApplicationFocus(bool focus)
        {
            SetPreviousData();
        }

        private void OnEnable()
        {
            camera = GetComponent<Camera>();

            SetPreviousData();
        }

        private void Update()
        {
            if (!prevData.Equals(camera))
            {
                PropertyChanged();
            }
            SetPreviousData();
        }

        private void SetPreviousData()
        {
            prevData.CopyFrom(camera);
        }
    }
}
