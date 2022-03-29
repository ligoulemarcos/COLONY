using UnityEngine;

public class Unit : MonoBehaviour
{
    void Start()
    {
        UnitSelections.Instance.unitsList.Add(this.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnDestroy()
    {
        UnitSelections.Instance.unitsList.Remove(this.gameObject);
    }
}
