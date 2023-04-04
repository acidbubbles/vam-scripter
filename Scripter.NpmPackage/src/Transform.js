// @ts-check

import { Vector3 } from "./Vector3";

/**
 * An object in 3D space that have a position and rotation
 */

export class Transform {
   /**
    * The world position
    * @type {Vector3}
    */
   position;

   /**
    * The local position
    * @type {Vector3}
    */
   localPosition;

   /**
    * The world eulerAngles
    * @type {Vector3}
    */
   eulerAngles;

   /**
    * The local eulerAngles
    * @type {Vector3}
    */
   localEulerAngles;

   /**
    * The distance between two transforms
    * @param {Transform} other
    * @return number
    */
   distance(other) { return 0; }
}
