// @ts-check

/**
 * Emulation for the JavaScript console
 */
export class Console {
    /**
     * Writes to the Scripter log window.
     * @param {...*} message
     * @returns {void}
     */
    log(message) { }

    /**
     * Writes in red to the Scripter log window. If the log window is not visible, errors will be sent to the Virt-A-Mate errors window.
     * @param {...*} message
     * @returns {void}
     */
    error(message) { }

    /**
     * Clears the Scripter log window.
     * @returns {void}
     */
    clear() { }
}
