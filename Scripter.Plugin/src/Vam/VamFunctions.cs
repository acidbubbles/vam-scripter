using System;
using ScripterLang;

namespace Vam
{
    public class VamFunctions
    {
        public static void Register(GlobalLexicalContext lexicalContext)
        {
            lexicalContext.Functions.Add("logMessage", LogMessage);
            lexicalContext.Functions.Add("logError", LogError);
            lexicalContext.Functions.Add("getFloatParamValue", GetFloatParamValue);
            lexicalContext.Functions.Add("setFloatParamValue", SetFloatParamValue);
            lexicalContext.Functions.Add("invokeTrigger", InvokeTrigger);
            lexicalContext.Functions.Add("invokeKeybinding", InvokeKeybinding);
        }

        private static Value LogMessage(Value[] args)
        {
            SuperController.LogMessage(args[0].ToString());
            return Value.Undefined;
        }

        private static Value LogError(Value[] args)
        {
            SuperController.LogError(args[0].ToString());
            return Value.Undefined;
        }

        private static Value GetFloatParamValue(Value[] args)
        {
            var storable = GetStorable(args[0], args[1]);
            var param = storable.GetFloatJSONParam(args[2].StringValue);
            return Value.CreateFloat(param.val);
        }

        private static Value SetFloatParamValue(Value[] args)
        {
            var storable = GetStorable(args[0], args[1]);
            var param = storable.GetFloatJSONParam(args[2].StringValue);
            param.val = args[3].FloatValue;
            return args[3];
        }

        private static Value InvokeTrigger(Value[] args)
        {
            var storable = GetStorable(args[0], args[1]);
            var param = storable.GetAction(args[2].StringValue);
            param.actionCallback.Invoke();
            return Value.Undefined;
        }

        private static Value InvokeKeybinding(Value[] args)
        {
            throw new NotImplementedException();
        }

        private static JSONStorable GetStorable(Value atomName, Value storableName)
        {
            var atom = SuperController.singleton.GetAtomByUid(atomName.StringValue);
            if (atom == null) throw new ScripterPluginException($"Could not find an atom named '{atomName}'");
            var storable = atom.GetStorableByID(storableName.StringValue);
            if (storable == null) throw new ScripterPluginException($"Could not find an storable named '{storableName}' in atom '{atomName}'");
            return storable;
        }
    }
}
