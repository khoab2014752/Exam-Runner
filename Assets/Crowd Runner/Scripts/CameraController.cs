using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraController : MonoBehaviour
{
    [Header(" Camera Settings ")]
    [SerializeField] private CinemachineVirtualCamera virtualCamera;
    [SerializeField] private Transform playerTransform;
    [SerializeField] private float followSpeed = 5f;
    [SerializeField] private Vector3 offset = new Vector3(0, 13.26f, -12.89f);
    
    [Header(" Camera Effects ")]
    [SerializeField] private float shakeIntensity = 0.1f;
    [SerializeField] private float shakeDuration = 0.2f;
    
    private CinemachineTransposer transposer;
    private Vector3 originalOffset;
    private bool isShaking = false;
    
    private void Start()
    {
        if (virtualCamera == null)
            virtualCamera = FindObjectOfType<CinemachineVirtualCamera>();
            
        if (playerTransform == null)
            playerTransform = PlayerController.instance?.transform;
            
        if (virtualCamera != null)
        {
            transposer = virtualCamera.GetCinemachineComponent<CinemachineTransposer>();
            if (transposer != null)
            {
                originalOffset = transposer.m_FollowOffset;
            }
        }
        
        GameManager.onGameStateChanged += OnGameStateChanged;
    }
    
    private void OnDestroy()
    {
        GameManager.onGameStateChanged -= OnGameStateChanged;
    }
    
    private void OnGameStateChanged(GameManager.GameState gameState)
    {
        switch (gameState)
        {
            case GameManager.GameState.Game:
                StartCameraFollow();
                break;
            case GameManager.GameState.Gameover:
                ShakeCamera();
                break;
            case GameManager.GameState.LevelComplete:
                // Optional: Add celebration camera effect
                break;
        }
    }
    
    private void StartCameraFollow()
    {
        if (virtualCamera != null && playerTransform != null)
        {
            virtualCamera.Follow = playerTransform;
            virtualCamera.LookAt = playerTransform;
        }
    }
    
    public void ShakeCamera()
    {
        if (!isShaking)
            StartCoroutine(CameraShake());
    }
    
    private IEnumerator CameraShake()
    {
        isShaking = true;
        float elapsed = 0f;
        
        while (elapsed < shakeDuration)
        {
            if (transposer != null)
            {
                Vector3 shakeOffset = originalOffset + Random.insideUnitSphere * shakeIntensity;
                transposer.m_FollowOffset = shakeOffset;
            }
            
            elapsed += Time.deltaTime;
            yield return null;
        }
        
        if (transposer != null)
            transposer.m_FollowOffset = originalOffset;
            
        isShaking = false;
    }
    
    public void SetCameraTarget(Transform target)
    {
        if (virtualCamera != null)
        {
            virtualCamera.Follow = target;
            virtualCamera.LookAt = target;
        }
    }
    
    public void SetCameraOffset(Vector3 newOffset)
    {
        if (transposer != null)
        {
            transposer.m_FollowOffset = newOffset;
            originalOffset = newOffset;
        }
    }
} 