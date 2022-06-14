using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class OpenCloseSideBar : MonoBehaviour
{
    [SerializeField] Button openButton;
    [SerializeField] Button closeButton;
    [SerializeField] GameObject panel;
    // Start is called before the first frame update
    private void Awake()
    {
        openButton.onClick.AddListener(() => OpenSideBar());
        closeButton.onClick.AddListener(() => CloseSideBar());
    }

    void OpenSideBar()
    {
        openButton.gameObject.SetActive(false);
        panel.SetActive(true);
    }

    void CloseSideBar()
    {
        panel.SetActive(false);
        openButton.gameObject.SetActive(true);
    }
}
