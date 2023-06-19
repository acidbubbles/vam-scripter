// @ts-check

/**
 * Functions to read and write files
 */
export class FileSystem {
    /**
     * Creates and writes to a file that will only be available for this specific scene
     * @param {string} path - The file path
     * @param {string} content - The content to write
     */
    writeSceneFileSync(path, content) {}
    
    /**
     * Reads a file that was created with writeSceneFileSync
     * @param {string} path - The file path
     * @returns {string}
     */
    readSceneFileSync(path) { return ''; }

    /**
     * Deletes a file
     * @param {string} path - The file path
     */
    unlinkSceneFileSync(path) {}
}
