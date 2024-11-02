using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public Inventory inventoryOne;
    public Inventory inventoryFig;

    public void OnSaveOneButtonClicked()
    {
        inventoryOne.SaveInventory("One");
    }

    public void OnLoadOneButtonClicked()
    {
        StartCoroutine(inventoryOne.LoadInventoryCoroutine("One"));
    }

    public void OnSaveFigButtonClicked()
    {
        inventoryFig.SaveInventory("Fig");
    }

    public void OnLoadFigButtonClicked()
    {
        StartCoroutine(inventoryFig.LoadInventoryCoroutine("Fig"));
    }

}
