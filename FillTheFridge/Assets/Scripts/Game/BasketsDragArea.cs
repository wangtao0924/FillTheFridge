using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasketsDragArea : MonoBehaviour
{
    private Vector3 oriMousePos;
    private Vector3 oriObjectScreenPos;
    private void OnMouseDown()
    {
        oriMousePos = Input.mousePosition;
    }

    private void OnMouseDrag()
    {
        List<Basket> baskets = GameManager.instance.levelManager.curLevel.baskets;
        Vector3 curMousePos = Input.mousePosition;
        Vector3 mouseOffset = curMousePos - oriMousePos;
        Vector3 moveOffset= new Vector3(0, 0, mouseOffset.x * 0.001f);
        Vector3 leftMax = baskets[0].gameObject.transform.position - moveOffset;
        Vector3 rightMax = baskets[baskets.Count - 1].gameObject.transform.position - moveOffset;
        if (leftMax.z < 0 || rightMax.z > 0)
        {
            return;
        }
        GameManager.instance.levelManager.curLevel.basketsTrans.position = GameManager.instance.levelManager.curLevel.basketsTrans.position - moveOffset;
        oriMousePos = Input.mousePosition;
    }
}
