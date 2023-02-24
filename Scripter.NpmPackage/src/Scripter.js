// @ts-check

import { Coroutine, CoroutineIterator } from "./coroutine";

import { ActionDeclaration } from "./Declarations/ActionDeclaration";
import { BoolParamDeclaration } from "./Declarations/BoolParamDeclaration";
import { FloatParamDeclaration } from "./Declarations/FloatParamDeclaration";
import { StringParamDeclaration } from "./Declarations/StringParamDeclaration";

/**
 * The Scripter plugin
 */
export class Scripter {
    /**
     * @param {Object} args
     * @param {string} args.name - The name of the JSON Storable Param
     * @param {number} [args.default=0] - The default value
     * @param {number} [args.min=0] - The minimum value
     * @param {number} [args.max=1] - The maximum value
     * @param {boolean} [args.constrain=true] - Whether the value can exceed the min/max settings
     * @param {function(function(number): void): void} args.onChange - Callback when the value changes
     * @returns {FloatParamDeclaration}
     */
    declareFloatParam(args) { return new FloatParamDeclaration(); }

    /**
     * @param {Object} args
     * @param {string} args.name - The name of the JSON Storable Param
     * @param {boolean} [args.default=false] - The default value
     * @param {function(function(boolean): void): void} args.onChange - Callback when the value changes
     * @returns {BoolParamDeclaration}
     */
    declareBoolParam(args) { return new BoolParamDeclaration(); }

    /**
     * @param {Object} args
     * @param {string} args.name - The name of the JSON Storable Param
     * @param {string} [args.default=""] - The default value
     * @param {function(function(string): void): void} args.onChange - Callback when the value changes
     * @returns {StringParamDeclaration}
     */
    declareStringParam(args) { return new StringParamDeclaration(); }

    /**
     * @param {string} name
     * @param {function(function(): void): void} callback
     * @returns {ActionDeclaration}
     */
    declareAction(name, callback) { return new ActionDeclaration(); }

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
     * @param {function(CoroutineIterator): Coroutine} fn
     * @returns {Coroutine}
     */
    startCoroutine(fn) { return new Coroutine(); }

    /**
     * Stops an existing coroutine
     * @param {Coroutine} co
     * @returns {void}
     */
    stopCoroutine(co) {}
}
