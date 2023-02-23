export class Storable {
   /**
    * Get the name of all params of all types
    * @returns {string[]}
    */
   getAllParamNames() {}

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
    * Gets a storable url
    * @param {string} name
    * @returns {UrlParamReference}
    */
   getUrl(name) { }

   /**
    * Gets a storable audio action
    * @param {string} name
    * @returns {AudioActionParamReference}
    */
   getAudioAction(name) { }
}
