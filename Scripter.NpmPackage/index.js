import { DateTime } from "./src/DateTime";
import { Keybindings } from "./src/Keybindings";
import { Player } from "./src/Player";
import { Random } from "./src/Random";
import { Scene } from "./src/Scene";
import { Scripter } from "./src/Scripter";
import { Time } from "./src/Time";

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
export const datetime = new DateTimeClass();
