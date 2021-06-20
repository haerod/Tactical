using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class M_UI : MonoBehaviour
{
    public static M_UI instance;

    [Header("REFERENCES")]

    [SerializeField] private Text actionPointsText = null;
    [SerializeField] private Text actionCostText = null;

    [Header("ACTION COST TEXT SETTINGS")]

    [SerializeField] private float actionCostOffset = 30f;
    [SerializeField] private Color inRangeColor = Color.yellow;
    [SerializeField] private Color outRangeColor = Color.grey;

    // ======================================================================
    // MONOBEHAVIOUR
    // ======================================================================

    private void Awake()
    {
        if (!instance)
        {
            instance = this;
        }
    }

    // ======================================================================
    // PUBLIC METHODS
    // ======================================================================

    public void SetActionPointText(string text, Character c)
    {
        if (c != M_Characters.instance.IsCurrentCharacter(c)) return;

        actionPointsText.text = text;
    }

    public void SetActionCostText(string value, Vector3 position, bool outRange = false)
    {
        actionCostText.gameObject.SetActive(true);
        actionCostText.text = value;
        actionCostText.transform.position = Camera.main.WorldToScreenPoint(position);
        actionCostText.transform.position += Vector3.up * actionCostOffset;

        if (outRange)
        {
            actionCostText.color = outRangeColor;
        }
        else
        {
            actionCostText.color = inRangeColor;
        }
    }

    public void DisableActionCostText()
    {
        actionCostText.gameObject.SetActive(false);
    }

    public void ClickOnAddActionPoint()
    {
        M_Characters.instance.currentCharacter.actionPoints.AddActionPoints();
    }

    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================
}
