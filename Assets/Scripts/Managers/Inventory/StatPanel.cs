using UnityEngine;
using DungeonCrawler.CharacterStats;

public class StatPanel : MonoBehaviour
{
    [SerializeField] StatDisplay[] statDisplays = null;
    [SerializeField] string[] statNames = null;

    private CharacterStat[] stats;

    private void OnValidate() {
        statDisplays = GetComponentsInChildren<StatDisplay>();
        UpdateStatNames();
    }

    public void SetStats(params CharacterStat[] charStats){
        stats = charStats;

        if(stats.Length > statDisplays.Length){
            Debug.LogError("Not Enough Stat Displays!");
            return;
        }

        for(int i = 0; i < statDisplays.Length; i++){
            statDisplays[i].gameObject.SetActive(i < stats.Length);
        }
    }

    public void UpdateStatValues(){
        for (int i = 0; i < stats.Length; i++){
            statDisplays[i].valueText.text = stats[i].calculatedValue.ToString();
        }
    }
    public void UpdateStatNames(){
        for (int i = 0; i < statNames.Length; i++){
            statDisplays[i].nameText.text = statNames[i];
        }
    }
}
