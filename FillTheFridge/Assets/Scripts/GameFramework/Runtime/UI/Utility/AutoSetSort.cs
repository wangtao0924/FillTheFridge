using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoSetSort : MonoBehaviour
{   
    public int SortIndex = -1;
    int parentIndex = -1;
    string parentLayer;
    Canvas _canvas;

    public void SetCanvas()
    {
        _canvas = GetComponent<Canvas>();
        if (_canvas == null)
        {
            _canvas = gameObject.AddComponent<Canvas>();
        }
        GetParentSort(transform);
        _canvas.overrideSorting = true;
        _canvas.sortingLayerName = parentLayer;
        _canvas.sortingOrder = SortIndex + parentIndex;
    }

    void GetParentSort(Transform trans)
    {
        Canvas ca = trans.parent.GetComponent<Canvas>();
        if (ca == null)
        {
            GetParentSort(trans.parent);
        }
        else
        {
            parentIndex = ca.sortingOrder;
            parentLayer = ca.sortingLayerName;
        }
    }
}
