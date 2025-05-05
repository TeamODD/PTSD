using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;

public class SubtitleManager : MonoBehaviour
{
    [System.Serializable]
    public class SubtitleEntry
    {
        public string text;
        public float duration;
    }

    public TextMeshProUGUI subtitleText;
    private List<SubtitleEntry> subtitles = new List<SubtitleEntry>();

    void Start()
    {
        if (subtitleText == null)
        {
            subtitleText = FindObjectOfType<TextMeshProUGUI>();
            if (subtitleText == null)
            {
                Debug.LogError("TextMeshProUGUI 오브젝트를 찾을 수 없습니다! Unity에서 연결해 주세요.");
                return;
            }
        }

        LoadCSV();
        StartCoroutine(PlaySubtitles());
    }


    void LoadCSV()
    {
        string filePath = Path.Combine(Application.dataPath, "UI/Resources.csv");

        if (File.Exists(filePath))
        {
            string[] lines = File.ReadAllLines(filePath);

            for (int i = 1; i < lines.Length; i++) // 헤더 건너뜀
            {
                string[] parts = lines[i].Split(',');

                if (parts.Length == 2 && float.TryParse(parts[1].Trim(), out float duration))
                {
                    subtitles.Add(new SubtitleEntry
                    {
                        text = parts[0].Trim(),
                        duration = duration
                    });
                    Debug.Log("Added subtitle: " + parts[0].Trim() + " with duration: " + duration);
                }
                else
                {
                    Debug.LogError("Invalid subtitle entry: " + lines[i]);
                }
            }
        }
        else
        {
            Debug.LogError("CSV 파일을 찾을 수 없습니다: " + filePath);
        }
    }

    IEnumerator PlaySubtitles()
    {
        foreach (var entry in subtitles)
        {
            subtitleText.text = entry.text;
            yield return new WaitForSeconds(entry.duration);
            subtitleText.text = "";
        }
    }
}