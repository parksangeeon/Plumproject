using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryItemBase : MonoBehaviour, IInventoryItem
{
    public virtual string Name
    {
        get
        {
            return "_base item_";
        }
    }

    public Sprite _Image;

    public Sprite Image
    {
        get { return _Image; }
    }

    public virtual void OnUse()
    {
        
    }

    /*
        public virtual void OnDrop()
        {
            RaycastHit2D hit = new RaycastHit2D;

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics2D.Raycast(ray, out hit)) {
                gameObject.SetActive(true);

                gameObject.transform.position = hit.point;
            }
        }
    */

    public virtual void OnPickup()
    {
        gameObject.SetActive(false);
    }

}
