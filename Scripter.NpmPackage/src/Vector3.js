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
    * @param {Vector3} other
    */
   set(other) {}

   /**
    * Changes the x, y, z components of this vector to the given values.
    * @param {number} x
    */
   set(x, y, z) {}

   /**
    * The distance between two Vector3 positions
    * @param {Vector3} other
    * @return number
    */
   distance(other) { return 0; }
}
