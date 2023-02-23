import { Console } from "./Console";
import { Math } from "./Math";

/**
 * @global
 */
export const console = new Console();

/**
 * @global
 */
export const Math = new Math();

/**
 * Equivalent of {@link https://docs.unity3d.com/ScriptReference/MonoBehaviour.Invoke.html|Unity: Invoke}
 * @global
 * @param {function(): void} fn The function to call after the delay
 * @param {number} delay - Duration in milliseconds before calling fn
 * @returns {SetTimeoutToken}
 */
export function setTimeout(fn, delay){}

/**
 * Equivalent of {@link https://docs.unity3d.com/ScriptReference/MonoBehaviour.CancelInvoke.html|Unity: CancelInvoke}
 * @global
 * @param {SetTimeoutToken} token 
 */
export function clearTimeout(token){}

/**
 * Allows canceling a setTimeout function
 */
class SetTimeoutToken {
}
