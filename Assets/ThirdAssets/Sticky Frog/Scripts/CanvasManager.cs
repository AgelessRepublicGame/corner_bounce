using UnityEngine;

namespace stickyfrogdemo
{
    public class CanvasManager : MonoBehaviour
    {
        public void openLink(string url)
        {
            Application.OpenURL(url);
        }
    }
}