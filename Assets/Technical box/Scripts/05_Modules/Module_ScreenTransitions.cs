using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static M__Managers;

/// <summary>
/// Class description
/// </summary>
public class Module_ScreenTransitions : MonoBehaviour
{
    [SerializeField] private Image _fadeImage;
    [SerializeField] private Color _imageColor = Color.black;
    [SerializeField] private float _fadeDuration = .5f;
    [SerializeField] private AnimationCurve _fadeCurve = AnimationCurve.EaseInOut(0,1,1,0);
    
    private float currentTime = 0;
    
    // ======================================================================
    // MONOBEHAVIOUR
    // ======================================================================

    private void Awake()
    {
        _fadeImage.color = _imageColor;
        _fadeImage.gameObject.SetActive(true);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += SceneManager_OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= SceneManager_OnSceneLoaded;
    }
    
    // ======================================================================
    // PUBLIC METHODS
    // ======================================================================

    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================

    private void FadeOut()
    {
        StopAllCoroutines();
        currentTime = 0;
        StartCoroutine(FadeOut_Coroutine());
    }
    
    private IEnumerator FadeOut_Coroutine()
    {
        while (currentTime < _fadeDuration)
        {
            yield return new WaitForEndOfFrame();
            currentTime += Time.deltaTime;
            _fadeImage.color = new Color(_fadeImage.color.r, _fadeImage.color.g, _fadeImage.color.b, _fadeCurve.Evaluate(currentTime/_fadeDuration));
        }
        
        _fadeImage.gameObject.SetActive(false);
    }
    
    // ======================================================================
    // EVENTS
    // ======================================================================
    
    private void SceneManager_OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        FadeOut();
    }
}