using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

namespace GRD.FSM.Examples
{
    public class AboutText : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] TextMeshProUGUI _myText;

        public void OnPointerClick(PointerEventData eventData)
        {
            int linkIndex = TMP_TextUtilities.FindIntersectingLink(_myText, Input.mousePosition, null);
            if (linkIndex != -1)
            { // was a link clicked?
                TMP_LinkInfo linkInfo = _myText.textInfo.linkInfo[linkIndex];

                // open the link id as a url, which is the metadata we added in the text field
                Application.OpenURL(linkInfo.GetLinkID());
            }
        }
    }
}
