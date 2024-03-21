using UnityEngine;

public class AssetSystem : StaticInstance<AssetSystem>
{
    [SerializeField] private GameObject example;
    public GameObject Example => example;
}