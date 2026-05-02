using UnityEngine;

public static class VectorExtensions
    {
        public static Vector3 Set(this ref Vector3 pos, Vector3 newPos)
        {
            pos = newPos;
            return pos;
        }
        
        public static Vector3 SetX(this ref Vector3 pos, float newX)
        {
            pos.x = newX;
            return pos;
        }
        
        public static Vector3 SetY(this ref Vector3 pos, float newY)
        {
            pos.y = newY;
            return pos;
        }
        
        public static Vector3 SetZ(this ref Vector3 pos, float newZ)
        {
            pos.z = newZ;
            return pos;
        }
        
        public static Vector3 Reset(this ref Vector3 pos)
        {
            pos = Vector3.zero;
            return pos;
        }

        public static bool IsNearDistance(Vector3 startPos, Vector3 endPos, float targetDistance)
        {
            Vector2 pointA = new Vector2(startPos.x, startPos.z);
            Vector2 pointB = new Vector2(endPos.x, endPos.z);

            if(Vector2.Distance(pointA, pointB) <= targetDistance)
            {
                return true;
            }

            return false;
        }
    }