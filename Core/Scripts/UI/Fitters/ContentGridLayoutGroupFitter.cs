using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MustHave.UI
{
    [ExecuteInEditMode]
    public class ContentGridLayoutGroupFitter : UIBehaviour
    {
        [SerializeField]
        private RectTransform content = default;
        [SerializeField]
        private GridLayoutGroup gridLayout = default;
        [SerializeField, Range(0f, 1f)]
        private float spacingNormalizedX = default;
        [SerializeField, Range(0f, 1f)]
        private float spacingNormalizedY = default;
        [SerializeField, Range(0f, 1f)]
        private float paddingNormalizedLeft = default;
        [SerializeField, Range(0f, 1f)]
        private float paddingNormalizedRight = default;
        [SerializeField, Range(0f, 1f)]
        private float paddingNormalizedTop = default;
        [SerializeField, Range(0f, 1f)]
        private float paddingNormalizedBottom = default;
        [SerializeField, Range(0f, 5f), Tooltip("height / width")]
        private float cellSizeAspectRatio = 1f;

        protected override void OnEnable()
        {
            OnRectTransformDimensionsChange();
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            OnRectTransformDimensionsChange();
        }
#endif

        protected override void OnRectTransformDimensionsChange()
        {
            if (!EditorApplicationUtils.IsCompilingOrUpdating && enabled && gridLayout)
            {
                RectTransform gridRectTransform = gridLayout.transform as RectTransform;
                switch (gridLayout.constraint)
                {
                    case GridLayoutGroup.Constraint.Flexible:
                        throw new NotImplementedException();
                    case GridLayoutGroup.Constraint.FixedColumnCount:
                        int colCount = gridLayout.constraintCount;
                        int rowCount = gridLayout.transform.childCount / colCount;
                        float cellWidth = content.rect.width / (colCount + paddingNormalizedLeft + paddingNormalizedRight + spacingNormalizedX * (colCount - 1));
                        float cellHeight = cellWidth * cellSizeAspectRatio;
                        float gridHeightTrimmed = cellHeight * rowCount;
                        gridLayout.cellSize = new Vector2(cellWidth, cellHeight);
                        gridLayout.spacing = new Vector2(spacingNormalizedX * cellWidth, spacingNormalizedY * cellHeight);
                        gridLayout.padding.left = (int)(paddingNormalizedLeft * cellWidth);
                        gridLayout.padding.right = (int)(paddingNormalizedRight * cellWidth);
                        gridLayout.padding.top = (int)(paddingNormalizedTop * cellHeight);
                        gridLayout.padding.bottom = (int)(paddingNormalizedBottom * cellHeight);
                        float gridHeight = rowCount * cellHeight + gridLayout.padding.top + gridLayout.padding.bottom + gridLayout.spacing.y * (rowCount - 1);
                        gridRectTransform.sizeDelta = new Vector2(gridRectTransform.sizeDelta.x, gridHeight);
                        break;
                    case GridLayoutGroup.Constraint.FixedRowCount:
                        throw new NotImplementedException();
                    default:
                        throw new ArgumentException();
                }
            }
        }
    }
}
