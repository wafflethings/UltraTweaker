using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UltraTweaker.UIElements
{
    public class ShowBoxOnHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public GameObject Box;

        public void Start()
        {
            if (Box == null)
                Box = transform.parent.gameObject.ChildByName("Box");
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            Box.SetActive(true);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            Box.SetActive(false);
        }
    }
}
