/**
 * Represents a JSONStorableBool in the Scripter plugin
 */

export class BoolParamDeclaration {
    /**
    * Updates the value
     * @type {boolean}
     */
    val;

    /**
    * Updates the value without invoking onChange
     * @type {boolean}
     */
    valNoCallback;

    /**
     * Called when the value is changed
     * @param {function(boolean): void} callback
     */
    onChange(callback) { }
}
