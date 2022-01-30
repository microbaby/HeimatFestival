using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

using TMPro;

namespace RhythmHeavenMania.Editor
{
    public class Tooltip : MonoBehaviour
    {
        private RectTransform rectTransform;
        [SerializeField] private RectTransform canvasRect;
        [SerializeField] private RectTransform background;
        [SerializeField] private TMP_Text text;
        [SerializeField] private CanvasGroup group;

        public static Tooltip instance { get; private set; }

        private void Awake()
        {
            instance = this;
            rectTransform = GetComponent<RectTransform>();
            group.alpha = 0;
        }

        private void Update()
        {
            Vector2 anchoredPosition = Input.mousePosition;

            if (anchoredPosition.x + background.rect.width > canvasRect.rect.width)
            {
                anchoredPosition.x = canvasRect.rect.width - background.rect.width;
            }
            if (anchoredPosition.x < 0)
            {
                anchoredPosition.x = 0;
            }

            if (anchoredPosition.y + background.rect.height > canvasRect.rect.height)
            {
                anchoredPosition.y = canvasRect.rect.height - background.rect.height;
            }
            if (anchoredPosition.y < 0)
            {
                anchoredPosition.y = 0;
            }

            rectTransform.anchoredPosition = anchoredPosition;
        }

        public static void OnEnter(string tooltipText)
        {
            instance.OnEnterPrivate(tooltipText);
        }

        public static void OnExit()
        {
            instance.OnExitPrivate();
        }

        private void OnEnterPrivate(string tooltipText)
        {
            group.alpha = 1;
            SetText(tooltipText);
        }

        private void OnExitPrivate()
        {
            group.alpha = 0;
        }

        private void SetText(string tooltipText)
        {
            text.text = tooltipText;
            text.ForceMeshUpdate();

            Vector2 textSize = text.GetRenderedValues(false);
            Vector2 paddingSize = new Vector2(8, 8);

            background.sizeDelta = textSize + paddingSize;
        }

        public static void AddTooltip(GameObject g, string tooltipText)
        {
            EventTrigger et = g.AddComponent<EventTrigger>();

            EventTrigger.Entry pointerEnter = new EventTrigger.Entry();
            pointerEnter.eventID = EventTriggerType.PointerEnter;
            pointerEnter.callback.AddListener((data) => { OnEnter(tooltipText); });

            EventTrigger.Entry pointerExit = new EventTrigger.Entry();
            pointerExit.eventID = EventTriggerType.PointerExit;
            pointerExit.callback.AddListener((data) => { OnExit(); });

            et.triggers.Add(pointerEnter);
            et.triggers.Add(pointerExit);
        }
    }
}