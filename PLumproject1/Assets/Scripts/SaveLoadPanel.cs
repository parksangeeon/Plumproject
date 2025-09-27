using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using ClearSky;

public enum SaveLoadMode { Save, Load }

public class SaveLoadPanel : MonoBehaviour
{
    [Header("Slots(4)")]
    public SaveSlotUI[] slots = new SaveSlotUI[SaveSystem.SlotCount];

    [Header("Confirm Overwrite")]
    public GameObject overwriteDialog;
    public Button yesButton;
    public Button noButton;

    SaveLoadMode mode;
    int pendingSlot = -1;

    void Awake()
    {
        gameObject.SetActive(false);
        if (overwriteDialog) overwriteDialog.SetActive(false);

        if (yesButton) yesButton.onClick.AddListener(OnConfirmYes);
        if (noButton) noButton.onClick.AddListener(OnConfirmNo);
    }

    public void Open(SaveLoadMode m)
    {
        mode = m;
        Refresh();
        gameObject.SetActive(true);
    }

    public void Close()
    {
        gameObject.SetActive(false);
        if (overwriteDialog) overwriteDialog.SetActive(false);
        pendingSlot = -1;
    }

    public void Refresh()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            var sum = SaveSystem.GetSummary(i);
            slots[i].Bind(i, sum, OnSlotClicked);
        }
    }

    void OnSlotClicked(int index)
    {
        if (mode == SaveLoadMode.Save)
        {
            if (SaveSystem.Exists(index))
            {
                pendingSlot = index;
                if (overwriteDialog) overwriteDialog.SetActive(true);
            }
            else
            {
                DoSave(index);
            }
        }
        else // Load
        {
            if (SaveSystem.Exists(index))
                StartCoroutine(DoLoad(index));
        }
    }

    void OnConfirmYes()
    {
        if (pendingSlot >= 0) DoSave(pendingSlot);
        if (overwriteDialog) overwriteDialog.SetActive(false);
        pendingSlot = -1;
    }

    void OnConfirmNo()
    {
        if (overwriteDialog) overwriteDialog.SetActive(false);
        pendingSlot = -1;
    }

    void DoSave(int slot)
    {
        var player = FindObjectOfType<Player>();
        var bridge = FindObjectOfType<SaveLoadBridge>();
        SaveSystem.Save(slot, player, bridge != null ? bridge.ExportItems : null);
        Refresh();
    }

    IEnumerator DoLoad(int slot)
    {
        yield return SaveSystem.Load(slot);
        Close();
    }
}
