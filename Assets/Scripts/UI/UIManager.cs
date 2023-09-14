using System;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : SingletonMonobehaviour<UIManager>
{
    [SerializeField] private GameObject pauseMenu = null;
    [SerializeField] private GameObject[] menuTabs = null;
    [SerializeField] private Button[] menuButtons = null;
    [SerializeField] private UIInventoryBar uiInventoryBar = null;
    [SerializeField] private PauseMenuInventoryManagement pauseMenuInventoryManagement = null;

    private bool _pauseMenuOn = false;
    public bool PauseMenuOn { get => _pauseMenuOn; set => _pauseMenuOn = value; }

    protected override void Awake()
    {
        base.Awake();

        pauseMenu.SetActive(false);
    }

    private void Update()
    {
        PauseMenu();
    }

    private void PauseMenu()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (PauseMenuOn)
            {
                DisablePauseMenu();
            }
            else
            {
                EnablePauseMenu();
            }
        }
    }

    private void EnablePauseMenu()
    {
        //���esc����ʱ��ȡ����ǰ����ק
        uiInventoryBar.DestroyCurrentlyDraggedItems();
        uiInventoryBar.ClearCurrentlySelectedItems();

        PauseMenuOn = true;
        Player.Instance.PlayerInputIsDisabled = true;
        Time.timeScale = 0;
        pauseMenu.SetActive(true);

        //ǿ�ƽ��м�ʱ�������գ����ڴ����Ѳ���Ҫ����Ϸ����������ճ��ڴ�ռ�������
        System.GC.Collect();

        HighlightButtonForSelectedTab();
    }

    private void DisablePauseMenu()
    {
        pauseMenuInventoryManagement.DestroyCurrentDraggedItems();
        
        PauseMenuOn = false;
        Player.Instance.PlayerInputIsDisabled = false;
        Time.timeScale = 1;
        pauseMenu.SetActive(false);
    }

    public void SwitchPauseMenuTab(int tabNum)
    {
        for (int i = 0; i < menuTabs.Length; i++)
        {
            if (i != tabNum)
            {
                menuTabs[i].SetActive(false);
            }
            else
            {
                menuTabs[i].SetActive(true);
            }
        }
        HighlightButtonForSelectedTab();
    }

    private void HighlightButtonForSelectedTab()
    {
        for (int i = 0; i < menuTabs.Length; i++)
        {
            if (menuTabs[i].activeSelf)
            {
                SetButtonColorToActive(menuButtons[i]);
            }
            else
            {
                SetButtonColorToInActive(menuButtons[i]);
            }
        }
    }

    private void SetButtonColorToInActive(Button button)
    {
        ColorBlock colors = button.colors;
        colors.normalColor = colors.disabledColor;
        button.colors = colors;
    }

    private void SetButtonColorToActive(Button button)
    {
        ColorBlock colors = button.colors;
        colors.normalColor = colors.pressedColor;
        button.colors = colors;
    }
}
