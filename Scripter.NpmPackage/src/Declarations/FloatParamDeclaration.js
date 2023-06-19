/**
 * Represents a JSONStorableFloat in the Scripter plugin
 */

export class FloatParamDeclaration {
    /**
    * Updates the value
     * @type {number}
     */
    val;

    /**
    * Updates the value without invoking onChange
     * @type {number}
     */
    valNoCallback;

    /**
     * Called when the value is changed
     * @param {function(number): void} callback
     */
    onChange(callback) { }
}
