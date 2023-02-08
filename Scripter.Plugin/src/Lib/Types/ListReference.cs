﻿using System.Collections.Generic;

namespace ScripterLang
{
    public class ListReference : Reference
    {
        private readonly List<Value> _values;

        public ListReference(List<Value> values)
        {
            _values = values;
        }
    }
}
