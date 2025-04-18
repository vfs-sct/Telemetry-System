using System.Collections.Generic;
using System.IO;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class SaveButton : MonoBehaviour
{
    public Button saveButton;
    public Slider rotateSlider;
    public TextMeshProUGUI score;


    private string _housePos;
    private string _houseRotation;
    List<Transform> objectsWithLayer = new List<Transform>();
    private string trackScore = "";
    private void Start()
    {
        if (saveButton != null)
        {
            saveButton.onClick.AddListener(OnButtonClick);
        }
    }

    public void TrackScore()
    {
        trackScore += score.text.ToString() + ", ";
    }

    void OnButtonClick()
    {
        GameObject[] allObjects = FindObjectsOfType<GameObject>();

        int i = 0;
        foreach (GameObject obj in allObjects)
        {
            if (obj.layer == 3)
            {
                objectsWithLayer.Add(obj.transform);
                _houseRotation += objectsWithLayer[i].transform.rotation.ToString();
                _housePos += objectsWithLayer[i].transform.position.ToString();
                i++;
            }
        }

        LocalSaveData data = new LocalSaveData
        {
            buttonName = saveButton.name,
            token = SessionManager.Instance.AuthToken,
            score = trackScore,
            finalScore = score.text,
            sliderValue = rotateSlider.value,
            housePositions = _housePos,
            houseRotations = _houseRotation,
            clickTime = System.DateTime.UtcNow.ToString("o")
        };

        string path = Path.Combine(Application.dataPath, "LocalSave.json");

        LocalSaveList log = new LocalSaveList();

        // Load existing data
        if (File.Exists(path))
        {
            string existingJson = File.ReadAllText(path);
            if (!string.IsNullOrWhiteSpace(existingJson))
            {
                log = JsonUtility.FromJson<LocalSaveList>(existingJson);
            }
        }

        log.logs.Add(data); 

        string updatedJson = JsonUtility.ToJson(log, true);
        File.WriteAllText(path, updatedJson);



        TelemetryManager.Instance.LogEvent("button_click", new Dictionary<string, object>
        {
            { "buttonName", saveButton.name },
            { "token", SessionManager.Instance.AuthToken },
            { "score", trackScore },
            { "final score", score.text },
            { "slider", rotateSlider.value},
            { "Houses position", _housePos},
            { "Houses rotation", _houseRotation},
            { "clickTime", System.DateTime.UtcNow.ToString("o") }
        });
    }
}


[System.Serializable]
public class LocalSaveData
{
    public string buttonName;
    public string token;
    public string score;
    public string finalScore;
    public float sliderValue;
    public string housePositions;
    public string houseRotations;
    public string clickTime;
}

[System.Serializable]
public class LocalSaveList
{
    public List<LocalSaveData> logs = new List<LocalSaveData>();
}