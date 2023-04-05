// @ts-check

/**
 * An object in 3D space that have a position and rotation
 */

export class Vector3 {
   /**
    * 
    * @type number - Vector3.x
    */
   x;

   /**
    * 
    * @type number - Vector3.y
    */
   y;

   /**
    * 
    * @type number - Vector3.z
    */
   z;

   /**
    * Changes the x, y, z components of this vector to the given values.
   * @param {number | Vector3} [x] - The x coordinate or another Vector3.
   * @param {number} [y] - The y coordinate. Only used if 'x' and 'z' are also provided.
   * @param {number} [z] - The z coordinate. Only used if 'x' and 'y' are also provided.
   */
   set(x, y, z) {}

   /**
    * The distance between two Vector3 positions
    * @param {Vector3} other
    * @return number
    */
   distance(other) { return 0; }
}
