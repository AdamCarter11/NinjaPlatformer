using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Screenshake : MonoBehaviour
{
    [SerializeField] float shakeIntensity = 0.1f;
    [SerializeField] float shakeDuration = 0.5f;

    [SerializeField] float lerpSpeed = 2;

    private Coroutine shakeCoroutine;
    private Transform targetPos;


    // Method to trigger screen shake
    public void Shake()
    {
        if (shakeCoroutine != null)
        {
            StopCoroutine(shakeCoroutine);
        }
        shakeCoroutine = StartCoroutine(DoShake());
    }

    // Coroutine to perform the screen shake effect
    private IEnumerator DoShake()
    {
        Vector3 originalPosition = transform.localPosition;
        float elapsedTime = 0f;

        while (elapsedTime < shakeDuration)
        {
            // Generate random offset within specified intensity
            float offsetX = Random.Range(-shakeIntensity, shakeIntensity);
            float offsetY = Random.Range(-shakeIntensity, shakeIntensity);

            // Apply the offset to the object's position
            transform.localPosition = originalPosition + new Vector3(offsetX, offsetY, 0f);

            // Increment elapsed time
            elapsedTime += Time.deltaTime;

            yield return null;
        }

        // Reset the object's position to its original position
        transform.localPosition = originalPosition;
    }
    private void Update()
    {
        if (targetPos != null)
        {
            Vector3 targetPosition = new Vector3(targetPos.position.x, targetPos.position.y, transform.position.z);
            transform.position = Vector3.Lerp(transform.position, targetPosition, lerpSpeed * Time.deltaTime);
            if(Vector2.Distance(transform.position, targetPos.position) < .1f)
            {
                transform.position = new Vector3(targetPos.position.x, targetPos.position.y, transform.position.z);
                targetPos = null;
            }
        }
    }
    public void SetTargetPos(Transform newTargetPos)
    {
        targetPos = newTargetPos;
    }
}
