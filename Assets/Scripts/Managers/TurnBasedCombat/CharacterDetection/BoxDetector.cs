using UnityEngine;

public class BoxDetector : MonoBehaviour
{
    // setting it inside box detector creates error even if we call it on start/awake function when
    // there is a collision right after it is being created.
    public CharacterDetection characterDetection = null;
    [SerializeField] Character targetCharacter;

    private void OnValidate() 
    {
        /*if(characterDetection ==  null)
            characterDetection = GetComponentInParent<CharacterDetection>();*/
    }

    private void Start() 
    {
        characterDetection = GetComponentInParent<CharacterDetection>();    
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Character charInRange = other.GetComponent<Character>();
       
        if(charInRange != null) // if is a character
        {
            if(targetCharacter != null && targetCharacter != charInRange) // Not same character, remove old targeted character
            {
                characterDetection.charactersInRange.Remove(targetCharacter);
            }
            if(characterDetection == null)
                Debug.LogError("Character Detection script is not set properly !");
            if(characterDetection.AddCharacter(charInRange))
                targetCharacter = charInRange;  // update targeted character
        }

        if(characterDetection.gameObject.CompareTag("Player"))
        {
            if(characterDetection.gameObject.transform.parent != transform.parent)
            {
                // Clear fog tile
                FogOfWarTilemapManager fogOfWarTilemapManager = other.GetComponent<FogOfWarTilemapManager>();

                if(fogOfWarTilemapManager != null)
                {
                    fogOfWarTilemapManager.ClearFog(transform.position, transform.parent.position);
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other) 
    {
        Character charOutOfRange = other.GetComponent<Character>();

        if(charOutOfRange != null) // if is a character
        {
            TriggerExit(charOutOfRange);
        }
    }

    private void TriggerExit(Character character)
    {
        if(targetCharacter == character)   // check if it is the same character
        {
            if(characterDetection.RemoveCharacter(character))
                targetCharacter = null;
        }
        else
            Debug.LogError("Leaving character was not targeted, this shouldn't happen in normal case\nTarget Character: " + 
                            targetCharacter.name + "\nCurrent Character: " + 
                            GetComponentInParent<Character>().gameObject.name + "\nPosition: " + transform.position);
    }

    public void DisableBox()
    {
        /*if(targetCharacter != null) // if there is a character on this box
            TriggerExit(targetCharacter);*/
        
        gameObject.SetActive(false);
    }
}
