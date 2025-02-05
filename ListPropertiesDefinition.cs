using Newtonsoft.Json;

namespace Plugfy.Core.Commons
{
    [Serializable]
    public class ListPropertiesDefinition: List<PropertyDefinition>
    {

        public ListPropertiesDefinition()
        {
        }
        public PropertyDefinition GetField(string IDName)
        {
            var parameter = this.SingleOrDefault(x => x.IDName == IDName) ?? throw new Exception(string.Format(@"Parameter field ""{0}"" not exists.", IDName));
            return parameter;
        }

        public dynamic? GetValue(string IDName)
        {
            var field = this.GetField(IDName);
            return field?.Value;
        }

        public dynamic? getValue(string IDName, dynamic defaultValue, bool ignoreIfNotExists = true)
        {
            var parameter = this.Where(x => x.IDName.ToLower() == IDName.ToLower()).Take(1).SingleOrDefault();

            if(parameter == null && !ignoreIfNotExists)
                throw new Exception(string.Format(@"Parameter field ""{0}"" not exists.", IDName));

            if(parameter == null && ignoreIfNotExists)
                return defaultValue;

            return this.GetValue(IDName);
        }

        public T GetValue<T>(string IDName, T defaultValue = default(T), bool ignoreIfNotExists = true)
        {
            try
            {
                var parameter = this.Where(x => x.IDName.ToLower() == IDName.ToLower()).Take(1).SingleOrDefault();

                if(parameter == null && !ignoreIfNotExists)
                    throw new Exception(string.Format(@"Parameter field ""{0}"" not exists.", IDName));

                if(parameter == null && ignoreIfNotExists)
                    return defaultValue;

                Type t = typeof(T);
                t = Nullable.GetUnderlyingType(t) ?? t;


                if(typeof(T).Name.Contains("List<") || typeof(T).Name.Contains("List`") || (parameter?.Value != null && (parameter?.Value?.GetType().FullName.Contains("JObject") || parameter?.Value?.GetType().FullName.Contains("JArray"))))
                {
                    SetValue(parameter?.IDName, parameter?.Value, parameter?.Value?.GetType());
                }

                if(parameter?.Value != null)
                {
                    if(typeof(T) != parameter.Value.GetType())
                    {
                        try
                        {
                            return (parameter.Value == null) ?
                                defaultValue :
                                (T)Convert.ChangeType((string)parameter.Value, typeof(T));
                        }
                        catch(Exception)
                        {
                            try
                            {
                                return (T)Convert.ChangeType((T)parameter.Value, typeof(T));
                            }
                            catch(Exception)
                            {
                                try
                                {
                                    return (T)Convert.ChangeType((string)parameter.Value.ToString(), typeof(T));
                                }
                                catch(Exception)
                                {
                                    return defaultValue;
                                }

                            }

                        }
                    }
                }
                try
                {
                    return (parameter?.Value == null) ?
                        defaultValue : (T)Convert.ChangeType(parameter.Value, typeof(T));
                }
                catch(Exception)
                {

                    return (parameter?.Value == null) ?
                    defaultValue : (T)Convert.ChangeType((string)parameter.Value, typeof(T));

                }

            }
            catch(Exception)
            {

                throw new Exception(string.Format("invalid value on parameter: {0}", IDName));

            }

        }



        public void SetValue(string IDName, object value, Type type = null)
        {
            if(value == null)
                value = "";

            if(!string.IsNullOrEmpty(value.ToString()) && value.ToString().Contains('{') && value.ToString().Contains('}') && value.ToString().Contains(':'))
            {

                try
                {
                    var jsonSettings = new JsonSerializerSettings
                    {
                        DateParseHandling = DateParseHandling.None
                    };

                    var parameterlist = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(value.ToString(), jsonSettings);

                    if(parameterlist != null && parameterlist.Count > 0)
                    {
                        var valueList = new ListPropertiesDefinition();
                        foreach(var itemParamater in parameterlist)
                        {
                            valueList.SetValue(itemParamater.Key, itemParamater.Value);
                        }

                        value = valueList;
                        type = typeof(ListPropertiesDefinition);
                    }
                }
                catch(Exception)
                {

                }
            }


            if(this.Where(x => x.IDName.ToLower() == IDName.ToLower()).Take(1).SingleOrDefault() == null)
                this.Add(new PropertyDefinition()
                {
                    IDName = IDName,
                    Value = value,
                    HiddenField = true,
                    usedParameter = true,
                    ParameterFullTypeName = type != null ? type.GetType().FullName : value.GetType().FullName,
                    ParameterFullType = type != null ? type : value.GetType()
                });


            this.Where(x => x.IDName.ToLower() == IDName.ToLower()).Take(1).SingleOrDefault().Value = value;
            this.Where(x => x.IDName.ToLower() == IDName.ToLower()).Take(1).SingleOrDefault().usedParameter = true;

            ConvertParameterJsonToType(IDName, value);

        }



        private void ConvertParameterJsonToType(string IDName, object value)
        {
            var parameter = this.Where(x => x.IDName.ToLower() == IDName.ToLower()).Take(1).SingleOrDefault();

            var jsonSettings = new JsonSerializerSettings
            {
                DateParseHandling = DateParseHandling.None
            };

            if(!string.IsNullOrEmpty(parameter.ParameterFullTypeName) && parameter.ParameterFullType != null && value.GetType() != parameter.ParameterFullType)
            {

                if(parameter.ParameterFullType != null)
                {

                    if(parameter.ParameterType == enumDefaultUIValueType.List)
                    {
                        var listType = typeof(List<>).MakeGenericType(new Type[] { parameter.ParameterFullType });

                        var obj = Newtonsoft.Json.JsonConvert.DeserializeObject((string)value, listType, jsonSettings);
                        parameter.Value = obj;
                    }
                    else
                    {
                        var obj = Newtonsoft.Json.JsonConvert.DeserializeObject((string)value, parameter.ParameterFullType, jsonSettings);
                        parameter.Value = obj;
                    }


                }

            }
        }

        public Boolean IsNullOrEmpty(string IDName)
        {
            if(this.Where(x => x.IDName.ToLower() == IDName.ToLower()).Take(1).SingleOrDefault() == null)
                throw new Exception(string.Format(@"Parameter field ""{0}"" not exists.", IDName));

            return this.Where(x => x.IDName.ToLower() == IDName.ToLower()).Take(1).SingleOrDefault().Value == null || (string)this.Where(x => x.IDName.ToLower() == IDName.ToLower()).Take(1).SingleOrDefault().Value == "";
        }

        public new void Add(PropertyDefinition item)

        {
            if(item.Order == 0)
                item.Order = this.Count();

            base.Add(item);
        }

        protected static JsonSerializerSettings GetSerializerSettings()
        {
            return new JsonSerializerSettings()
            {
                ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
                DateFormatHandling = DateFormatHandling.IsoDateFormat,
                DateParseHandling = DateParseHandling.None,
                DefaultValueHandling = DefaultValueHandling.Ignore,
                FloatFormatHandling = FloatFormatHandling.DefaultValue,
                FloatParseHandling = FloatParseHandling.Decimal,
                MetadataPropertyHandling = MetadataPropertyHandling.ReadAhead,
                MissingMemberHandling = MissingMemberHandling.Ignore,
                NullValueHandling = NullValueHandling.Ignore,
                ObjectCreationHandling = ObjectCreationHandling.Reuse,
                DateTimeZoneHandling = DateTimeZoneHandling.Utc,
                PreserveReferencesHandling = PreserveReferencesHandling.None,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                StringEscapeHandling = StringEscapeHandling.EscapeNonAscii,
                TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple,
                TypeNameHandling = TypeNameHandling.None
            };
        }

        public Dictionary<string, object> ToDictionary(ListPropertiesDefinition parms = null, string parentKey = "", Dictionary<string, object> parentDict = null) {
            Dictionary<string, object> currentDict = new Dictionary<string, object>();

            if(parms == null)
                parms = this;

            foreach(var item in parms)
            {

                if(item.Value.GetType().FullName.StartsWith("System.Collections.Generic.List") && item.Value.GetType().FullName.Contains("ListDefaultUIParameterModel"))
                {
                    List<Dictionary<string, object>> listDictionary = new List<Dictionary<string, object>>();

                    foreach(var itemList in (List<ListPropertiesDefinition>)item.Value)
                    {
                        listDictionary.Add(itemList.ToDictionary());
                    }
                    currentDict.Add(item.IDName, listDictionary);

                }
                else if(!item.Value.GetType().FullName.StartsWith("System.Collections.Generic.List") && item.Value.GetType().FullName.Contains("ListDefaultUIParameterModel"))
                {
                    if(currentDict.ContainsKey(item.IDName))
                    {
                        var listValue = currentDict[item.IDName];


                        currentDict.Remove(item.IDName);

                        List<Dictionary<String, object>> listDict = new List<Dictionary<string, object>>();

                        if(listValue.GetType().FullName.Contains("System.Collections.Generic.List"))
                            listDict.AddRange((List<Dictionary<String, object>>)listValue);
                        else
                            listDict.Add((Dictionary<String, object>)listValue);

                        listDict.Add(((ListPropertiesDefinition)item.Value).ToDictionary(item.Value, item.IDName, currentDict));

                        if(parentDict != null && parentDict.ContainsKey(parentKey))
                            parentDict.Remove(parentKey);

                        currentDict.Add(item.IDName, listDict);
                    }
                    else
                        currentDict.Add(item.IDName, ((ListPropertiesDefinition)item.Value).ToDictionary(item.Value, item.IDName, currentDict));

                }
                else
                {
                        currentDict.Add(item.IDName, item.Value);
                }
            }

            if(parentDict != null && parentDict.ContainsKey(parentKey))
                parentDict.Remove(parentKey);

            return currentDict;
        }

    }
}

