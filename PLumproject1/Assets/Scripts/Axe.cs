using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Axe : MonoBehaviour, IInventoryItem
{

    public string Name
    {
        get
        {
            return "Axe";
        }

    }

    public Sprite _Image;

    public Sprite Image
    {
        get
        {
            return _Image;
        }


    }
    
    public void OnPickup()
    {
        gameObject.SetActive(false);
    }

    public void OnDrop()
    {
        /*Debug.Log("d");
        RaycastHit hit = new RaycastHit();
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit, 1000))
        {

            gameObject.SetActive(true);
            gameObject.transform.position = hit.point;
        }*/

    }

}

