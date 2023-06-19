/**
 * Represents a JSONStorableString in the Scripter plugin
 */

export class StringParamDeclaration {
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
     * Called when the value is changed
     * @param {function(boolean): void} callback
     */
    onChange(callback) { }
}
