export class Storable {
   /**
    * Calls a trigger
    * @param {string} name
    * @returns {void}
    */
   invokeAction(name) { }

   /**
    * Gets a storable float
    * @param {string} name
    * @returns {FloatParamReference}
    */
   getFloat(name) { }

   /**
    * Gets a storable bool
    * @param {string} name
    * @returns {BoolParamReference}
    */
   getBool(name) { }

   /**
    * Gets a storable string
    * @param {string} name
    * @returns {StringParamReference}
    */
   getString(name) { }

   /**
    * Gets a storable string chooser (drop down)
    * @param {string} name
    * @returns {StringChooserParamReference}
    */
   getStringChooser(name) { }

   /**
    * Gets a storable audio action
    * @param {string} name
    * @returns {AudioActionParamReference}
    */
   getAudioAction(name) { }
}
