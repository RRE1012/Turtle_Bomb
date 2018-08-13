using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage_Clear : MonoBehaviour {

    Animation m_Animations;

	void Start ()
    {
        m_Animations = GetComponent<Animation>();
    }

    public void Stage_Clear_Direction_Play()
    {
        m_Animations.Play(m_Animations.GetClip("Stage_Clear").name);
    }
}
