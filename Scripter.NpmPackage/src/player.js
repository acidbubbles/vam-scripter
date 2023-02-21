/**
 * The user playing Virt-A-Mate
 */
export class Player {
    /**
     * Whether the user is currently using Virt-A-Mate with a VR headset
     * @type boolean
     */
    isVR;

    /**
     * The player's VR hands
     * @type {Transform}
     */
    lHand;

    /**
     * The player's VR hands
     * @type {Transform}
     */
    rHand;

    /**
     * The player's camera (the monitor on Desktop, and the center eye camera in VR)
     * @type {Transform}
     */
    head;
}
