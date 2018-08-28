using System;

[Serializable]
public class SensorCoordinates {
    public string name;
    public int[] valid_begin;
    public int[] valid_end;
    public float[] x;
    public float[] y;
    public float[] z;
}
