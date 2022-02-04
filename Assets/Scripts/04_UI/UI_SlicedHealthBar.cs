using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_SlicedHealthBar : MonoBehaviour
{
    [SerializeField] private float topDownOffset = 1;
    [SerializeField] private float spacing = 2;
    [SerializeField] private RectTransform lifeLayoutGroup = null;
    [SerializeField] private GameObject lifeImage = null;
    [SerializeField] private C_Health health = null;

    // ======================================================================
    // MONOBEHAVIOUR
    // ======================================================================

    private void Start()
    {
        SetLifeBarActive(true);
        InitialiseBar();
    }

    // ======================================================================
    // PUBLIC METHODS
    // ======================================================================

    /// <summary>
    /// Display the current life on life bar.
    /// </summary>
    public void DisplayCurrentHealth()
    {
        GameObject currentLifeBar;

        for (int i = 0; i < health.maxHealth; i++)
        {
            currentLifeBar = lifeLayoutGroup.GetChild(i).gameObject;

            if (i < health.health)
            {
                currentLifeBar.SetActive(true);   
            }
            else
            {
                currentLifeBar.SetActive(false);
            }
        }
    }

    /// <summary>
    /// Enable or disable the life bar.
    /// </summary>
    /// <param name="value"></param>
    public void SetLifeBarActive (bool value)
    {
        transform.parent.gameObject.SetActive(value);
    }

    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================

    /// <summary>
    /// Display max life on health bar.
    /// </summary>
    private void DisplayMaxHealth()
    {
        // Destroy ancient objects
        for (int i = 0; i < lifeLayoutGroup.childCount; i++)
        {
            Destroy(lifeLayoutGroup.GetChild(i).gameObject);
        }

        // Display life
        for (int i = 0; i < health.maxHealth; i++)
        {
            RectTransform rt = Instantiate(lifeImage, lifeLayoutGroup).GetComponent<RectTransform>();
            float parentWidth = lifeLayoutGroup.rect.width;
            float lifeWidth = 0;

            // Size
            Vector2 sizeDeltaAdjusted = new Vector2(
                (parentWidth) / health.maxHealth - (spacing * 2),
                lifeLayoutGroup.rect.height - topDownOffset *2);
            rt.sizeDelta = sizeDeltaAdjusted;

            lifeWidth = sizeDeltaAdjusted.x;

            // Position
            Vector2 localPosition = new Vector2(
                ((parentWidth / health.maxHealth) * i) - (parentWidth / 2) + (lifeWidth / 2) + spacing / (health.maxHealth) + spacing,
                0);
            rt.localPosition = localPosition;
        }
    }

    /// <summary>
    /// Display the current life and max life on life bar.
    /// </summary>
    private void InitialiseBar()
    {
        DisplayMaxHealth();
        DisplayCurrentHealth();
    }
}
