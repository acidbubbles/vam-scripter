// @ts-check

/**
 * Integration with AcidBubbles Keybindings (see {@link https://hub.virtamate.com/resources/keybindings.4400/|Keybindings on Virt-A-Mate Hub})
 */
export class Keybindings {
    /**
     * Invokes a Keybinding command, if it exists
     * @returns {void}
     */
    invokeCommand() {}

    /**
     * Creates a Keybindings command that can be bound to a key and invoked
     * @param {string} commandName - The command that you wanto to invoke; will be prefixed by "Scripter."
     * @returns {KeybindingsDeclaration}
     */
    declareCommand(commandName, callback) {}
}

