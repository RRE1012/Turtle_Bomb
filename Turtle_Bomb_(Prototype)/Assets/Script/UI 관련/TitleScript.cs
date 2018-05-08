using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleScript : MonoBehaviour {
    public GameObject m_bomb;
    public GameObject m_effect;
    public Animator m_Animator;

    public void BombExplode()
    {
        m_bomb.SetActive(false);
        m_effect.SetActive(true);
    }

    public void MakeWave()
    {
        m_Animator.SetTrigger("Touched_Start");
    }
}
