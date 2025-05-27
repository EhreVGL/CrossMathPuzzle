using UnityEngine;
using UnityEngine.EventSystems;

public class InventorySlot : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        Debug.Log("A");

        GameObject dropped = eventData.pointerDrag;
        DraggableItem draggableItem = dropped.GetComponent<DraggableItem>();

        if (draggableItem == null) return;

        Transform oldParent = draggableItem.parentAfterDrag; // Sürüklenen nesnenin eski parent'ı

        // Eğer bırakılan slotun içinde başka bir DraggableItem varsa, onunla yer değiştir
        if (transform.childCount > 0)
        {
            Transform existingItem = transform.GetChild(0);
            DraggableItem existingDraggable = existingItem.GetComponent<DraggableItem>();

            if (existingDraggable != null && transform.gameObject.name != "Grid")
            {
                // Önce mevcut nesneyi eski parent'a taşı
                existingDraggable.transform.SetParent(oldParent);
                existingDraggable.transform.SetAsFirstSibling();
                existingDraggable.parentAfterDrag = oldParent;
            }
        }

        // Sürüklenen nesneyi yeni parent'a taşı
        draggableItem.transform.SetParent(transform);
        draggableItem.transform.SetAsFirstSibling();
        draggableItem.parentAfterDrag = transform;
    }
}
