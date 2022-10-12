using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Excalibur;
using UnityEngine.UI;
using TMPro;

public class TestGridKit : MonoBehaviour
{
    public TestGrid[] tests;

    TestGrid showd;

    private void Awake()
    {
        for (int i = 0; i < tests.Length; ++i)
        {
            tests[i].gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        for (int i = 0; i < tests.Length; ++i)
        {
            if (tests[i].gameObject.activeInHierarchy)
            {
                if (showd == null)
                {
                    showd = tests[i];
                    break;
                }
                else if (showd != tests[i])
                {
                    showd.gameObject.SetActive(false);
                    showd = tests[i];
                }
            }
        }
    }
}
