using UnityEngine;

[CreateAssetMenu(fileName = "NewAugment", menuName = "Game/Augment")]
public class AugmentData : ScriptableObject
{
    public string augmentName;
    [TextArea] public string description;

    public AugmentType type;
    public float value;
}
