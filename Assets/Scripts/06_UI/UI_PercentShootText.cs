using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UI_PercentShootText : MonoBehaviour
{
    [SerializeField] private float percentShootOffset = 50f;
    [Space]
    [SerializeField] private Color zeroColor = Color.grey;
    [SerializeField] private Color basicColor = Color.white;
    [SerializeField] private Color criticalColor = Color.yellow;

    [Header("REFERENCES")]

    [SerializeField] private Text percentShootText = null;

    // ======================================================================
    // MONOBEHAVIOUR
    // ======================================================================

    private void Update()
    {
        SetPercentShootTextPosition();
    }

    // ======================================================================
    // PUBLIC METHODS
    // ======================================================================

    /// <summary>
    /// Enable the percent shoot text and displays the value in percent.
    /// </summary>
    /// <param name="percent"></param>
    public void SetPercentShootText(int percent)
    {
        percentShootText.gameObject.SetActive(true);
        percentShootText.text = percent + "%";

        if (percent <= 0) // 0
        {
            percentShootText.color = zeroColor;
            percentShootText.fontStyle = FontStyle.Normal;
        }
        else if (percent > 0 && percent < 100) // Regular
        {
            percentShootText.color = basicColor;
            percentShootText.fontStyle = FontStyle.Normal;
        }
        else // Critical
        {
            percentShootText.color = criticalColor;
            percentShootText.fontStyle = FontStyle.Bold;
        }
    }

    /// <summary>
    /// Disable the percent shoot text.
    /// </summary>
    public void DisablePercentShootText()
    {
        percentShootText.gameObject.SetActive(false);
    }

    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================

    /// <summary>
    /// Set the text position at mouse position + offset.
    /// </summary>
    private void SetPercentShootTextPosition()
    {
        percentShootText.transform.position = Input.mousePosition;
        percentShootText.transform.position += Vector3.right * percentShootOffset;
    }
}