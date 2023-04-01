// @ts-check

/**
 * Integration with Unity's Input (see {@link https://docs.unity3d.com/ScriptReference/Input.html|Unity: Input})
 */
export class InputClass {
    /**
     * Binds a function to key down
     * @param {string} key - The name of the key to bind
     * @param {function(function(): void): void} fn - The function to call
     * @returns {void}
     */
    onKeyDown(key, fn) {}

    /**
     * Binds a function to key up
     * @param {string} key - The name of the key to bind
     * @param {function(function(): void): void} fn - The function to call
     * @returns {void}
     */
    onKeyUp(key, fn) {}
}

