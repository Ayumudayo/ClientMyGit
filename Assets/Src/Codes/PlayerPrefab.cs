using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerPrefab : MonoBehaviour
{
    public RuntimeAnimatorController[] animCon;
    private Animator anim;
    private SpriteRenderer spriter;
    private Vector3 lastPosition;
    private Vector3 currentPosition;
    private uint playerId;
    TextMeshPro myText;

    private float interpolationStartTime;
    private const float InterpolationDuration = 0.1f;

    void Awake()
    {
        anim = GetComponent<Animator>();
        spriter = GetComponent<SpriteRenderer>();
        myText = GetComponentInChildren<TextMeshPro>();
    }

    public void Init(uint playerId, string id)
    {
        anim.runtimeAnimatorController = animCon[playerId];
        lastPosition = transform.position;
        currentPosition = transform.position;
        this.playerId = playerId;

        if (id.Length > 5)
        {
            myText.text = id[..5];
        }
        else
        {
            myText.text = id;
        }
        myText.GetComponent<MeshRenderer>().sortingOrder = 6;
    }

    void OnEnable()
    {
        anim.runtimeAnimatorController = animCon[playerId];
    }

    // Easing function
    private float EaseInOutQuad(float x)
    {
        return x < 0.5f ? 2f * x * x : 1f - Mathf.Pow(-2f * x + 2f, 2f) / 2f;
    }

    // 서버로부터 위치 업데이트를 수신할 때 호출될 메서드
    public void UpdatePosition(float x, float y)
    {
        lastPosition = transform.position;
        currentPosition = new Vector3(x, y);
        interpolationStartTime = Time.time;
    }

    void Update()
    {
        if (!GameManager.instance.isLive)
            return;

        float elapsed = Time.time - interpolationStartTime;
        float t = Mathf.Clamp01(elapsed / InterpolationDuration);
        float easedT = EaseInOutQuad(t);

        transform.position = Vector3.Lerp(lastPosition, currentPosition, easedT);

        UpdateAnimation();
    }

    private void UpdateAnimation()
    {
        // 현재 위치와 이전 위치를 비교하여 이동 벡터 계산
        Vector2 inputVec = currentPosition - lastPosition;

        anim.SetFloat("Speed", inputVec.magnitude);

        if (inputVec.x != 0)
        {
            spriter.flipX = inputVec.x < 0;
        }
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        if (!GameManager.instance.isLive)
        {
            return;
        }
    }
}