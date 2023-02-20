import { DateTime } from "./src/datetime";
import { Keybindings } from "./src/keybindings";
import { Player } from "./src/player";
import { Random } from "./src/random";
import { Scene } from "./src/scene";
import { Scripter } from "./src/scripter";
import { Time } from "./src/time";

/**
 * @module vam-scripter
 */

/**
 * Export for accessing the current instance of the Scripter plugin
 * @type {Scripter}
 */
export const scripter = new Scripter();

/**
 * Export for accessing Unity's Time
 * @type {Time}
 */
export const time = new Time();

/**
 * Export for accessing Unity's Random
 * @type {Random}
 */
export const random = new Random();

/**
 * Export for accessing the Virt-A-Mate scene information
 * @type {Scene}
 */
export const scene = new Scene();

/**
 * Export for accessing player information
 * @type {Player}
 */
export const player = new Player();

/**
 * Export for integrating with the Keybindings plugin
 * @type {Keybindings}
 */
export const keybindings = new Keybindings();

/**
 * Export for accessing .NET DateTime
 * @type {DateTime}
 */
export const datetime = new DateTime();
