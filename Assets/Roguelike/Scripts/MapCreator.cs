using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum Tile
{
    Wall,
    Floor,
    Food,
    Passage
}

public enum Direction
{
    up,
    down,
    left,
    right
}

public class MapCreator : MonoBehaviour
{

    public GameObject[] wallPrefabs;
    public GameObject[] floorPrefabs;
    public GameObject passgePrefabs;
    public GameObject foodPrefab;


    private static int rows = 50;
    private static int cols = 50;

    private static int minRoomWidth = 6;
    private static int maxRoomWidth = 20;
    private static int minRoomHeight = 6;
    private static int maxRoomHeight = 20;

    private Tile[,] map = new Tile[cols, rows];
    private List<RectInfo> roomInfos = new List<RectInfo>();

    private int roomIndex = 0;

    //创建新元素的基准点
    private int anchorPosX;
    private int anchorPosY;
    int roomNum;
    Direction curDirection;

    #region debug用

    private int tryTimes = 100;
    private List<GameObject> objects = new List<GameObject>();


    #endregion

    void Start()
    {
        //while (tryTimes>0)
        //{
        //for (int i = 0; i < objects.Count; i++)
        //{
        //    Destroy(objects[i]);
        //}
        roomNum = Random.Range(3, 6);
        CalculateMap();
        CreateMap();
        //tryTimes = tryTimes - 1;
        //}
        //Debug.Log("测试100次完毕，正常");





    }
	
    // Update is called once per frame
    void Update()
    {
		
    }

    private bool IsWall(int x, int y)
    {
        return map[x, y] == Tile.Wall;
    }

    private void CalculateMap()
    {
        
        //1.创建墙壁
        FillWall();
        //2.随机产生一个房间
        FillRoom();

        
        //直到创建足够的房间数
        while (roomInfos.Count < roomNum)
        {
            //3.随机一面墙,创建通道
            //随机一个方向
            int wallPosX;
            int wallPosY;

            bool passOver = false;
            do
            {
                int dirNum = Random.Range(0, 4);
                int passLength = Random.Range(2, 10);
                //DebugMap();
                int roomWidth = Random.Range(minRoomWidth, maxRoomWidth);
                int roomHeight = Random.Range(minRoomHeight, maxRoomHeight);

                switch (dirNum)
                {
                    case 0:
                        wallPosX = Random.Range(roomInfos[roomIndex].posX, roomInfos[roomIndex].posX + roomInfos[roomIndex].width);
                        wallPosY = roomInfos[roomIndex].posY + roomInfos[roomIndex].height + 1;
                        anchorPosX = wallPosX;
                        anchorPosY = wallPosY + passLength;
                        curDirection = Direction.up;
                        if (wallPosY >= rows - passLength - minRoomHeight || !CheckFillRect(wallPosX, wallPosY - 1, 1, passLength) || !CheckFillRoomByAnchor(roomWidth, roomHeight))
                            continue;

                        Debug.Log("创建上通道");
                        FillRect(wallPosX, wallPosY - 1, 1, passLength, Tile.Passage);


                        passOver = true;

                        break;
                    case 1:
                        wallPosX = Random.Range(roomInfos[roomIndex].posX, roomInfos[roomIndex].posX + roomInfos[roomIndex].width);
                        wallPosY = roomInfos[roomIndex].posY - 1;
                        anchorPosX = wallPosX;
                        anchorPosY = wallPosY - passLength;
                        curDirection = Direction.down;
                        if (wallPosY <= passLength + minRoomHeight || !CheckFillRect(wallPosX, wallPosY - passLength + 1, 1, passLength) || !CheckFillRoomByAnchor(roomWidth, roomHeight))
                            continue;


                        Debug.Log("创建下通道");
                        FillRect(wallPosX, wallPosY - passLength + 1, 1, passLength, Tile.Passage);

                        passOver = true;

                        break;
                    case 2:
                        wallPosX = roomInfos[roomIndex].posX - 1;
                        wallPosY = Random.Range(roomInfos[roomIndex].posY, roomInfos[roomIndex].posY + roomInfos[roomIndex].height);
                        anchorPosX = wallPosX - passLength;
                        anchorPosY = wallPosY;
                        curDirection = Direction.left;
                        if (wallPosX <= passLength + minRoomWidth || !CheckFillRect(wallPosX - passLength + 1, wallPosY, passLength, 1) || !CheckFillRoomByAnchor(roomWidth, roomHeight))
                            continue;


                        Debug.Log("创建左通道");
                        FillRect(wallPosX - passLength + 1, wallPosY, passLength, 1, Tile.Passage);

                        passOver = true;

                        break;
                    case 3:
                        wallPosX = roomInfos[roomIndex].posX + roomInfos[roomIndex].width + 1;
                        wallPosY = Random.Range(roomInfos[roomIndex].posY, roomInfos[roomIndex].posY + roomInfos[roomIndex].height);
                        anchorPosX = wallPosX + passLength;
                        anchorPosY = wallPosY;
                        curDirection = Direction.right;
                        if (wallPosX >= cols - passLength - minRoomWidth || !CheckFillRect(wallPosX - 1, wallPosY, passLength, 1) || !CheckFillRoomByAnchor(roomWidth, roomHeight))
                            continue;


                        Debug.Log("创建右通道");
                        FillRect(wallPosX - 1, wallPosY, passLength, 1, Tile.Passage);

                        passOver = true;

                        break;

                }
            } while (!passOver);

        }

        

    }

    //填充墙壁
    private void FillWall()
    {
        for (int x = 0; x < cols; x++)
        {
            for (int y = 0; y < rows; y++)
            {
                FillTile(x, y, Tile.Wall);
            }
        }
    }

    //填充Tile
    private void FillTile(int posX, int posY, Tile tile)
    {
        try
        {
            map[posX, posY] = tile;
        }
        catch (System.Exception)
        {
            Debug.Log("posX:" + posX + ",posY:" + posY);
            throw;
        }
        
    }

    //检测是否能填充矩形Tile
    private bool CheckFillRect(int posX, int posY, int width, int height)
    {
        for (int x = posX; x < posX + width; x++)
        {
            for (int y = posY; y < posY + height; y++)
            {
                try
                {
                    if (map[x, y] != Tile.Wall)
                        return false;
                }
                catch (System.Exception)
                {
                    Debug.Log("x:" + x + ",y:" + y);
                    throw;
                }
                
            }
        }
        return true;
    }

    //填充矩形Tile
    private void FillRect(int posX, int posY, int width, int height, Tile tile)
    {
        for (int x = posX; x < posX + width; x++)
        {
            for (int y = posY; y < posY + height; y++)
            {
                FillTile(x, y, tile);
            }
        }
    }


    private void FillRoom()
    {

        //随机产生一个坐标，并且随机房间的大小，生成房间
        int posX;
        int posY;

        int width;
        int height;

        do
        {
            posX = Random.Range(1, cols);
            posY = Random.Range(1, rows);

            width = Random.Range(minRoomWidth, maxRoomWidth);
            height = Random.Range(minRoomHeight, maxRoomHeight);
        }
        while (posX + width > cols - 2 || posY + height > rows - 2 || map[posX, posY] != Tile.Wall || !CheckFillRect(posX, posY, width, height));

        RectInfo roomInfo = new RectInfo(posX, posY, width, height);
        FillRect(posX, posY, width, height, Tile.Floor);
        roomInfos.Add(roomInfo);

    }

    //private bool FillRoomByAnchor()
    //{
    //    int width;
    //    int height;

    //    do
    //    {
    //        width = Random.Range(minRoomWidth, maxRoomWidth);
    //        height = Random.Range(minRoomHeight, maxRoomHeight);
    //    } while (CheckFillRoomByAnchor(width,height));
    //    return true;
    //}

    private bool CheckFillRoomByAnchor(int width, int height)
    {
        int posX = -1;
        int posY = -1; 
        switch (curDirection)
        {
            case Direction.up:
                posX = Random.Range(anchorPosX - width + 1, anchorPosX);
                posY = anchorPosY - 1;
                break;
            case Direction.down:
                posX = Random.Range(anchorPosX - width + 1, anchorPosX);
                posY = anchorPosY - height + 1;
                break;
            case Direction.left:
                posX = anchorPosX - width + 1;
                posY = Random.Range(anchorPosY - height + 1, anchorPosY);
                break;
            case Direction.right:
                posX = anchorPosX - 1;
                posY = Random.Range(anchorPosY - height + 1, anchorPosY);
                break;
            
        }
        if (posX + width > cols - 2 || posY + height > rows - 2 || posX < 2 || posY < 2)
            return false;
        if (CheckFillRect(posX, posY, width, height))
        {
            FillRect(posX, posY, width, height, Tile.Floor);
            RectInfo roomInfo = new RectInfo(posX, posY, width, height);
            roomInfos.Add(roomInfo);
            roomIndex++;
            return true;
        }
        else
        {
            return false;
        }
        
    }

    //private bool CheckCanFill(int posX,int posY,int width,int height )
    //{
    //    for (int x = posX; x < posX + width; x++)
    //    {
    //        for (int y = posY; y < posY + height; y++)
    //        {
    //            if (map[x, y] != Tile.Wall)
    //                return false;
    //        }
    //    }
    //    return true;
    //}

    //填充地板
    private void FillFloor(int posX, int posY, int width, int height)
    {
        for (int x = posX; x < posX + width; x++)
        {
            for (int y = posY; y < posY + height; y++)
            {
                if (map[x, y] != Tile.Passage)
                    map[x, y] = Tile.Passage;
            }
        }
    }

    //随机选择房间的一面墙
    private Vector2 RandomRoomWall(int posX, int posY, int width, int height)
    {
        int wallPosX;
        int wallPosY;

        do
        {
            wallPosX = Random.Range(posX - 1, posX + width + 1);
            wallPosY = Random.Range(posY - 1, posY + height + 1);
        } while (IsInRoom(wallPosX, wallPosY, posX, posY, width, height) || wallPosX == 0 || wallPosX == cols || wallPosY == 0 || wallPosY == rows);
        //map[wallPosX, wallPosY] = Tile.Passage;
        return new Vector2(wallPosX, wallPosY);
    }

    private bool IsInRoom(int targetPosX, int targetPosY, int roomPosX, int roomPosY, int width, int height)
    {
        if (targetPosX >= roomPosX && targetPosX <= roomPosX + width && targetPosY >= roomPosY && targetPosY <= roomPosY + height)
            return true;
        return false;
    }

    private Direction GetWallDirection(int roomPosX, int roomPosY, int roomWidth, int roomHeight, int wallPosX, int wallPosY)
    {
        Direction direction = Direction.up;
        if (wallPosX > roomPosX + roomWidth)
        {
            direction = Direction.right;
        }
        else if (wallPosX < roomPosX)
        {
            direction = Direction.left;
        }
        else if (wallPosY > roomPosY + roomHeight)
        {
            direction = Direction.up;
        }
        else if (wallPosY < roomPosY)
        {
            direction = Direction.down;
        }
        return direction;
    }

    


    private void InitMap()
    {
        for (int x = 0; x < cols; x++)
        {
            for (int y = 0; y < rows; y++)
            {
                int num = Random.Range(0, 2);
                GameObject.Instantiate(wallPrefabs[num], new Vector3(x, y, 0), Quaternion.identity);
            }
        }
    }


    private void DebugMap()
    {
        string result = "";
        for (int x = 0; x < cols; x++)
        {

            result += "\n";
            
            for (int y = 0; y < rows; y++)
            {
                string s = "";
                switch (map[x, y])
                {
                    case Tile.Wall:
                        s = "W";
                        break;
                    case Tile.Floor:
                        s = "#";
                        break;
                    case Tile.Food:
                        s = "S";
                        break;
                    case Tile.Passage:
                        s = "P";
                        break;
                    default:
                        s = " ";
                        break;
                }
                result += s;
                
            }
        }
        Debug.Log(result);
        System.Console.WriteLine(result);
    }

    private void CreateMap()
    {
        for (int x = 0; x < cols; x++)
        {
            for (int y = 0; y < rows; y++)
            {
                Tile tag = map[x, y];
                int num;
                switch (tag)
                {
                    case Tile.Wall:
                        num = Random.Range(0, 2);
                        objects.Add((GameObject)GameObject.Instantiate(wallPrefabs[num], new Vector3(x, y, 0), Quaternion.identity));
                        break;
                    case Tile.Floor:
                        num = Random.Range(0, 2);
                        objects.Add((GameObject)GameObject.Instantiate(floorPrefabs[num], new Vector3(x, y, 0), Quaternion.identity));
                        break;
                    case Tile.Passage:
                        num = Random.Range(0, 2);
                        objects.Add((GameObject)GameObject.Instantiate(passgePrefabs, new Vector3(x, y, 0), Quaternion.identity));
                        break;
                }
            }
        }
    }
}
