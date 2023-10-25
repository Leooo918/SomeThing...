using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.Events;

public class ItemDevider : MonoBehaviour
{
    private TextMeshProUGUI itemNameText = null;
    private TMP_InputField inputField = null;
    private TextMeshProUGUI maxNumText = null;
    private Slider slider = null;
    private Button acceptBtn = null;
    private Button cancelBtn = null;
    private Button valueUpBtn = null;
    private Button valueDownBtn = null;

    public ExpendableItem item = null;
    public event UnityAction onAccept = null;
    public event Action<int> acceptAction = null;

    private int maxNum = 15; //임시 나중에 아이템에서 현재 아이템 수에서 가져와야함

    private void Awake()
    {
        itemNameText = transform.Find("ItemDeviderHelper/TopPanel/Text").GetComponent<TextMeshProUGUI>();
        inputField = transform.Find("ItemDeviderHelper/Slider/InputField").GetComponent<TMP_InputField>();
        maxNumText = transform.Find("ItemDeviderHelper/Slider/InputField/MaxText").GetComponent<TextMeshProUGUI>();
        slider = transform.Find("ItemDeviderHelper/Slider").GetComponent<Slider>();
        acceptBtn = transform.Find("ItemDeviderHelper/TopPanel/Accept").GetComponent<Button>();
        cancelBtn = transform.Find("ItemDeviderHelper/TopPanel/Cancel").GetComponent<Button>();
        valueUpBtn = slider.transform.Find("Add").GetComponent<Button>();
        valueDownBtn = slider.transform.Find("Minus").GetComponent<Button>();


        onAccept += this.OnAccept;
        onAccept += this.CloseScreen;
    }

    public void Init(int maxNum, string itemName)
    {
        InventoryManager.instance.isDeviding = true;
        this.maxNum = maxNum;
        this.itemNameText.SetText(itemName);
        maxNumText.SetText("/<size=35><sub>" + maxNum.ToString() + "</sub></size>");
        slider.maxValue = maxNum;

        acceptBtn.onClick.AddListener(onAccept);
        inputField.onValueChanged.AddListener(this.OnValueChange);
        slider.onValueChanged.AddListener(this.OnSliderValueChange);
        valueUpBtn.onClick.AddListener(() =>
        {
            int a = (int)slider.value;
            a = Mathf.Clamp(a, 0, maxNum);
            OnValueChange((a + 1).ToString());
        });
        valueDownBtn.onClick.AddListener(() =>
        {
            int a = (int)slider.value;
            a = Mathf.Clamp(a, 0, maxNum);
            OnValueChange((a - 1).ToString());
        });
        cancelBtn.onClick.AddListener(() =>
        {
            item.CancelDeviding();
            CloseScreen();
        });

        transform.SetAsLastSibling();
    }


    private void OnAccept()
    {
        acceptAction?.Invoke((int)slider.value);
    }

    private void OnValueChange(string value)
    {
        if (value == "") return;

        int devideNum = int.Parse(value);
        if (int.TryParse(value, out int i) == false)
        {
            devideNum = 1;
        }


        devideNum = Mathf.Clamp(devideNum, 1, maxNum);
        inputField.text = devideNum.ToString();

        slider.value = devideNum;
    }

    private void OnSliderValueChange(float value)
    {
        slider.value = (int)value;
        inputField.text = slider.value.ToString();
    }

    private void CloseScreen()
    {
        InventoryManager.instance.isDeviding = false;
        acceptAction = null;
        gameObject.SetActive(false);
    }
}
