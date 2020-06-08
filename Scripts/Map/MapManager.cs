using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    public MapUI mapUI;
    public IslandManager islandManager;
    public EventManager eventManager;
    public SectorName sectorName;

    public int size;
    public int seed;

    [Header("Armymarker")]
    public GameObject armyMarker;
    public float armySpeed = 3f;

    [Header("Prefabs")]
    public Island islandPrefab;
    public Bridge bridgePrefab;
    public GameObject islandContainer;
    public GameObject bridgeContainer;

    [Header("Settings")]
    public float bridgeBreakDelay;
    public float combatStartDelay;

    [Header("Sound")]
    public AudioSource audioSource;
    public AudioClip breakingSound;
    public AudioClip travelingSound;
    public AudioClip eventSound;
    public AudioClip combatSound;

    public Sector sectorBase;
    
    List<Island> islands;
    List<Bridge> bridges;
    List<Island> adjacentIslands;
    List<Sector> sectors;

    bool playerControl;
    Island currentIsland;
    public Sector currentSector;

    private void Start()
    {
        sectors = new List<Sector>();
        islands = new List<Island>();
        bridges = new List<Bridge>();

        seed = GameData.instance.data.seed;

        // Create starting sector
        GenerateMap(new Point(0, 0));
        
        // Load visited islands
        foreach(Point point in GameData.instance.data.visitedIslands)
        {
            Island island = FindIsland(point.x, point.y);
            if (island != null)
            {
                island.Visited();
                island.RemoveEvent();
                if (currentIsland != null)
                {
                    Bridge b = FindBridge(currentIsland, island);
                    if (b != null)
                    {
                        b.Walk();
                    }
                    else
                    {
                        Debug.Log("No such bridge " + currentIsland.point + " - " + island.point);
                    }
                }
                RevealIsland(island);
                currentIsland = island;
            }
        }

        // Set player
        SetPlayer();

        // Load event
        LoadCurrentEvent();

        // Update UI
        mapUI.UpdateAll();
        
        // Save game
        // SaveManager.Save(GameData.instance);
    }

    private void Update()
    {
        if (playerControl)
        {
            if (Input.GetKeyDown(KeyCode.W))
            {
                Island island = FindIsland(currentIsland.point.x, currentIsland.point.y + 2);
                if (island != null)
                {
                    island.OnMouseDown();
                }
            }
            else if(Input.GetKeyDown(KeyCode.S))
            {
                Island island = FindIsland(currentIsland.point.x, currentIsland.point.y - 2);
                if (island != null)
                {
                    island.OnMouseDown();
                }
            }
            else if (Input.GetKeyDown(KeyCode.A))
            {
                Island island = FindIsland(currentIsland.point.x - 2, currentIsland.point.y);
                if (island != null)
                {
                    island.OnMouseDown();
                }
            }
            else if (Input.GetKeyDown(KeyCode.D))
            {
                Island island = FindIsland(currentIsland.point.x + 2, currentIsland.point.y);
                if (island != null)
                {
                    island.OnMouseDown();
                }
            }
        }
    }

    private void LoadCurrentEvent()
    {
        if (GameData.instance.data.eventName != "none")
        {
            Event currentEvent = Config.GetEvent(GameData.instance.data.eventName);
            eventManager.StartEvent(currentEvent);
        }
    }

    private Island FindIsland(int x, int y)
    {
        foreach (Island island in islands)
        {
            if (x == island.point.x && y == island.point.y)
            {
                return island;
            }
        }
        return null;
    }

    private void GenerateMap(Point sectorPoint)
    {
        // Check if sector already exists
        foreach(Sector sector in sectors)
        {
            if (sector.point == sectorPoint){
                return;
            }
        }

        // Create new sector
        Sector newSector = Instantiate(sectorBase);

        MapGenerator.size = size;
        MapObject[,] grid = MapGenerator.CreateMap(seed, newSector, sectorPoint.x, sectorPoint.y);
        newSector.point = sectorPoint;
        newSector.GenerateName(seed);
        newSector.grid = grid;

        Point offSet = new Point(sectorPoint.x * size - sectorPoint.x, sectorPoint.y * size - sectorPoint.y);

        // Build given map
        for (int x = 0; x < grid.GetLength(0); x++)
        {
            for (int y = 0; y < grid.GetLength(1); y++)
            {
                
                // Create islands
                if (grid[x, y] == MapObject.island)
                {
                    Island island = Instantiate(islandPrefab, islandContainer.transform);
                    island.transform.localPosition += new Vector3(x + offSet.x, y + offSet.y);
                    island.point = new Point(island.transform.localPosition);
                    island.mapManager = this;
                    island.SetEvent(sectorBase.GetEvent(seed, x, y));
                    islands.Add(island);
                }
                
                // Create bridges
                else if (grid[x, y] == MapObject.bridge || grid[x, y] == MapObject.brokenBridge)
                {
                    Bridge bridge = Instantiate(bridgePrefab, bridgeContainer.transform);
                    bridge.transform.localPosition += new Vector3(x + offSet.x, y + offSet.y);
                    bridge.SetSprite(grid[x, y] == MapObject.bridge, y % 2 == 1);
                    bridges.Add(bridge);
                }
            }
        }
        if (sectorPoint.x == 0 && sectorPoint.y == 0)
        {
            Point start = new Point(size / 2, size / 2);
            Island found = FindIsland(start.x, start.y);

            // Search for island
            int range = 2;
            while (found == null && range < size)
            {
                for (int x = -range; x < range; x += 2)
                {
                    for (int y = -range; y < range; y += 2)
                    {
                        if (Mathf.Abs(x) + Mathf.Abs(y) == range)
                        {
                            found = FindIsland(start.x + x, start.y + y);
                            if (found != null)
                            {
                                break;
                            }
                        }
                    }
                    if (found != null)
                    {
                        break;
                    }
                }
                range += 2;
            }
            if (found != null)
                found.SetSprite(islandManager.castle);
        }
        sectors.Add(newSector);
    }

    private void SetPlayer()
    {
        Data data = GameData.instance.data;
        if (data.mapPosition.x == -1)
        {
            Point start = new Point(size / 2, size / 2);
            bool found = FindIsland(start.x, start.y) != null;
            // Search for island
            int range = 2;
            while (!found && range < size)
            {
                for (int x = -range; x < range; x += 2)
                {
                    for (int y = -range; y < range; y += 2)
                    {
                        if (Mathf.Abs(x) + Mathf.Abs(y) == range)
                        {
                            if (FindIsland(start.x + x, start.y + y) != null)
                            {
                                found = true;
                                start = new Point(start.x + x, start.y + y);
                                break;
                            }
                        }
                    }
                    if (found)
                    {
                        break;
                    }
                }
                range += 2;
            }
            armyMarker.transform.localPosition = new Vector3Int(start.x, start.y, 0);
        }
        else
        {
            armyMarker.transform.localPosition = new Vector3Int(data.mapPosition.x, data.mapPosition.y, 0);
        }

        Point position = new Point(armyMarker.transform.localPosition);
        currentIsland = FindIsland(position.x, position.y);
        if (currentIsland == null)
        {
            Debug.Log("Starting island is null!");
        }

        adjacentIslands = new List<Island>();
        SetAdjacentIslands();

        playerControl = true;
    }

    private void SetAdjacentIslands()
    {
        adjacentIslands.Clear();
        Vector3 position = armyMarker.transform.localPosition;
        foreach(Island island in islands)
        {
            float distance = Vector2.Distance(island.transform.localPosition, position);
            if (Mathf.Abs(distance) == 2)
            {
                Bridge bridge = FindBridge(currentIsland, island);
                if (bridge != null)
                {
                    adjacentIslands.Add(island);
                    island.IsAdjacent(true);
                }
                else
                {
                    island.IsAdjacent(false);
                }
            }
            else
            {
                island.IsAdjacent(false);
            }
        }

        if (adjacentIslands.Count == 0)
        {
            mapUI.ShowGameOver("Dead end");
            playerControl = false;
        }
    }

    private Bridge FindBridge(Island first, Island second)
    {
        if (first == null ||second == null)
        {
            Debug.Log(first + " " + second);
            Debug.Log(first?.point);
            Debug.Log(second?.point);
            return null;
        }
        Point middle = new Point((second.point.x - first.point.x) / 2, (second.point.y - first.point.y) / 2);
        middle.x += first.point.x;
        middle.y += first.point.y;

        foreach(Bridge b in bridges)
        {
            if (b.point == middle)
            {
                return b;
            }
        }
        return null;
    }

    private Sector FindSector(Point point)
    {
        Point sectorPoint = new Point(point.x / (size - 1), point.y / (size - 1));

        if (point.x < 0)
        {
            sectorPoint.x -= 1;
        }
        if (point.y < 0)
        {
            sectorPoint.y -= 1;
        }

        foreach (Sector sector in sectors)
        {
            if (sector.point == sectorPoint)
            {
                return sector;
            }
        }
        return null;
    }

    public void IslandClicked(Island island)
    {
        if (!playerControl)
        {
            return;
        }

        if (adjacentIslands.Contains(island))
        {
            // Update game data
            GameData.instance.data.mapPosition = island.point;
            int resources = GameData.instance.data.resources;

            if (resources > 0)
            {
                GameData.instance.data.resources--;
                mapUI.UpdateResources();
            }
            else if (GameData.instance.data.maxResources > 0)
            {
                GameData.instance.data.maxResources--;
                mapUI.UpdateResources();
            }
            else
            {
                mapUI.ShowGameOver("Out of resources");
                playerControl = false;
                return;
            }

            playerControl = false;
            island.Visited();
            StartCoroutine(Travel(island));
        }
    }

    public void EndOfEvent()
    {
        currentIsland.RemoveEvent();

        // Remove event info from saveData
        GameData.instance.data.eventName = "none";

        // Make sure that current island is visited
        if (!GameData.instance.data.visitedIslands.Contains(currentIsland.point))
        {
            GameData.instance.data.visitedIslands.Add(currentIsland.point);
        }

        // Save game
        SaveManager.Save(GameData.instance);

        SetAdjacentIslands();
        playerControl = true;
    }

    IEnumerator Travel(Island island)
    {
        currentSector = FindSector(currentIsland.point);
        Sector sector = FindSector(island.point);
        if (currentSector != sector)
        {
            sectorName.Show(sector);
        }
        
        Bridge bridge = FindBridge(currentIsland, island);
        RevealIsland(island);

        // Move towards target
        while (Vector2.Distance(island.transform.localPosition, armyMarker.transform.localPosition) != 0)
        {
            armyMarker.transform.localPosition = Vector2.MoveTowards(armyMarker.transform.localPosition, island.transform.localPosition, Time.deltaTime * armySpeed);
            yield return null;
        }

        if (bridge.fragile)
        {
            bridges.Remove(bridge);
            bridge.Walk();
            audioSource.PlayOneShot(breakingSound);
            yield return new WaitForSeconds(bridgeBreakDelay);
        }

        // Update position
        GameData.instance.data.mapPosition = island.point;

        // Add island to visited locations list
        if (!GameData.instance.data.visitedIslands.Contains(currentIsland.point))
        {
            GameData.instance.data.visitedIslands.Add(currentIsland.point);
        }

        // Start event
        if (island.islandEvent != null)
        {
            GameData.instance.data.eventName = island.islandEvent.name;
            eventManager.StartEvent(island.islandEvent);
        }
        else
        {
            SetAdjacentIslands();
            playerControl = true;
        }

        GameData.instance.data.day++;
        mapUI.UpdateDate();

        SaveManager.Save(GameData.instance);
    }

    private void RevealIsland(Island island)
    {
        currentIsland = island;

        // See if new sector needs to be generated
        int offSize = size - 1;
        Point sectorPoint = new Point(island.point.x % offSize, island.point.y % offSize);

        // Negative space offset
        int negativeX = 0;
        int negativeY = 0;

        // Negative sectors
        if (sectorPoint.x < 0)
        {
            sectorPoint.x = offSize + sectorPoint.x;
            negativeX -= 1;
        }
        if (sectorPoint.y < 0)
        {
            sectorPoint.y = offSize + sectorPoint.y;
            negativeY -= 1;
        }

        int range = 7;

        // Horizontal
        if (sectorPoint.x <= range)
        {
            Point sector = new Point(island.point.x / offSize - 1 + negativeX, island.point.y / offSize + negativeY);
            GenerateMap(sector);
        }
        else if (sectorPoint.x >= offSize - range)
        {
            Point sector = new Point(island.point.x / offSize + 1 + negativeX, island.point.y / offSize + negativeY);
            GenerateMap(sector);
        }

        // Vertical
        if (sectorPoint.y <= range)
        {
            Point sector = new Point(island.point.x / offSize + negativeX, island.point.y / offSize - 1 + negativeY);
            GenerateMap(sector);
        }
        else if (sectorPoint.y >= offSize - range)
        {
            Point sector = new Point(island.point.x / offSize + negativeX, island.point.y / offSize + 1 + negativeY);
            GenerateMap(sector);
        }
    }

    public void StartingCombat()
    {
        audioSource.PlayOneShot(combatSound);
    }

    public void StartingEvent()
    {
        audioSource.PlayOneShot(eventSound);
    }

    public void GameOver()
    {
        // ToDo: Load EndScene
        GameData.instance.LoadScene("EndScene");
    }
}
