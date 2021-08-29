using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class HubScript : ParticlePainter.Singleton<HubScript>
{
    [SerializeField] Button A2Button;
    [SerializeField] Button EventManagerButton;
    [SerializeField] Button ParticlePainterButton;
    [SerializeField] Button SplatoonPainterButton;
    [SerializeField] Button BackButton;

    [SerializeField] GameObject Hub;
    [SerializeField] GameObject InGame;

    bool mode = false;

    private void Start()
    {
        DontDestroyOnLoad(this.gameObject);

        InGame.SetActive(false);
        Hub.SetActive(true);

        A2Button.onClick.AddListener(() =>
        {
            InGame.SetActive(true);
            Hub.SetActive(false);
            SceneManager.LoadScene("A2Scene");
        });

        EventManagerButton.onClick.AddListener(() =>
        {
            InGame.SetActive(true);
            Hub.SetActive(false);
            SceneManager.LoadScene("EventManagerDemo");
        });

        ParticlePainterButton.onClick.AddListener(() =>
        {
            InGame.SetActive(true);
            Hub.SetActive(false);
            SceneManager.LoadScene("PaintScene");
        });

        SplatoonPainterButton.onClick.AddListener(() =>
        {
            InGame.SetActive(true);
            Hub.SetActive(false);
            SceneManager.LoadScene("Splatoon");
        });

        BackButton.onClick.AddListener(() =>
        {
            InGame.SetActive(false);
            Hub.SetActive(true);
            SceneManager.LoadScene("Hub");
        });

        SceneManager.LoadScene("Hub");
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.P) && mode == false)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            mode = true;
        }
        else if (Input.GetKeyDown(KeyCode.P) && mode == true)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = true;
            mode = false;
        }
    }
}
