using System;
using System.Collections.Generic;
using Cards.Data;
using Core.Loaders.Cards;
using Entities;
using UnityEngine;

namespace Reward.Factories {
    public static class RewardCardFactory {
        public static GameObject CreateRewardCard(EntityClasses targetClass, GameObject prefab, Transform parent, Action<RewardCard> OnCardSelected) {
            List<CardData> dataList = CardDataLoader.LoadByClass(targetClass);
            CardData data = dataList[UnityEngine.Random.Range(0, dataList.Count)];

            
            GameObject newItem = GameObject.Instantiate(prefab, parent);
            RewardCard item = newItem.GetComponent<RewardCard>();
            item.Init(data, OnCardSelected);
            return newItem;
        }
    }
}
