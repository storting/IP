using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Timeline;
using UnityEngine;

public class playerMovement : MonoBehaviour
{
    public float GraundCheckRadius;
    public float speed;
    public float jumpForce;
    public float spleshForce;
    public float spleshTimeKD;
    public Transform GraundCheck;
    public LayerMask WhatIsGraund;
    public GameObject DamageZone;
    public float TimeForMove;

    private float _elapsedTime;
    private bool _cunSplesh = true;
    private bool _cunBoom = true;
    private bool _doBoom = false;
    private bool _stopMove = false;
    private bool hisJump = false;
    private bool _isGraunded;
    private Rigidbody2D _rb;
    private Vector2 _moveInput;
    private int _jumpCount = 1;
    

    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    IEnumerator jumpKD()
    {
        yield return new WaitForSeconds(0.3f);
        _jumpCount = 1;
        hisJump = true;
    }
    IEnumerator boomKD()
    {
        yield return new WaitForSeconds(0.5f);
        _cunBoom = true;
        _doBoom = false;
        _stopMove = false;
        DamageZone.SetActive(false);
    }

    IEnumerator spleshKD()
    {
        yield return new WaitForSeconds(spleshTimeKD);
        _cunSplesh = true;
    }
    IEnumerator doSplesh()
    {
        yield return new WaitForSeconds(0.2f);
        _stopMove = false;
    }
    private void FixedUpdate()
    {
        _isGraunded = Physics2D.OverlapCircle(GraundCheck.position, GraundCheckRadius, WhatIsGraund);
        Movement();
    }
    
    private void Movement()
    {
        if(_isGraunded)
        {
            hisJump = false;
            StartCoroutine(jumpKD());
        }
        if(!_isGraunded)
        {
            
        }
        _moveInput = new Vector2(Input.GetAxisRaw("Horizontal"), 0);
        if(!_stopMove)
        {
            _rb.AddForce(_moveInput * speed);
        }
        
        if (Input.GetKeyDown(KeyCode.Space) && _jumpCount == 1 && _isGraunded && !_doBoom)
        {
            _rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            Debug.Log("Jump");
            _jumpCount = 0;
            StartCoroutine(jumpKD());
        }
        else if (Input.GetKeyDown(KeyCode.Space) && hisJump)
        {
            if (_cunBoom)
            {
                _cunBoom = false;
                _rb.AddForce(-Vector2.up * 260f, ForceMode2D.Impulse);
                _doBoom = true;
                _stopMove = true;
                Debug.Log("BOOOm");
                StartCoroutine(boomKD());
            }
        } 
        if (Input.GetKeyDown(KeyCode.LeftShift) && _cunSplesh)
        {
            //_rb.AddForce(_moveInput * spleshForce, ForceMode2D.Impulse);

            _cunSplesh = false;
            StartCoroutine(spleshKD());
        }
    }
    private void OnCollisionEnter2D(Collision2D other)
    {
        if(_doBoom) 
        {
            Debug.Log("QWEWQEQWEW");
            DamageZone.SetActive(true);
        }
    }
}
