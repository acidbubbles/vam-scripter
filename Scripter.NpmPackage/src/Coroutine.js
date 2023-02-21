/**
 * Represents a Unity coroutine
 */
export class Coroutine {

}

export class CoroutineIterator {
    /**
     * Stop calling the function
     */
    stop;

    /**
     * Call the function next frame
     */
    nextFrame;

    /**
     * Call the function again in N seconds
     * @param {number} seconds 
     */
    waitForSeconds(seconds) {}
}