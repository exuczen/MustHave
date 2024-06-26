﻿using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace MustHave
{
    [ExecuteInEditMode]
    public class SceneCameraFocus : MonoBehaviour
    {
        [SerializeField, HideInInspector] private int forwardAxisSign = 1;

        public int ForwardAxisSign { get { return Math.Sign(forwardAxisSign); } set { forwardAxisSign = Math.Sign(value); } }

        public void AlignToForwardAxis()
        {
#if UNITY_EDITOR
            SceneView sceneView = SceneView.lastActiveSceneView;
            if (sceneView)
            {
                Vector3 forward = -ForwardAxisSign * transform.forward;
                sceneView.pivot = transform.position;
                sceneView.rotation = Quaternion.LookRotation(forward, transform.up);
                sceneView.Repaint();
            }
#endif
        }
    }
}
