// @ts-check

/**
 * Subset of the JavaScript Math object and Unity Mathf static functions. Uses the Unity Mathf implementations.
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
     * Clamps the given value between the given minimum float and maximum float values. Returns the given value if it is within the minimum and maximum range.
     * @param {number} value
     * @param {number} min
     * @param {number} max
     * @returns {number}
     */
    clamp(value,min,max) { return Number(); }

    /**
     * Clamps the given value between 0 and 1. Returns the given value if it is between 0 and 1.
     * @param {number} value
     * @returns {number}
     */
    clamp01(value) { return Number(); }

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
     * Calculates the linear parameter t that produces the interpolant value within the range [a, b], see https://docs.unity3d.com/2018.1/Documentation/ScriptReference/Mathf.InverseLerp.html
     * @param {number} value
     * @param {number} a
     * @param {number} b
     * @returns {number} t
     */
    inverseLerp(a,b,value) { return Number(); }

    /**
     * Linearly interpolates between a and b by t.
     * @param {number} a
     * @param {number} b
     * @param {number} t
     * @returns {number}
     */
     lerp(a,b,t) { return Number(); }

     /**
     * Same as Lerp but makes sure the values interpolate correctly when they wrap around 360 degrees.
     * @param {number} a
     * @param {number} b
     * @param {number} t
     * @returns {number}
     */
     lerpAngle(a,b,t) { return Number(); }

     /**
     * 	Linearly interpolates between a and b by t with no limit to t.
     * @param {number} a
     * @param {number} b
     * @param {number} t
     * @returns {number}
     */
     lerpUnclamped(a,b,t) { return Number(); }

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
     * PingPongs the value t, so that it is never larger than length and never smaller than 0.
     * @param {number} t
     * @param {number} length
     * @returns {number}
     */
    pingPong(t, length) { return Number(); }

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
     * Interpolates between min and max with smoothing at the limits.
     * @param {number} min
     * @param {number} max
     * @param {number} t
     * @returns {number}
     */
    smoothStep(min,max,t) { return Number(); }

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