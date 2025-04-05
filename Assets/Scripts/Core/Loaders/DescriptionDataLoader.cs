using System.Collections.Generic;
using System.Linq;
using Descriptions.Models;
using UnityEngine;

namespace Core.Loaders.Descriptions {
    public static class DescriptionLoader {
        public static List<DescriptionData> LoadAll() {
            return Resources.LoadAll<DescriptionData>("Descriptions").ToList();
        }
    }
}
