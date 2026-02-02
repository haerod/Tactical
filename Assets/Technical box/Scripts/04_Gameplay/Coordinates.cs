using UnityEngine;

[System.Serializable]
public class Coordinates
{
    public int x;
    public int y;
    
    public Coordinates(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    public override string ToString() => $"({x}, {y})";
    public static bool operator ==(Coordinates a, Coordinates b)
    {
        if (a is null)
        {
            if (b is null)
                return true;

            return false;
        }
        
        return a.Equals(b);
    }
    public static bool operator !=(Coordinates a, Coordinates b) => !(a == b);
    public static Coordinates operator  +(Coordinates a, Coordinates b) => new Coordinates(a.x + b.x, a.y + b.y);
    public static Coordinates operator -(Coordinates a, Coordinates b) => new Coordinates(a.x - b.x, a.y - b.y);
    public override int GetHashCode() => base.GetHashCode();
    public override bool Equals(object obj) => Equals(obj as Coordinates);

    private bool Equals(Coordinates c)
    {
        if (c is null)
            return false;
        
        if(ReferenceEquals(this, c))
            return true;
        
        if(GetType() != c.GetType())
            return false;
        
        return x == c.x && y == c.y;
    }
    
    public void SetCoordinates(Coordinates newCoordinates)
    {
        this.x = newCoordinates.x;
        this.y = newCoordinates.y;
    }
    
    public Vector3 ToVector3() => new Vector3(x,0,y);
    
    public Vector2 ToVector2() => new Vector2(x,y);
}