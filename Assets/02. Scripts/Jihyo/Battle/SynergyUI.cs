using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 필드 기준 시너지 집계를 표시합니다. 상위 3개는 슬롯에, 그 외는 Overflow 영역에 표시합니다.
/// </summary>
public class SynergyUI : MonoBehaviour
{
    [System.Serializable]
    public class SynergySlot
    {
        public GameObject root;
        public Image icon;
        public Image gauge;
    }

    /// <summary>
    /// SynergyData.ID(예: 210001)와 동일한 문자열로 매칭
    /// </summary>
    [System.Serializable]
    public class SynergyVisualBinding
    {
        public string synergyId;
        public Sprite icon;
        public Sprite[] gaugeSprites;
    }

    [SerializeField] private SynergySlot[] _slots = new SynergySlot[3];

    [Header("시너지별 스프라이트 (synergyId = SynergyData.ID 또는 Name, 또는 Synergy_ 접두 제거한 뒤 이름)")]
    [SerializeField] private SynergyVisualBinding[] _synergyVisuals;

    [Header("3개 초과 시")]
    [SerializeField] private GameObject _overflowRoot;

    private Dictionary<string, SynergyVisualBinding> _visualById;

    private void Awake()
    {
        BuildVisualLookup();
    }

    private void BuildVisualLookup()
    {
        _visualById = new Dictionary<string, SynergyVisualBinding>(StringComparer.OrdinalIgnoreCase);
        if (_synergyVisuals == null)
        {
            return;
        }

        foreach (SynergyVisualBinding binding in _synergyVisuals)
        {
            if (binding == null || string.IsNullOrEmpty(binding.synergyId))
            {
                continue;
            }

            _visualById[binding.synergyId.Trim()] = binding;
        }
    }

    private void Start()
    {
        if (GameData.Instance == null)
        {
            return;
        }

        GameData.Instance.SynergyChange += OnSynergyChange;
        GameData.Instance.GetSynergyData();
    }

    private void OnDestroy()
    {
        if (GameData.Instance != null)
        {
            GameData.Instance.SynergyChange -= OnSynergyChange;
        }
    }

    private void OnSynergyChange(Dictionary<string, SynergyTotalData> synergyMap)
    {
        if (_slots == null || _slots.Length == 0)
        {
            return;
        }

        List<SynergyTotalData> ordered = synergyMap.Values
                                                   .Where(s => s.synergyData != null)
                                                   .OrderByDescending(s => s.count)
                                                   .ThenBy(s => s.synergyData.Tier)
                                                   .ToList();

        int total = ordered.Count;
        int showSlots = Mathf.Min(_slots.Length, total);

        for (int i = 0; i < _slots.Length; i++)
        {
            SynergySlot slot = _slots[i];
            if (slot?.root == null)
            {
                continue;
            }

            bool show = i < showSlots;
            slot.root.SetActive(show);
            if (!show)
            {
                continue;
            }

            SynergyTotalData entry = ordered[i];
            SynergyData sd = entry.synergyData;

            TryGetBinding(sd, out SynergyVisualBinding visual);
            ApplyIcon(slot.icon, visual);
            ApplyGauge(slot.gauge, visual, entry);
        }

        int overflow = total - _slots.Length;
        if (_overflowRoot != null)
        {
            bool hasOverflow = overflow > 0;
            _overflowRoot.SetActive(hasOverflow);
        }
    }

    private bool TryGetBinding(SynergyData sd, out SynergyVisualBinding binding)
    {
        binding = null;
        if (sd == null || _visualById == null)
        {
            return false;
        }

        if (!string.IsNullOrEmpty(sd.ID) && _visualById.TryGetValue(sd.ID.Trim(), out binding))
        {
            return true;
        }

        string name = sd.Name != null ? sd.Name.Trim() : string.Empty;
        if (name.Length > 0)
        {
            if (_visualById.TryGetValue(name, out binding))
            {
                return true;
            }

            const string prefix = "Synergy_";
            if (name.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
            {
                string shortName = name.Substring(prefix.Length);
                if (shortName.Length > 0 && _visualById.TryGetValue(shortName, out binding))
                {
                    return true;
                }
            }
        }

        return false;
    }

    private static void ApplyIcon(Image iconImage, SynergyVisualBinding visual)
    {
        if (iconImage == null)
        {
            return;
        }

        if (visual != null && visual.icon != null)
        {
            iconImage.sprite = visual.icon;
            iconImage.enabled = true;
        }
        else
        {
            iconImage.sprite = null;
            iconImage.enabled = false;
        }
    }

    private static void ApplyGauge(Image gaugeImage, SynergyVisualBinding visual, SynergyTotalData entry)
    {
        if (gaugeImage == null)
        {
            return;
        }

        if (visual == null || visual.gaugeSprites == null || visual.gaugeSprites.Length == 0)
        {
            gaugeImage.sprite = null;
            gaugeImage.enabled = false;
            return;
        }

        int idx = GetGaugeSpriteIndex(entry, visual.gaugeSprites.Length);
        Sprite frame = visual.gaugeSprites[idx];
        if (frame != null)
        {
            gaugeImage.sprite = frame;
            gaugeImage.enabled = true;
        }
        else
        {
            gaugeImage.sprite = null;
            gaugeImage.enabled = false;
        }
    }

    /// <summary>
    /// GameData의 count를 게이지 프레임 인덱스로 매핑, 임계값 기반 단계는 이후 데이터 설계에 맞게 교체 가능
    /// </summary>
    private static int GetGaugeSpriteIndex(SynergyTotalData entry, int spriteCount)
    {
        return Mathf.Clamp(entry.count, 0, spriteCount - 1);
    }
}
