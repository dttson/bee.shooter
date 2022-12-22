using System;
using UnityEngine;
using UnityEngine.Events;

public enum ItemType
{
    WEAPON,
    SHIELD,
    HEALTH,
    GIANT_BEEHIVE,
    HIVE
}

public enum ItemMoveType
{
    NONE,
    MOVE_STRAIGHT,
    MOVE_TO_PLAYER
}
public class Item : MonoBehaviour
{
    public ItemType Type => m_Type;
        
    private const float MOVE_SPEED = 1.2f;
    private const float DISTANCE_CAUGHT = 1f;
    
    [SerializeField] private SpriteRenderer m_SpriteRenderer;

    private Player m_Player;
    private ItemType m_Type;
    private UnityAction<Item> m_OnCaught;
    private UnityAction<Item> m_OnDestroy;
    private ItemMoveType m_MoveType = ItemMoveType.NONE;
    protected Rect m_ScreenRect;

    public static Item randomItem(UnityAction<Item> onCaught, UnityAction<Item> onDestroy)
    {
        var itemTypes = Enum.GetValues(typeof(ItemType));
        ItemType type = (ItemType) UnityEngine.Random.Range(0, itemTypes.Length - 1);
        return createItem(ItemType.GIANT_BEEHIVE, onCaught, onDestroy);
    }
    
    public static Item createItem(ItemType type, UnityAction<Item> onCaught, UnityAction<Item> onDestroy)
    {
        Item item;
        switch (type)
        {
            case ItemType.WEAPON:
                item = Instantiate(Resources.Load<Item>("ItemWeapon"));
                break;
            case ItemType.HEALTH:
                item = Instantiate(Resources.Load<Item>("ItemHealth"));
                break;
            case ItemType.GIANT_BEEHIVE:
                item = Instantiate(Resources.Load<Item>("ItemGiantBeeHive"));
                break;
            case ItemType.HIVE:
                item = Instantiate(Resources.Load<Item>("ItemHive"));
                break;
            case ItemType.SHIELD:
                item = Instantiate(Resources.Load<Item>("ItemShield"));
                break;
            default:
                return null;
        }

        item.m_Type = type;
        item.m_OnCaught = onCaught;
        item.m_OnDestroy = onDestroy;
        return item;
    }

    public void getCaught()
    {
        m_OnCaught?.Invoke(this);
    }

    public void destroy()
    {
        m_MoveType = ItemMoveType.NONE;
        m_OnDestroy?.Invoke(this);
    }
    
    // Start is called before the first frame update
    private void Start()
    {
        m_Player = GameScene.Instance.CurrentPlayer;
        m_ScreenRect = Camera.main.getScreenRect();
        m_MoveType = ItemMoveType.MOVE_STRAIGHT;
    }

    // Update is called once per frame
    private void Update()
    {
        if (m_MoveType == ItemMoveType.MOVE_STRAIGHT)
        {
            transform.Translate(0f, -MOVE_SPEED * Time.deltaTime, 0f);

            if (Vector2.Distance(transform.position.toVector2(), m_Player.transform.position.toVector2()) < DISTANCE_CAUGHT)
            {
                m_MoveType = ItemMoveType.MOVE_TO_PLAYER;
            }
            else
            {
                var bounds = m_SpriteRenderer.bounds;
                if (bounds.max.x < m_ScreenRect.xMin || bounds.min.x > m_ScreenRect.xMax || bounds.max.y < m_ScreenRect.yMin ||
                    bounds.min.y > m_ScreenRect.yMax)
                {
                    destroy();
                }
            }
        }
        else
        {
            var direction = (m_Player.transform.position - transform.position).normalized * (Time.deltaTime * MOVE_SPEED * 2);
            transform.Translate(direction);
            var distanceToPlayer =
                Vector2.Distance(transform.position.toVector2(), m_Player.transform.position.toVector2()); 
            if ( distanceToPlayer <  0.05f)
            {
                getCaught();
            }
            else if (distanceToPlayer > DISTANCE_CAUGHT)
            {
                m_MoveType = ItemMoveType.MOVE_STRAIGHT;
            }
        }
        
    }
}
