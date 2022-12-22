using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Config
{
    public const float PLAYER_MAX_HP = 100f;
    public const int PLAYER_MAX_LIFE = 5;
}

public static class TagDefine
{
    public const string PLAYER = "Player";
    public const string ENEMY = "Enemy";
    public const string PLAYER_BULLET = "PlayerBullet";
    public const string ENEMY_BULLET = "EnemyBullet";
}

public static class LayerMaskDefine
{
    public const string PLAYER = "Player";
    public const string ENEMY = "Enemy";
}

public enum SceneName
{
    Home, Game, Leaderboard
}
