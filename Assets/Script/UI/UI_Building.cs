namespace DevelopersHub.ClashOfWhatecer
{

    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine.UI;
    using UnityEngine;

    public class UI_Building : MonoBehaviour
    {
        [SerializeField] private int _prefabIndex = 0;
        [SerializeField] private Button _button = null;


        private void Start()
        {
            _button.onClick.AddListener(Clicked);
        }


        private void Clicked()
        {
            UI_Shop.instance.SetStatus(false);
            UI_Shop.instance.SetStatus(true);

            Vector3 position = Vector3.zero;

            Building building = Instantiate(UI_Main.instance._buildingPrefabs[_prefabIndex], position, Quaternion.identity);
        }






    }
}