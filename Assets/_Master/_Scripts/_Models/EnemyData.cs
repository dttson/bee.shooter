using System.Diagnostics.CodeAnalysis;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "Enemy Data", order = 51)]
[SuppressMessage("ReSharper", "InconsistentNaming")]
public class EnemyData : ScriptableObject
{
    public Sprite sprite;
    public float speed;
    public float maxHP;
    public float damage; //damage for player when hit enemy
    public bool hasBullet;
    public float shootingSpeed;
    public BulletData bulletData;
    public EffectData destroyEffect;
}