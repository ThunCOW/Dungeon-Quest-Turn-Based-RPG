using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] GameObject activeCharacter = null;

    void OnValidate()
    {
        if(activeCharacter == null)
            activeCharacter = FindObjectOfType<Player>().gameObject;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(activeCharacter.transform.position != transform.position)
            transform.position = new Vector3(activeCharacter.transform.position.x, activeCharacter.transform.position.y, -10);
    }
}
