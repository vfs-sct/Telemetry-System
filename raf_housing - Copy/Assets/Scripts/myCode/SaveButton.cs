using System.Collections.Generic;
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

        Debug.Log($"{_houseRotation}");

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
