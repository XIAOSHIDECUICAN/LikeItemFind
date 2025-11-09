using System;
using Duckov.Quests.Conditions;
using System.Collections.Generic;
using System.Linq;
using Duckov.UI;
using Duckov.Utilities;
using ItemStatsSystem;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Duckov.MiniMaps;
using Duckov.Scenes;
using static UnityEngine.Splines.SplineInstantiate;
using Duckov.MiniMaps.UI;
using System.Reflection;
using LikeItemFind.Models;
using System.Xml.Linq;
using UnityEngine.InputSystem;
using static UnityEngine.Rendering.DebugUI;

namespace LikeItemFind
{

    public class ModBehaviour : Duckov.Modding.ModBehaviour
    {
        private string _logPrefix = "likeItemFind";
        private bool _mapActive;
        private HashSet<GameObject> _questCircleObjects = new HashSet<GameObject>();
        public InteractableLootbox[] AllLootboxesCache;
        public  InteractablePickup[] AllPickupsCache;
        public bool ShowHighValueItem = false;

        void Awake()
        {
            Debug.Log("likeItemFind Loaded!!!");
        }
        void OnDestroy()
        {
        }
        void OnEnable()
        {
            Debug.Log(_logPrefix + "已启用。订阅 关卡初始化成功跟监听视图变化 事件。");
            View.OnActiveViewChanged += OnActiveViewChanged;
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        
        void OnDisable()
        {
            Debug.Log(_logPrefix + "已禁用。取消订阅 取消关卡初始化成功跟监听视图变化 事件并清理缓存。");
            View.OnActiveViewChanged -= OnActiveViewChanged;
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
        void Start()
        {
        }
        // 不知道为什么我用这个方法电脑特别卡，暂时删除了
        //void Update()
        //{
        //    // 按下pageUpKey展示高价值物品
        //    if (Keyboard.current.pageUpKey.wasReleasedThisFrame)
        //    {
        //        ShowHighValueItem = true;
        //        Debug.Log($"按下pageup1.3");
        //    }
        //    // 按下pageDownKey隐藏高价值物品
        //    if (Keyboard.current.pageDownKey.wasReleasedThisFrame)
        //    {
        //        ShowHighValueItem = false;
        //        Debug.Log($"按下pageup1.3");
        //    }
            
        //}
        // 场景加载完以后扫描地图上掉落的东西，根据手动标记，是否显示地图上的位置
        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            EventPerformanceMonitor.StartTiming("SceneLoaded");
            string currentMapId = scene.name;
            
            if (string.IsNullOrEmpty(currentMapId)) { Debug.LogWarning(_logPrefix + "无法获取当前地图 ID。"); return; }

            Debug.Log(currentMapId + $"关卡已加载完。");
            AllLootboxesCache= UnityEngine.Object.FindObjectsOfType<InteractableLootbox>();
            AllPickupsCache = UnityEngine.Object.FindObjectsOfType<InteractablePickup>();
            Debug.Log(currentMapId + $"关卡箱子数量。{AllLootboxesCache.Length}");
            Debug.Log(currentMapId + $"关卡掉落物数量。{AllPickupsCache.Length}");
            EventPerformanceMonitor.EndTiming("SceneLoaded");
        }
        
        // 开始绘制
        private void BeginDraw()
        {
            EventPerformanceMonitor.StartTiming("BeginDraw");
            if (_mapActive) return;
            Debug.Log(_logPrefix + "地图已打开。更新位置并绘制...");

            _mapActive = true;

            DrawCircles();
            EventPerformanceMonitor.EndTiming("BeginDraw");
        }
        // 绘制结束
        private void EndDraw()
        {
            if (_mapActive)
            {
                Debug.Log(_logPrefix + "地图已关闭。清理圆圈...");
                _mapActive = false;
                ClearCircle();
            }
        }
        // 清理地图绘制圆圈
        private void ClearCircle()
        {
            Debug.Log(_logPrefix + $"正在清理 {_questCircleObjects.Count} 个任务圆圈...");
            foreach (GameObject circle in _questCircleObjects)
            {
                if (circle != null)
                {
                    Destroy(circle);
                }
            }
            _questCircleObjects.Clear();
        }
        // 绘制圆圈
        private void DrawCircles()
        {
            Debug.Log(_logPrefix + "DrawCircles 调用。");
            ClearCircle();
            AllLootboxesCache = UnityEngine.Object.FindObjectsOfType<InteractableLootbox>();
            AllPickupsCache = UnityEngine.Object.FindObjectsOfType<InteractablePickup>();
            string playerSubSceneID = LevelManager.GetCurrentLevelInfo().activeSubSceneID;
            Debug.Log(_logPrefix + $"Player SubSceneID: '{playerSubSceneID ?? "null"}'");

            int circlesDrawn = 0;
            //Debug.Log(_logPrefix + $"尝试掉落物被标记的东西标记...");
            // 绘制掉落物被标记的标记
            foreach (var pickup in AllPickupsCache)
            {
                if (ItemWishlist.GetWishlistInfo(pickup.ItemAgent.Item.TypeID).isManuallyWishlisted
                    ||(ShowHighValueItem && pickup.ItemAgent.Item.Value>10000))
                {
                    DrawQuestMarker(pickup.ItemAgent.transform.position, 10f, pickup.ItemAgent.Item.DisplayName);
                    circlesDrawn++;
                }
            }
            Debug.Log(_logPrefix + $"尝试绘制箱子里被标记的东西标记...");

            //// 绘制箱子里被标记的东西
            foreach (var lootbox in AllLootboxesCache)
            {
                string boxName = lootbox.name ?? string.Empty;
                // 排除玩家仓库跟宠物背包
                if (boxName.IndexOf("PetProxy") < 0 && boxName.IndexOf("PlayerStorage") < 0)
                {
                    Debug.Log($"箱子方法里的ShowHighValueItem={ShowHighValueItem}");
                    // 筛选被标记物品的箱子，或者有价值高的物品的箱子
                    var isManuallyWishlistedList = lootbox.Inventory.Content
                        .Where(x => x != null && ItemWishlist.GetWishlistInfo(x.TypeID).isManuallyWishlisted
                                    || (ShowHighValueItem && x.Value > 10000));

                    // 有被标记物品的箱子跟有价值高物品的箱子就在地图上绘制出来

                    if (isManuallyWishlistedList?.Count() > 0)
                    {
                        foreach (var item in isManuallyWishlistedList)
                        {
                            DrawQuestMarker(lootbox.transform.position, 10f, item.DisplayName);
                            circlesDrawn++;
                        }
                    }
                }
               
            }

            Debug.Log(_logPrefix + $"绘制了 {circlesDrawn} 个圆圈。");
        }
        // 具体画图方法
        private void DrawQuestMarker(Vector3 position, float radius, string itemName)
        {
            Debug.Log(_logPrefix + $"正在为物品 '{itemName}' 在 {position} 处绘制半径为 {radius} 的标记。");

            GameObject markerObject = new GameObject($"item_{itemName}");
            markerObject.transform.position = position;

            SimplePointOfInterest poi;
            try
            {
                poi = markerObject.AddComponent<SimplePointOfInterest>();
            }
            catch (Exception e)
            {
                Debug.LogError(_logPrefix + $"AddComponent<SimplePointOfInterest> 失败: {e.Message}。请确保你的 Mod 引用了包含该类的 Assembly-CSharp.dll。");
                Destroy(markerObject);
                return;
            }

            Sprite iconToUse = GetQuestIcon();

            try
            {
                poi.Setup(iconToUse, itemName, followActiveScene: true);
            }
            catch (Exception e)
            {
                Debug.LogError(_logPrefix + $"poi.Setup 失败: {e.Message}。图标(iconToUse)是否为null?");
            }

            poi.Color = Color.green;
            poi.IsArea = true;
            poi.AreaRadius = radius;

            poi.ShadowColor = Color.black;
            poi.ShadowDistance = 0f;

            if (MultiSceneCore.MainScene.HasValue)
            {
                SceneManager.MoveGameObjectToScene(markerObject, MultiSceneCore.MainScene.Value);
            }

            _questCircleObjects.Add(markerObject);
        }
        // 获取图标方法
        private Sprite GetQuestIcon()
        {

            List<Sprite> allIcons = MapMarkerManager.Icons;

            if (allIcons == null)
            {
                Debug.LogError(_logPrefix + "MapMarkerManager.Icons 列表为 null! 无法获取图标。");
                return null;
            }

            if (allIcons.Count > 0)
            {
                Debug.LogWarning(_logPrefix + "使用列表中的第一个图标作为后备。");
                return allIcons[0];
            }

            Debug.LogError(_logPrefix + "MapMarkerManager.Icons 列表为空! 没有任何图标。");
            return null;
        }
        // 监听活动视图改变事件
        private void OnActiveViewChanged()
        {
            if (IsMapOpen())
            {
                BeginDraw();
            }
            else
            {
                EndDraw();
            }
        }
        // 判断当前视图是否是地图视图
        private bool IsMapOpen()
        {
            MiniMapView view = MiniMapView.Instance;

            if (view != null)
            {
                return view == View.ActiveView;
            }

            return false;
        }
        

    }
}