using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class Logging : MonoBehaviour
    {
        [SerializeField] private InputField _userName;
        [SerializeField] private Client _client;
        [SerializeField] private GameObject _logPanel;

        public void LogIn()
        {
            var userName = _userName.text;
            _client.Init(userName);

            _logPanel.SetActive(false);
        }
    }
}