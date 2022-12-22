using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using EZCameraShake;
using PathCreation;
using UnityEngine;
using UnityEngine.Assertions;
using Random = UnityEngine.Random;
// ReSharper disable InconsistentNaming

public enum GameState
{
    NOT_START,
    PLAYING,
    GAME_OVER
}
public class GameScene : MonoBehaviour
{
    public static GameScene Instance { get; private set; }
    public GameState CurrentGameState { get; private set; } = GameState.NOT_START;
    public Player CurrentPlayer => m_Player;
    
    [SerializeField] private Camera m_Camera;
    [SerializeField] private Player m_Player;
    [Header("Bullet")]
    [SerializeField] private Transform m_BulletContainer;
    [SerializeField] private BulletData[] m_BulletDatas;
    [Header("Enemy")] 
    [SerializeField] private float m_EnemyAppearFrequency = 1.0f; //enemy appear every 1 seconds
    [SerializeField] private Transform m_EnemyContainer;
    [SerializeField] private EnemyData[] m_EnemyDatas;
    [SerializeField] private PathCreator[] m_PathCreators;
    [Header("Effect")]
    [SerializeField] private Transform m_EffectContainer;
    [SerializeField] private EffectData[] m_EffectDatas;
    [Header("Asteroid")]
    [SerializeField] private Transform m_AsteroidContainer;
    [SerializeField] private Sprite[] m_AsteroidSprites;
    
    private Dictionary<BulletType, List<Bullet>> m_PoolBullets = new Dictionary<BulletType, List<Bullet>>();
    private List<EnemyBase> m_PoolEnemies = new List<EnemyBase>();
    private List<EnemyBase> m_ActiveEnemies = new List<EnemyBase>();
    private Dictionary<EffectType, List<EffectObject>> m_PoolEffects = new Dictionary<EffectType, List<EffectObject>>();
    private List<Asteroid> m_PoolAsteroids = new List<Asteroid>();
    

    private Rect m_ScreenRect;

    #region Unity methods

    private void Awake()
    {
        Instance = this;
        Application.targetFrameRate = 60;
        if (m_BulletContainer == null)
        {
            m_BulletContainer = transform;
        }
        
        m_ScreenRect = m_Camera.getScreenRect();
        setupPlayer();
        setupBullet();
        setupEnemy();
        setupEffect();
        setupAsteroid();
    }
    
    private void Start()
    {
        CurrentGameState = GameState.PLAYING;
        StartCoroutine(coroutineSpawnEnemy());
        StartCoroutine(coroutineSpawnAsteroids());
    }

    #endregion

    private void setupPlayer()
    {
        var position = m_Player.transform.position;
        position.x = m_ScreenRect.xMin + m_ScreenRect.width / 2;
        position.y = m_ScreenRect.yMin + m_ScreenRect.height / 10;
        m_Player.transform.position = position;
        m_Player.OnHitEnemy = onPlayerHitEnemy;
        m_Player.OnDie = onPlayerDie;
        m_Player.OnHitEnemyBullet = onPlayerHitBullet;
    }

    private void setupBullet()
    {
        initBulletPool();
    }

    private void setupEnemy()
    {
        initEnemyPool();
    }

    private void setupEffect()
    {
        initEffectPool();
    }

    private void setupAsteroid()
    {
        initAsteroidPool();
    }

    #region Game State

    public void gameOver()
    {
        CurrentGameState = GameState.GAME_OVER;

        StartCoroutine(coroutineDelayGameOver());
    }

    private IEnumerator coroutineDelayGameOver()
    {
        yield return new WaitForSeconds(2.0f);
        
        UnityEngine.SceneManagement.SceneManager.LoadScene("Game");
    }

    #endregion

    #region Player

    private void onPlayerDie()
    {
        m_Player.gameObject.SetActive(false);
        spawnEffect(m_Player.transform.position, m_Player.DestroyEffect);
        gameOver();
    }

    private void onPlayerHitEnemy(EnemyBase enemy)
    {
        onEnemyDestroy(enemy);
        GameUIController.Instance.updateHeathBar(m_Player.CurrentLifeCount);
    }

    private void onPlayerHitBullet(Bullet bullet)
    {
        //TODO: show some effect here
        pushBullet(bullet);
        GameUIController.Instance.updateHeathBar(m_Player.CurrentLifeCount);
    }

    #endregion

    #region Enemy

    private void destroyAllEnemies()
    {
        while (m_ActiveEnemies.Count > 0)
        {
            m_ActiveEnemies[0].destroy();
        }
    }
    private IEnumerator coroutineSpawnEnemy()
    {
        WaitForSeconds waitForSeconds = new WaitForSeconds(m_EnemyAppearFrequency);
        while (true)
        {
            var enemy = popEnemy();
            enemy.reloadData(m_EnemyDatas[Random.Range(0, m_EnemyDatas.Length)], m_PathCreators[Random.Range(0, m_PathCreators.Length)]);
            enemy.startMoving();
            yield return waitForSeconds;
        }
    }
    
    private void initEnemyPool()
    {
        var defaultData = m_EnemyDatas[0];
        var defaultPathCreator = m_PathCreators[0];
        for (int i = 0; i < 10; i++)
        {
            var enemy = EnemyBase.createEnemy(defaultData, defaultPathCreator, onEnemyHitBullet, onEnemyDestroy);
            enemy.transform.SetParent(m_EnemyContainer);
            enemy.gameObject.SetActive(false);
            m_PoolEnemies.Add(enemy);
        }
        m_ActiveEnemies.Clear();
    }
    
    private EnemyBase popEnemy()
    {
        if (m_PoolEnemies.Count == 0)
        {
            initEnemyPool();
        }

        var enemy = m_PoolEnemies[0];
        enemy.gameObject.SetActive(true);
        m_PoolEnemies.RemoveAt(0);
        m_ActiveEnemies.Add(enemy);
        return enemy;
    }
    
    private void pushEnemy(EnemyBase enemy)
    {
        enemy.gameObject.SetActive(false);
        m_ActiveEnemies.Remove(enemy);
        m_PoolEnemies.Add(enemy);
    }

    private void onEnemyHitBullet(EnemyBase enemy, Bullet bullet)
    {
        var effectData = m_EffectDatas.First(data => data.type == EffectType.PARTICLE);
        spawnEffect(bullet.transform.position, effectData);
        pushBullet(bullet);
    }

    private void onEnemyDestroy(EnemyBase enemy, bool hasEffect = true)
    {
        if (hasEffect)
        {
            spawnEffect(enemy.transform.position, enemy.DestroyEffect);
        }
        
        if (GameBalance.Instance.shouldSpawnItem())
        {
            createItem(ItemType.WEAPON, enemy.transform.position);
        }
        
        pushEnemy(enemy);
    }

    #endregion
    
    #region Bullet
    
    private void initBulletPool()
    {
        var bulletTypes = Enum.GetValues(typeof(BulletType));
        foreach (BulletType bulletType in bulletTypes)
        {
            var defaultData = m_BulletDatas.FirstOrDefault(data => data.type == bulletType);
            if (defaultData.Equals(default(BulletData)))
            {
                continue;
            }

            var listBullets = new List<Bullet>();
            for (int i = 0; i < 10; i++)
            {
                var bullet = Bullet.createBullet(defaultData, onBulletDestroy);
                bullet.transform.SetParent(m_BulletContainer);
                bullet.gameObject.SetActive(false);
                listBullets.Add(bullet);
            }
            m_PoolBullets.Add(bulletType, listBullets);
        }
    }

    private void initNewBulletForType(BulletType type)
    {
        List<Bullet> listBullets;
        if (m_PoolBullets.ContainsKey(type))
        {
            listBullets = m_PoolBullets[type];
        }
        else
        {
            m_PoolBullets.Add(type, new List<Bullet>());
            listBullets = m_PoolBullets[type];
        }
        
        var defaultData = m_BulletDatas.FirstOrDefault(data => data.type == type);
        Assert.AreNotEqual(defaultData, default(BulletData), "Not found data in list bullet datas");

        for (int i = 0; i < 10; i++)
        {
            var bullet = Bullet.createBullet(defaultData, onBulletDestroy);
            bullet.transform.SetParent(m_BulletContainer);
            listBullets.Add(bullet);
        }
    }

    public void spawnBullet(Vector3 position, BulletData data, Vector2 bulletDirection)
    {
        var bullet = popBullet(data.type);
        bullet.reload(data, bulletDirection);
        bullet.transform.position = position;
    }

    private Bullet popBullet(BulletType type)
    {
        if (m_PoolBullets.Count == 0)
        {
            initBulletPool();
        }

        if (!m_PoolBullets.ContainsKey(type) || m_PoolBullets[type].Count == 0)
        {
            initNewBulletForType(type);
        }

        if (m_PoolBullets.ContainsKey(type))
        {
            var listBullets = m_PoolBullets[type];
            var bullet = listBullets[0];
            bullet.gameObject.SetActive(true);
            listBullets.RemoveAt(0);
            return bullet;
        }

        return null;
    }

    private void pushBullet(Bullet bullet)
    {
        bullet.gameObject.SetActive(false);
        var listBullets = m_PoolBullets[bullet.Type];
        if (listBullets == null)
        {
            m_PoolBullets.Add(bullet.Type, new List<Bullet>());
            listBullets = m_PoolBullets[bullet.Type];
        }
        listBullets.Add(bullet);
    }

    private void onBulletDestroy(Bullet bullet)
    {
        pushBullet(bullet);
    }
    #endregion

    #region Effects

    public void spawnEffect(Vector3 position, EffectData data)
    {
        var effect = popEffect(data.type);
        effect.reload(data);
        effect.transform.position = position;
    }

    private void initEffectPool()
    {
        var effectTypes = Enum.GetValues(typeof(EffectType));
        foreach (EffectType effectType in effectTypes)
        {
            var defaultData = m_EffectDatas.First(data => data.type == effectType);
            if (defaultData == null)
            {
                continue;
            }

            var listEffects = new List<EffectObject>();
            for (int i = 0; i < 10; i++)
            {
                var effect = EffectObject.createEffect(defaultData, onEffectComplete);
                effect.transform.SetParent(m_EffectContainer);
                effect.gameObject.SetActive(false);
                listEffects.Add(effect);
            }
            m_PoolEffects.Add(effectType, listEffects);
        }
    }
    
    private void initNewEffectForType(EffectType type)
    {
        List<EffectObject> listEffects;
        if (m_PoolEffects.ContainsKey(type))
        {
            listEffects = m_PoolEffects[type];
        }
        else
        {
            listEffects = new List<EffectObject>();
            m_PoolEffects.Add(type, listEffects);
        }
        var defaultData = m_EffectDatas.First(data => data.type == type);
        Assert.AreNotEqual(defaultData, null, "Not found data in list bullet datas");

        for (int i = 0; i < 10; i++)
        {
            var effect = EffectObject.createEffect(defaultData, onEffectComplete);
            effect.transform.SetParent(m_EffectContainer);
            listEffects.Add(effect);
        }
    }
    
    private EffectObject popEffect(EffectType type)
    {
        if (m_PoolEffects.Count == 0)
        {
            initEffectPool();
        }

        if (!m_PoolEffects.ContainsKey(type) || m_PoolEffects[type].Count == 0)
        {
            initNewEffectForType(type);
        }

        var effects = m_PoolEffects[type];
        var effect = effects[0];
        effect.gameObject.SetActive(true);
        effects.RemoveAt(0);
        return effect;
    }
    
    private void pushEffect(EffectObject effect)
    {
        effect.gameObject.SetActive(false);
        var listBullets = m_PoolEffects[effect.Type];
        if (listBullets == null)
        {
            m_PoolEffects.Add(effect.Type, new List<EffectObject>());
            listBullets = m_PoolEffects[effect.Type];
        }
        listBullets.Add(effect);
    }

    private void onEffectComplete(EffectObject effect)
    {
        pushEffect(effect);
    }

    #endregion

    #region ITEM

    private void createItem(ItemType type, Vector3 position)
    {
        var item = Item.randomItem(onItemGetCaught, onItemDestroy);
        item.transform.position = position;
    }

    private void onItemDestroy(Item item)
    {
        Destroy(item.gameObject);
    }

    private void onItemGetCaught(Item item)
    {
        switch (item.Type)
        {
            case ItemType.WEAPON:
                m_Player.upgradeBullet();
                break;
            case ItemType.HEALTH:
                m_Player.increaseLifeCount(1);
                GameUIController.Instance.updateHeathBar(m_Player.CurrentLifeCount);
                break;
            case ItemType.SHIELD:
                m_Player.becomImmortalIn(GameBalance.Instance.ShieldTime);
                break;
            case ItemType.GIANT_BEEHIVE:
                var c = CameraShaker.Instance.ShakeOnce(2.71f, 5.57f, 0f, 1.0f);
                c.PositionInfluence = new Vector3(0.2f, 0f, 0.4f);
                c.RotationInfluence = new Vector3(0.3f, 0f, 0.23f);
                destroyAllEnemies();
                break;
        }
        Destroy(item.gameObject);
    }

    #endregion

    #region ASTEROID
    
    private IEnumerator coroutineSpawnAsteroids()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(2f, 3f));

            var asteroid = popAsteroid();
            asteroid.reload(m_AsteroidSprites[Random.Range(0, m_AsteroidSprites.Length)]);
            float minX = m_ScreenRect.xMin + m_ScreenRect.width * 0.1f;
            float maxX = m_ScreenRect.xMax - m_ScreenRect.width * 0.1f;
            asteroid.transform.position = new Vector3(Random.Range(minX, maxX), m_ScreenRect.yMax + 1f, 0f);
        }
    }

    private void initAsteroidPool()
    {
        for (int i = 0; i < 10; i++)
        {   
            var asteroid = Asteroid.createAsteroid(onAsteroidDestroy);
            asteroid.transform.SetParent(m_AsteroidContainer);
            asteroid.gameObject.SetActive(false);
            m_PoolAsteroids.Add(asteroid);
        }
    }

    private void onAsteroidDestroy(Asteroid asteroid)
    {
        pushAsteroid(asteroid);
    }

    private Asteroid popAsteroid()
    {
        if (m_PoolAsteroids.Count == 0)
        {
            initAsteroidPool();
        }

        var asteroid = m_PoolAsteroids[0];
        asteroid.gameObject.SetActive(true);
        m_PoolAsteroids.RemoveAt(0);
        return asteroid;
    }
    
    private void pushAsteroid(Asteroid asteroid)
    {
        asteroid.gameObject.SetActive(false);
        m_PoolAsteroids.Add(asteroid);
    }

    #endregion
}
