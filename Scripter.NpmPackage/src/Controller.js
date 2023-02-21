import { Transform } from "./Transform";

/**
 * A free controller (the node you can move around)
 */

export class Controller extends Transform {
   /**
    * @param {Transform} target
    * @param {number} maxDistanceDelta
    */
   moveTowards(target, maxDistanceDelta) { }

   /**
    * @param {Transform} target
    * @param {number} maxDegreesDelta
    */
   rotateTowards(target, maxDegreesDelta) { }
}
