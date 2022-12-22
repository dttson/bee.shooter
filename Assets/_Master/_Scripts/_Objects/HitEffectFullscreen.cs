using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class HitEffectFullscreen : MonoBehaviour
{
    public static HitEffectFullscreen Instance { get; private set; }
    private static readonly Color TransparentColor = new Color(1f, 1f, 1f, 0f);
    
    [SerializeField] private Image m_Image;
    
    // Start is called before the first frame update
    private void Awake()
    {
        Instance = this;
        m_Image = GetComponent<Image>();
    }

    public void playEffect()
    {
        m_Image.color = TransparentColor;
        m_Image.DOKill();
        m_Image.DOFade(1f, 0.1f).SetEase(Ease.OutQuad).SetLoops(2, LoopType.Yoyo).SetUpdate(true);
    }
}
