/**
* Represents a JSONStorableStringChooser in the Scripter plugin
*/

export class StringChooserParamReference {
   /**
    * Updates the value
    * @type {string}
    */
   val;

    /**
    * Updates the value without invoking onChange
     * @type {string}
     */
    valNoCallback;

   /**
    * The available choices for val
    * @returns {string[]}
    */
   get choices() {}
}
