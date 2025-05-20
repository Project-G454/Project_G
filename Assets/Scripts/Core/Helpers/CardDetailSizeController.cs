using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[ExecuteInEditMode]
[DisallowMultipleComponent]
[RequireComponent(typeof(RectTransform))]
public class CardDetailSizeController : MonoBehaviour
{
    public RectTransform contentHolder;

    private RectTransform _rectTransform;

    void OnEnable() {
        _rectTransform = GetComponent<RectTransform>();
        // if (Application.isPlaying)
        //     StartCoroutine(DelayedUpdateSize());
        // else
        //     UpdateSize();
        UpdateSize();
        Canvas.ForceUpdateCanvases();
    }

    void Update()
    {
        if (!Application.isPlaying)
        {
            UpdateSize();
        }
    }

    IEnumerator DelayedUpdateSize()
    {
        yield return null; // wait one frame
        yield return null;
        yield return null;
        UpdateSize();
    }

    public void UpdateSize()
    {
        if (contentHolder == null || _rectTransform == null) return;

        LayoutRebuilder.ForceRebuildLayoutImmediate(contentHolder);

        float preferredHeight = LayoutUtility.GetPreferredHeight(contentHolder);
        _rectTransform.sizeDelta = new Vector2(_rectTransform.sizeDelta.x, preferredHeight);
    }

    #if UNITY_EDITOR
    void OnValidate()
    {
        if (_rectTransform != null && contentHolder != null)
        {
            UpdateSize();
        }
    }
    #endif
}
