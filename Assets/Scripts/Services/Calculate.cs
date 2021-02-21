using Unity.Mathematics;

namespace PetOne.Services
{
    internal static class Calculate
    {
        /// <summary>
        /// OpenGL realisation http://www.opengl-tutorial.org/ru/intermediate-tutorials/tutorial-17-quaternions/
        /// </summary>
        public static quaternion FromToRotation(float3 from, float3 to)
        {
            from = math.normalize(from);
            to = math.normalize(to);

            float dotProduct = math.dot(from, to); //= cos of angle between from and to
            float3 rotationAxis;

            if (dotProduct < -1 + 0.001f)
            {
                /*special case when vectors in opposite directions: 
                there is no "ideal" rotation axis So guess one;
                any will do as long as it's perpendicular to start*/
                rotationAxis = math.cross(math.up(), from);

                if (math.lengthsq(rotationAxis) < 0.01f) // bad luck, they were parallel, try again!
                    rotationAxis = math.cross(math.right(), from);

                rotationAxis = math.normalize(rotationAxis);
                return quaternion.AxisAngle(rotationAxis, math.radians(180f));
            }

            rotationAxis = math.cross(from, to);
            float sqrt = math.sqrt((1 + dotProduct) * 2);
            return new quaternion(rotationAxis.x / sqrt, rotationAxis.y / sqrt, rotationAxis.z / sqrt, sqrt * 0.5f);
        }
    }
}