// @ts-check

import { AudioActionParamReference } from "./StorableParams/AudioActionParamReference";
import { BoolParamReference } from "./StorableParams/BoolParamReference";
import { FloatParamReference } from "./StorableParams/FloatParamReference";
import { StringChooserParamReference } from "./StorableParams/StringChooserParamReference";
import { StringParamReference } from "./StorableParams/StringParamReference";
import { UrlParamReference } from "./StorableParams/UrlParamReference";

/**
 * A storable, e.g. a plugin, audio source, geometry, etc.
 */
export class Storable {
   /**
    * Get the name of all params of all types
    * @returns {string[]}
    */
   getAllParamNames() { return []; }

   /**
    * Calls a trigger
    * @param {string} name
    * @returns {void}
    */
   invokeAction(name) { }

   /**
    * Gets a storable audio action
    * @param {string} name
    * @returns {AudioActionParamReference}
    */
   getAudioClipAction(name) { return new AudioActionParamReference(); }

   /**
    * Gets a storable float
    * @param {string} name
    * @returns {FloatParamReference}
    */
   getFloatParam(name) { return new FloatParamReference(); }

   /**
    * Gets a storable bool
    * @param {string} name
    * @returns {BoolParamReference}
    */
   getBoolParam(name) { return new BoolParamReference(); }

   /**
    * Gets a storable string
    * @param {string} name
    * @returns {StringParamReference}
    */
   getStringParam(name) { return new StringParamReference(); }

   /**
    * Gets a storable string chooser (drop down)
    * @param {string} name
    * @returns {StringChooserParamReference}
    */
   getStringChooserParam(name) { return new StringChooserParamReference(); }

   /**
    * Gets a storable url
    * @param {string} name
    * @returns {UrlParamReference}
    */
   getUrlParam(name) { return new UrlParamReference(); }
}
