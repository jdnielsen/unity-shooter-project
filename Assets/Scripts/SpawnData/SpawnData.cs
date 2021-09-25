using System.Collections;
using System.Collections.Generic;

public class SpawnData
{
    public float minX;
    public float maxX;
    public float minY;
    public float maxY;
    public float rotationAngle;

    public SpawnData(float minX = -8f, float maxX = 8f, float angle = 0f, float minY = 7f, float maxY = 7f)
    {
        this.minX = minX;
        this.maxX = maxX;
        this.minY = minY;
        this.maxY = maxY;
        this.rotationAngle = angle;
    }
}
