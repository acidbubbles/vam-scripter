
/**
 * The Virt-A-Mate scene root
 */
export class Scene {
    /**
     * Gets one atom in the scene
     * @param {string} id - The atom's name
     * @returns {Atom}
     */
    getAtom(id) {}
    
    /**
     * Gets a list of all atoms in the scene
     * @returns {Atom[]}
     */
    getAtoms() {}

    /**
     * Gets a list of all atom IDs in the scene
     * @returns {string[]}
     */
    getAtomIds() {}

    /**
     * Returns an audio clip that was created in the scene audio tab
     * @param {string} type - Either "Embedded" (built-in) or "URL" (custom)
     * @param {string} category - "web" if the type is "URL"
     * @param {string} clip - The audio clip name
     * @returns {AudioClip}
     */
    getAudioClip(type, category, clip) {}
}
