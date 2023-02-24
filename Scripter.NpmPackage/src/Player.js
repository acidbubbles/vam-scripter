// @ts-check

import { Transform } from "./Transform";

/**
 * The user playing Virt-A-Mate
 */
export class Player {
    /**
     * Whether the user is currently using Virt-A-Mate with a VR headset
     * @type boolean
     */
    get isVR() { return true; }

    /**
     * The player's VR hands
     * @type {Transform}
     */
    get lHand() { return new Transform(); }

    /**
     * The player's VR hands
     * @type {Transform}
     */
    get rHand() { return new Transform(); }

    /**
     * The player's camera (the monitor on Desktop, and the center eye camera in VR)
     * @type {Transform}
     */
    get head() { return new Transform(); }
}
