using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraHandler : MonoBehaviour
{
	public float m_DampTime = 10f;
	public Transform m_Target;
	public float m_XOffset = 0;
	public float m_YOffset = 0;

	private Animator animator;

	void Start()
	{
		if (m_Target == null)
		{
			m_Target = GameObject.FindGameObjectWithTag("Player").transform;
		}

		animator = transform.GetChild(0).GetComponent<Animator>();
	}

	void Update()
	{
		if (m_Target)
		{
			float targetX = m_Target.position.x + m_XOffset;
			float targetY = m_Target.position.y + m_YOffset;

			targetX = Mathf.Lerp(transform.position.x, targetX, 1 / m_DampTime * Time.deltaTime);

			targetY = Mathf.Lerp(transform.position.y, targetY, m_DampTime * Time.deltaTime);

			transform.position = new Vector3(targetX, targetY, transform.position.z);
		}
	}

	public void Shake()
    {
		int randomNumber = Random.Range(1, 3);
		switch (randomNumber)
        {
			case 1:
				animator.SetTrigger("Shake1");
				break;
			case 2:
				animator.SetTrigger("Shake2");
				break;
			case 3:
				animator.SetTrigger("Shake3");
				break;
		}
    }
}
