using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ArenaManager : MonoBehaviour
{
    [Header("Tiles")]
    public List<blockMover> tiles = new List<blockMover>();

    [Header("Match Timing")]
    public float matchLengthSeconds = 180f;
    private float matchTimer = 0f;

    [Header("Chaos Growth")]
    [Tooltip("Higher values make the end of the match more chaotic. Try 2, 3, or 4.")]
    public float chaosExponent = 2.5f;

    public float startMinDelay = 8f;
    public float startMaxDelay = 14f;

    public float endMinDelay = 0.4f;
    public float endMaxDelay = 1.2f;

    [Header("Blocks Per Event")]
    public int startMinBlocksChanged = 1;
    public int startMaxBlocksChanged = 2;

    public int endMinBlocksChanged = 4;
    public int endMaxBlocksChanged = 10;

    [Header("Pillar Settings")]
    public float minPillarHeight = 1f;
    public float maxPillarHeight = 15f;

    [Header("Change Chances")]
    [Range(0f, 1f)]
    public float dropChance = 0.35f;

    [Range(0f, 1f)]
    public float terrainChangeChance = 0.25f;

    [Header("Safety")]
public int minimumBlocksRemaining = 15;

    private void Start()
    {
        if (tiles.Count == 0)
        {
            tiles.AddRange(FindObjectsByType<blockMover>(FindObjectsSortMode.None));
        }

        StartCoroutine(RandomArenaRoutine());
    }

    private void Update()
    {
        matchTimer += Time.deltaTime;

        // Optional test controls
        if (Keyboard.current != null)
        {
            if (Keyboard.current.pKey.wasPressedThisFrame)
                RandomPillars(GetCurrentBlocksChangedCount());

            if (Keyboard.current.dKey.wasPressedThisFrame)
                RandomDrops(GetCurrentBlocksChangedCount());

            if (Keyboard.current.tKey.wasPressedThisFrame)
                RandomTerrains(GetCurrentBlocksChangedCount());

            if (Keyboard.current.rKey.wasPressedThisFrame)
                ResetRandomTiles(GetCurrentBlocksChangedCount());
        }
    }

    private IEnumerator RandomArenaRoutine()
    {
        while (true)
        {
            float waitTime = GetCurrentRandomDelay();
            yield return new WaitForSeconds(waitTime);

            RandomArenaChange();
        }
    }

    private float GetChaosAmount()
    {
        float matchProgress = Mathf.Clamp01(matchTimer / matchLengthSeconds);
        return Mathf.Pow(matchProgress, chaosExponent);
    }

    private float GetCurrentRandomDelay()
    {
        float chaos = GetChaosAmount();

        float currentMinDelay = Mathf.Lerp(startMinDelay, endMinDelay, chaos);
        float currentMaxDelay = Mathf.Lerp(startMaxDelay, endMaxDelay, chaos);

        return Random.Range(currentMinDelay, currentMaxDelay);
    }

    private int GetCurrentBlocksChangedCount()
    {
        float chaos = GetChaosAmount();

        int currentMin = Mathf.RoundToInt(Mathf.Lerp(startMinBlocksChanged, endMinBlocksChanged, chaos));
        int currentMax = Mathf.RoundToInt(Mathf.Lerp(startMaxBlocksChanged, endMaxBlocksChanged, chaos));

        currentMax = Mathf.Max(currentMin, currentMax);

        return Random.Range(currentMin, currentMax + 1);
    }

    public void RandomArenaChange()
    {
        int blocksToChange = GetCurrentBlocksChangedCount();

        for (int i = 0; i < blocksToChange; i++)
        {
            float roll = Random.value;

            if (roll < dropChance)
            {
                RandomDrop();
            }
            else if (roll < dropChance + terrainChangeChance)
            {
                RandomTileTerrain();
            }
            else
            {
                RandomPillar();
            }
        }
    }

    public void RandomPillar()
    {
        blockMover tile = GetRandomTile();

        if (tile == null)
            return;

        float height = Random.Range(minPillarHeight, maxPillarHeight);
        tile.RaisePillar(height);
    }

    public void RandomDrop()
{
    if (GetActiveBlockCount() <= minimumBlocksRemaining)
        return;

    blockMover tile = GetRandomTile();

    if (tile == null)
        return;

    tile.DropIntoWater();
}

    public void RandomTileTerrain()
    {
        blockMover tile = GetRandomTile();

        if (tile == null)
            return;

        TerrainType randomTerrain = (TerrainType)Random.Range(0, 4);
        tile.SetTerrain(randomTerrain);
    }

    public void ResetRandomTile()
    {
        blockMover tile = GetRandomTile();

        if (tile == null)
            return;

        tile.RaisePillar(1f);
        tile.SetTerrain(TerrainType.Normal);
    }

    public void RandomPillars(int count)
    {
        List<blockMover> chosenTiles = GetRandomTiles(count);

        foreach (blockMover tile in chosenTiles)
        {
            float height = Random.Range(minPillarHeight, maxPillarHeight);
            tile.RaisePillar(height);
        }
    }

    public void RandomDrops(int count)
{
    List<blockMover> chosenTiles = GetRandomTiles(count);

    foreach (blockMover tile in chosenTiles)
    {
        if (GetActiveBlockCount() <= minimumBlocksRemaining)
            break;

        tile.DropIntoWater();
    }
}

    public void RandomTerrains(int count)
    {
        List<blockMover> chosenTiles = GetRandomTiles(count);

        foreach (blockMover tile in chosenTiles)
        {
            TerrainType randomTerrain = (TerrainType)Random.Range(0, 4);
            tile.SetTerrain(randomTerrain);
        }
    }

    public void ResetRandomTiles(int count)
    {
        List<blockMover> chosenTiles = GetRandomTiles(count);

        foreach (blockMover tile in chosenTiles)
        {
            tile.RaisePillar(1f);
            tile.SetTerrain(TerrainType.Normal);
        }
    }

    private blockMover GetRandomTile()
{
    List<blockMover> availableTiles = new List<blockMover>();

    foreach (blockMover tile in tiles)
    {
        if (tile != null && !tile.IsGone)
            availableTiles.Add(tile);
    }

    if (availableTiles.Count == 0)
        return null;

    return availableTiles[Random.Range(0, availableTiles.Count)];
}

    private List<blockMover> GetRandomTiles(int count)
{
    List<blockMover> availableTiles = new List<blockMover>();

    foreach (blockMover tile in tiles)
    {
        if (tile != null && !tile.IsGone)
            availableTiles.Add(tile);
    }

    List<blockMover> chosenTiles = new List<blockMover>();

    count = Mathf.Clamp(count, 0, availableTiles.Count);

    for (int i = 0; i < count; i++)
    {
        int index = Random.Range(0, availableTiles.Count);
        chosenTiles.Add(availableTiles[index]);
        availableTiles.RemoveAt(index);
    }

    return chosenTiles;
}

private int GetActiveBlockCount()
{
    int count = 0;

    foreach (blockMover tile in tiles)
    {
        if (tile != null && !tile.IsGone)
            count++;
    }

    return count;
}
}