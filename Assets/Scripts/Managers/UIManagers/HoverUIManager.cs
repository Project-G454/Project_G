using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Entities;
using Entities.Categories;
using Effects;
using System.Collections.Generic;
using Core.Interfaces;

namespace Core.Managers {
    public class HoverUIManager : MonoBehaviour, IManager
    {
        public static HoverUIManager Instance;

        public GameObject panel;
        public TextMeshProUGUI nameText;
        public Slider hpBar;
        public TextMeshProUGUI hpText;
        public TextMeshProUGUI drawText;
        //public Transform buffIconContainer;

        private Entity currentEntity;

        private void Awake()
        {   
            Hide();
            if (Instance != null && Instance != this) {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);       
        }

        public void Init() {
        }

        public void Show(Entity entity)
        {
            currentEntity = entity;
            panel.SetActive(true);
            entity.OnHpChanged += UpdateHp;
            entity.deckManager.draw.OnPileChanged += UpdateDraw;
            nameText.text = entity.entityName;
            UpdateHp();
            UpdateDraw();
            //UpdateBuffIcons(entity.effects); // 待實作
        }

        public void Hide()
        {
            if (currentEntity != null) {
                currentEntity.OnHpChanged -= UpdateHp;
                currentEntity.deckManager.draw.OnPileChanged -= UpdateDraw;
                currentEntity = null;
            }
            panel.SetActive(false);
        }

        private void UpdateHp()
        {
            hpBar.maxValue = currentEntity.maxHp;
            hpBar.value = currentEntity.currentHp;
            hpText.text = $"{currentEntity.currentHp}/{currentEntity.maxHp}";
        }

        private void UpdateDraw()
        {
            drawText.text = currentEntity.deckManager.draw.Count.ToString();
        }

        // private void UpdateBuffIcons(List<Effect> effects)
        // {
        //     // TODO: 清空並重新生成 buff icon prefab
        //     foreach (Transform child in buffIconContainer)
        //         Destroy(child.gameObject);

        //     foreach (var effect in effects)
        //     {
        //         //GameObject icon = Instantiate(effect.iconPrefab, buffIconContainer);
        //         // 可加上 tooltip hover 顯示說明
        //     }
        // }
    }
}
