using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class HitEffect : MonoBehaviour
{
    public HitEffectData hitEffectData;

    [HideInInspector]
    public delegate void DestroyAction();
    [HideInInspector]
    public event DestroyAction OnDestroyAction;

    [System.Serializable]
    public class HitEffectData 
    {
        public HitEffectData(bool useNoColor)
        {
            this.useNoColor = useNoColor;
        }

        public bool useNoColor;

        [ConditionalHide("useNoColor", false)]
        public Color hitColor = new Color(255, 255, 255, 255);
    }

    private Animator animator;
    void Start()
    {
        animator = GetComponent<Animator>();
        Destroy(gameObject, 0.6f);
    }

    void Update()
    {
        if (hitEffectData.useNoColor)
            Destroy(gameObject);
        else
        {
            animator.Play("Hit");
            GetComponent<SpriteRenderer>().color = hitEffectData.hitColor;
        }
    }

    private void OnDestroy()
    {
        OnDestroyAction?.Invoke();
    }
}
