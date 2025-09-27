using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class SaveSlotUI : MonoBehaviour
{
public Button button;
public TextMeshProUGUI titleText; // 예: "Slot 1"
public TextMeshProUGUI metaText; // 예: "Scene @ 2025-09-01 13:20 (Items 3)"


int index;
Action<int> onClick;


void Awake()
{
if (button == null) button = GetComponent<Button>();
}


public void Bind(int slotIndex, SaveSummary summary, Action<int> click)
{
index = slotIndex;
onClick = click;
titleText.text = $"File {slotIndex + 1}";


if (summary == null)
{
metaText.text = "---- Empty ----";
}
else
{
metaText.text = $"{summary.sceneName} | {summary.timestamp} | Items {summary.itemCount}";
}


button.onClick.RemoveAllListeners();
button.onClick.AddListener(() => onClick?.Invoke(index));
}
}