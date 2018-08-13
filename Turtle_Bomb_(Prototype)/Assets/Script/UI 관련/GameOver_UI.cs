using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOver_UI : MonoBehaviour {

    Animation m_Animations;

	void Start ()
    {
        m_Animations = GetComponent<Animation>();
        gameObject.SetActive(false);
	}

    public void GameOver_Direction_Play()
    {
        m_Animations.Play(m_Animations.GetClip("Game_Over_Direction").name);
    }
}
