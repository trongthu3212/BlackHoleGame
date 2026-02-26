namespace BlackHole.Utilities
{
    [System.Serializable]
    public struct JsonFriendlyVector3
    {
        public float x;
        public float y;
        public float z;

        public JsonFriendlyVector3(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }
        
        public JsonFriendlyVector3(UnityEngine.Vector3 vector)
        {
            x = vector.x;
            y = vector.y;
            z = vector.z;
        }

        public readonly UnityEngine.Vector3 ToVector3()
        {
            return new UnityEngine.Vector3(x, y, z);
        }
        
        // Add implicit conversion from Vector3 to JsonFriendlyVector3
        public static implicit operator JsonFriendlyVector3(UnityEngine.Vector3 vector)
        {
            return new JsonFriendlyVector3(vector);
        }
    }
    
    [System.Serializable]
    public struct JsonFriendlyVector2
    {
        public float x;
        public float y;

        public JsonFriendlyVector2(float x, float y)
        {
            this.x = x;
            this.y = y;
        }
        
        public JsonFriendlyVector2(UnityEngine.Vector2 vector)
        {
            x = vector.x;
            y = vector.y;
        }

        public UnityEngine.Vector2 ToVector2()
        {
            return new UnityEngine.Vector2(x, y);
        }
        
        // Add implicit conversion from Vector2 to JsonFriendlyVector2
        public static implicit operator JsonFriendlyVector2(UnityEngine.Vector2 vector)
        {
            return new JsonFriendlyVector2(vector);
        }
    }

    [System.Serializable]
    public struct JsonFriendlyRectInt
    {
        public int x;
        public int y;
        public int width;
        public int height;

        public JsonFriendlyRectInt(int x, int y, int width, int height)
        {
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
        }
        
        public JsonFriendlyRectInt(UnityEngine.RectInt rect)
        {
            x = rect.x;
            y = rect.y;
            width = rect.width;
            height = rect.height;
        }

        public UnityEngine.RectInt ToRectInt()
        {
            return new UnityEngine.RectInt(x, y, width, height);
        }
        
        // Add implicit conversion from RectInt to JsonFriendlyRectInt
        public static implicit operator JsonFriendlyRectInt(UnityEngine.RectInt rect)
        {
            return new JsonFriendlyRectInt(rect);
        }
    }
}