import { Coroutine } from "./coroutine";

/**
 * Emulation for the JavaScript console
 */
class Console {
    /**
     * Writes to the Scripter log window.
     * @param {...*} message
     */
    log() {}

    /**
     * Writes in red to the Scripter log window. If the log window is not visible, errors will be sent to the Virt-A-Mate errors window.
     * @param {...*} message
     */
    error() {}

    /**
     * Clears the Scripter log window.
     */
    clear() {}
}

/**
 * @global
 */
export const console = new Console();

/**
 * Equivalent of {@link https://docs.unity3d.com/ScriptReference/MonoBehaviour.Invoke.html|Unity: Invoke}
 * @global
 * @param {function(): void} fn The function to call after the delay
 * @param {number} delay - Duration in milliseconds before calling fn
 * @returns {Coroutine}
 */
export function setTimeout(fn, delay){}

/**
 * Equivalent of {@link https://docs.unity3d.com/ScriptReference/MonoBehaviour.CancelInvoke.html|Unity: CancelInvoke}
 * @global
 * @param {Coroutine} token 
 */
export function clearTimeout(token){}
