import { Console } from "./Console";
import { Coroutine } from "./Coroutine";

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
