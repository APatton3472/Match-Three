﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainScript : MonoBehaviour
{
    
    public ArrayLayout boardLayout;
    
    [Header("UI Elements")]
    public Sprite[] pieces;
    public RectTransform gameBoard;

    [Header("Prefabs")]
    public GameObject nodePiece;

    int width=20;
    int height = 12;
    Node[,] board;
    
    List<NodePiece> update;
    List<FlippedPieces> flipped;
    List<NodePiece> dead;
    System.Random random;
    

   
    void Start()
    {
        StartGame();
        ScoreDisplay.Score = 0;
    }

    void Update()
    {
        
        List<NodePiece> finishedUpdating = new List<NodePiece>();
        for(int i = 0; i < update.Count; i++)
        {
            NodePiece piece = update[i];
            if (!piece.UpdatePiece())
            {
                finishedUpdating.Add(piece);
            }
        }
        for (int i = 0; i <finishedUpdating.Count; i++)
        {
            NodePiece piece = finishedUpdating[i];
            FlippedPieces flip = getFlipped(piece);
            NodePiece flippedPiece = null;
            

            List<Point> connected = isConnected(piece.index, true);
            bool wasFlipped = (flip != null);

            if (wasFlipped)
            {
                flippedPiece = flip.getOtherPiece(piece);
                AddPoints(ref connected, isConnected(flippedPiece.index, true));
                
            }
            if (connected.Count == 0)
            {
                if (wasFlipped)
                    FlipPieces(piece.index, flippedPiece.index ,false);
            }
            else
            {
                foreach(Point pnt in connected)
                {
                    ScoreDisplay.Score += 100;
                    Node node = getNodeatPoint(pnt);
                    NodePiece piece1 = node.getPiece();
                    if (piece1 != null)
                    {
                        piece1.gameObject.SetActive(false);
                        dead.Add(piece1);
                    }
                    node.SetPiece(null);
                }
                ApplyGravityToBoard();
            }
            flipped.Remove(flip);
            update.Remove(piece);
        }
        
    }

    void ApplyGravityToBoard()
    {
        for(int x = 0; x < width; x++)
        {
            for(int y = (height - 1); y >= 0; y--)
            {
                Point p = new Point(x, y);
                Node node = getNodeatPoint(p);
                int val = getValueatPoint(p);
                if (val != 0) continue;
                for(int ny = (y - 1); ny >= -1; ny--)
                {
                    Point next = new Point(x, ny);
                    int nextVal = getValueatPoint(next);
                    if (nextVal == 0)
                        continue;
                    if (nextVal != -1)
                    {
                        Node got = getNodeatPoint(next);
                        NodePiece piece = got.getPiece();

                        node.SetPiece(piece);
                        update.Add(piece);

                        got.SetPiece(null);
                    }
                    else
                    {
                        int newVal = fillPiece();
                        NodePiece piece;
                        Point fallPnt = new Point(x, -1);
                        if (dead.Count > 0)
                        {
                            NodePiece revived = dead[0];
                            revived.gameObject.SetActive(true);
                            revived.rect.anchoredPosition = getPointPosition(new Point(x, -1));
                            piece = revived;


                            
                            dead.RemoveAt(0);
                        }
                        else
                        {
                            GameObject obj = Instantiate(nodePiece, gameBoard);
                            NodePiece n = obj.GetComponent<NodePiece>();
                            RectTransform rect = obj.GetComponent<RectTransform>();
                            rect.anchoredPosition = getPointPosition(new Point(x, -1));
                            piece = n;
                             
                        }
                        piece.Initialize(newVal, p, pieces[newVal - 1]);
                        Node hole = getNodeatPoint(p);
                        hole.SetPiece(piece);
                        ResetPiece(piece);
                    }
                    break;
                }
            }
        }
    }
    
    FlippedPieces getFlipped(NodePiece p)
    {
        FlippedPieces flip = null;
        for (int i = 0; i < flipped.Count; i++)
        {

            if (flipped[i].getOtherPiece(p) != null) 
            {
                flip = flipped[i];
                break;
            }
                
        }
        return flip;
    }

    void StartGame()
    {
        string seed = getRandomSeed();
        random = new System.Random(seed.GetHashCode());
        update = new List<NodePiece>();
        flipped = new List<FlippedPieces>();
        dead = new List<NodePiece>();
        InitializeBoard();
        VerifyBoard();
        InstantiateBoard();
    }   

    void InitializeBoard()
    {
        board = new Node[width, height];
        for(int y = 0; y < height; y++)
        {
            for(int x = 0; x < width; x++)
            {
                board[x, y] = new Node((boardLayout.rows[y].row[x]) ? -1:fillPiece(), new Point(x, y)) ;
            }
        }
    }

    void VerifyBoard()
    {
        List<int> remove;
        for (int x = 0; x < height; x++)
        {
            for (int y = 0; y < width; y++)
            {
                Point p = new Point(x, y);
                int val = getValueatPoint(p);
                if (val <= 0) continue;

                remove = new List<int>();
                while (isConnected(p, true).Count > 0)
                {
                    val = getValueatPoint(p);
                    if (!remove.Contains(val))
                    {
                        remove.Add(val);
                    }
                    setValueatPoint(p, newValue(ref remove));
                }
            }
        }
    }

    void InstantiateBoard()
    {
        for(int x = 0; x < width; x++)
        {
            for(int y = 0; y < height; y++)
            {
                Node node = getNodeatPoint(new Point(x, y));

                int val = node.value;
                if (val <= 0)
                {
                    continue;
                }
                GameObject p = Instantiate(nodePiece, gameBoard);
                NodePiece piece = p.GetComponent<NodePiece>();
                RectTransform rect = p.GetComponent<RectTransform>();
                rect.anchoredPosition = new Vector2(32 + (64 * x), -32 - (64 * y));
                piece.Initialize(val, new Point(x, y), pieces[val - 1]);
                node.SetPiece(piece);
            }
        }
    }

    public void ResetPiece(NodePiece piece)
    {
        piece.ResetPosition();
        update.Add(piece);
    }
    

    public void FlipPieces(Point one,Point two,bool main)
    {
        if (getValueatPoint(one) < 0) return;

        Node nodeOne = getNodeatPoint(one);
        NodePiece pieceOne = nodeOne.getPiece();
        

        if (getValueatPoint(two) > 0)
        {
            Node nodeTwo = getNodeatPoint(two);
            NodePiece pieceTwo = nodeTwo.getPiece();

            nodeTwo.SetPiece(pieceOne);
            nodeOne.SetPiece(pieceTwo);
            

            if(main)
                flipped.Add(new FlippedPieces(pieceOne, pieceTwo));

            update.Add(pieceOne);
            update.Add(pieceTwo);
            
        }
        else
        {
            ResetPiece(pieceOne);
            
        }
    }

    List<Point> isConnected(Point p, bool main)
    {
        List<Point> connected = new List<Point>();
        int val = getValueatPoint(p);
        Point[] directions =
        {
            Point.up,
            Point.right,
            Point.down,
            Point.left
        };

        foreach(Point dir in directions)//checking if there are two or more of the same shape in any direction
        {
            List<Point> line = new List<Point>();

            int same = 0;
            for(int i = 1; i < 3; i++)
            {
                Point check = Point.add(p, Point.mult(dir, i));
                if (getValueatPoint(check) == val)
                {
                    line.Add(check);
                    same++;
                }
            }

            if (same > 1)
            {
                AddPoints(ref connected, line);//add these points to the connected list
            }
        }

        for(int i = 0; i < 2; i++)
        {
            List<Point> line = new List<Point>();

            int same = 0;
            Point[] check = { Point.add(p, directions[i]), Point.add(p, directions[i + 2]) };
            foreach (Point next in check)
            {
                if (getValueatPoint(next) == val)
                {
                    line.Add(next);
                    same++;
                }
            }

            if (same > 1)
            {
                AddPoints(ref connected, line);
            }
        }

        for(int i = 0; i < 4; i++)
        {
            List<Point> square = new List<Point>();

            int same = 0;
            int next = i + 1;
            if (next >= 4)
            {
                next -= 4;
            }

            Point[] check = { Point.add(p, directions[i]), Point.add(p, directions[next]),Point.add(p, Point.add(directions[i],directions[next])) };
            foreach (Point pnt in check)
            {
                if (getValueatPoint(pnt) == val)
                {
                    square.Add(pnt);
                    same++;
                }
            }

            if (same > 2)
            {
                AddPoints(ref connected, square);
            }
        }

        if (main)
        {
            for (int i = 0; i < connected.Count; i++)
            {
                AddPoints(ref connected,isConnected(connected[i],false));
            }
        }
        

        return connected;
    }

    void AddPoints(ref List<Point> points, List<Point> add)
    {
        foreach (Point p in add)
        {
            bool doAdd = true;
            for (int i = 0; i < points.Count; i++)
            {
                if (points[i].Equals(p))
                {
                    doAdd = false;
                    break;
                }
            }

            if (doAdd) points.Add(p);
        }
    }
    int fillPiece()
    {
        int val = 1;
        val = (random.Next(0, 100) / (100 / pieces.Length))+1;
        return val;
    }

    int getValueatPoint(Point p)
    {
        if (p.x < 0 || p.x >= width || p.y < 0 || p.y >= height) return -1;
        return board[p.x, p.y].value;
    }

    void setValueatPoint(Point p,int v)
    {
        board[p.x, p.y].value = v;
    }

    int newValue(ref List<int> remove)
    {
        List<int> available = new List<int>();
        for(int i = 0; i < pieces.Length; i++)
        {
            available.Add(i + 1);
        }
        foreach(int i in remove)
        {
            available.Remove(i);
        }

        if (available.Count <= 0) return 0;
        return available[random.Next(0, available.Count)];
    }
    
    string getRandomSeed()
    {
        string seed = "";
        string acceptableChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz1234567890";
        for(int i = 0; i < 20; i++)
        {
            seed += acceptableChars[Random.Range(0, acceptableChars.Length)];
        }
        return seed;
    }

    public Vector2 getPointPosition(Point p)
    {
        return new Vector2(32 + (64 * p.x), -32 - (64 * p.y));
    }

    Node getNodeatPoint(Point p)
    {
        return board[p.x, p.y];
    }
}

[System.Serializable]
public class Node
{
    public int value;// 0=blank, 1=cube, 2=sphere, 3=cylinder, 4=pyramid, 5=diamond; -1=hole
    public Point index;
    public NodePiece piece;
    public Node(int v,Point i)
    {
        value = v;
        index = i;
    }

    public void SetPiece(NodePiece p)
    {
        piece = p;
        value = (piece == null) ? 0 : piece.value;
        if (piece == null) 
        { 
            return;
        }
        piece.SetIndex(index);
    }

    public NodePiece getPiece()
    {
        return piece;
    }
}

[System.Serializable]
public class FlippedPieces
{
    public NodePiece one;
    public NodePiece two;

    public FlippedPieces(NodePiece o,NodePiece t)
    {
        one = o;
        two = t;
    }

    public NodePiece getOtherPiece(NodePiece p)
    {
        if (p == one)
            return two;
        else if (p == two)
            return one;
        else
            return null;
    }
}
