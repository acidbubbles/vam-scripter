// @ts-check

/**
 * Subset of the JavaScript Math object. Uses the Unity Mathf implementations.
 */
export class Mathf {
    /**
     * @param {number} value
     * @returns {number}
     */
    abs(value) { return Math.abs(value); }

    /**
     * @param {number} value
     * @returns {number}
     */
    acos(value) { return Math.acos(value); }

    /**
     * @param {number} value
     * @returns {number}
     */
    asin(value) { return Math.asin(value); }

    /**
     * @param {number} value
     * @returns {number}
     */
    atan(value) { return Math.atan(value); }

    /**
     * @param {number} y
     * @param {number} x
     * @returns {number}
     */
    atan2(y, x) { return Math.atan2(y, x); }

    /**
     * @param {number} value
     * @returns {number}
     */
    ceil(value) { return Math.ceil(value); }

    /**
     * @param {number} value
     * @returns {number}
     */
    cos(value) { return Math.cos(value); }

    /**
     * @param {number} value
     * @returns {number}
     */
    exp(value) { return Math.exp(value); }

    /**
     * @param {number} value
     * @returns {number}
     */
    floor(value) { return Math.floor(value); }

    /**
     * @param {number} value
     * @returns {number}
     */
    log(value) { return Math.log(value); }

    /**
     * @param {number} value
     * @returns {number}
     */
    log10(value) { return Math.log10(value); }

    /**
     * @param {number} value
     * @returns {number}
     */
    max(value) { return Math.max(value); }

    /**
     * @param {number} value
     * @returns {number}
     */
    min(value) { return Math.min(value); }

    /**
     * @param {number} x
     * @param {number} y
     * @returns {number}
     */
    pow(x, y) { return Math.pow(x, y); }

    /**
     * Returns a random number between 0 and 1. Uses the Unity Random.value implementation.
     * @returns {number}
     */
    random() { return Math.random(); }

    /**
     * @param {number} value
     * @returns {number}
     */
    round(value) { return Math.round(value); }

    /**
     * @param {number} value
     * @returns {number}
     */
    sign(value) { return Math.sign(value); }

    /**
     * @param {number} value
     * @returns {number}
     */
    sin(value) { return Math.sin(value); }

    /**
     * @param {number} value
     * @returns {number}
     */
    sqrt(value) { return Math.sqrt(value); }

    /**
     * @param {number} value
     * @returns {number}
     */
    tan(value) { return Math.tan(value); }
}