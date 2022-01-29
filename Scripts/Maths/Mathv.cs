﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace MustHave
{
    public struct Mathv
    {
        public const float Epsilon = Vector2.kEpsilon;

        public static Vector3 Random(Vector3 min, Vector3 max)
        {
            return new Vector3(
                UnityEngine.Random.Range(min.x, max.x),
                UnityEngine.Random.Range(min.y, max.y),
                UnityEngine.Random.Range(min.z, max.z)
            );
        }

        public static Vector3 Random(float min, float max)
        {
            return new Vector3(
                UnityEngine.Random.Range(min, max),
                UnityEngine.Random.Range(min, max),
                UnityEngine.Random.Range(min, max)
            );
        }

        public static Vector3 Abs(Vector3 v)
        {
            return new Vector3(
                Mathf.Abs(v.x),
                Mathf.Abs(v.y),
                Mathf.Abs(v.z)
            );
        }

        public static Vector2Int Abs(Vector2Int v)
        {
            return new Vector2Int(
                Mathf.Abs(v.x),
                Mathf.Abs(v.y)
            );
        }

        public static Vector3Int Abs(Vector3Int v)
        {
            return new Vector3Int(
                Mathf.Abs(v.x),
                Mathf.Abs(v.y),
                Mathf.Abs(v.z)
            );
        }

        public static Vector3 Mul(Vector3 v1, Vector3 v2)
        {
            return new Vector3(
                v1.x * v2.x,
                v1.y * v2.y,
                v1.z * v2.z
            );
        }

        public static Vector3 Div(Vector3 v1, Vector3 v2)
        {
            return new Vector3(
                v1.x / v2.x,
                v1.y / v2.y,
                v1.z / v2.z
            );
        }

        public static Vector3 Sin(Vector3 v)
        {
            return new Vector3(
                Mathf.Sin(v.x),
                Mathf.Sin(v.y),
                Mathf.Sin(v.z)
            );
        }

        public static float Max(Vector3 v)
        {
            return Mathf.Max(v.x, v.y, v.z);
        }

        public static float Min(Vector3 v)
        {
            return Mathf.Min(v.x, v.y, v.z);
        }

        public static Vector3 Clamp(Vector3 v, float min, float max)
        {
            return new Vector3(
                Mathf.Clamp(v.x, min, max),
                Mathf.Clamp(v.y, min, max),
                Mathf.Clamp(v.z, min, max)
            );
        }

        public static Vector3 Clamp(Vector3 v, Vector3 min, Vector3 max)
        {
            return new Vector3(
                Mathf.Clamp(v.x, min.x, max.x),
                Mathf.Clamp(v.y, min.y, max.y),
                Mathf.Clamp(v.z, min.z, max.z)
            );
        }

        public static Vector3Int RoundToInt(Vector3 v)
        {
            return new Vector3Int(
                Mathf.RoundToInt(v.x),
                Mathf.RoundToInt(v.y),
                Mathf.RoundToInt(v.z)
            );
        }

        public static Vector2Int Signs(Vector2Int v)
        {
            return new Vector2Int(
                Math.Sign(v.x),
                Math.Sign(v.y)
            );
        }

        public static Vector3Int Signs(Vector3Int v)
        {
            return new Vector3Int(
                Math.Sign(v.x),
                Math.Sign(v.y),
                Math.Sign(v.z)
            );
        }

        public static Vector3 SnapToPlaneXZ(Vector3 worldPoint, Transform planeTransform, float lengthX, float lengthZ)
        {
            Vector3 localPoint = planeTransform.InverseTransformPoint(worldPoint);
            localPoint.x = Mathf.Clamp(localPoint.x, 0f, lengthX);
            localPoint.y = 0;
            localPoint.z = Mathf.Clamp(localPoint.z, 0f, lengthZ);
            return planeTransform.TransformPoint(localPoint);
        }

        public static Vector3 SnapToBoxColliderXZ(Vector3 worldPoint, BoxCollider collider, float normalizedY)
        {
            var colliderTransform = collider.transform;
            var extents = collider.size * 0.5f;
            Vector3 colliderTransformPos = colliderTransform.position;
            colliderTransform.position = colliderTransform.TransformPoint(collider.center);
            Vector3 localPoint = colliderTransform.InverseTransformPoint(worldPoint);
            localPoint.x = Mathf.Clamp(localPoint.x, -extents.x, extents.x);
            localPoint.y = normalizedY * extents.y;
            localPoint.z = Mathf.Clamp(localPoint.z, -extents.z, extents.z);
            worldPoint = colliderTransform.TransformPoint(localPoint);
            colliderTransform.position = colliderTransformPos;
            return worldPoint;
        }

        public static Vector3 SnapToScaledPlaneXZ(Vector3 worldPoint, Transform planeTransform, float lengthX, float lengthZ, float normalizedScale)
        {
            Vector3 localPoint = planeTransform.InverseTransformPoint(worldPoint);
            normalizedScale = Mathf.Clamp01(normalizedScale);
            float scaleFactor = (1f - normalizedScale) * 0.5f;
            float minX = lengthX * scaleFactor;
            float minZ = lengthZ * scaleFactor;
            localPoint.x = Mathf.Clamp(localPoint.x, minX, lengthX - minX);
            localPoint.y = 0;
            localPoint.z = Mathf.Clamp(localPoint.z, minZ, lengthZ - minZ);
            return planeTransform.TransformPoint(localPoint);
        }

        public static Vector2 WorldToScreenPoint(Transform cameraTransform, float fov, RectTransform canvasRect, Vector3 worldPoint)
        {
            Rect rect = canvasRect.rect;
            Vector3 planePoint = cameraTransform.InverseTransformPoint(worldPoint);
            Vector2 planeSize = GetWorldRectSize(fov, rect.width / rect.height, planePoint.z);

            float screenPointX = rect.width * planePoint.x / planeSize.x;
            float screenPointY = rect.height * planePoint.y / planeSize.y;

            return new Vector2(screenPointX, screenPointY);
        }

        public static Vector2 GetWorldRectSize(float fov, float ratio, float distance)
        {
            float height = GetWorldRectHeight(fov, distance);
            float width = height * ratio;
            return new Vector2(width, height);
        }

        public static float GetWorldRectHeight(float fov, float distance)
        {
            // tan(fov/2) = (h/2) / distance;
            return 2f * distance * Mathf.Tan(Mathf.Deg2Rad * fov * 0.5f);
        }

        public static bool GetLinesIntersection(Vector3 beg1, Vector3 end1, Vector3 beg2, Vector3 end2, out Vector3 isecPoint)
        {
            Vector3 dir1 = end1 - beg1;
            Vector3 dir2 = end2 - beg2;
            Vector3 dir3 = beg2 - beg1;
            Vector3 dirs12Cross = Vector3.Cross(dir1, dir2);

            float planarFactor = Vector3.Dot(dir3, dirs12Cross);
            float dirs12CrossSqrLength;

            //is coplanar, and not parallel
            if (Mathf.Abs(planarFactor) < Epsilon && (dirs12CrossSqrLength = dirs12Cross.sqrMagnitude) > Epsilon)
            {
                Vector3 dirs32Cross = Vector3.Cross(dir3, dir2);
                float t = Vector3.Dot(dirs32Cross, dirs12Cross) / dirs12CrossSqrLength;
                isecPoint = beg1 + (dir1 * t);
                return true;
            }
            else
            {
                isecPoint = Vector3.zero;
                return false;
            }
        }
    }
}
