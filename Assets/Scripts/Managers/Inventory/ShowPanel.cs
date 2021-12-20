using UnityEngine;

public class ShowPanel : MonoBehaviour
{
    [SerializeField] private GameObject panel = null;

    public void OnClick()
    {
        panel.SetActive(!panel.activeSelf);
        if(panel.activeSelf)
            StaticClass.OnUI = true;
        else
            StaticClass.OnUI = false;
    }
}
