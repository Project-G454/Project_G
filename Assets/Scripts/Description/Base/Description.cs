using Descriptions.Models;
using UnityEngine;

namespace Descriptions {
    public class Description {
        public int id;
        public string title;
        public string description;

        public Description(DescriptionData data) {
            this.id = data.id;
            this.title = data.title;
            this.description = data.description;
        }

        public Description(string title, string description) {
            this.title = title;
            this.description = description;
        }
    }
}
