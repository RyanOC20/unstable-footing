using System.Collections;
using UnityEngine;

public enum TerrainType
{
    Normal,
    Ice,
    Mud,
    Fire
}

public class blockMover : MonoBehaviour
{
    [Header("Movement")]
    public float loweredAmount = 5f;
    public float moveDuration = 1.5f;

    [Header("Terrain Materials")]
    public Material normalMaterial;
    public Material iceMaterial;
    public Material mudMaterial;
    public Material fireMaterial;

    [Header("Physics Materials")]
    public PhysicsMaterial normalPhysics;
    public PhysicsMaterial icePhysics;
    public PhysicsMaterial mudPhysics;
    public PhysicsMaterial firePhysics;

    private Vector3 raisedPosition;
    private Vector3 loweredPosition;
    private Renderer tileRenderer;
    private Collider tileCollider;
    private Coroutine moveRoutine;

    private Vector3 originalScale;
    private Vector3 originalPosition;

    public bool IsGone { get; private set; } = false;

    //private Renderer tileRenderer;
private Color originalColor;

    private void Awake()
    {
        raisedPosition = transform.position;
        loweredPosition = raisedPosition + Vector3.down * loweredAmount;

        tileRenderer = GetComponent<Renderer>();
        tileCollider = GetComponent<Collider>();

        originalScale = transform.localScale;
        originalPosition = transform.position;

        tileRenderer = GetComponent<Renderer>();

if (tileRenderer != null)
{
    originalColor = tileRenderer.material.color;
}
    }

    public void Raise()
    {
        MoveTo(raisedPosition);
    }

    public void Lower()
    {
        MoveTo(loweredPosition);
    }

    public void ToggleHeight()
    {
        float distanceToRaised = Vector3.Distance(transform.position, raisedPosition);

        if (distanceToRaised < 0.1f)
            Lower();
        else
            Raise();
    }

    public void SetTerrain(TerrainType type)
    {
        Material chosenMaterial = normalMaterial;
        PhysicsMaterial chosenPhysics = normalPhysics;

        switch (type)
        {
            case TerrainType.Ice:
                chosenMaterial = iceMaterial;
                chosenPhysics = icePhysics;
                break;

            case TerrainType.Mud:
                chosenMaterial = mudMaterial;
                chosenPhysics = mudPhysics;
                break;

            case TerrainType.Fire:
                chosenMaterial = fireMaterial;
                chosenPhysics = firePhysics;
                break;

            case TerrainType.Normal:
            default:
                chosenMaterial = normalMaterial;
                chosenPhysics = normalPhysics;
                break;
        }

        if (tileRenderer != null && chosenMaterial != null)
            tileRenderer.material = chosenMaterial;

        if (tileCollider != null)
            tileCollider.material = chosenPhysics;
    }

    private void MoveTo(Vector3 targetPosition)
    {
        if (moveRoutine != null)
            StopCoroutine(moveRoutine);

        moveRoutine = StartCoroutine(MoveRoutine(targetPosition));
    }

    private IEnumerator MoveRoutine(Vector3 targetPosition)
    {
        Vector3 startPosition = transform.position;
        float elapsed = 0f;

        while (elapsed < moveDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / moveDuration;

            // Smooth movement instead of linear robot movement
            t = t * t * (3f - 2f * t);

            transform.position = Vector3.Lerp(startPosition, targetPosition, t);

            yield return null;
        }

        transform.position = targetPosition;
    }

    public void RaisePillar(float targetHeight)
{
    if (IsGone)
        return;

    if (moveRoutine != null)
        StopCoroutine(moveRoutine);

    moveRoutine = StartCoroutine(RaisePillarWithWarning(targetHeight));
}

private IEnumerator RaisePillarWithWarning(float targetHeight)
{
    yield return StartCoroutine(
        FlashWarning(Color.green, 2));

    yield return StartCoroutine(
        RaisePillarRoutine(targetHeight));
}

private IEnumerator RaisePillarRoutine(float targetHeight)
{
    Vector3 startScale = transform.localScale;
    Vector3 targetScale = startScale;

    targetScale.y = targetHeight;

    float elapsed = 0f;

    while (elapsed < moveDuration)
    {
        elapsed += Time.deltaTime;

        float t = elapsed / moveDuration;
        t = t * t * (3f - 2f * t);

        float newYScale = Mathf.Lerp(
            startScale.y,
            targetScale.y,
            t);

        Vector3 scale = transform.localScale;
        scale.y = newYScale;
        transform.localScale = scale;

        float bottomY = originalPosition.y - originalScale.y / 2f;

        Vector3 pos = transform.position;
        pos.y = bottomY + newYScale / 2f;
        transform.position = pos;

        yield return null;
    }
}

public float dropAmount = 20f;
public float dropDuration = 5f;

public void DropIntoWater()
{
    if (IsGone)
        return;

    IsGone = true;

    if (moveRoutine != null)
        StopCoroutine(moveRoutine);

    moveRoutine = StartCoroutine(DropWithWarning());
}

private IEnumerator MoveWholeBlockRoutine(Vector3 targetPosition)
{
    Vector3 startPosition = transform.position;
    float elapsed = 0f;

    while (elapsed < dropDuration)
    {
        elapsed += Time.deltaTime;

        float t = elapsed / dropDuration;
        t = t * t * (3f - 2f * t);

        transform.position = Vector3.Lerp(startPosition, targetPosition, t);

        yield return null;
    }

    transform.position = targetPosition;
}

private IEnumerator FlashWarning(Color flashColor, int flashes)
{
    if (tileRenderer == null)
        yield break;

    for (int i = 0; i < flashes; i++)
    {
        tileRenderer.material.color = flashColor;
        yield return new WaitForSeconds(0.25f);

        tileRenderer.material.color = originalColor;
        yield return new WaitForSeconds(0.25f);
    }
}
private IEnumerator DropWithWarning()
{
    yield return StartCoroutine(
        FlashWarning(Color.red, 2));

    Vector3 targetPosition =
        transform.position + Vector3.down * dropAmount;

    yield return StartCoroutine(
        MoveWholeBlockRoutine(targetPosition));
}
}