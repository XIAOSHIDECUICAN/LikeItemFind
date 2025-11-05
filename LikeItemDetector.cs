using DuckTest.Models;
using ItemStatsSystem;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Unity.VisualScripting.Member;

namespace DuckTest;

public class LikeItemDetector
{
    public readonly HashSet<InteractableLootbox> AllLootboxesCache = new HashSet<InteractableLootbox>();
    public readonly HashSet<InteractablePickup> AllPickupsCache = new HashSet<InteractablePickup>();

    public List<LikeItemSpawn> LikeItemSpawnList = new List<LikeItemSpawn>();

    // 添加新的标点列表数据
    //public void AddLikeItem(Item item)
    //{
    //    foreach (var interactablePickup in AllPickupsCache)
    //    {
    //        if (interactablePickup != null)
    //        {
    //            if (IsValuableItem(interactablePickup.ItemAgent.Item))
    //            {
    //                LikeItemSpawn likeItemSpawn = new LikeItemSpawn
    //                {
    //                    LikeItem = interactablePickup.ItemAgent.Item,
    //                    Position = interactablePickup.ItemAgent.Item.transform.position,
    //                };
    //                if (LikeItemSpawnList != null) LikeItemSpawnList.Add(likeItemSpawn);
    //            }
    //        }
    //    }

    //    if (LikeItemSpawnList != null) LikeItemSpawnList.Remove(LikeItemSpawnList.Single(x => x.LikeItem == item));
    //}

    // 更新地图上标点
    public void UpdatePickupLikeItem()
    {
        //LikeItemSpawnList.Clear();
        //Debug.Log($"根据地上掉落物更新需要绘制的物品列表");
        //foreach (InteractablePickup interactablePickup in Object.FindObjectsOfType<InteractablePickup>())
        //{
        //    if (interactablePickup != null)
        //    {
        //        if (IsValuableItem(interactablePickup.ItemAgent.Item))
        //        {
        //            LikeItemSpawn likeItemSpawn = new LikeItemSpawn
        //            {
        //                LikeItem = interactablePickup.ItemAgent.Item,
        //                Position = interactablePickup.transform.position,
        //            };
        //            LikeItemSpawnList.Add(likeItemSpawn);
        //        }
        //    }
        //}
        //Debug.Log($"查找箱子里，获取更新需要绘制的物品列表");
        //foreach (InteractableLootbox interactableLootbox in AllLootboxesCache)
        //{
        //    if (interactableLootbox != null && interactableLootbox.Inventory != null)
        //    {
        //        string boxName = interactableLootbox.name ?? string.Empty;
        //        Debug.Log($"箱子名字：{boxName}");
        //        if (boxName.IndexOf("PetProxy") < 0 && boxName.IndexOf("PlayerStorage") < 0)
        //        {
        //            foreach (var item in interactableLootbox.Inventory)
        //            {
        //                if (IsValuableItem(item))
        //                {
        //                    LikeItemSpawn likeItemSpawn = new LikeItemSpawn
        //                    {
        //                        LikeItem = item,
        //                        Position = interactableLootbox.transform.position,
        //                    };
        //                    LikeItemSpawnList.Add(likeItemSpawn);
        //                }
        //            }
        //        }
        //    }
        //}

    }
    // 根据掉落物的item， 删除单个标点数据
    public void RemoveLikeItem(Item item)
    {
        Debug.Log($"根据item获取标点数量：{LikeItemSpawnList.Count(x => x.LikeItem == item)}个");
        LikeItemSpawnList.Remove(LikeItemSpawnList.Single(x => x.LikeItem == item));
    }
    // 搜索全图掉落物品跟箱子物品创建缓存，并生成标点列表
    public void CreateObjectCache()
    {
        //LikeItemSpawnList.Clear();
        AllLootboxesCache.RemoveWhere((InteractableLootbox obj) => obj == null);
        AllPickupsCache.RemoveWhere((InteractablePickup obj) => obj == null);

        //bool hasDuplicates = (Object.FindObjectsOfType<InteractableLootbox>()).GroupBy(x => x.GetInstanceID()).Any(g => g.Count() > 1);
        //Debug.Log($"是否有重复的箱子：{(hasDuplicates?"是":"否")}");
        foreach (InteractableLootbox interactableLootbox in Object.FindObjectsOfType<InteractableLootbox>())
        {
            if (interactableLootbox != null && interactableLootbox.Inventory != null)
            {
                string boxName = interactableLootbox.name ?? string.Empty;
                Debug.Log($"箱子名字：{boxName}");
                if (boxName.IndexOf("PetProxy") < 0 && boxName.IndexOf("PlayerStorage") < 0)
                {
                    AllLootboxesCache.Add(interactableLootbox);
                    //var lootboxCount = interactableLootbox.Inventory.GetItemCount();
                    //Debug.Log($"箱子物品数量1：{lootboxCount}");
                    //Debug.Log($"箱子物品数量2：{interactableLootbox.Inventory.Content.Count}");
                    //for (int i = 0; i < interactableLootbox.Inventory.GetItemCount(); i++)
                    //{

                    //    if (IsValuableItem(interactableLootbox.Inventory[i]))
                    //    {
                    //        LikeItemSpawn likeItemSpawn = new LikeItemSpawn
                    //        {
                    //            LikeItem = interactableLootbox.Inventory[i],
                    //            Position = interactableLootbox.transform.position,
                    //            From = interactableLootbox.GetInstanceID(),
                    //            Index = i
                    //        };
                    //        LikeItemSpawnList.Add(likeItemSpawn);
                    //    }
                    //}
                    //foreach (var item in interactableLootbox.Inventory)
                    //{
                    //    if (IsValuableItem(item))
                    //    {
                    //        LikeItemSpawn likeItemSpawn = new LikeItemSpawn
                    //        {
                    //            LikeItem = item,
                    //            Position = interactableLootbox.transform.position,
                    //        };
                    //        LikeItemSpawnList.Add(likeItemSpawn);
                    //    }
                    //}
                }
            }
        }

        foreach (InteractablePickup interactablePickup in Object.FindObjectsOfType<InteractablePickup>())
        {
            if (interactablePickup != null)
            {
                AllPickupsCache.Add(interactablePickup);
                //if (IsValuableItem(interactablePickup.ItemAgent.Item))
                //{
                //    LikeItemSpawn likeItemSpawn = new LikeItemSpawn
                //    {
                //        LikeItem = interactablePickup.ItemAgent.Item,
                //        Position = interactablePickup.transform.position,
                //        Radius = 10f
                //    };
                //    LikeItemSpawnList.Add(likeItemSpawn);
                //}
            }
        }

        //Debug.Log($"喜爱跟高价值的物品数量：{LikeItemSpawnList.Count}。");
    }

    public void UpdateAllLootboxesCache(InteractableLootbox lootbox)
    {
        var existingLootbox = AllLootboxesCache.SingleOrDefault(x =>
            x != null && x.GetInstanceID() == lootbox.GetInstanceID());

        if (existingLootbox != null)
        {
            // 移除旧的，添加新的
            AllLootboxesCache.Remove(existingLootbox);
            AllLootboxesCache.Add(lootbox);
        }
        else
        {
            // 如果不存在，直接添加
            AllLootboxesCache.Add(lootbox);
        }
    }
    public void RefreshObjectCache(InteractablePickup interactablePickupItem)
    {
        AllPickupsCache.Remove(interactablePickupItem);
    }
    public void Clear()
    {
        AllLootboxesCache.Clear();
        AllPickupsCache.Clear();
    }

    private bool IsValuableItem(Item item)
    {
        if (item == null) return false;
        var wishlistInfo = ItemWishlist.GetWishlistInfo(item.TypeID);
        return wishlistInfo.isManuallyWishlisted || item.Value > 10000;
    }
}