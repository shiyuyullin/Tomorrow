using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

public class Arrive : MonoBehaviour
{

    #region Public Fields

    public Transform target;

    #endregion

    #region  Private Fields

    private LookWhereYouGo _lookWhereYouGo;
    private Rigidbody rigidbody;
    private Animator animator;
    private float targetSpeed;
    private float turnSmoothTime = 0.1f;
    private float turnSmoothVelocity;
    private Vector3 direction;
    private float distance;
    private Vector3 targetVelocity;
    private Vector3 acceleration;
    // This variable will only be used for Drones
    private GameObject player;

    #endregion

    #region Serializable Private Fields

    [SerializeField] private float maxSpeed = 20f;
    [SerializeField] private float targetRadius = 4f;
    [SerializeField] private float slowDownRadius = 10f;
    [SerializeField] private float timeToTarget = 0.75f;
    [SerializeField] private float maxAcceleration = 12.0f;
    
    #endregion

    #region MonoBehaviour Callbacks

    private void Awake()
    {
        _lookWhereYouGo = GetComponent<LookWhereYouGo>();
    }

    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (gameObject.tag == "Drone")
        {
            player = GameObject.FindGameObjectWithTag("Player");
            target = player.transform;
            CalculateAcceleration();
            ApplyAcceleration();
        }
        else
        {
            if (target != null)
            {
                CalculateAcceleration();
                ApplyAcceleration();
            }
        }
        
    }

    #endregion

    #region Custom
    
    
    
    // Always call CalculateAcceleration() + ApplyAcceleration()
    // Make sure to call CalculateAcceleration() before ApplyAcceleration()
    public void CalculateAcceleration()
    {
        // Calculating the direction without normalize it
        direction = new Vector3(target.position.x - transform.position.x, 0.0f,
            target.position.z - transform.position.z);
        distance = direction.magnitude;
        // Face at the direction that is moving toward currently
        _lookWhereYouGo.LookAtDirection(target.position);
        // If the character is outside of slow radius, move with full speed
        if (distance > slowDownRadius)
        {
            targetSpeed = maxSpeed;
            animator.SetBool("isWalking",true); 
            animator.SetBool("isRunning",true);
            
            
        }
        // If the character is inside the slow radius but outside target radium, slow it down
        else if (distance <= slowDownRadius && distance > targetRadius)
        {
            targetSpeed = maxSpeed * (distance / slowDownRadius);
            // Handling Animation
            
            animator.SetBool("isRunning",false);
            animator.SetBool("isWalking",true);
            
            
        }
        // If the character is within the target radius, stop
        else if (distance <= targetRadius)
        {
            targetSpeed = 0.0f;
            // Handling animation
            animator.SetBool("isRunning",false); 
            animator.SetBool("isWalking",false);
            
            
        }
        targetVelocity = direction.normalized * targetSpeed;
        acceleration = (targetVelocity - rigidbody.velocity) / timeToTarget;
        // Clip acceleration if it exceeds max acceleration
        if (acceleration.magnitude > maxAcceleration)
        {
            acceleration = acceleration.normalized * maxAcceleration;
        }
    }

    public void ApplyAcceleration()
    {
        transform.Translate(Vector3.forward * (targetSpeed * Time.deltaTime));
    }

    #endregion

    public void SetTarget(Transform targetPosition)
    {
        this.target = targetPosition;
    }

}
