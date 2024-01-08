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

    private NavMeshAgent agent; // ��ΰ�� AI ������Ʈ
    private Animator animator; // �ִϸ����� ������Ʈ

    public Transform attackRoot;
    public Transform eyeTransform;

    private AudioSource audioPlayer; // ����� �ҽ� ������Ʈ
    public AudioClip hitClip; // �ǰݽ� ����� �Ҹ�
    public AudioClip deathClip; // ����� ����� �Ҹ�

    private Renderer skinRenderer; // ������ ������Ʈ

    public float runSpeed = 10f;
    [Range(0.01f, 2f)] public float turnSmoothTime = 0.1f;
    private float turnSmoothVelocity;

    public float damage = 30f;
    public float attackRadius = 2f;
    private float attackDistance;

    public float fieldOfView = 50f;
    public float viewDistance = 10f;
    public float patrolSpeed = 3f;

    public LivingEntity targetEntity; // ������ ���
    public LayerMask whatIsTarget; // ���� ��� ���̾�

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

    // �� AI�� �ʱ� ������ �����ϴ� �¾� �޼���
    public void Setup(float health, float damage,
        float runSpeed, float patrolSpeed, Color skinColor)
    {
        // ü�� ����
        this.startingHealth = health;
        this.health = health;

        // ����޽� ������Ʈ�� �̵� �ӵ� ����
        this.runSpeed = runSpeed;
        this.patrolSpeed = patrolSpeed;

        this.damage = damage;

        // �������� ������� ���׸����� �÷��� ����, ���� ���� ����
        skinRenderer.material.color = skinColor;

        agent.speed = patrolSpeed;
    }

    private void Start()
    {
        // ���� ������Ʈ Ȱ��ȭ�� ���ÿ� AI�� ���� ��ƾ ����
        StartCoroutine(UpdatePath());
    }

    private bool isAttacking = true;
    private float attackCooldown = 1.0f;
    private float attackWait = 0.3f;
    private float elapsedTime = 0.0f; // ���� �ð�
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
                    // ���� �� ���� ���¸� ��Ȱ��ȭ�ϰ� ���� ���� �ڷ�ƾ�� ����
                    isAttacking = false;
                }
                
            }
        }




        // ���� ����� ���� ���ο� ���� �ٸ� �ִϸ��̼��� ���
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

               

                Debug.Log("������!!" + hp2);

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

    // �ֱ������� ������ ����� ��ġ�� ã�� ��θ� ����
    private IEnumerator UpdatePath()
    {
        // ����ִ� ���� ���� ����
        while (!dead)
        {
            if (hasTarget)
            {
                if (state == State.Patrol)
                {
                    state = State.Tracking;
                    agent.speed = runSpeed;
                }

                // ���� ��� ���� : ��θ� �����ϰ� AI �̵��� ��� ����
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

                // 20 ������ �������� ���� ������ ���� �׷�����, ���� ��ġ�� ��� �ݶ��̴��� ������
                // ��, whatIsTarget ���̾ ���� �ݶ��̴��� ���������� ���͸�
                var colliders = Physics.OverlapSphere(eyeTransform.position, viewDistance, whatIsTarget);

                // ��� �ݶ��̴����� ��ȸ�ϸ鼭, ����ִ� LivingEntity ã��
                foreach (var collider in colliders)
                {
                    if (!IsTargetOnSight(collider.transform)) continue;

                    var livingEntity = collider.GetComponent<LivingEntity>();

                    // LivingEntity ������Ʈ�� �����ϸ�, �ش� LivingEntity�� ����ִٸ�,
                    if (livingEntity != null && !livingEntity.dead)
                    {
                        // ���� ����� �ش� LivingEntity�� ����
                        targetEntity = livingEntity;

                        // for�� ���� ��� ����
                        break;
                    }
                }
            }

            // 0.05 �� �ֱ�� ó�� �ݺ�
            yield return new WaitForSeconds(0.05f);
        }
    }

    // �������� �Ծ����� ������ ó��
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
        Debug.Log("����"); // ������ ������ ����˴ϴ�.

        // WaitForSeconds�� ����Ͽ� ���� �ð� ���� ����մϴ�.
        yield return new WaitForSeconds(1.5f); // 2�� ���� ���

        Debug.Log("������ ��"); // ������ ���Ŀ� ����˴ϴ�.
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

    // ��� ó��
    public override void Die()
    {
        // LivingEntity�� Die()�� �����Ͽ� �⺻ ��� ó�� ����
        base.Die();

        // �ٸ� AI���� �������� �ʵ��� �ڽ��� ��� �ݶ��̴����� ��Ȱ��ȭ
        //GetComponent<Collider>().enabled = false;

        // AI ������ �����ϰ� ����޽� ������Ʈ�� ��Ȱ��ȭ
        agent.enabled = false;

        // ��� �ִϸ��̼� ���
        GetComponent<Animator>().enabled = false;
        GetComponent<Rigidbody>().isKinematic = false;
        //animator.applyRootMotion = true;
        //animator.SetTrigger("Die");

        // ��� ȿ���� ���
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