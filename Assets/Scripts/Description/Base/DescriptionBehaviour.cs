using UnityEngine;

namespace Descriptions {
    public class DescriptionBehaviour: MonoBehaviour {
        public Description desc;

        [HideInInspector] 
        public DescriptionView view;
        
        [HideInInspector] 
        public GameObject obj;

        public void Init(GameObject obj, Description data) {
            this.desc = data;
            this.view = GetComponent<DescriptionView>();
            this.view.SetView(data);
            this.obj = obj;
        }
    }
}
