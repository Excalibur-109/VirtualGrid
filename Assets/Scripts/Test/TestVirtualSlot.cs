using Excalibur;
using UnityEngine;
using TMPro;

public class TestSlotData : IItemData
{
    public int id;
    public TestSlotData(int id)
    {
        this.id = id;
    }
}

public class TestVirtualSlot : VirtualSlot
{
    TextMeshProUGUI text;
    TestSlotData data;

    protected override void Awake()
    {
        base.Awake();
        text = GetComponentInChildren<TextMeshProUGUI>();
        float r = Random.Range(0, 256) / 255f;
        float g = Random.Range(0, 256) / 255f;
        float b = Random.Range(0, 256) / 255f;
        background.color = new Color(r, g, b);
    }

    protected override void OnRefresh()
    {
        data = GetItemData<TestSlotData>();
        if (text != null)
            text.text = data.id.ToString();
    }
}
