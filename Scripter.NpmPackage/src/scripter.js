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
     * @param {FloatParamDeclaration~onChange} onChange
     * @returns {FloatParamDeclaration}
     */
    declareFloatParam(args) {}

    /**
     * @param {Object} args
     * @param {string} args.name - The name of the JSON Storable Param
     * @param {boolean} args.default - The default value
     * @param {BoolParamDeclaration~onChange} args.onChange
     * @returns {BoolParamDeclaration}
     */
    declareBoolParam(args) {}

    /**
     * @param {Object} args
     * @param {string} args.name - The name of the JSON Storable Param
     * @param {string} args.default - The default value
     * @param {StringParamDeclaration~onChange} args.onChange
     * @returns {StringParamDeclaration}
     */
    declareStringParam(args) {}

    /**
     * @param {string} name
     * @param {ActionDeclaration~onTrigger} callback
     * @returns {ActionDeclaration}
     */
    declareAction(name, callback) {}

    /**
     * @returns void
     */
    onUpdate() {};

    /**
     * @returns void
     */
    onFixedUpdate() {};
}

/**
 * @callback FloatParamDeclaration~onChange
 * @param {number} val
 */
export class FloatParamDeclaration {
    /**
     * @type {float}
     */
    get val() {}

    /**
     * @type {float}
     */
    set val(v) {}
}

/**
 * @callback BoolParamDeclaration~onChange
 * @param {boolean} val
 */
export class BoolParamDeclaration {

}

/**
 * @callback StringParamDeclaration~onChange
 * @param {string} val
 */
export class StringParamDeclaration {

}

/**
 * @callback ActionDeclaration~onTrigger
 */
export class ActionDeclaration {

}