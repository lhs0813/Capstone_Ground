using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;
using Photon.Realtime;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class Enemy : LivingEntity
{
    private enum State
    {
        Patrol,
        Tracking,
        AttackBegin,
        Attacking
    }

    public PhotonView Zombie11;

    private State state;

    private NavMeshAgent agent; // 경로계산 AI 에이전트
    private Animator animator; // 애니메이터 컴포넌트

    public Transform attackRoot;
    public Transform eyeTransform;

    private AudioSource audioPlayer; // 오디오 소스 컴포넌트
    public AudioClip hitClip; // 피격시 재생할 소리
    public AudioClip deathClip; // 사망시 재생할 소리

    private Renderer skinRenderer; // 렌더러 컴포넌트

    public float runSpeed = 10f;
    [Range(0.01f, 2f)] public float turnSmoothTime = 0.1f;
    private float turnSmoothVelocity;

    public float damage = 30f;
    public float attackRadius = 2f;
    private float attackDistance;

    public float fieldOfView = 50f;
    public float viewDistance = 10f;
    public float patrolSpeed = 3f;

    public LivingEntity targetEntity; // 추적할 대상
    public LayerMask whatIsTarget; // 추적 대상 레이어

    public float hp;
    public GameObject hp2;

    private RaycastHit[] hits = new RaycastHit[10];
    private List<LivingEntity> lastAttackedTargets = new List<LivingEntity>();

    private bool hasTarget => targetEntity != null && !targetEntity.dead;


#if UNITY_EDITOR

    private void OnDrawGizmosSelected()
    {
        if (attackRoot != null)
        {
            Gizmos.color = new Color(1f, 0f, 0f, 0.5f);
            Gizmos.DrawSphere(attackRoot.position, attackRadius);
        }

        var leftRayRotation = Quaternion.AngleAxis(-fieldOfView * 0.5f, Vector3.up);
        var leftRayDirection = leftRayRotation * transform.forward;
        Handles.color = new Color(1f, 1f, 1f, 0.2f);
        Handles.DrawSolidArc(eyeTransform.position, Vector3.up, leftRayDirection, fieldOfView, viewDistance);
    }

#endif

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        audioPlayer = GetComponent<AudioSource>();
        skinRenderer = GetComponentInChildren<Renderer>();

        attackDistance = Vector3.Distance(transform.position,
                             new Vector3(attackRoot.position.x, transform.position.y, attackRoot.position.z)) +
                         attackRadius;

        attackDistance += agent.radius;

        agent.stoppingDistance = 0;
        agent.speed = patrolSpeed;
    }

    [PunRPC]

    // 적 AI의 초기 스펙을 결정하는 셋업 메서드
    public void Setup(float health, float damage,
        float runSpeed, float patrolSpeed, Color skinColor)
    {
        // 체력 설정
        this.startingHealth = health;
        this.health = health;

        // 내비메쉬 에이전트의 이동 속도 설정
        this.runSpeed = runSpeed;
        this.patrolSpeed = patrolSpeed;

        this.damage = damage;

        // 렌더러가 사용중인 머테리얼의 컬러를 변경, 외형 색이 변함
        skinRenderer.material.color = skinColor;

        agent.speed = patrolSpeed;
    }

    private void Start()
    {
        // 게임 오브젝트 활성화와 동시에 AI의 추적 루틴 시작
        StartCoroutine(UpdatePath());
    }

    private bool isAttacking = true;
    private float attackCooldown = 1.0f;
    private float attackWait = 0.3f;
    private float elapsedTime = 0.0f; // 누적 시간
    private IEnumerator AttackCooldown()
    {
        while (true)
        {
            yield return new WaitForSeconds(attackCooldown);
            isAttacking = true;
        }
    }

    private IEnumerator AttackCooldown2()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.5f);
            isAttacking = true;
        }
    }

    private void Update()
    {
        if (dead) return;

        

        Debug.Log(state);

        if (state == State.Tracking &&
        Vector3.Distance(targetEntity.transform.position, transform.position) <= attackDistance)
        {
            elapsedTime += Time.deltaTime;

            if (elapsedTime >= attackWait)
            {
                BeginAttack();
                if(elapsedTime>=attackCooldown)
                {
                    hp2 = targetEntity.transform.gameObject;

                    Zombie11.RPC("attack", RpcTarget.All);
                    elapsedTime = 0.0f;
                    // 공격 후 공격 상태를 비활성화하고 공격 간격 코루틴을 시작
                    isAttacking = false;
                }
                
            }
        }




        // 추적 대상의 존재 여부에 따라 다른 애니메이션을 재생
        animator.SetFloat("Speed", agent.speed);
    }

    private void FixedUpdate()
    {
        if (dead) return;

        if (state == State.Patrol)
        {
            agent.stoppingDistance = 0;
        }
        else if (state == State.AttackBegin || state == State.Attacking || state == State.Tracking)
        {
            agent.stoppingDistance = 2;
        }

        if (state == State.AttackBegin || state == State.Attacking)
        {
            var lookRotation =Quaternion.LookRotation(targetEntity.transform.position - transform.position);
            var targetAngleY = lookRotation.eulerAngles.y;

            transform.eulerAngles = Vector3.up * Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngleY,
                                        ref turnSmoothVelocity, turnSmoothTime);
        }

        if (state == State.Attacking)
        {
            var direction = transform.forward;
            var deltaDistance = agent.velocity.magnitude * Time.deltaTime;

            var size = Physics.SphereCastNonAlloc(attackRoot.position, attackRadius, direction, hits, deltaDistance,
                whatIsTarget);

            for (var i = 0; i < size; i++)
            {
                var attackTargetEntity = hits[i].collider.GetComponent<LivingEntity>();

               

                Debug.Log("공격함!!" + hp2);

                if (attackTargetEntity != null && !lastAttackedTargets.Contains(attackTargetEntity))
                {
                    var message = new DamageMessage();
                    message.amount = damage;
                    message.damager = gameObject;

                    if (hits[i].distance <= 0f)
                    {
                        message.hitPoint = attackRoot.position;
                    }
                    else
                    {
                        message.hitPoint = hits[i].point;
                    }

                    message.hitNormal = hits[i].normal;
                    
                    

                    attackTargetEntity.ApplyDamage(message);

                    lastAttackedTargets.Add(attackTargetEntity);
                    break;
                }
            }
        }
    }

    // 주기적으로 추적할 대상의 위치를 찾아 경로를 갱신
    private IEnumerator UpdatePath()
    {
        // 살아있는 동안 무한 루프
        while (!dead)
        {
            if (hasTarget)
            {
                if (state == State.Patrol)
                {
                    state = State.Tracking;
                    agent.speed = runSpeed;
                }

                // 추적 대상 존재 : 경로를 갱신하고 AI 이동을 계속 진행
                agent.SetDestination(targetEntity.transform.position);
            }
            else
            {
                if (targetEntity != null) targetEntity = null;

                if (state != State.Patrol)
                {
                    state = State.Patrol;
                    agent.speed = patrolSpeed;
                }

                if (agent.remainingDistance <= 1f)
                {
                    var patrolPosition = Utility.GetRandomPointOnNavMesh(transform.position, 20f, NavMesh.AllAreas);
                    agent.SetDestination(patrolPosition);
                }

                // 20 유닛의 반지름을 가진 가상의 구를 그렸을때, 구와 겹치는 모든 콜라이더를 가져옴
                // 단, whatIsTarget 레이어를 가진 콜라이더만 가져오도록 필터링
                var colliders = Physics.OverlapSphere(eyeTransform.position, viewDistance, whatIsTarget);

                // 모든 콜라이더들을 순회하면서, 살아있는 LivingEntity 찾기
                foreach (var collider in colliders)
                {
                    if (!IsTargetOnSight(collider.transform)) continue;

                    var livingEntity = collider.GetComponent<LivingEntity>();

                    // LivingEntity 컴포넌트가 존재하며, 해당 LivingEntity가 살아있다면,
                    if (livingEntity != null && !livingEntity.dead)
                    {
                        // 추적 대상을 해당 LivingEntity로 설정
                        targetEntity = livingEntity;

                        // for문 루프 즉시 정지
                        break;
                    }
                }
            }

            // 0.05 초 주기로 처리 반복
            yield return new WaitForSeconds(0.05f);
        }
    }

    // 데미지를 입었을때 실행할 처리
    public override bool ApplyDamage(DamageMessage damageMessage)
    {
        if (!base.ApplyDamage(damageMessage)) return false;

        if (targetEntity == null)
        {
            targetEntity = damageMessage.damager.GetComponent<LivingEntity>();
        }

        EffectManager.Instance.PlayHitEffect(damageMessage.hitPoint, damageMessage.hitNormal, transform, EffectManager.EffectType.Flesh);
        audioPlayer.PlayOneShot(hitClip);

        return true;
    }

    public void BeginAttack()
    {
        state = State.AttackBegin;

        
        Debug.Log(hp);
        Debug.Log(hp2);

        // hp2 = hits[i].transform.gameObject;
        
        agent.isStopped = true;
        
        animator.SetTrigger("Attack");
        
        StartCoroutine(DelayedAction());

        DisableAttack();

    }

    IEnumerator DelayedAction()
    {
        Debug.Log("시작"); // 딜레이 이전에 실행됩니다.

        // WaitForSeconds를 사용하여 일정 시간 동안 대기합니다.
        yield return new WaitForSeconds(1.5f); // 2초 동안 대기

        Debug.Log("딜레이 후"); // 딜레이 이후에 실행됩니다.
    }

    public void EnableAttack()
    {
        state = State.Attacking;

        lastAttackedTargets.Clear();
    }

    public void DisableAttack()
    {
        if (hasTarget)
        {
            state = State.Tracking;
        }
        else
        {
            state = State.Patrol;
        }

        agent.isStopped = false;
    }

    private bool IsTargetOnSight(Transform target)
    {
        RaycastHit hit;

        var direction = target.position - eyeTransform.position;

        direction.y = eyeTransform.forward.y;

        if (Vector3.Angle(direction, eyeTransform.forward) > fieldOfView * 0.5f)
        {
            return false;
        }

        direction = target.position - eyeTransform.position;

        if (Physics.Raycast(eyeTransform.position, direction, out hit, viewDistance, whatIsTarget))
        {
            if (hit.transform == target) return true;
        }

        return false;
    }

    // 사망 처리
    public override void Die()
    {
        // LivingEntity의 Die()를 실행하여 기본 사망 처리 실행
        base.Die();

        // 다른 AI들을 방해하지 않도록 자신의 모든 콜라이더들을 비활성화
        //GetComponent<Collider>().enabled = false;

        // AI 추적을 중지하고 내비메쉬 컴포넌트를 비활성화
        agent.enabled = false;

        // 사망 애니메이션 재생
        GetComponent<Animator>().enabled = false;
        GetComponent<Rigidbody>().isKinematic = false;
        //animator.applyRootMotion = true;
        //animator.SetTrigger("Die");

        // 사망 효과음 재생
        audioPlayer.PlayOneShot(deathClip);

    }

    [PunRPC]
    void attack()
    {
        hp = hp2.GetComponent<Player_HP>().VirtualHP;

        hp = hp - 15;

        
        hp2.GetComponent<Player_HP>().VirtualHP = hp;
    }
}