using UnityEngine;

public class BoardBounds
{
    public int highestX;
    public int highestY;
    public int lowestX;
    public int lowestY;
    
    public Coordinates topRightCornerCoordinates;
    public Coordinates topLeftCornerCoordinates;
    public Coordinates bottomLeftCornerCoordinates;
    public Coordinates bottomRightCornerCoordinates;
    
    public Vector3 topRightCornerPosition;
    public Vector3 topLeftCornerPosition;
    public Vector3 bottomLeftCornerPosition;
    public Vector3 bottomRightCornerPosition;
    
    public BoardBounds(int highestX, int highestY, int lowestX, int lowestY)
    {
        this.highestX = highestX;
        this.highestY = highestY;
        this.lowestX = lowestX;
        this.lowestY = lowestY;
        
        topRightCornerCoordinates = new Coordinates(highestX, highestY);
        topLeftCornerCoordinates = new Coordinates(lowestX, highestY);
        bottomLeftCornerCoordinates = new Coordinates(lowestX, lowestY);
        bottomRightCornerCoordinates = new Coordinates(highestX, lowestY);

        topRightCornerPosition = topRightCornerCoordinates.ToVector3();
        topLeftCornerPosition = topLeftCornerCoordinates.ToVector3();
        bottomLeftCornerPosition = bottomLeftCornerCoordinates.ToVector3();
        bottomRightCornerPosition = bottomRightCornerCoordinates.ToVector3();
    }

    public override string ToString()
    {
        return $"Highest X: {highestX}, Highest Y: {highestY}, Lowest X: {lowestX}, Lowest Y: {lowestY}\n" +
               $"Top Right Corner Coordinates: {topRightCornerCoordinates}, Top Left Corner Coordinates: {topLeftCornerCoordinates}, Bottom Right Corner Coordinates: {bottomRightCornerCoordinates}, Bottom Left Corner Coordinates: {bottomLeftCornerCoordinates}\n" +
               $"Top Right Corner Position: {topRightCornerPosition}, Top Left Corner Position {topLeftCornerPosition}, Bottom Right Corner Position: {bottomRightCornerPosition}, Bottom Left Corner Position: {bottomLeftCornerPosition}";
    }
}