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
    [SerializeField] Button testbutton1, textbutton2, textbutton3;
    [SerializeField] TextMeshProUGUI logtext;
    public SynergyTotalData synergyTotalData;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        testbutton1.onClick.AddListener(() =>
        {
            synergyTotalData = DataCenter.Instance.GetSynergyTotalData("210002");
            synergyTotalData.synergyData.Description = string.Format(synergyTotalData.synergyData.Description, 10);
            Match m = Regex.Match(synergyTotalData.statusEffectDataa.Description, @"\(([^)]+)\)");
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
                synergyTotalData.statusEffectDataa.Description = string.Format(synergyTotalData.statusEffectDataa.Description, damage);
            }

            logtext.text = synergyTotalData.synergyData.Description + "\n" +
                            synergyTotalData.statusEffectDataa.Description;
        });
        textbutton2.onClick.AddListener(() =>
        {
            DataCenter.Instance.SortUserCards(SortType.Defense);
        });

        textbutton3.onClick.AddListener(() =>
        {
            DataCenter.Instance.SortUserCards(SortType.Grade);
        });

        StartCoroutine("skil_1234","1234");
        SendMessage("skil_4321", 4321);

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
