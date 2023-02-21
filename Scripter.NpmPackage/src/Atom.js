/**
 * Represents a Virt-A-Mate atom
 */

export class Atom {
   /**
    * The name of this atom
    * @type {string}
    */
   get name() { }

   /**
    * The type of this atom
    * @type {string}
    */
   get type() { }

   /**
    * Whether the atom is on or off
    */
   on;

   /**
    * Returns the list of all storable IDs
    * @returns {string[]}
    */
   getStorableIds() {}

   /**
    * Returns a storable (e.g. a plugin or a native atom component)
    * @param {string} name - The storable name (for plugins, "plugin#0_YourPluginName")
    * @returns {Storable}
    */
   getStorable(name) { }

   /**
    * Returns a free controller (the node you can select and move around in 3D)
    * @param {string} name - The controller name (e.g. "control" or "head")
    * @returns {Controller}
    */
   getController(name) { }
}
