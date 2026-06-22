using System.Collections.Generic;
using UnityEngine;

public static class DialogueLoader
{
    public static List<string> LoadDialogueFromFile(string fileName)
    {
        TextAsset textAsset = Resources.Load<TextAsset>(fileName);

        if (textAsset == null)
        {
            Debug.LogError("파일을 찾을 수 없습니다: " + fileName);
            return new List<string>();
        }

        string[] lines = textAsset.text.Split(new[] { '\n', '\r' }, System.StringSplitOptions.RemoveEmptyEntries);
        return new List<string>(lines);
    }
}