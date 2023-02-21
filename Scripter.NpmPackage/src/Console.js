/**
 * Emulation for the JavaScript console
 */
export class Console {
    /**
     * Writes to the Scripter log window.
     * @param {...*} message
     */
    log() { }

    /**
     * Writes in red to the Scripter log window. If the log window is not visible, errors will be sent to the Virt-A-Mate errors window.
     * @param {...*} message
     */
    error() { }

    /**
     * Clears the Scripter log window.
     */
    clear() { }
}
