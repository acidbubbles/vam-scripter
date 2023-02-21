/**
 * The Scripter plugin
 */
export class Scripter {
    /**
     * @param {Object} args
     * @param {string} args.name - The name of the JSON Storable Param
     * @param {number} args.default - The default value
     * @param {number} args.min - The minimum value
     * @param {number} args.max - The maximum value
     * @param {boolean} args.constrain - Whether the value can exceed the min/max settings
     * @param {function(function(number): void): void} onChange
     * @returns {FloatParamDeclaration}
     */
    declareFloatParam(args) {}

    /**
     * @param {Object} args
     * @param {string} args.name - The name of the JSON Storable Param
     * @param {boolean} args.default - The default value
     * @param {function(function(boolean): void): void} args.onChange
     * @returns {BoolParamDeclaration}
     */
    declareBoolParam(args) {}

    /**
     * @param {Object} args
     * @param {string} args.name - The name of the JSON Storable Param
     * @param {string} args.default - The default value
     * @param {function(function(string): void): void} args.onChange
     * @returns {StringParamDeclaration}
     */
    declareStringParam(args) {}

    /**
     * @param {string} name
     * @param {function(function(): void): void} callback
     * @returns {ActionDeclaration}
     */
    declareAction(name, callback) {}

    /**
     * Called every frame
     * @returns {void}
     */
    onUpdate() {};

    /**
     * Called every physics frame
     * @returns {void}
     */
    onFixedUpdate() {};

    /**
     * Starts a Unity coroutine
     * @param {function(iterator: CoroutineIterator): Coroutine} fn
     * @return {Coroutine}
     */
    startCoroutine(fn) {}

    /**
     * Stops an existing coroutine
     * @param {Coroutine} co
     * @return {void}
     */
    stopCoroutine(co) {}
}
