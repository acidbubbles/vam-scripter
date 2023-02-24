// @ts-check

/**
 * Represents a Unity coroutine
 */
export class Coroutine {

}

export class CoroutineIterator {
    /**
     * Stop calling the function
     * @returns {number} - Instruction internal code for the Coroutine
     */
    get stop() { return 0; }

    /**
     * Call the function next frame
     * @returns {number} - Instruction internal code for the Coroutine
     */
    get nextFrame() { return 0; }

    /**
     * Call the function again in N seconds
     * @param {number} seconds 
     * @returns {number} - Instruction internal code for the Coroutine
     */
    waitForSeconds(seconds) { return 0; }
}
