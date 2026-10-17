using TMPro;
using UnityEngine;
using UnityEngine.UI;


namespace DWFX
{
    public class UIEffectController : MonoBehaviour
    {
        [Header("References")]
        [Tooltip("Parent node containing all the child objects to switch between")]
        public Transform parentNode;

        [Tooltip("Button to navigate to the previous item")]
        public Button lastBtn;

        [Tooltip("Button to navigate to the next item")]
        public Button nextBtn;

        [Tooltip("Text component to display current particle Index")]
        public TMP_Text particleIndexText;

        [Tooltip("Text component to display current child object name")]
        public TMP_Text nameText;

        // Current index of the displayed child object
        private int currentIndex = 0;

        private void Start()
        {
            // Initial validation
            if (parentNode == null || lastBtn == null || nextBtn == null ||
                particleIndexText == null || nameText == null)
            {
                Debug.LogError("Please set all necessary references in the Inspector!");
                return;
            }

            // Initialize display with the first child object
            UpdateDisplay();

            // Add button click event listeners
            lastBtn.onClick.AddListener(ShowLastItem);
            nextBtn.onClick.AddListener(ShowNextItem);
        }

        // Show the previous child object
        public void ShowLastItem()
        {
            if (currentIndex > 0)
            {
                currentIndex--;
            }
            else if (currentIndex == 0) currentIndex = parentNode.childCount - 1;
            UpdateDisplay();
        }

        // Show the next child object
        public void ShowNextItem()
        {
            if (currentIndex < parentNode.childCount - 1)
            {
                currentIndex++;

            }
            else if (currentIndex == parentNode.childCount - 1) currentIndex = 0;
            UpdateDisplay();
        }

        // Update display
        private void UpdateDisplay()
        {

            print("currentIndex" + currentIndex);
            // Hide all child objects
            for (int i = 0; i < parentNode.childCount; i++)
            {
                parentNode.GetChild(i).gameObject.SetActive(false);
            }

            // Show the current index child object
            if (parentNode.childCount > 0)
            {
                parentNode.GetChild(currentIndex).gameObject.SetActive(true);
            }

            // Update particle text
            UpdateparticleText();

            // Update child object name text
            UpdateNameText();

        }

        // Update particle text
        private void UpdateparticleText()
        {
            if (particleIndexText != null)
            {
                // particle number starts from 1, total count is number of child objects
                particleIndexText.text = $"{currentIndex + 1} / {parentNode.childCount}";
            }
        }

        // Update child object name text
        private void UpdateNameText()
        {
            if (nameText != null && parentNode.childCount > 0)
            {
                // Display the name of the current child object
                nameText.text = parentNode.GetChild(currentIndex).name;
            }
        }

    }
}