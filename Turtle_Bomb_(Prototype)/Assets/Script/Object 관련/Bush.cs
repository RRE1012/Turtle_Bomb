using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bush : MonoBehaviour
{
    public Material m_Material;

    ParticleSystem m_Fire_Effect;

    MeshRenderer m_MeshRenderer;
    bool m_is_Burning = false;

    float m_Red_Relv_Value = 2.0f;

    void Start()
    {
        m_MeshRenderer = gameObject.GetComponent<MeshRenderer>();
        m_Fire_Effect = GetComponentInChildren<ParticleSystem>();
        m_Fire_Effect.gameObject.SetActive(false);
    }

    void OnTriggerEnter(Collider other)
    {
        if (!m_is_Burning && other.gameObject.CompareTag("Flame"))
        {
            m_MeshRenderer.material = m_Material;
            m_Fire_Effect.gameObject.SetActive(true);
            m_Fire_Effect.Play();
            gameObject.tag = "Flame_Bush";
            StartCoroutine("Burning");
            m_is_Burning = true;
        }
    }

    IEnumerator Burning()
    {
        while(true)
        {
            if (m_MeshRenderer.material.color.r >= 1.0f || m_MeshRenderer.material.color.r <= 0.0f)
                m_Red_Relv_Value *= -1;
            m_MeshRenderer.material.color = new Color(m_MeshRenderer.material.color.r + m_Red_Relv_Value * Time.deltaTime, 0.0f, 0.0f);
            yield return null;
        }
    }

}
