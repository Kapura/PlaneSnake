[System.Serializable]
public struct Point3 {
    public int x;
    public int y;
    public int z;

    public Point3(int x, int y, int z) {
        this.x = x;
        this.y = y;
        this.z = z;
    }

    public override bool Equals(object obj) {
        if (!(obj is Point3)) {
            return false;
        }

        return this.Equals((Point3)obj);
    }

    public bool Equals(Point3 p) {
        return (x == p.x) && (y == p.y) && (z == p.z);
    }

    public static bool operator ==(Point3 p1, Point3 p2) {
        return p1.Equals(p2);
    }

    public static bool operator !=(Point3 p1, Point3 p2) {
        return !(p1 == p2);
    }

    public static Point3 operator +(Point3 p1, Point3 p2) {
        return new Point3(p1.x + p2.x, p1.y + p2.y, p1.z + p2.z);
    }

    public static Point3 operator -(Point3 p1, Point3 p2) {
        return new Point3(p1.x - p2.x, p1.y - p2.y, p1.z - p2.z);
    }

    public static Point3 operator /(Point3 p, int f) {
        return new Point3(p.x / f, p.y / f, p.z / f);
    }

    public override int GetHashCode() {
        return x ^ y ^ z;
    }

    public override string ToString() {
        return "Point3: (" + x + ", " + y + ", " + z + ")";
    }

    public static Point3 Zero {
        get { return new Point3(0, 0, 0); }
    }
    
}

