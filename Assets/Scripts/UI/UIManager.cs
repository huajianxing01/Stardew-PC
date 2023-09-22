using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : SingletonMonobehaviour<UIManager>
{
    [SerializeField] private GameObject pauseMenu = null;
    [SerializeField] private GameObject[] menuTabs = null;
    [SerializeField] private Button[] menuButtons = null;
    [SerializeField] private UIInventoryBar uiInventoryBar = null;
    [SerializeField] private PauseMenuInventoryManagement pauseMenuInventoryManagement = null;
    [SerializeField] private ToggleGroup hairStyle = null;
    [SerializeField] private TMP_InputField hairColorInput = null;
    [SerializeField] private ToggleGroup skinStyle = null;
    [SerializeField] private ToggleGroup shirtStyle = null;
    [SerializeField] private TMP_InputField trouserColorInput = null;
    [SerializeField] private Toggle hatStyle = null;
    [SerializeField] private ToggleGroup adornments = null;
    private Toggle[] hairToggles;
    private Toggle[] skinToggles;
    private Toggle[] shirtToggles;
    private Toggle[] adornmentsToggles;
    [SerializeField] private ApplyCharacterCustomisation customisation = null;

    private bool _pauseMenuOn = false;
    public bool PauseMenuOn { get => _pauseMenuOn; set => _pauseMenuOn = value; }

    protected override void Awake()
    {
        base.Awake();

        pauseMenu.SetActive(false);
        
        hairToggles = hairStyle.transform.GetComponentsInChildren<Toggle>();
        GetHairStyle();

        skinToggles = skinStyle.transform.GetComponentsInChildren<Toggle>();
        GetSkinStyle();

        shirtToggles = shirtStyle.transform.GetComponentsInChildren<Toggle>();
        GetShirtStyle();

        adornmentsToggles = adornments.transform.GetComponentsInChildren<Toggle>();
        GetHatAndAdornmentsStyle();
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
        //点击esc按键时，取消当前的拖拽
        uiInventoryBar.DestroyCurrentlyDraggedItems();
        uiInventoryBar.ClearCurrentlySelectedItems();

        PauseMenuOn = true;
        Player.Instance.PlayerInputIsDisabled = true;
        Time.timeScale = 0;
        pauseMenu.SetActive(true);

        //强制进行即时垃圾回收，把内存中已不需要的游戏数据清除，空出内存空间再利用
        System.GC.Collect();

        HighlightButtonForSelectedTab();
        
    }

    public void DisablePauseMenu()
    {
        //拖拽时按esc退出则取消当前拖拽效果
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
    /// <summary>
    /// 退出游戏
    /// </summary>
    public void QuitGame()
    {
        Application.Quit();
    }
    /// <summary>
    /// 重开游戏
    /// </summary>
    public void RestartGame()
    {
        SceneControllerManager.Instance.Restart();
        DisablePauseMenu();
    }

    private void GetHairStyle()
    {
        for (int i = 0; i < hairToggles.Length; i++)
        {
            Toggle toggle = hairToggles[i];
            //获取设定好的发型款式
            if (i == customisation.inputHairStyleNo)
            {
                toggle.isOn = true;
            }
            toggle.onValueChanged.AddListener((bool value) => OnHairStyleChange(toggle));
        }
    }

    private void GetSkinStyle()
    {
        for (int i = 0; i < skinToggles.Length; i++)
        {
            Toggle toggle = skinToggles[i];
            //获取设定好的皮肤款式
            if (i == customisation.inputSkinStyleNo)
            {
                toggle.isOn = true;
            }
            toggle.onValueChanged.AddListener((bool value) => OnSkinStyleChange(toggle));
        }
    }

    private void GetShirtStyle()
    {
        for (int i = 0; i < shirtToggles.Length; i++)
        {
            Toggle toggle = shirtToggles[i];
            //获取设定好的衣服款式
            if (i == customisation.inputShirtStyleNo)
            {
                toggle.isOn = true;
            }
            toggle.onValueChanged.AddListener((bool value) => OnShirtStyleChange(toggle));
        }
    }

    private void GetHatAndAdornmentsStyle()
    {
        if(customisation.inputHatsStyleNo == 1) hatStyle.isOn = true;
        hatStyle.onValueChanged.AddListener((bool value) => OnHatAndAdornmentsChange(hatStyle));

        for (int i = 0; i < adornmentsToggles.Length; i++) 
        {
            Toggle toggle = adornmentsToggles[i];
            if (i == 0 && customisation.inputAdornmentsStyleNo == 1) toggle.isOn = true;
            if (i == 1 && customisation.inputAdornmentsStyleNo == 3) toggle.isOn = true;
            if (i == 2 && customisation.inputAdornmentsStyleNo == 2) toggle.isOn = true;
            toggle.onValueChanged.AddListener((bool value) => OnHatAndAdornmentsChange(toggle));
        }
    }

    private void OnHairStyleChange(Toggle toggle)
    {
        if (toggle.isOn)
        {
            switch (toggle.name)
            {
                case "HairStyle1":
                    customisation.ChangeHair(0);
                    DisablePauseMenu();
                    break;
                case "HairStyle2":
                    customisation.ChangeHair(1);
                    DisablePauseMenu();
                    break;
                default:
                    break;
            }
        }
    }

    private void OnSkinStyleChange(Toggle toggle)
    {
        if (toggle.isOn)
        {
            switch (toggle.name)
            {
                case "SkinStyle1":
                    customisation.ChangeSkinStyle(0);
                    DisablePauseMenu();
                    break;
                case "SkinStyle2":
                    customisation.ChangeSkinStyle(1);
                    DisablePauseMenu();
                    break;
                case "SkinStyle3":
                    customisation.ChangeSkinStyle(2);
                    DisablePauseMenu();
                    break;
                case "SkinStyle4":
                    customisation.ChangeSkinStyle(3);
                    DisablePauseMenu();
                    break;
                default:
                    break;
            }
        }
    }

    private void OnShirtStyleChange(Toggle toggle)
    {
        if (toggle.isOn)
        {
            switch (toggle.name)
            {
                case "ShirtStyle1":
                    customisation.ChangeShirtStyle(0);
                    DisablePauseMenu();
                    break;
                case "ShirtStyle2":
                    customisation.ChangeShirtStyle(1);
                    DisablePauseMenu();
                    break;
                default:
                    break;
            }
        }
    }

    private void OnHatAndAdornmentsChange(Toggle toggle)
    {
        if (toggle.isOn)
        {
            switch (toggle.name)
            {
                case "HatStyle1":
                    customisation.ChangeHat(1);
                    DisablePauseMenu();
                    break;
                case "AdornmentStyle1":
                    customisation.ChangeAdornments(1);
                    DisablePauseMenu();
                    break;
                case "AdornmentStyle2":
                    customisation.ChangeAdornments(3);
                    DisablePauseMenu();
                    break;
                case "AdornmentStyle3":
                    customisation.ChangeAdornments(2);
                    DisablePauseMenu();
                    break;
                default:
                    break;
            }
        }
        else
        {
            switch (toggle.name)
            {
                case "HatStyle1":
                    customisation.ChangeHat(0);
                    DisablePauseMenu();
                    break;
                case "AdornmentStyle1":
                case "AdornmentStyle2":
                case "AdornmentStyle3":
                    customisation.ChangeAdornments(0);
                    DisablePauseMenu();
                    break;
                default:
                    break;
            }
        }
    }

    public void ChangeHairColor()
    {
        string newColor = "#" + hairColorInput.text;
        Color color;
        ColorUtility.TryParseHtmlString(newColor, out color);
        customisation.ChangeHair(color);
        DisablePauseMenu();
    }

    public void ChangeTrouserColor()
    {
        string newColor = "#" + trouserColorInput.text;
        Color color;
        ColorUtility.TryParseHtmlString(newColor, out color);
        customisation.ChangeTrouser(color);
        DisablePauseMenu();
    }
}
