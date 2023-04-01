// @ts-check

import { DateTimeClass } from "./src/DateTime";
import { Keybindings } from "./src/Keybindings";
import { Player } from "./src/Player";
import { RandomClass } from "./src/RandomClass";
import { Scene } from "./src/Scene";
import { Scripter } from "./src/Scripter";
import { TimeClass } from "./src/TimeClass";
import { InputClass } from "./src/InputClass";

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
 * @type {TimeClass}
 */
export const Time = new TimeClass();

/**
 * Export for accessing Unity's Random
 * @type {RandomClass}
 */
export const Random = new RandomClass();

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
 * The Unity Input class
 * @type {InputClass}
 */
export const Input = new InputClass();

/**
 * Export for accessing .NET DateTime
 * @type {DateTimeClass}
 */
export const DateTime = new DateTimeClass();
