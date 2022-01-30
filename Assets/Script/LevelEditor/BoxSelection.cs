using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Starpelly;

using RhythmHeavenMania.Editor.Track;

namespace RhythmHeavenMania.Editor
{
    public class BoxSelection : MonoBehaviour
    {
        [SerializeField] private RectTransform boxVisual;
        private Rect selectionBox;

        private Vector2 startPosition = Vector2.zero;
        private Vector2 endPosition = Vector2.zero;

        public bool selecting = false;

        public static BoxSelection instance { get; private set; }

        private void Awake()
        {
            instance = this;
        }

        private void Start()
        {
            DrawVisual();

            Color boxCol = EditorTheme.theme.properties.BoxSelectionCol.Hex2RGB();
            boxVisual.GetComponent<Image>().color = new Color(boxCol.r, boxCol.g, boxCol.b, 0.3f);
            boxVisual.transform.GetChild(0).GetComponent<Image>().color = EditorTheme.theme.properties.BoxSelectionOutlineCol.Hex2RGB();
        }

        private void Update()
        {
            if (Selections.instance.eventsSelected.Count > 0 && Timeline.instance.InteractingWithEvents())
            {
                startPosition = Vector2.zero;
                endPosition = Vector2.zero;
                DrawVisual();
                return;
            }

            if (Conductor.instance.NotStopped())
            {
                startPosition = Vector2.zero;
                endPosition = Vector2.zero;
                DrawVisual();
                return;
            }

            // click
            if (Input.GetMouseButtonDown(0))
            {
                startPosition = MousePosition();
                selectionBox = new Rect();
            }

            // dragging
            if (Input.GetMouseButton(0))
            {
                endPosition = MousePosition();
                DrawVisual();
                DrawSelection();
            }

            // release click
            if (Input.GetMouseButtonUp(0))
            {
                startPosition = Vector2.zero;
                endPosition = Vector2.zero;
                DrawVisual();
                SelectEvents();
            }

            // selecting = (selectionBox.size != Vector2.zero); -- doesn't work really

            // for real time selection just move SelectEvents() to here, but that breaks some shit. might fix soon idk --pelly
        }

        private void DrawVisual()
        {
            Vector2 boxStart = startPosition;
            Vector2 boxEnd = endPosition;

            Vector2 boxCenter = (boxStart + boxEnd) / 2;
            boxVisual.position = boxCenter;

            Vector2 boxSize = new Vector2(Mathf.Abs(boxStart.x - boxEnd.x), Mathf.Abs(boxStart.y - boxEnd.y));

            boxVisual.sizeDelta = boxSize;
        }

        private void DrawSelection()
        {
            // X
            if (MousePosition().x < startPosition.x)
            {
                // dragging left
                selectionBox.xMin = MousePosition().x;
                selectionBox.xMax = startPosition.x;
            }
            else
            {
                // dragging right
                selectionBox.xMin = startPosition.x;
                selectionBox.xMax = MousePosition().x;
            }

            // Y
            if (MousePosition().y < startPosition.y)
            {
                // dragging down
                selectionBox.yMin = MousePosition().y;
                selectionBox.yMax = startPosition.y;
            }
            else
            {
                // dragging up
                selectionBox.yMin = startPosition.y;
                selectionBox.yMax = MousePosition().y;
            }
        }

        private void SelectEvents()
        {
            int selected = 0;

            // if (!Input.GetKeyDown(KeyCode.LeftShift)) Selections.instance.DeselectAll();
            for (int i = 0; i < GameManager.instance.Beatmap.entities.Count; i++)
            {
                TimelineEventObj e = GameManager.instance.Beatmap.entities[i].eventObj;


                if (selectionBox.Overlaps(GetWorldRect(e.GetComponent<RectTransform>())))
                {
                    Selections.instance.DragSelect(e);
                    selected++;
                }
            }

            selecting = selected > 0;
        }

        public Vector3 MousePosition()
        {
            var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            return new Vector3(mousePos.x, mousePos.y, 0);
        }

        public Rect GetWorldRect(RectTransform rectTransform)
        {
            Vector3[] corners = new Vector3[4];
            rectTransform.GetWorldCorners(corners);
            // Get the bottom left corner.
            Vector3 position = corners[0];

            Vector2 size = new Vector2(
                rectTransform.lossyScale.x * rectTransform.rect.size.x,
                rectTransform.lossyScale.y * rectTransform.rect.size.y);

            return new Rect(position, size);
        }
    }
}