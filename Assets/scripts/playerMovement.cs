using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Timeline;
using UnityEngine;

public class playerMovement : MonoBehaviour
{
    
    public float speed;
    public float jumpForce;
    public float LungeImpulse;
    public float spleshTimeKD;
    public float boomKd;
    public float boomMaxUp;
    public LayerMask WhatIsGraund;
    public float GraundCheckRadius;
    public GameObject DamageZone;
    public float TimeForMove;


    public bool _isGraunded;
    private bool _cunSplesh = true;
    public bool _cunBoom = true;
    private bool _doBoom = false;
    private bool _stopMove = false;
    public bool _cunJump = false;
    private Rigidbody2D _rb;
    private BoxCollider2D _boxCollider2D;
    private Vector2 _moveInput;
    private Vector3 _localScale;
    private int _jumpCount = 1;
    private bool _faceRight;
    public bool rayBoom;

    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _localScale = _rb.transform.localScale;
        _boxCollider2D = GetComponent<BoxCollider2D>();
    }
    IEnumerator cayoteTimer()
    {
        yield return new WaitForSeconds(0.1f);
        _cunJump = true;
    }
    IEnumerator jumpKD()
    {
        yield return new WaitForSeconds(0.3f);
        _jumpCount = 1;
        _cunJump = true;
    }
    IEnumerator doBoom()
    {
        yield return new WaitForSeconds(0.5f);
        _doBoom = false;
        _stopMove = false;
        DamageZone.SetActive(false);
    }
    IEnumerator boomKD()
    {
        yield return new WaitForSeconds(boomKd);
        _cunBoom = true;
    }

    IEnumerator spleshKD()
    {
        yield return new WaitForSeconds(spleshTimeKD);
        _cunSplesh = true;
    }
    private void FixedUpdate()
    {

        _isGraunded = _isGraund();
        Movement();
        Reflect();
        Lunge();
        GraundLogic();
    }

    private bool _isGraund()
    {
        float extraHeightText = 0.15f;
        RaycastHit2D raycastHit =  Physics2D.BoxCast(_boxCollider2D.bounds.center, _boxCollider2D.bounds.size - new Vector3(0.02f, -0.3f, 0), 0, Vector2.down, _boxCollider2D.bounds.extents.y + extraHeightText, WhatIsGraund);
        Color rayColor;
        if(raycastHit.collider != null)
        {
            rayColor = Color.green;
        }
        else
        {
            rayColor = Color.red;
        }
        Debug.DrawRay(_boxCollider2D.bounds.center + new Vector3(_boxCollider2D.bounds.extents.x, 0), Vector2.down * (_boxCollider2D.bounds.extents.y + extraHeightText), rayColor);
        Debug.DrawRay(_boxCollider2D.bounds.center - new Vector3(_boxCollider2D.bounds.extents.x, 0), Vector2.down * (_boxCollider2D.bounds.extents.y + extraHeightText), rayColor);
        Debug.DrawRay(_boxCollider2D.bounds.center - new Vector3(_boxCollider2D.bounds.extents.x, _boxCollider2D.bounds.extents.y + extraHeightText), Vector2.right * (_boxCollider2D.bounds.extents.x + extraHeightText*9), rayColor);
        return raycastHit.collider != null;
    }

    private void GraundLogic()
    {
        if (_isGraunded)
        {
            _cunJump = false;
            if(!_cunJump) {
                StartCoroutine(jumpKD());
            }
        }
        else if (!_isGraunded)
        {
            StartCoroutine(cayoteTimer());
        }
    }
    private void Lunge()
    {
        if(Input.GetKeyDown((KeyCode.LeftShift)) && _cunSplesh)
        {
            _rb.velocity = new Vector2(0, 0);
            _cunSplesh = false;
            StartCoroutine(spleshKD());
            Debug.Log("Splesh");
            if(!_faceRight)
            {
                _rb.AddForce(Vector2.left * LungeImpulse, ForceMode2D.Impulse);
            }
            if (_faceRight)
            {
                _rb.AddForce(Vector2.right * LungeImpulse, ForceMode2D.Impulse);
            }

        }
            
    }
    private void Movement()
    {
        if (_faceRight)
        {
            rayBoom = Physics2D.Raycast(_boxCollider2D.bounds.center + new Vector3(_boxCollider2D.bounds.extents.x - 0.02f, 0), Vector2.down, boomMaxUp, WhatIsGraund);
            Debug.DrawRay(_boxCollider2D.bounds.center + new Vector3(_boxCollider2D.bounds.extents.x - 0.02f, 0), Vector2.down * boomMaxUp, Color.green);
        }
        else if (!_faceRight)
        {
            rayBoom = Physics2D.Raycast(_boxCollider2D.bounds.center - new Vector3(_boxCollider2D.bounds.extents.x - 0.02f, 0), Vector2.down, boomMaxUp, WhatIsGraund);
            Debug.DrawRay(_boxCollider2D.bounds.center - new Vector3(_boxCollider2D.bounds.extents.x - 0.02f, 0), Vector2.down * boomMaxUp, Color.green);
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
        else if (Input.GetKeyDown(KeyCode.Space) && _cunJump && !rayBoom)
        {
            if (_cunBoom)
            {
                _rb.velocity = new Vector2(0, 0);
                _cunBoom = false;
                _rb.AddForce(-Vector2.up * 300f, ForceMode2D.Impulse);
                _doBoom = true;
                _stopMove = true;
                Debug.Log("BOOOm");
                StartCoroutine(doBoom());
                StartCoroutine(boomKD());
            }
        } 
    }
    private void Reflect()
    {
        if (_moveInput.x == -1)
        {
            _rb.transform.localScale = new Vector3(-_localScale.x, _rb.transform.localScale.y, _rb.transform.localScale.z);
            _faceRight = false;
        }
        if (_moveInput.x == 1) 
        {
            _rb.transform.localScale = new Vector3(+_localScale.x, _rb.transform.localScale.y, _rb.transform.localScale.z);
            _faceRight = true;
        }
        if (_moveInput.x == 0)
        {
            //_rb.transform.localScale = new Vector3(Math.Abs(_rb.transform.localScale.x), _rb.transform.localScale.y, _rb.transform.localScale.z);
            _faceRight = true;
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
