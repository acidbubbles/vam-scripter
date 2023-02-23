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
    * Gets a storable audio action
    * @param {string} name
    * @returns {AudioActionParamReference}
    */
   getAudioClipAction(name) { }

   /**
    * Gets a storable float
    * @param {string} name
    * @returns {FloatParamReference}
    */
   getFloatParam(name) { }

   /**
    * Gets a storable bool
    * @param {string} name
    * @returns {BoolParamReference}
    */
   getBoolParam(name) { }

   /**
    * Gets a storable string
    * @param {string} name
    * @returns {StringParamReference}
    */
   getStringParam(name) { }

   /**
    * Gets a storable string chooser (drop down)
    * @param {string} name
    * @returns {StringChooserParamReference}
    */
   getStringChooserParam(name) { }

   /**
    * Gets a storable url
    * @param {string} name
    * @returns {UrlParamReference}
    */
   getUrlParam(name) { }
}
