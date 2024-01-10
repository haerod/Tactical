using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using static M__Managers;

public class UI_ActionCostText : MonoBehaviour
{
    [Header("ACTION COST TEXT SETTINGS")]

    [SerializeField] private float actionCostOffset = 30f;
    [SerializeField] private Color inRangeColor = Color.yellow;
    [SerializeField] private Color outRangeColor = Color.grey;

    [Header("REFERENCES")]

    [SerializeField] private Text actionCostText = null;

    private Transform actionCostTarget;
    private Camera cam;

    // ======================================================================
    // MONOBEHAVIOUR
    // ======================================================================

    private void Start()
    {
        cam = _camera.GetComponentInChildren<Camera>();
    }

    private void Update()
    {
        SetActionCostTextPosition();
    }

    // ======================================================================
    // PUBLIC METHODS
    // ======================================================================

    /// <summary>
    /// Write text in action cost text and specify its target and its color.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="target"></param>
    /// <param name="outRange"></param>
    public void SetActionCostText(string value, Transform target, bool outRange = false)
    {
        if(_rules.actionsByTurn == M_Rules.ActionsByTurn.OneActionByTurn) // EXIT : No action cost text in this mode
        {
            actionCostText.gameObject.SetActive(false);
            return;
        }

        actionCostText.gameObject.SetActive(true);
        actionCostText.text = value;
        actionCostTarget = target;

        if (outRange)
        {
            actionCostText.color = outRangeColor;
        }
        else
        {
            actionCostText.color = inRangeColor;
        }
    }

    /// <summary>
    /// Disable the action point text.
    /// </summary>
    public void DisableActionCostText()
    {
        actionCostText.gameObject.SetActive(false);
    }

    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================

    /// <summary>
    /// Set the position of the action cost text relative to its target.
    /// </summary>
    private void SetActionCostTextPosition()
    {
        if (!actionCostTarget) return;

        actionCostText.transform.position = cam.WorldToScreenPoint(actionCostTarget.position);
        actionCostText.transform.position += Vector3.up * actionCostOffset;
    }
}
