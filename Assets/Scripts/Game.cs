using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Button = UnityEngine.UI.Button;
using Image = UnityEngine.UI.Image;

public class Game : MonoBehaviour
{
    private Save _sv = new Save();

    public double _score = 0;
    private double _addAmount = 1;
    private double _bonusPerSec = 0;

    public Animator _clickableObjAnim;

    public Sprite[] _crystalsSkins;

    [Space(10)]
    [Header("Buttons")]
    public Button _clickableObjButton;
    [Space(5)]
    public Button _shopButton;
    public Button _closeShopButton;
    [Space(5)]
    public Button _shopBonusButton;
    public Button _closeShopBonusButton;
    [Space(5)]
    public Button _crystalButton;
    public Button _closeCrystalButton;

    [Space(10)]
    [Header("Panels")]
    public GameObject _shopPanel;
    public GameObject _shopBonusPanel;
    public GameObject _crystalPanel;

    [Space(10)]
    [Header("Text")]
    public Text _scoreText;
    public Text _clickText;
    public Text _bonusText;

    public GameObject[] _shopButtons;
    public double[] _itemCost;

    public GameObject[] _shop2Buttons;
    public double[] _bonusCost;

    public GameObject[] _crystalButtons;
    public double[] _crystalCost;
    public bool[] _crystalBuy;
    public bool[] _crystalSet = { true, false, false, false};

    private void Awake()
    {
        DataLoad();
    }

    private void Start()
    {
        Screen.orientation = ScreenOrientation.Portrait;

        _clickableObjButton.onClick.AddListener(OnClick);

        _shopButton.onClick.AddListener(ShowAndHideShop);
        _shopBonusButton.onClick.AddListener(ShowAndHideShop2);

        _closeShopButton.onClick.AddListener(ShowAndHideShop);
        _closeShopBonusButton.onClick.AddListener(ShowAndHideShop2);

        _crystalButton.onClick.AddListener(ShowAndHideCrystal);
        _closeCrystalButton.onClick.AddListener(ShowAndHideCrystal);

        if (_score > 0)
        {
            _scoreText.text = $"{PolyLabs.ShortScale.ParseDouble(_score)}";
        }
        else
        {
            _scoreText.text = "0";
        }

        _clickText.text = $"+{PolyLabs.ShortScale.ParseDouble(_addAmount)} crystals/click";

        if (_bonusPerSec == 0)
        {
            _bonusText.text = "+0 crystals/per second";
        }
        else
        {
            _bonusText.text = $"+{PolyLabs.ShortScale.ParseDouble(_bonusPerSec)} crystals/per second";
        }

        for (int i = 0; i < _crystalButtons.Length; i++)
        {
            if (_crystalSet[i])
            {
                _clickableObjButton.GetComponent<Image>().sprite = _crystalsSkins[i];
                break;
            }
        }

        foreach (var button in _shopButtons)
        {
            button.gameObject.GetComponent<Button>().onClick.AddListener(BuyItem);
            button.gameObject.transform.GetChild(1).GetComponent<Text>().text = $"{PolyLabs.ShortScale.ParseDouble(_itemCost[int.Parse(button.name)])}";
        }

        foreach (var button in _shop2Buttons)
        {
            button.gameObject.GetComponent<Button>().onClick.AddListener(BuyBonus);
            button.gameObject.transform.GetChild(1).GetComponent<Text>().text = $"{PolyLabs.ShortScale.ParseDouble(_bonusCost[int.Parse(button.name)])}";
        }

        foreach (var button in _crystalButtons)
        {
            button.gameObject.GetComponent<Button>().onClick.AddListener(BuyCrystal);
            button.gameObject.transform.GetChild(0).GetComponent<Text>().text = $"Skin {int.Parse(button.name) + 1}";
            if (_crystalBuy[int.Parse(button.name)])
            {
                if (_crystalSet[int.Parse(button.name)])
                {
                    button.transform.GetChild(1).GetComponent<Text>().text = $"Set";
                    //button.transform.GetChild(2).GetChild(0).GetComponent<Image>().sprite = _checkMarkSprite;
                    button.transform.GetChild(2).GetChild(0).gameObject.SetActive(true);
                    continue;
                }
                else
                {
                    button.gameObject.transform.GetChild(1).GetComponent<Text>().text = $"Click to set";
                    continue;
                }
            }
            if (button.name == "0")
            {
                button.gameObject.transform.GetChild(1).GetComponent<Text>().text = $"Set default";
            }
            else
            {
                button.gameObject.transform.GetChild(1).GetComponent<Text>().text = $"{PolyLabs.ShortScale.ParseDouble(_crystalCost[int.Parse(button.name)])}";
            }
        }

        StartCoroutine(PerSecBonus());
    }

    private void BuyItem()
    {
        var buttonId = int.Parse(EventSystem.current.currentSelectedGameObject.name);

        if (_score >= _itemCost[buttonId])
        {
            _score -= _itemCost[buttonId];
            UpdateScoreText();
            _addAmount *= 1.2;
            _itemCost[buttonId] *= 2;
            _shopButtons[buttonId].transform.GetChild(1).GetComponent<Text>().text = PolyLabs.ShortScale.ParseDouble(_itemCost[buttonId]);
            UpdateClickText();
        }
    }

    private void BuyBonus()
    {
        var buttonId = int.Parse(EventSystem.current.currentSelectedGameObject.name);

        if (_score >= _bonusCost[buttonId])
        {
            _score -= _bonusCost[buttonId];
            UpdateScoreText();
            _bonusPerSec += 100;
            _bonusCost[buttonId] *= 2;
            _shop2Buttons[buttonId].transform.GetChild(1).GetComponent<Text>().text = PolyLabs.ShortScale.ParseDouble(_bonusCost[buttonId]);
            UpdateBonusText();
        }
    }

    private void BuyCrystal()
    {
        var buttonId = int.Parse(EventSystem.current.currentSelectedGameObject.name);

        if (_score >= _crystalCost[buttonId] && _crystalBuy[buttonId] == false)
        {
            _score -= _crystalCost[buttonId];
            UpdateScoreText();
            _crystalButtons[buttonId].transform.GetChild(1).GetComponent<Text>().text = $"Purchased";
            _crystalBuy[buttonId] = true;
            //_crystalButtons[buttonId].GetComponent<Button>().interactable = false;
        }

        if (_crystalBuy[buttonId] == true)
        {
            SetSkin(buttonId);
        }
    }

    private void SetSkin(int id)
    {
        for (int i = 0; i < _crystalButtons.Length; i++)
        {
            if (i == id)
            {
                _crystalSet[i] = true;
                //_crystalButtons[i].transform.GetChild(2).GetChild(0).GetComponent<Image>().sprite = _checkMarkSprite;
                _crystalButtons[i].transform.GetChild(2).GetChild(0).gameObject.SetActive(true);
                _clickableObjButton.GetComponent<Image>().sprite = _crystalsSkins[i];
                _crystalButtons[i].transform.GetChild(1).GetComponent<Text>().text = $"Set";
            }
            else
            {
                _crystalSet[i] = false;
                //_crystalButtons[i].transform.GetChild(2).GetChild(0).GetComponent<Image>().sprite = _checkMarkSprite;
                _crystalButtons[i].transform.GetChild(2).GetChild(0).gameObject.SetActive(false);
                if (_crystalBuy[i] == true)
                    _crystalButtons[i].transform.GetChild(1).GetComponent<Text>().text = $"Click to set";
            }
        }
    }

    private void OnClick()
    {
        if (Input.touchCount == 1)
        {
            _clickableObjAnim.SetTrigger("Click");
            _score += _addAmount;
            UpdateScoreText();
        }
    }

    private void UpdateScoreText()
    {
        if (_score == 0)
        {
            _scoreText.text = $"{_score}";
        }
        else
        {
            _scoreText.text = $"{PolyLabs.ShortScale.ParseDouble(_score)}";
        }
    }

    private void UpdateClickText()
    {
        _clickText.text = $"+{PolyLabs.ShortScale.ParseDouble(_addAmount)} crystals/click";
    }

    private void UpdateBonusText()
    {
        _bonusText.text = $"+{PolyLabs.ShortScale.ParseDouble(_bonusPerSec)} crystals/per second";
    }

    private void ShowAndHideShop()
    {
        _shopPanel.SetActive(!_shopPanel.activeSelf);
    }

    private void ShowAndHideShop2()
    {
        _shopBonusPanel.SetActive(!_shopBonusPanel.activeSelf);
    }

    private void ShowAndHideCrystal()
    {
        _crystalPanel.SetActive(!_crystalPanel.activeSelf);
    }

    private void DataLoad()
    {
        if (PlayerPrefs.HasKey("save"))
        {
            _sv = JsonUtility.FromJson<Save>(PlayerPrefs.GetString("save"));
            _score = _sv._score;
            _addAmount = _sv._addAmount;
            _bonusPerSec = _sv._bonusPerSec;

            _itemCost = _sv._itemCost;
            _bonusCost = _sv._bonusCost;

            _crystalCost = _sv._crystalCost;
            _crystalBuy = _sv._crystalBuy;
            _crystalSet = _sv._crystalSet;
        }
    }

    private void DataSave()
    {
        _sv._score = _score;
        _sv._addAmount = _addAmount;
        _sv._bonusPerSec = _bonusPerSec;

        _sv._itemCost = new double[_itemCost.Length];
        _sv._bonusCost = new double[_bonusCost.Length];
        _sv._crystalCost = new double[_crystalCost.Length];
        _sv._crystalBuy = new bool[_crystalBuy.Length];
        _sv._crystalSet = new bool[_crystalSet.Length];

        _sv._itemCost = _itemCost;
        _sv._bonusCost = _bonusCost;

        _sv._crystalCost = _crystalCost;
        _sv._crystalBuy = _crystalBuy;
        _sv._crystalSet = _crystalSet;

        PlayerPrefs.SetString("save", JsonUtility.ToJson(_sv));
    }

    void OnApplicationQuit()
    {
        DataSave();
    }

    IEnumerator PerSecBonus()
    {
        while (true)
        {
            _score += _bonusPerSec;
            UpdateScoreText();
            yield return new WaitForSeconds(1f);
        }
    }
}
