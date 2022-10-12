using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Excalibur;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

public class TestGrid : MonoBehaviour
{
    public TextMeshProUGUI text;
    public TextMeshProUGUI text2;
    public VirtualGrid gird;
    public int count = 5;
    public Button[] button;
    List<TestSlotData> list = new List<TestSlotData>();

    private void Awake()
    {
        button[0].onClick.AddListener(CreateNew);
        button[1].onClick.AddListener(DeleteSelected);
        button[2].onClick.AddListener(IncreaseSelectedID);
        button[3].onClick.AddListener(DecreaseSelectedID);
    }

    private void Start()
    {
        gird.onSelectEvent += Show;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            text.text = string.Empty;
            text2.text = string.Empty;
            list.Clear();
            for (int i = 0; i < count; ++i)
            {
                list.Add(new TestSlotData(i + 1));
            }
            gird.ProvideDatas(list);
        }
    }

    public void Show(VirtualSlot slot)
    {
        if (slot != null && slot.IsSelected)
        {
            text.text = (gird.SelectedData as TestSlotData).id.ToString();
            text2.text = (slot.ItemData as TestSlotData).id.ToString();
        }
        else
        {
            text.text = string.Empty;
            text2.text = string.Empty;
        }
    }

    void CreateNew()
    {
        if (!CanOperate())
            return;

        int count = Random.Range(1, 5);
        List<TestSlotData> temp = new List<TestSlotData>();
        while (count > 0)
        {
            temp.Add(new TestSlotData(Random.Range(1, this.count)));
            --count;
        }
        list.AddRange(temp);
        gird.OnAddItem(list);
    }

    void DeleteSelected()
    {
        if (!CanOperate())
            return;

        if (gird.SelectedData != null)
        {
            TestSlotData data = gird.SelectedData as TestSlotData;
            list.Remove(data);
            gird.OnDeleteSelectedData();
        }
    }

    void IncreaseSelectedID()
    {
        if (!CanOperate())
            return;

        if (gird.SelectedData != null)
        {
            ++(gird.SelectedData as TestSlotData).id;
            gird.OnRefreshSelectedData();
        }
    }

    void DecreaseSelectedID()
    {
        if (!CanOperate())
            return;

        if (gird.SelectedData != null)
        {
            TestSlotData data = gird.SelectedData as TestSlotData;
            --data.id;
            if (data.id == 0)
            {
                DeleteSelected();
                return;
            }
            gird.OnRefreshSelectedData();
        }
    }

    bool CanOperate()
    {
        return gird.gameObject.activeInHierarchy;
    }
}
