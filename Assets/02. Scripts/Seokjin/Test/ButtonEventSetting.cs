using System;
using System.Collections.Generic;
using System.Data;
using System.Text.RegularExpressions;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ButtonEventSetting : MonoBehaviour
{
    [SerializeField] Button testbutton, testbutton1, testbutton2, testbutton3;
    [SerializeField] Slider testslider1, testslider2, testslider3, testslider4;
    [SerializeField] TextMeshProUGUI logtext;
    public SynergyTotalData synergyTotalData;
    public string testID;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        AkBankManager.LoadBank("Test", false, false);
    }
    void Start()
    {
        testbutton1.onClick.AddListener(() =>
        {
            AudioManager.Instance.PlayBGM("TOG_Battle_1");
        });
        testbutton2.onClick.AddListener(() =>
        {
            AudioManager.Instance.PlaySFX("TOG_Hero_Shield");
        });

        testbutton3.onClick.AddListener(() =>
        {
            AudioManager.Instance.PlaySFX("TOG_UI_Battle_Deck_AttackButton_Click");
            AudioManager.Instance.PlaySFX("TOG_UI_Battle_Deck_AttackButton_Hover");
        });

        testbutton.onClick.AddListener(() =>
        {
            AkSoundEngine.PostEvent("TOG_Battle_1", gameObject);
            /*List<CardData> tcardlist = DataCenter.Instance.GetFront4_End3_CardList("1100", "001");
            Debug.Log(tcardlist);

            synergyTotalData = DataCenter.Instance.GetSynergyTotalData(testID);
            synergyTotalData.synergyData.Description = string.Format(synergyTotalData.synergyData.Description, 10);
            Match m = Regex.Match(synergyTotalData.synergyData.Description, @"\(([^)]+)\)");
            if (m.Success)
            {
                string replace = m.Groups[1].Value;
                string processedFormula = string.Format(replace, 30);
                string playerDef = "30";

                // 거듭제곱(^) 처리 (DataTable은 ^를 지원 안 하므로 단순 곱셈으로 치환 예시)
                // ※ 복잡한 수식이라면 처음에 추천드린 NCalc 라이브러리가 훨씬 편합니다.
                processedFormula = processedFormula.Replace($"{playerDef}^{2}", $"({playerDef}*{playerDef})");

                // 4. 계산 진행
                DataTable dt = new DataTable();
                var result = dt.Compute(processedFormula, "");
                float damage = Convert.ToSingle(result);
                synergyTotalData.synergyData.Description = string.Format(synergyTotalData.synergyData.Description, damage);
            }

            logtext.text = synergyTotalData.synergyData.Name + "\n" +
                            synergyTotalData.synergyData.Description;*/
        });

        StartCoroutine("skil_1234","1234");
        SendMessage("skil_4321", 4321);

        testslider1.onValueChanged.AddListener(value => AudioManager.Instance.SetMasterVolume(value));
        testslider2.onValueChanged.AddListener(value => AudioManager.Instance.SetBGMVolume(value));
        testslider3.onValueChanged.AddListener(value => AudioManager.Instance.SetSFXVolume(value));
        testslider4.onValueChanged.AddListener(value => AudioManager.Instance.SetUIVolume(value));
    }
    public void skil_1234(object value)
    {
        if (int.TryParse(value.ToString(), out int result))
            Debug.Log("TEST skil_1234 LOG" + result);
    }
    public void skil_4321(object value)
    {
        if (int.TryParse(value.ToString(), out int result))
            Debug.Log("TEST skil_4321 LOG" + result);
    }
}
