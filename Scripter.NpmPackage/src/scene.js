/**
 * The Virt-A-Mate scene root
 */
export class Scene {
    /**
     * Gets one atom in the scene
     * @param {string} id - The atom's name
     * @returns {Atom}
     */
    getAtom(id) {}

    /**
     * Gets a list of all atom IDs in the scene
     * @returns {string[]}
     */
    getAtomIds() {}

    /**
     * Returns an audio clip that was created in the scene audio tab
     * @param {string} type - Either "Embedded" (built-in) or "URL" (custom)
     * @param {string} category - "web" if the type is "URL"
     * @param {string} clip - The audio clip name
     * @returns {AudioClip}
     */
    getAudioClip(type, category, clip) {}
}

/**
 * Represents a Virt-A-Mate atom
 */
export class Atom {
    /**
     * Returns a storable (e.g. a plugin or a native atom component)
     * @param {string} name - The storable name (for plugins, "plugin#0_YourPluginName")
     */
    getStorable(name) {}

    /**
     * Returns a free controller (the node you can select and move around in 3D)
     * @param {string} name - The controller name (e.g. "control" or "head")
     * @returns {Controller}
     */
    getController(name) {}
}

export class Storable {
    /**
     * Calls a trigger
     * @param {string} name 
     * @returns void
     */
    invokeAction(name) {}

    /**
     * Gets a storable float
     * @param {string} name
     * @returns {FloatParamReference}
     */
    getFloat(name) {}

    /**
     * Gets a storable bool
     * @param {string} name
     * @returns {BoolParamReference}
     */
    getBool(name) {}

    /**
     * Gets a storable string
     * @param {string} name
     * @returns {StringParamReference}
     */
    getString(name) {}

    /**
     * Gets a storable string chooser (drop down)
     * @param {string} name
     * @returns {StringChooserParamReference}
     */
    getStringChooser(name) {}

    /**
     * Gets a storable audio action
     * @param {string} name
     * @returns {AudioActionParamReference}
     */
    getAudioAction(name) {}
}

/**
* Represents a JSONStorableFloat in the Scripter plugin
*/
export class FloatParamReference {
   /**
    * @type {number}
    */
   val;

   /**
    * Called when the value is changed
    * @param {function(number): void} callback 
    */
   onChange(callback) {}
}

/**
* Represents a JSONStorableBool in the Scripter plugin
*/
export class BoolParamReference {
   /**
    * @type {boolean}
    */
   val;
       
   /**
    * Called when the value is changed
    * @param {function(boolean): void} callback 
    */
   onChange(callback) {}
}

/**
* Represents a JSONStorableString in the Scripter plugin
*/
export class StringParamReference {
   /**
    * @type {string}
    */
   val;

   /**
    * Called when the value is changed
    * @param {function(boolean): void} callback 
    */
   onChange(callback) {}
}

/**
* Represents a JSONStorableStringChooser in the Scripter plugin
*/
export class StringChooserParamReference {
   /**
    * @type {string}
    */
   val;

   /**
    * Called when the value is changed
    * @param {function(boolean): void} callback 
    */
   onChange(callback) {}
}

/**
* Represents a JSONStorableAudioAction in the Scripter plugin
*/
export class AudioActionParamReference {
   /**
    * Plays the specified audio
    * @param {AudioClip} clip
    */
   play(clip) {}
}

/**
* Represents a JSONStorableAction in the Scripter plugin
*/
export class ActionReference {

}

/**
 * References a Virt-A-Mate audio clip
 */
export class AudioClip {
}

/**
 * A free controller (the node you can move around)
 */
export class Controller {
    /**
     * The distance between two controllers
     * @param {Controller} other 
     * @return number
     */
    distance(other) {}

    /**
     * @param {Controller} target
     * @param {number} maxDistanceDelta
     */
    moveTowards(target, maxDistanceDelta) {}

    /**
     * @param {Controller} target
     * @param {number} maxDegreesDelta
     */
    rotateTowards(target, maxDegreesDelta) {}
}