using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class Speed : MonoBehaviour
{
    private TextMeshProUGUI m_text;
    private readonly string pattern = "<size=36><b>{0}</b></size>/min";
    Spinner m_spinner => Spinner.Instance;
    private void Awake()
    {
        m_text = GetComponent<TextMeshProUGUI>();
    }

    private void Update()
    {
        //result = SpeedCurrentAbs(deg/s) * 60(s) / 360(deg)
        m_text.text = string.Format(pattern, Mathf.FloorToInt((m_spinner.SpeedCurrentAbs / 6f)).ToString());

    }
}
