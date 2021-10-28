using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace it.dhlworks.minicliptest {
    public static class Utils {
        /// <summary>
        /// Converts a whole Color32 image to its byte array representation.
        /// </summary>
        /// <returns>The byte array representation of the image.</returns>
        public static byte[] ToByteArray(this Color32[] self) {
            byte[] result = new byte[self.Length * 3 * sizeof(float)];
            int idx = 0;
            int offset = sizeof(float);
            for (int i = 0; i < self.Length; i++) {
                byte[] r = BitConverter.GetBytes(self[i].r);
                byte[] g = BitConverter.GetBytes(self[i].g);
                byte[] b = BitConverter.GetBytes(self[i].b);
                Array.Copy(r, 0, result, idx, r.Length);
                idx += offset;
                Array.Copy(g, 0, result, idx, g.Length);
                idx += offset;
                Array.Copy(b, 0, result, idx, b.Length);
                idx += offset;
            }

            return result;
        }

        /// <summary>
        /// Checks if values of a vector2 are between some max and mins
        /// </summary>
        /// <param name="self">The vector2 itself</param>
        /// <param name="MinX">minimum x</param>
        /// <param name="MaxX">maximum x</param>
        /// <param name="MinY">minimum y</param>
        /// <param name="MaxY">maximum y</param>
        /// <returns>True if it is between, false otherwise</returns>
        public static bool IsBetween(this Vector2 self, float MinX, float MaxX, float MinY, float MaxY) {
            if (self.x > MinX && self.x < MaxX && self.y > MinY && self.y < MaxY)
                return true;
            return false;
        }

    }
}