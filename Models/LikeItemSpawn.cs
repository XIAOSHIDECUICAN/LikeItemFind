using ItemStatsSystem;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace LikeItemFind.Models
{
    public class LikeItemSpawn
    {
        public Item LikeItem { get; set; }
        public Vector3 Position { get; set; }
        public float Radius { get; set; } = 10f;

        public int From { get; set; }

        public int Index { get; set; }
        public string BoxName { get; set; }
    }
}
