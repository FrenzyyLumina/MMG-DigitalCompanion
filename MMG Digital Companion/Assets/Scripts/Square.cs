using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Square
{
    private int[] coordinate;
    private GameEnums.SquareType type;
    private bool isCompleted;

    public Square(int x, int y, GameEnums.SquareType type)
    {
        coordinate = new int[2] { x, y };
        isCompleted = false;
        this.type = type;
    }

    public bool hasCoordinate(int x, int y)
    {
        return coordinate[0] == x && coordinate[1] == y;
    }

    public bool getIsCompleted()
    {
        return isCompleted;
    }

    public void setIsCompleted(bool val)
    {
        isCompleted = val;
    }

    public GameEnums.SquareType getType()
    {
        return type;
    }
    public void setType(GameEnums.SquareType type)
    {
        this.type = type;
    }
}
