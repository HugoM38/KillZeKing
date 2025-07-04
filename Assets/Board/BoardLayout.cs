using System;

[Serializable]
public class BoardLayout
{
    public Row[] rows;
}

[Serializable]
public class Row
{
    public TileType[] tiles;
}
