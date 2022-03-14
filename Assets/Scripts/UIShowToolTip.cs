using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIShowToolTip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

    [SerializeField]
    GameObject tooltip_prefab;
    [TextArea(1, 30)]
    public string _text = "";
    public Vector3 pos;

    GameObject current;

    void CreateToolTip()
    {
        current = (GameObject)Instantiate(tooltip_prefab, transform.position+pos, Quaternion.identity);
        current.transform.SetParent(transform.root, true); //canvas
        current.transform.SetAsLastSibling(); // last one means foreground
        current.GetComponentInChildren<Text>().text = _text;
    }

    void DestroyToolTip()
    {
        CancelInvoke("CreateToolTip");
        Destroy(current);
    }

    public void OnPointerEnter(PointerEventData d) { Invoke("CreateToolTip", 0.5f); }
    public void OnPointerExit(PointerEventData d) { DestroyToolTip(); }
    void OnDisable() { DestroyToolTip(); }
    void OnDestroy() { DestroyToolTip(); }
}
