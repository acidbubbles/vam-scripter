// @ts-check

import { Controller } from "./Controller";
import { Storable } from "./Storable";

/**
 * Represents a Virt-A-Mate atom
 */

export class Atom {
   /**
    * The name of this atom
    * @returns {string}
    */
   get name() { return ""; }

   /**
    * The type of this atom
    * @returns {string}
    */
   get type() { return ""; }

   /**
    * Whether the atom is on or off
    */
   on;

   /**
    * Returns the list of all storable IDs
    * @returns {string[]}
    */
   getStorableIds() { return []; }

   /**
    * Returns a storable (e.g. a plugin or a native atom component)
    * @param {string} name - The storable name (for plugins, "plugin#0_YourPluginName")
    * @returns {Storable}
    */
   getStorable(name) { return new Storable(); }

   /**
    * Returns a free controller (the node you can select and move around in 3D)
    * @param {string} name - The controller name (e.g. "control" or "head")
    * @returns {Controller}
    */
   getController(name) { return new Controller(); }
}
